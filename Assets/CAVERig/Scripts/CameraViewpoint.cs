using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraViewpoint : MonoBehaviour {

    private static Transform cameraTransform;

    private void Awake()
    {

    }

    // Use this for initialization
    void Start () {


        cameraTransform = this.transform;

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    

    public static RaycastHit GetRaycast()
    {

        Ray raycastRay = new Ray(cameraTransform.position, cameraTransform.forward);

        Debug.DrawRay(raycastRay.origin, raycastRay.direction * 1000);

        RaycastHit hitInfo;

        Physics.Raycast(raycastRay, out hitInfo, Mathf.Infinity);

        return hitInfo;


    }
}
