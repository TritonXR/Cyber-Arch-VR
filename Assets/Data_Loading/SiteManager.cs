using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SiteManager : MonoBehaviour {

    public List<CatalystSite> sites;
    // private GameManager gameManager;

    public static CatalystSite activeSite;

    void Start()
    {
        LoadSites(GameManager.dataJsonFile);
    }

    public void LoadSites(string dataPath)
    {
        // gameManager = GetComponentInParent<GameManager>();

        if (!File.Exists(GameManager.dataJsonFile))
        {

            SerializableCatalystSites sampleSites = new SerializableCatalystSites();

            sampleSites.sites = new SerializableCatalystSite[1];

            SerializableCatalystSite newSite = new SerializableCatalystSite();

            // UC San Diego Lat/Lon as sample
            newSite.latitude = 32.8801f;
            newSite.longitude = 117.2340f;
            newSite.name = "UC San Diego";
            newSite.description = "This site was generated as a sample of what the JSON file should look like, roughly";

            sampleSites.sites[0] = newSite;

            string jsonText = JsonUtility.ToJson(sampleSites);

            File.WriteAllText(GameManager.dataJsonFile, jsonText);

            return;

        }

        string jsonString = File.ReadAllText(GameManager.dataJsonFile);

        SerializableCatalystSites siteData = JsonUtility.FromJson<SerializableCatalystSites>(jsonString);

        if (siteData.sites != null && siteData.sites.Length > 0)
        {
            foreach (SerializableCatalystSite site in siteData.sites)
            {

                GameObject newSiteObject = new GameObject();
                newSiteObject.transform.parent = this.transform;
                CatalystSite newSite = newSiteObject.AddComponent<CatalystSite>();
                newSite.siteData = site;
                sites.Add(newSite);
                newSiteObject.name = newSite.siteName;

            }
        }
        else
        {
            Debug.LogErrorFormat("Error: No sites loaded. Please check the following file: {0}", GameManager.dataJsonFile);
            return;
        }

        Debug.LogFormat("Loaded {0} sites", sites.Count);

        // CreatePOIs();
    }

    /*
    public void CreatePOIs()
    {

        POIManager poiManager = gameManager.GetComponentInChildren<POIManager>();

        foreach (CatalystSite site in sites)
        {

            poiManager.CreateNewPOI(site);

        }
    }

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
    private class SerializableCatalystSites
    {

        public SerializableCatalystSite[] sites;

    }
}


