using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanoSet : SiteElementSet
{
    protected override SiteElement AddElementComponent(GameObject elementObject)
    {
        CAVECam newPano = elementObject.AddComponent<CAVECam>();
        return newPano;
    }
}
