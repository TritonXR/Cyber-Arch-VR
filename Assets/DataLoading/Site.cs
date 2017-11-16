using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Site : MonoBehaviour
{

    public string siteName
    {
        get
        {
            return siteData.name;
        }
    }

    public string siteDescription
    {
        get
        {
            return siteData.description;
        }
    }

    public SerializableSite siteData;

    public List<SiteElementSet> dataSets;

    public POI associatedPOI;

    public void InitializeSite(SerializableSite siteJSON)
    {
        siteData = siteJSON;
        InitializeSiteElements();
    }

    public void InitializeSiteElements()
    {

        dataSets = new List<SiteElementSet>();

        if (siteData.panos != null && siteData.panos.Length > 0)
        {
            GameObject panoSetObj = CreateElementSetObject(PanoSet.elementString);
            PanoSet panoSet = panoSetObj.AddComponent<PanoSet>();
            panoSet.Initialize(siteData.panos, this);
            dataSets.Add(panoSet);
        }

        if (siteData.models != null && siteData.models.Length > 0)
        {

            GameObject modelSetObj = CreateElementSetObject(ModelSet.elementString);
            ModelSet modelSet = modelSetObj.AddComponent<ModelSet>();
            modelSet.Initialize(siteData.models, this);
            dataSets.Add(modelSet);

        }

        // TODO: More supported data types.

    }

    private GameObject CreateElementSetObject(string name)
    {

        GameObject newElementSet = new GameObject(name);
        newElementSet.transform.SetParent(this.transform);
        newElementSet.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

        return newElementSet;

    }
}

[System.Serializable]
public class SerializableSite
{
    public string name;
    public string description;
    public float latitude;
    public float longitude;

    public SerializableCAVECam[] panos;
    public SerializableVideo[] videos;
    public SerializableModel[] models;
    public Serializable3DSite[] sites3D;
    public SerializableImage[] images;

}
