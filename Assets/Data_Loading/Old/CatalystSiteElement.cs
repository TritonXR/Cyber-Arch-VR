using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CatalystSiteElement : MonoBehaviour {

    public bool initialized = false;

    public int id;
    public string name;
    public string description;

    protected abstract IEnumerator InitializeCoroutine(SerializableCatalystSiteElement siteData);
    protected abstract IEnumerator ActivateCoroutine();
    protected abstract IEnumerator DeactivateCoroutine();

    public Coroutine Activate()
    {

        if (initialized == false)
        {

            Debug.LogError("Catalyst site element has not yet been initialized. Please call the Initialize function first. Aborting");
            return null;

        }

        Coroutine createdCoroutine = StartCoroutine(ActivateCoroutine());

        return createdCoroutine;

    }

    public Coroutine Deactivate()
    {

        return StartCoroutine(DeactivateCoroutine());

    }

    public Coroutine Initialize(SerializableCatalystSiteElement siteData)
    {

        name = siteData.name;
        description = siteData.description;
        id = siteData.id;

        return StartCoroutine(WaitForInitialize(siteData));

    }

    private IEnumerator WaitForInitialize(SerializableCatalystSiteElement siteData)
    {

        yield return StartCoroutine(InitializeCoroutine(siteData));
        initialized = true;

    }

    protected void PrintIncorrectTypeError(string siteName, string dataType)
    {


        Debug.LogErrorFormat("Could not load site element {0} at site {1}: Incorrect data passed to Activate method", dataType, siteName);

    }
}

[System.Serializable]
public abstract class SerializableCatalystSiteElement
{

    public int id;
    public string name;
    public string description;


}
