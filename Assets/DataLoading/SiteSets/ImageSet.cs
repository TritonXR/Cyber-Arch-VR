using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageSet : SiteElementSet
{

    public const string elementString = "Pictures";

    protected override SiteElement AddElementComponent(GameObject elementObject)
    {
        Picture newElement = elementObject.AddComponent<Picture>();
        return newElement;
    }

    protected override string GetSetType()
    {
        return elementString;
    }
}
