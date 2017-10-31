using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiteUI : MonoBehaviour {

    public SiteManager siteManager;

    public Object buttonPrefab;

    public Color buttonActiveColor = Color.green;
    public Color buttonInactiveColor = Color.grey;

    public Vector2 siteButtonStartPos = Vector2.zero;

    public float horizontalBuffer = 0.5f;
    public float verticalBuffer = 0.5f;

    private List<SiteButton> siteButtons;
    private List<SiteElementButton> siteElementButtons;

    private int selectedSiteIndex = 0;
    private int selectedElementIndex = -1;

	// Use this for initialization
	void Start ()
    {

        CreateButtons();

	}
	
	// Update is called once per frame
	void Update ()
    {
		
        if (GamepadInput.GetDown(InputOption.A_BUTTON))
        {

            if (selectedElementIndex < 0)
            {
                SelectSiteButton(siteButtons[selectedSiteIndex]);
            }
            else
            {
                SelectSiteSetButton(siteElementButtons[selectedElementIndex]);
            }
        }

        if (GamepadInput.GetDown(InputOption.B_BUTTON))
        {
            ClearElementButtons();
        }


        if (GamepadInput.GetDown(InputOption.RIGHT_STICK_HORIZONTAL))
        {
            float stickValue = GamepadInput.GetInputValue(InputOption.RIGHT_STICK_HORIZONTAL);

            if (stickValue > 0)
            {
                if (selectedElementIndex < 0)
                {
                    MoveSiteButtons(1);
                }
                else
                {
                    MoveElementButtons(1);
                }
            }
            else if (stickValue < 0)
            {
                if (selectedElementIndex < 0)
                {
                    MoveSiteButtons(-1);
                }
                else
                {
                    MoveElementButtons(-1);
                }
            }
        }
	}

    public void CreateButtons()
    {

        siteButtons = new List<SiteButton>();
        siteElementButtons = new List<SiteElementButton>();

        List<Site> allSites = siteManager.sites;

        for(int i = 0; i < allSites.Count; i++)
        {

            Site site = allSites[i];

            SiteButton newButton = (GameObject.Instantiate(buttonPrefab) as GameObject).AddComponent<SiteButton>();
            newButton.gameObject.name = site.siteName;
            newButton.SetSite(site);

            float newXPos = siteButtonStartPos.x + (i * (newButton.buttonSize.x + horizontalBuffer));

            newButton.transform.SetParent(this.transform);
            newButton.transform.localPosition = new Vector3(newXPos, siteButtonStartPos.y, 0.0f);

            siteButtons.Add(newButton);

            newButton.SetButtonColor(buttonInactiveColor);


        }

        if (siteButtons.Count > 0)
        {
            siteButtons[selectedSiteIndex].SetButtonColor(buttonActiveColor);

        }
    }

    public void SelectSiteButton(SiteButton siteButton)
    {

        List<SiteElementSet> dataSets = siteButton.associatedSite.dataSets;

        for (int i = 0; i < dataSets.Count; i++)
        {

            SiteElementSet dataSet = dataSets[i];

            SiteElementButton newButton = (GameObject.Instantiate(buttonPrefab) as GameObject).AddComponent<SiteElementButton>();
            newButton.gameObject.name = dataSet.setType;
            newButton.SetData(dataSet);

            float newXPos = i * (newButton.buttonSize.x + horizontalBuffer);
            float newYPos = -siteButton.buttonSize.y - verticalBuffer;

            newButton.transform.SetParent(siteButton.transform);
            newButton.transform.localPosition = new Vector3(newXPos, newYPos, 0.0f);

            siteElementButtons.Add(newButton);

            newButton.SetButtonColor(buttonInactiveColor);

        }

        if (dataSets.Count > 0)
        {
            selectedElementIndex = 0;
            siteElementButtons[selectedElementIndex].SetButtonColor(buttonActiveColor);
        }


    }

    public void SelectSiteSetButton(SiteElementButton siteElementButton)
    {

        siteElementButton.associatedElementSet.Activate();

    }

    public void MoveSiteButtons(int direction)
    {

        if (selectedSiteIndex + direction >= 0 && selectedSiteIndex + direction <= siteButtons.Count - 1)
        {

            siteButtons[selectedSiteIndex].SetButtonColor(buttonInactiveColor);

            foreach (SiteBaseButton button in siteButtons)
            {

                button.MoveButtonHorizontally(-direction * (button.buttonSize.x + horizontalBuffer));

            }

            selectedSiteIndex += direction;
            siteButtons[selectedSiteIndex].SetButtonColor(buttonActiveColor);

        }
    }

    public void MoveElementButtons(int direction)
    {

        if (selectedElementIndex + direction >= 0 && selectedElementIndex + direction <= siteElementButtons.Count - 1)
        {

            siteElementButtons[selectedElementIndex].SetButtonColor(buttonInactiveColor);

            foreach (SiteBaseButton button in siteElementButtons)
            {

                button.MoveButtonHorizontally(-direction * (button.buttonSize.x + horizontalBuffer));

            }

            selectedElementIndex += direction;
            siteElementButtons[selectedSiteIndex].SetButtonColor(buttonActiveColor);

        }
    }

    public void ClearElementButtons()
    {
        foreach (SiteElementButton button in siteElementButtons)
        {

            GameObject.Destroy(button.gameObject);

        }

        siteElementButtons.Clear();
        selectedElementIndex = -1;

    }
}
