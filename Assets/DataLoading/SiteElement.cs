using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public abstract class SiteElement : MonoBehaviour
{

    public bool loaded = false;
    public bool failed = false;

    public Site parentSite;

    protected SerializableSiteElement siteData;

    protected abstract IEnumerator LoadCoroutine();
    protected abstract IEnumerator UnloadCoroutine();
    protected abstract IEnumerator ActivateCoroutine();
    protected abstract IEnumerator DeactivateCoroutine();

    public void Initialize(SerializableSiteElement siteData, Site parentSite)
    {
        this.parentSite = parentSite;
        this.siteData = siteData;
    }

    public Coroutine Activate()
    {
        Debug.Log("ACTIVATING");

        CatalystEarth.Hide();
        SceneManager.LoadScene("DataScene");

        Coroutine activeCoroutine = StartCoroutine(LoadThenActivate());

        SiteManager.activeSiteElement = this;
        return activeCoroutine;
    }

    public Coroutine Deactivate()
    {
        Debug.Log("DEACTIVATING");
        Coroutine deactivateCoroutine = StartCoroutine(DeactivateCoroutine());

        if (SiteManager.activeSiteElement == this)
        {
            SiteManager.activeSiteElement = null;
        }

        return deactivateCoroutine;
    }

    public Coroutine Load()
    {
 
        return StartCoroutine(WaitForLoad());

    }

    public Coroutine Unload()
    {
        return StartCoroutine(WaitForUnload());
    }

    private IEnumerator WaitForLoad()
    {

        // Don't try to load two different elements at once. Could get messy.
        while (SiteManager.loading)
        {
            yield return null;
        }

        GamepadInput.LockInput(true);
        SiteManager.loading = true;

        StatusText.Show();

        if (string.IsNullOrEmpty(siteData.sceneName))
        {
            yield return StartCoroutine(LoadCoroutine());
        }
        else
        {
            AsyncOperation sceneLoad = SceneManager.LoadSceneAsync(siteData.sceneName);

            while (!sceneLoad.isDone)
            {
                yield return null;
            }
        }

        loaded = true;
        StatusText.Hide();

        GamepadInput.LockInput(false);
        SiteManager.loading = false;
    }

    private IEnumerator WaitForUnload()
    {
        yield return StartCoroutine(UnloadCoroutine());
        loaded = false;
    }

    private IEnumerator LoadThenActivate()
    {

        if (!loaded)
        {
            Debug.LogFormat(gameObject, "Site element {0} not yet loaded. Will activate when done loading.", gameObject.name);
            yield return Load();
        }

        yield return StartCoroutine(ActivateCoroutine());

    }

    protected void PrintIncorrectTypeError(string siteName, string dataType)
    {
        Debug.LogErrorFormat("Could not load site element {0} at site {1}: Incorrect data passed to Activate method", dataType, siteName);
    }

    public string GetAbsolutePath(string relativeDataPath)
    {

        string filePath = "/" + SiteManager.pathToDataFolder + "/" + relativeDataPath;

        if (!File.Exists(SiteManager.pathToDataFolder + "/" + "config_3.json"))
        {
            Debug.LogWarning("Could not find config file");
        }

        if (!File.Exists(filePath))
        {
            Debug.LogError("Could not find data file at: " + filePath);
        }

        return filePath;

    }
}

[System.Serializable]
public abstract class SerializableSiteElement
{

    public int id;
    public string name;
    public string description;

    // IF the scene string is present, Unity will just load the scene with the specified name, ignoring all other JSON elements.
    public string sceneName;

}
