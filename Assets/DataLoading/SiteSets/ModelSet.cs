using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelSet : SiteElementSet {

    public const string elementString = "3D Models";

    protected override SiteElement AddElementComponent(GameObject elementObject)
    {
        Model newElement = elementObject.AddComponent<Model>();
        return newElement;
    }

    protected override string GetSetType()
    {
        return elementString;
    }
}
