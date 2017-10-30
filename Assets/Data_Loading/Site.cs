using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Site : MonoBehaviour
{

    SerializableSite siteData;

    SiteElementSet panos;
    SiteElementSet videos;
    SiteElementSet artifacts;
    SiteElementSet images;
    SiteElementSet sites3D;

    public void InitializeSite(SerializableSite siteJSON)
    {
        siteData = siteJSON;
        InitializeSiteElements();
    }

    public void InitializeSiteElements()
    {

        GameObject panoSetObj = CreateElementSetObject("Panos");
        panos = panoSetObj.AddComponent<PanoSet>();
        panos.Initialize(siteData.panos, this);


        //GameObject artifactSetObj = CreateElementSetObject("Artifacts");
        //artifacts = artifactSetObj.AddComponent<ArtifactSet>();

        //GameObject site3DSetObj = CreateElementSetObject("3D Sites");

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
    public SerializableModel[] artifacts;
    public SerializableModel[] sites3D;
    public SerializableImage[] images;
    public SerializablePointCloud[] pointClouds;

}
