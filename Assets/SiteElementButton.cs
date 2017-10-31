using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SiteElementButton : SiteBaseButton {

    public SiteElementSet associatedElementSet;

    public void SetData(SiteElementSet set)
    {
        associatedElementSet = set;

        GetComponentInChildren<Text>().text = set.setType;
    }
}
