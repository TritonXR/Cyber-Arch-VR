﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SiteElementSet : MonoBehaviour
{
    private Site parentSite;

    public string setType = "Unknown Type";

    public List<SiteElement> siteElements;
    protected int activeElementIndex = -1;

    SiteElement activeElement = null;

    private bool activated = false;

    protected abstract SiteElement AddElementComponent(GameObject elementObject);
    protected abstract string GetSetType();

    public void Initialize(SerializableSiteElement[] serializableSiteElements, Site parentSite)
    {

        this.parentSite = parentSite;
        setType = GetSetType();


        Debug.Log("Initializing " + setType + " for site " + parentSite.siteName);

        siteElements = new List<SiteElement>();

        foreach (SerializableSiteElement element in serializableSiteElements)
        {

            GameObject newElementObj = CreateElementObject(element.name);
            SiteElement newElement = AddElementComponent(newElementObj);

            newElement.Initialize(element, parentSite);

            siteElements.Add(newElement);

        }
    }


    protected GameObject CreateElementObject(string name)
    {

        GameObject newElement = new GameObject(name);
        newElement.transform.SetParent(this.transform);
        newElement.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

        return newElement;

    }

    private Coroutine Activate()
    {

        if (!activated)
        {
            activated = true;
            return NextElement();
        }
        else
        {
            Debug.LogWarning("This site element has already been activated. Please deactivate before trying to activate again", this.gameObject);
        }

        return null;

    }

    public Coroutine Deactivate()
    {

        activeElementIndex = -1;
        activated = false;

        if (SiteManager.activeSiteElementSet == this)
        {
            SiteManager.activeSiteElementSet = null;
        }


        if (activeElement)
        {
            return activeElement.Deactivate();
        }


        return null;

    }

    public Coroutine NextElement()
    {
        Debug.Log("Selecting next element");
        return StartCoroutine(CycleElements(1));
      
    }

    public Coroutine PreviousElement()
    {
        return StartCoroutine(CycleElements(-1));
    }

    public bool IsMultipleElements()
    {

        if (siteElements.Count > 1)
        {
            return true;
        }

        return false;

    }

    private IEnumerator CycleElements(int direction)
    {

        if (IsMultipleElements())
        {

            if (activeElement != null)
            {
                yield return activeElement.Deactivate();
                activeElement = null;
            }

            activeElementIndex += direction;

            if (activeElementIndex >= siteElements.Count)
            {
                activeElementIndex = 0;
            }
            else if (activeElementIndex < 0)
            {
                activeElementIndex = siteElements.Count - 1;
            }

            activeElement = siteElements[activeElementIndex];

        }
        else
        {
            activeElement = siteElements[0];
        }

        yield return activeElement.Activate();
        SiteManager.activeSiteElementSet = this;

    }
}