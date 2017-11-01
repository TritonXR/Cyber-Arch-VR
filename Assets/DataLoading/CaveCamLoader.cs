using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CaveCamLoader : MonoBehaviour
{

    [System.Serializable]
    public struct StereoCubemap
    {

        public Material leftEyeCubemap;
        public Material rightEyeCubemap;

        public string text;

    }

    private StereoCubemap activeCubemap;

    [SerializeField]
    public List<StereoCubemap> cubemaps;

    private int cubemapIndex = 0;

    private List<Camera> leftEyeCameras;
    private List<Camera> rightEyeCameras;

    [System.Obsolete]
    private GameObject skyboxCameras;

    // Use this for initialization
    void Start()
    {

        leftEyeCameras = CAVECameraRig.leftEyeCameras;
        rightEyeCameras = CAVECameraRig.rightEyeCameras;

        CAVECameraRig.on3DToggled += Set3D;

        SetupCubemapOnCameras(cubemaps[cubemapIndex]);

        Set3D(CAVECameraRig.is3D);

        // GameManager.instance.platform.GetComponentInChildren<InputGuideController>().SetControlsImage(true);

        Debug.Log("Starting");

    }

    public void CycleCubemap()
    {

        cubemapIndex++;

        if (cubemapIndex > cubemaps.Count-1) {

            cubemapIndex = 0;

        }

        SetupCubemapOnCameras(cubemaps[cubemapIndex]);


    }

    public void Update()
    {

        if (GamepadInput.GetDown(InputOption.A_BUTTON))
        {

            if (cubemaps.Count > 1)
            {
                CycleCubemap();
                Debug.Log("Cycling Cubemap");

            }
        }
    }


    private void SetupCubemapOnCameras(StereoCubemap newCubemap)
    {

        activeCubemap = newCubemap;

        foreach (Camera cam in CAVECameraRig.allCameras)
        {

          //  Debug.Log("Setting material for Camera " + cam.name);

            StereoTargetEyeMask targetEye = cam.stereoTargetEye;

            Material skyboxMaterial = newCubemap.rightEyeCubemap;

            if (targetEye == StereoTargetEyeMask.Left)
            {

      //          Debug.Log("Setting material for left eye Camera: " + cam.name);

                skyboxMaterial = newCubemap.leftEyeCubemap;
            }

            Skybox camSkybox = cam.GetComponent<Skybox>();

            if (camSkybox == null)
            {
                camSkybox = cam.gameObject.AddComponent<Skybox>();
            }

            camSkybox.material = skyboxMaterial;
            camSkybox.enabled = true;

            if (skyboxMaterial == null)
            {
                //Debug.LogError("No skybox material found for Camera " + cam.name);
            }
        }

        Debug.Log("Added Cubemaps");

        Set3D(CAVECameraRig.is3D);

    }

    private void RemoveCubemapFromCameras()
    {
        foreach (Camera cam in CAVECameraRig.allCameras)
        {
            cam.GetComponent<Skybox>().enabled = false;
        }

      Debug.Log("Removed Cubemaps");

    }

    public void Set3D(bool is3D)
    {
        foreach (Camera cam in leftEyeCameras)
        {

            Skybox camSkybox = cam.GetComponent<Skybox>();

            if (is3D)
            {
                camSkybox.material = activeCubemap.leftEyeCubemap;
            }

            else
            {
                camSkybox.material = activeCubemap.rightEyeCubemap;
            }
        }
    }

    public void OnDestroy()
    {

        CAVECameraRig.on3DToggled -= Set3D;
        RemoveCubemapFromCameras();

    }

    [System.Obsolete]
    public void CreateSkyboxCameras()
    {
        skyboxCameras = new GameObject();
        skyboxCameras.transform.parent = leftEyeCameras[0].transform.parent;
        skyboxCameras.name = "Skybox Cameras";

        foreach (Camera camera in leftEyeCameras)
        {

            // First, add the right eye to this camera for non-3D mode.
            camera.gameObject.AddComponent<Skybox>().material = activeCubemap.rightEyeCubemap;

            // Then, create a new skybox camera that uses the correct cubemap.
            GameObject newCamera = GameObject.Instantiate(camera.gameObject, camera.transform.position, camera.transform.rotation) as GameObject;
            newCamera.transform.parent = skyboxCameras.transform;
            newCamera.GetComponent<Camera>().cullingMask = 0;
            newCamera.GetComponent<Skybox>().material = activeCubemap.leftEyeCubemap;
            newCamera.GetComponent<Camera>().depth = -1;
        }

        foreach (Camera camera in rightEyeCameras)
        {

            // First, add the right eye to this camera for non-3D mode.
            camera.gameObject.AddComponent<Skybox>().material = activeCubemap.rightEyeCubemap;

            // Then, create a new skybox camera that uses the correct cubemap.
            GameObject newCamera = GameObject.Instantiate(camera.gameObject, camera.transform.position, camera.transform.rotation) as GameObject;
            newCamera.transform.parent = skyboxCameras.transform;
            newCamera.GetComponent<Camera>().cullingMask = 0;
            newCamera.GetComponent<Skybox>().material = activeCubemap.rightEyeCubemap;
            newCamera.GetComponent<Camera>().depth = -1;

        }

        // Initially deactivate the skybox cameras.
        SetSkyboxCamerasActive(false);


    }

    [System.Obsolete]
    public void SetSkyboxCamerasActive(bool active)
    {

        // Sets the skybox cameras to their respective active state.
        skyboxCameras.SetActive(active);

        foreach (Camera camera in leftEyeCameras)
        {

            if (active)
            {
                // If the skybox cameras are active, the normal cameras shouldn't render a skybox.
                camera.GetComponent<Camera>().clearFlags = CameraClearFlags.Nothing;
            }
            else
            {
                camera.GetComponent<Camera>().clearFlags = CameraClearFlags.Skybox;
            }
        }

        foreach (Camera camera in rightEyeCameras)
        {

            if (active)
            {
                // If the skybox cameras are active, the normal cameras shouldn't render a skybox.
                camera.GetComponent<Camera>().clearFlags = CameraClearFlags.Nothing;
            }
            else
            {
                camera.GetComponent<Camera>().clearFlags = CameraClearFlags.Skybox;
            }
        }
    }
}
