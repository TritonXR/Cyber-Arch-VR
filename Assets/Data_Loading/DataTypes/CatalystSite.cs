using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;


public class CatalystSite : MonoBehaviour
{
    public string siteName
    {
        get
        {
            return siteData.name;
        }
    }

    public List<CAVECam> caveCams;
    private int camIndex = 0;

    private Transform modelParent;

    public List<CatalystModel> sites3D;
    private int modelIndex = 0;

    [SerializeField] private float modelDistanceFromCam = 3.0f;

    public SerializableCatalystSite siteData;

    public CatalystSiteElement activeElement;

    public IEnumerator ShowCAVECams()
    {

        DeactivateActiveElement();

        if (caveCams == null || caveCams.Count == 0)
        {

            yield return RoutineRunner.instance.StartCoroutine(LoadCAVECams());

        }

        camIndex = 0;

        SceneManager.LoadSceneAsync("Buffer Scene");

        if (camIndex < caveCams.Count)
        {

            caveCams[camIndex].Activate();
            //activeElement = caveCams[camIndex];

        }
        else
        {
            Debug.LogError("No panos loaded. Aborting.");
        }

    }

    public IEnumerator HideCAVECams()
    {

        DeactivateActiveElement();
        yield return null;

    }

    public IEnumerator Show3DSites()
    {

        DeactivateActiveElement();

        modelIndex = 0;

        Debug.Log("Showing Models!");
        yield return null;


        if (sites3D == null || sites3D.Count == 0)
        {

            Debug.Log("Loading Models!");
            yield return null;

            yield return RoutineRunner.instance.StartCoroutine(Load3DSites());

        }


        SceneManager.LoadSceneAsync("Buffer Scene");

      //  modelParent.localPosition = Vector3.zero;

        if (modelIndex < sites3D.Count)
        {

            sites3D[modelIndex].Activate();
            activeElement = sites3D[modelIndex];

        }

        /*
        List<Vector3> positions = CommonCatalystMath.GetPositionsOnUnitCircleBySides(models.Count);

        for (int i = 0; i < positions.Count; i++)
        {

            positions[i] *= modelDistanceFromCam;

        }

        for (int i = 0; i < models.Count; i++)
        {

            models[i].Activate();
            models[i].SetLocalPosition(positions[i]);
            activeElement = models[i];
            models[i].model.SetActive(true);

        }
        */
        
    }

    public IEnumerator Hide3DSites()
    {

        DeactivateActiveElement();
        yield return null;
    }

    public void DeactivateActiveElement()
    {

        if (activeElement != null)
        {
            activeElement.Deactivate();
            activeElement = null;
        }
    }

    public void Select()
    {

        //SiteManager.activeSite = this;

    }

    public void Deselect()
    {

        DeactivateActiveElement();
       // SiteManager.activeSite = null;

    }

    public void CycleCAVECams()
    {
        if (activeElement is CAVECam)
        {

            activeElement.Deactivate();

            camIndex++;
            if (camIndex > caveCams.Count-1)
            {
                camIndex = 0;
            }

            caveCams[camIndex].Activate();
            //activeElement = caveCams[camIndex];

        }
        else
        {

            Debug.LogWarning("Unable to cycle CAVECams: No CAVECam set is currently active. Please call ShowCAVECams first");

        }
    }

    public void Cycle3DSites()
    {

        if (activeElement is CatalystModel)
        {

            activeElement.Deactivate();

            modelIndex++;
            if (modelIndex > sites3D.Count-1)
            {
                modelIndex = 0;
            }

            sites3D[modelIndex].Activate();
            activeElement = sites3D[modelIndex];

        }
    }

    public void Update()
    {

        if (GamepadInput.GetDown(InputOption.A_BUTTON))
        {

            if (activeElement is CAVECam)
            {

                CycleCAVECams();

            }

            if (activeElement is CatalystModel)
            {

                if ((activeElement as CatalystModel).isSite)
                {

                    Cycle3DSites();

                }
            }
        }
    }

    public IEnumerator LoadCAVECams()
    {

        if (caveCams == null || caveCams.Count == 0)
        {

            caveCams = new List<CAVECam>();

            for (int i = 0; i < siteData.panos.Length; i++)
            {

                SerializableCAVECam camJSON = siteData.panos[i];

                Debug.LogFormat("Initializing pano {0} of {1}", i+1, siteData.panos.Length);

                CAVECam newCam = gameObject.AddComponent<CAVECam>();
                //yield return newCam.Initialize(camJSON);

                yield return null;

                caveCams.Add(newCam);

            }
        }
    }

    public IEnumerator Load3DSites()
    {

        Debug.Log("Beginning model load");
        yield return null;

        sites3D = new List<CatalystModel>();

        GameObject modelParentObj = new GameObject();

        modelParent = modelParentObj.transform;

        modelParent.name = siteData.name + " - Models";

        modelParent.parent = GameManager.instance.cameraRig.viewpoint.transform;

        modelParent.transform.localPosition = Vector3.zero;
        modelParent.transform.localRotation = Quaternion.identity;

        modelParent.parent = this.transform;

        foreach (SerializableModel modelData in siteData.sites3D)
        {

            Debug.Log("Initializing models");
            yield return null;

            CatalystModel newModel = gameObject.AddComponent<CatalystModel>();
            newModel.isSite = true;
            yield return newModel.Initialize(modelData);
            newModel.model.transform.parent = modelParent.transform;
            sites3D.Add(newModel);

        }

        modelParent.Translate(new Vector3(0.0f, -40.0f, 0.0f));

    }
}



[System.Serializable]
public class SerializableCatalystSite
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