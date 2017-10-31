using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SiteButton : SiteBaseButton {

    public Site associatedSite;

    public void SetSite(Site site)
    {

        associatedSite = site;
        GetComponentInChildren<Text>().text = site.siteName;

    }


}
