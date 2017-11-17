using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SiteManager : MonoBehaviour {

    public static SiteManager instance;

    public string pathToDataJsonFile = "./Config_Files/site_data.json";

    [HideInInspector]
    public List<Site> sites;
    // private GameManager gameManager;

    public Object poiPrefab;
    public Material poiActiveMat;
    public Material poiInactiveMat;


    public static SiteElement activeSiteElement;
    public static SiteElementSet activeSiteElementSet;

    void Awake()
    {

        if (instance == null)
        {
            instance = this;
            LoadSites(pathToDataJsonFile);
        }

    }

    public void LoadSites(string dataPath)
    {

        if (!File.Exists(pathToDataJsonFile))
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

            File.WriteAllText(pathToDataJsonFile, jsonText);

            return;

        }

        string jsonString = File.ReadAllText(pathToDataJsonFile);

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

    /*

    public IEnumerator PlacePOIsWhenReady()
    {

        while (CatalystEarth.earthTransform == null)
        {
            yield return null;
        }

        CreatePOIs();

    }
    */

        // Use this for initialization

	
    [System.Serializable]
    private class SerializableSites
    {

        public SerializableSite[] sites;

    }
}


