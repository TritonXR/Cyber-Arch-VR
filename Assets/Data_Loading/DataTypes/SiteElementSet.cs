using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SiteElementSet : MonoBehaviour
{

    Site parentSite;

    public List<SiteElement> siteElements;
    protected int activeElementIndex = 0;

    public void Initialize(SerializableSiteElement[] serializableSiteElements, Site parentSite)
    {

        this.parentSite = parentSite;

        foreach (SerializableSiteElement element in serializableSiteElements)
        {

            GameObject newElementObj = CreateElementObject(element.name);
            SiteElement newElement = AddElementComponent(newElementObj);

            newElement.Initialize(element, parentSite);

            siteElements.Add(newElement);

        }
    }

    protected abstract SiteElement AddElementComponent (GameObject elementObject);

    public GameObject CreateElementObject(string name)
    {

        GameObject newElement = new GameObject(name);
        newElement.transform.SetParent(this.transform);
        newElement.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

        return newElement;

    }

    public Coroutine NextElement()
    {

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

            yield return siteElements[activeElementIndex].Deactivate();

            activeElementIndex += direction;

            if (activeElementIndex >= siteElements.Count)
            {
                activeElementIndex = 0;
            }
            else if (activeElementIndex < 0)
            {
                activeElementIndex = siteElements.Count - 1;
            }

            yield return siteElements[activeElementIndex].Activate();
        }

    }
}