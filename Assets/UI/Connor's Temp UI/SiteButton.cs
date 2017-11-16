using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SiteButton : SiteBaseButton {

    // The site associated with this button
    public Site associatedSite;

    // The index of the site in the list of buttons
    public int siteIndex;

    // Sets the associate site, and also sets the text of the button.
    public void SetSite(Site site, int index)
    {

        // Store the site.
        associatedSite = site;

        // Be sure to set the button text.
        GetComponentInChildren<Text>().text = site.siteName;

        // Set the index of the button site
        siteIndex = index;

    }
}
