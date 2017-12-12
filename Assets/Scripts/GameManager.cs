using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public const string dataDirectory = "./CAVEkiosk_SiteData";
    public const string cacheDirectory = "./Cached_Data";
    public const string dataJsonFile = dataDirectory + "/site_data.json";

    // Is this running in the cave?
    public static bool isCave = false;

    public static GameManager instance;

    public bool loadAllDataOnStart = false;
    public float minutesBeforeIdle = 5.0f;
    public float secondsPerIdleScene = 10.0f;
    public GameObject user;

    [HideInInspector]
    public CAVECameraRig cameraRig;

    [SerializeField]
    public GameObject fadePlane;

    public static Object photoPrefab;
    public Object photoPrefabEditor;

    public Object labelPrefab;

    public enum State
    {

        IDLE,
        ACTIVE

    }

    public static State gameState = State.ACTIVE;

    // Use this for initialization
    /// <summary>
    /// Used to set up the static variables based on the dragged in variables.
    /// </summary>
    void Awake()
    {

        HandleSingleton();
        SetupGameManagers();



    }

    public void HandleSingleton()
    {

        if (GameManager.instance == null)
        {
            GameManager.instance = this;

            GameObject topLevelParent = gameObject;

            while (topLevelParent.transform.parent != null)
            {
                topLevelParent = topLevelParent.transform.parent.gameObject;
            }

            DontDestroyOnLoad(topLevelParent);

            photoPrefab = photoPrefabEditor;

        }
        else
        {
            GameObject.DestroyImmediate(this.gameObject);
            return;
        }
    }

    public void SetupGameManagers()
    {

        if (this != null && user != null) {

            /*
            platform = user.GetComponentInChildren<CatalystPlatform>();
            cameraRig = user.GetComponentInChildren<CAVECameraRig>();
            monitor = user.GetComponentInChildren<PlatformMonitor>();
            photoController = GetComponentInChildren<PhotoController>();
            inputGuide = user.GetComponentInChildren<InputGuideController>();
            platform.gameManager = this;
            */
        }
    }

    public IEnumerator GoHome()
    {
        Debug.Log("GOING HOME");
        
        if (SiteManager.activeSiteElementSet != null)
        {

            yield return SiteManager.activeSiteElementSet.Deactivate();

        }
        

        AsyncOperation loadScene = SceneManager.LoadSceneAsync(0);
        // PlatformMonitor.ResetMonitorText();

        while (!loadScene.isDone)
        {
            yield return null;
        }

        Debug.Log("MAIN SCENE LOADED");

        SiteManager siteManager = GetComponentInChildren<SiteManager>();

        CatalystEarth.Show();

        // siteManager.StartCoroutine(siteManager.PlacePOIsWhenReady());

        Debug.Log("DONE GOING HOME");

    }

    public void Update()
    {

        if (GamepadInput.GetDown(InputOption.BACK_BUTTON) && CAVECameraRig.instance != null)
        {
            CAVECameraRig.instance.Toggle3D();
        }

        if (GamepadInput.GetDown(InputOption.B_BUTTON))
        {
            Debug.Log("HOME PRESSED");
            // Loads the first scene. Assumed to be the home scene.
            StartCoroutine(GoHome());
        }

        if (GamepadInput.GetDown(InputOption.A_BUTTON))
        {
            if (SiteManager.activeSiteElementSet != null)
            {
                SiteManager.activeSiteElementSet.NextElement();
            }
        }

    }
}
