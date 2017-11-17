using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POI : MonoBehaviour {

    private Site associatedSite;
    public Material activeMat;
    public Material inactiveMat;

    public static POI selectedPOI;

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

    public void SetSelected(bool selected)
    {
        if (selected)
        {

            if (selectedPOI != null && selectedPOI != this)
            {
                selectedPOI.SetSelected(false);
            }

            selectedPOI = this;
            GetComponentInChildren<MeshRenderer>().material = activeMat;
            CatalystEarth.RotateToPOI(this);

        }
        else
        {
            if (selectedPOI = this)
            {
                selectedPOI = null;
            }

            GetComponentInChildren<MeshRenderer>().material = inactiveMat;
        }

    }
}
