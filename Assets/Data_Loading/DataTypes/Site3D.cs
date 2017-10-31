using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Site3D : SiteElement {

    protected override IEnumerator ActivateCoroutine()
    {
        throw new NotImplementedException();
    }

    protected override IEnumerator DeactivateCoroutine()
    {
        throw new NotImplementedException();
    }

    protected override IEnumerator LoadCoroutine()
    {
        throw new NotImplementedException();
    }

    protected override IEnumerator UnloadCoroutine()
    {
        throw new NotImplementedException();
    }
}

[System.Serializable]
public class Serializable3DSite : SerializableSiteElement
{
    public string filePath;
}
