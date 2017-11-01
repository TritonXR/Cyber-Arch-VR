using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

// Script for a specific data type button.

public class SiteElementButton : SiteBaseButton {

    // Associated site element.
    public SiteElementSet associatedElementSet;

    // Sets the associated data.
    public void SetData(SiteElementSet set)
    {
        associatedElementSet = set;

        GetComponentInChildren<Text>().text = set.setType;
    }
}
