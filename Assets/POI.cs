using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class POI : MonoBehaviour {

    private Site associatedSite;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void SetAssociatedSite(Site site)
    {
        gameObject.name = site.siteName;
        associatedSite = site;
        site.associatedPOI = this;

        SetPosition(site.siteData.latitude, site.siteData.longitude);

    }

    public void SetPosition(float latitude, float longitude)
    {
        Vector3 pos = CatalystEarth.Get3DPositionFromLatLon(latitude, longitude);
        Quaternion rot = Quaternion.FromToRotation(Vector3.forward, Vector3.up) * Quaternion.LookRotation(CatalystEarth.earthTransform.position);
        transform.SetPositionAndRotation(pos, rot);
        transform.SetParent(CatalystEarth.earthTransform);

       // transform.rotation = Quaternion.FromToRotation(Vector3.forward, Vector3.up) * Quaternion.LookRotation(CatalystEarth.earthTransform.position);
        transform.LookAt(CatalystEarth.earthTransform.position);
    }

    public void SetMaterial(Material mat)
    {

        GetComponent<MeshRenderer>().material = mat;

    }
}
