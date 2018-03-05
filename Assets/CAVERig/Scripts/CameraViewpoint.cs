using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script sends a raycast from a central camera viewpoint.
// Can be used to determine where the user/screen is looking.
public class CameraViewpoint : MonoBehaviour {

    // Transform of the Camera
    private static Transform cameraTransform;

    // Use this for initialization
    void Start () {

        // Set the transform value from this script.
        cameraTransform = this.transform;

	}
	
    // Gets a raycast going forward out of a central camera viewpoint.
    public static RaycastHit GetRaycast()
    {

        // Create a raycast to send from this camera viewpoint.
        Ray raycastRay = new Ray(cameraTransform.position, cameraTransform.forward);

        //Debug.DrawRay(raycastRay.origin, raycastRay.direction);
        Debug.DrawRay(raycastRay.origin, raycastRay.direction * 1000);

        // Variable to store raycast out information.
        RaycastHit hitInfo;
        
        // Actually do the raycast
        Physics.Raycast(raycastRay, out hitInfo, Mathf.Infinity);

        // Return the raycast informatino.
        return hitInfo;

    }
}
