using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SiteButton : SiteBaseButton {

    // The site associated with this button
    public Site associatedSite;

    // The index of the site in the list of buttons
    public int siteIndex;
    public Font latoBlack;

    // Sets the associate site, and also sets the text of the button.
    public void SetSite(Site site, int index)
    {

        // Store the site.
        associatedSite = site;

        // Be sure to set the button text.
        GetComponentInChildren<Text>().horizontalOverflow = HorizontalWrapMode.Wrap;

        GetComponentInChildren<Text>().text = site.siteName;// + "\n\n" + site.siteDescription;
        GetComponentInChildren<Text>().alignment = TextAnchor.UpperCenter;
        GetComponentInChildren<Text>().rectTransform.Translate(0f, -2.5f, 0f);
        //GetComponentInChildren<Text>().rectTransform.offsetMin = new Vector2(0f,1f);
        //GetComponentInChildren<Text>().rectTransform.offsetMax = new Vector2(0f,0f);

        // Set the index of the button site
        siteIndex = index;

    }

    public void SetDescription(Site site, Font font)
    {
        GameObject siteButtonDesc = new GameObject();
        siteButtonDesc.transform.parent = this.transform;
        siteButtonDesc.AddComponent<Text>();
        siteButtonDesc.GetComponent<Text>().text = site.siteDescription;
        siteButtonDesc.transform.localPosition = new Vector3(0, -4, 0);
        siteButtonDesc.GetComponent<Text>().font = font;
        siteButtonDesc.GetComponent<Text>().fontSize = 3;
        siteButtonDesc.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Wrap;
        siteButtonDesc.GetComponent<RectTransform>().sizeDelta = new Vector2(29, 28);
        siteButtonDesc.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
    }
            
}
