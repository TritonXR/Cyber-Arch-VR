using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class SiteManager : MonoBehaviour {

    public static SiteManager instance;

    public static string pathToDataFolder
    {
        get
        {
            string dataJSONFilePath = GameManager.instance.caveSettings.pathToDataJSONFile;
            return Path.GetDirectoryName(dataJSONFilePath);
        }
    }

    [HideInInspector]
    public List<Site> sites;
    // private GameManager gameManager;

    public Object poiPrefab;
    public Material poiActiveMat;
    public Material poiInactiveMat;

    public float modelRotationSpeed = 3.0f;
    public Material objectDefaultMat;
    public Material objectTransparentMat;

    public static SiteElement activeSiteElement;
    public static SiteElementSet activeSiteElementSet;

    public static bool loading = false;

    public List<SiteElement> allSiteElements
    {
        get
        {
            List<SiteElement> allElements = new List<SiteElement>();
            for (int i = 0; i < sites.Count; i++)
            {
                for (int j = 0; j < sites[i].dataSets.Count; j++)
                {
                    for (int k = 0; k < sites[i].dataSets[j].siteElements.Count; k++)
                    {
                        allElements.Add(sites[i].dataSets[j].siteElements[k]);
                    }
                }
            }

            return allElements;

        }
    }

    public List<SiteElement> allLoadedSiteElements
    {
        get
        {
            List<SiteElement> allLoadedElements = new List<SiteElement>();
            List<SiteElement> allElements = allSiteElements;

            for (int i = 0; i < allElements.Count; i++)
            {
                if (allElements[i].loaded)
                {
                    allLoadedElements.Add(allElements[i]);
                }
            }

            return allLoadedElements;
        }
    }

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            GameObject.Destroy(this.gameObject);
        }
    }


    IEnumerator Start()
    {
        if (GameManager.instance.caveSettings.loadAllDataOnStart)
        {
            yield return StartCoroutine(LoadAllSites());
        }

        StartCoroutine(CheckForIdle());

    }

    // Warning: This can take a LONG time! Best when scheduled when nobody will be using the application.
    public IEnumerator LoadAllSites()
    {
        StatusText.SetText("Loading All Sites. Please Wait.");
        GamepadInput.LockInput(true);
        if (GameManager.instance.caveSettings.loadAllDataOnStart)
        {
            for (int i = 0; i < sites.Count; i++)
            {

                Site site = sites[i];

                for (int j = 0; j < site.dataSets.Count; j++)
                {

                    SiteElementSet dataSet = site.dataSets[j];

                    for (int k = 0; k < dataSet.siteElements.Count; k++)
                    {

                        SiteElement dataElement = dataSet.siteElements[k];

                        string statusString = string.Format("Loading element {0} of {1} from set {2} of {3} from site {4} of {5}",
                            k, dataSet.siteElements.Count, j, site.dataSets.Count, i, sites.Count);
                        yield return dataElement.Load();

                    }
                }
            }
        }
        GamepadInput.LockInput(false);
    }

    public IEnumerator UnloadAllSites()
    {

        if (GameManager.instance.caveSettings.loadAllDataOnStart)
        {
            foreach (Site site in sites)
            {
                foreach (SiteElementSet dataSet in site.dataSets)
                {
                    foreach (SiteElement dataElement in dataSet.siteElements)
                    {

                        yield return dataElement.Unload();

                    }
                }
            }
        }
    }

    public void LoadSites(string dataPath)
    {

        if (!File.Exists(dataPath))
        {

            SerializableSites sampleSites = new SerializableSites();

            sampleSites.sites = new SerializableSite[1];

            SerializableSite newSite = new SerializableSite();

            // UC San Diego Lat/Lon as sample
            newSite.latitude = 32.8801f;
            newSite.longitude = 117.2340f;
            newSite.name = "UC San Diego";
            newSite.description = "This site was generated as a sample of what the JSON file should look like, roughly";

            sampleSites.sites[0] = newSite;

            string jsonText = JsonUtility.ToJson(sampleSites);

            File.WriteAllText(dataPath, jsonText);

            return;

        }

        string jsonString = File.ReadAllText(dataPath);

        SerializableSites siteData = JsonUtility.FromJson<SerializableSites>(jsonString);

        if (siteData.sites != null && siteData.sites.Length > 0)
        {
            foreach (SerializableSite site in siteData.sites)
            {

                GameObject newSiteObject = new GameObject(site.name);
                newSiteObject.transform.parent = this.transform;
                Site newSite = newSiteObject.AddComponent<Site>();

                newSite.InitializeSite(site);

                sites.Add(newSite);

            }
        }
        else
        {
            Debug.LogErrorFormat("Error: No sites loaded. Please check the following file: {0}", GameManager.dataJsonFile);

            return;
        }

        Debug.LogFormat("Loaded {0} sites", sites.Count);

        StartCoroutine(CreatePOIs());
    }

    
    public IEnumerator CreatePOIs()
    {

        while (CatalystEarth.earthTransform == null)
        {
            yield return null;
        }

        foreach (Site site in sites)
        {

            POI newPOI = (GameObject.Instantiate(poiPrefab) as GameObject).GetComponent<POI>();
            newPOI.SetAssociatedSite(site);
            newPOI.activeMat = poiActiveMat;
            newPOI.inactiveMat = poiInactiveMat;

        }
    }

    public bool ShouldIdle()
    {
        if (GamepadInput.timeSinceLastInput >= (GameManager.instance.minutesBeforeIdle * 60.0f) && !SiteManager.loading)
        {
            return true;
        }

        return false;
    }

    public IEnumerator CheckForIdle()
    {

        bool idling = false;

        while (true)
        {
            if (ShouldIdle())
            {
                Debug.Log("Starting Idle!");

                yield return StartCoroutine(GameManager.instance.GoHome());

                Quaternion originalPlayerRotation = Player.instance.transform.rotation;
                KioskUI.instance.Show(false);
                idling = true;
                yield return StartCoroutine(IdleScroll());
                idling = false;

                yield return StartCoroutine(GameManager.instance.GoHome());
                Player.instance.transform.rotation = originalPlayerRotation;
                KioskUI.instance.Show(true);


            }

            yield return null;

        }
    }

    public IEnumerator IdleScroll()
    {

        List<SiteElement> loadedElements = allLoadedSiteElements;

        if (loadedElements.Count <= 0)
        {

            Debug.LogWarning("No data already loaded. Attempting to load very first data set");

            yield return allSiteElements[0].Load();
            loadedElements = allLoadedSiteElements;

            if (loadedElements.Count <= 0)
            {
                Debug.LogError("Could not go to screensaver mode because no data was found! Exiting application in 1 minute to prevent burn-in.");
                yield return new WaitForSeconds(60.0f);
                Application.Quit();
            }

        }

        loadedElements = allLoadedSiteElements;

        int currentElementIndex = 0;
        while (ShouldIdle())
        {

            SiteElement currentElement = loadedElements[currentElementIndex];

            yield return currentElement.Activate();

            float elapsedTime = 0.0f;

            Quaternion originalPlayerRotation = Player.instance.transform.rotation;

            while (elapsedTime < GameManager.instance.secondsPerIdleScene && ShouldIdle())
            {

                if (currentElement is Panorama)
                {
                    float camRotationSpeed = 0.05f;

                    Player.instance.transform.Rotate(Vector3.up, camRotationSpeed);


                }
                else if (currentElement is Model)
                {
                }

                yield return null;
                elapsedTime += Time.deltaTime;

            }

            Player.instance.transform.rotation = originalPlayerRotation;

            yield return currentElement.Deactivate();

            currentElementIndex++;
            if (currentElementIndex >= loadedElements.Count)
            {
                currentElementIndex = 0;
            }

            yield return null;

        }
    }
	
    [System.Serializable]
    private class SerializableSites
    {

        public SerializableSite[] sites;
   
    }

}


