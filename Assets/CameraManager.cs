using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

public class CameraManager : MonoBehaviour {

    public GameObject TestCamera;
    public GameObject CaveCamera;
    public GameObject ViveCamera;

    public bool CaveMode = false;

	// Use this for initialization
	void Start () {
        ViveCamera.SetActive(false);
        TestCamera.SetActive(false);
        CaveCamera.SetActive(false);
        if (VRDevice.isPresent)
        {
            ViveCamera.SetActive(true);
        } else if (CaveMode)
        {
            CaveCamera.SetActive(true);
        } else
        {
            TestCamera.SetActive(true);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
