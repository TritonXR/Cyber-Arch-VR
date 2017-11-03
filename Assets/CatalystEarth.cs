using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CatalystEarth : MonoBehaviour {

    private static float planetRadius;
    public static Transform earthTransform;

    public struct LatLon
    {

        public float lat;
        public float lon;

        public LatLon(float lat, float lon)
        {
            this.lat = lat;
            this.lon = lon;
        }

    }

    private void Awake()
    {

        CalculateRadius();
        earthTransform = this.transform;

    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void CalculateRadius()
    {

        planetRadius = GetComponent<SphereCollider>().radius * 5.6f;

    }

    public static Vector3 Get3DPositionFromLatLon(float latitude, float longitude)
    {

        return Get3DPositionFromLatLon(new LatLon(latitude, longitude));

    }

    public static Vector3 Get3DPositionFromLatLon(LatLon latLon)
    {

        float lat = latLon.lat;
        float lon = latLon.lon;

        /*
        Vector3 worldPos = Quaternion.AngleAxis(lon, -Vector3.up) * 
                           Quaternion.AngleAxis(lat, -Vector3.right) * 
                           new Vector3(0.0f, 0.0f, planetRadius);

    */
        float existingYRotation = earthTransform.rotation.eulerAngles.y;

        Vector3 worldPos = Quaternion.AngleAxis(lon - existingYRotation, -Vector3.up) *
                           Quaternion.AngleAxis(lat, -Vector3.right) *
                           new Vector3(0.0f, 0.0f, planetRadius);

        worldPos += earthTransform.position;

        return worldPos;

}

    public static LatLon GetLatLongFromVector3(Vector3 position)
    {
        float lat = (float)Mathf.Acos(position.y / planetRadius); //theta
        float lon = (float)Mathf.Atan(position.x / position.z); //phi
        return new LatLon(lat, lon);
    }
}
