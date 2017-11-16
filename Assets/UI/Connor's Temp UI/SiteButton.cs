using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SiteButton : SiteBaseButton {

    // The site associated with this button
    public Site associatedSite;

    // Sets the associate site, and also sets the text of the button.
    public void SetSite(Site site)
    {

        // Store the site.
        associatedSite = site;

        // Be sure to set the button text.
        GetComponentInChildren<Text>().horizontalOverflow = HorizontalWrapMode.Wrap;

        GetComponentInChildren<Text>().text = site.siteName + "\n\n" + site.siteDescription;
        //GetComponentInChildren<Text>().rectTransform.
    }
}
