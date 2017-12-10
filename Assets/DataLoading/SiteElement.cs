using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        Coroutine activeCoroutine = StartCoroutine(LoadThenActivate());

        SiteManager.activeSiteElement = this;
        return activeCoroutine;
    }

    public Coroutine Deactivate()
    {
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
