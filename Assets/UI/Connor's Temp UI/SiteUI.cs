using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// UI Class for selecting sites and their data types.
public class SiteUI : MonoBehaviour {

    // Site manager. Must be dragged in.
    public SiteManager siteManager;

    // Button prefab. Must be dragged in.
    public Object buttonPrefab;

    // Colors for selected/unselected buttons.
    public Color buttonActiveColor = Color.green;
    public Color buttonInactiveColor = Color.black;
    public Color buttonTextColor = Color.red;
    public int buttonTextSize = 3;

    // Where buttons should start creating on the UI.
    public Vector2 siteButtonStartPos = Vector2.zero;

    // Space button buttons, vertically/horizontally.
    public float horizontalBuffer = 0.5f;
    public float verticalBuffer = 0.5f;

    // Lists of the active buttons.
    private List<SiteButton> siteButtons;
    private List<SiteElementButton> siteElementButtons;

    // Active indices for the selected buttons.
    private int selectedSiteIndex = 0;
    private int selectedElementIndex = -1;

	// Use this for initialization
	void Start ()
    {
        // Create the buttons as soon as the game starts.
        CreateButtons();

	}
	
	// Update is called once per frame
	void Update ()
    {
		
        // If the A button is pressed, select the active button.
        if (GamepadInput.GetDown(InputOption.A_BUTTON))
        {

            // If there's no selected element, we first must choose a site. Otherwise, select the data type.
            if (selectedElementIndex < 0)
            {
                SelectSiteButton(siteButtons[selectedSiteIndex]);
            }
            else
            {
                SelectSiteSetButton(siteElementButtons[selectedElementIndex]);
            }
        }

        // If B is pressed, go back and deselect the active element.
        if (GamepadInput.GetDown(InputOption.B_BUTTON))
        {
            ClearElementButtons();
        }

        // If the right stick is pushed, move buttons left or right.
        if (GamepadInput.GetDown(InputOption.LEFT_STICK_HORIZONTAL))
        {
            // The direction of the stick. Helps us determine what direction it was pushed.
            float stickValue = GamepadInput.GetInputValue(InputOption.LEFT_STICK_HORIZONTAL);

            // If pushed to the right, move selected button to the right.
            if (stickValue > 0)
            {
                // If there's no selected element, move the site buttons. Otherwise move the data type buttons.
                if (selectedElementIndex < 0)
                {
                    MoveSiteButtons(1);
                }
                else
                {
                    MoveElementButtons(1);
                }
            }

            // If pushed to the left, move selected button to the left.
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
    
    // Instantiate buttons on the UI.
    public void CreateButtons()
    {
        // Set up lists to hold sites.
        siteButtons = new List<SiteButton>();

        // Get all the sites from the site manager.
        List<Site> allSites = siteManager.sites;

        // Create a button for each site.
        for(int i = 0; i < allSites.Count; i++)
        {

            // The current site.
            Site site = allSites[i];

            // Instantiate the button from prefab, and add the SiteButton component. Name it the site name.
            SiteButton newButton = (GameObject.Instantiate(buttonPrefab) as GameObject).AddComponent<SiteButton>();
            newButton.gameObject.name = site.siteName;
            newButton.GetComponentInChildren<Text>().color = buttonTextColor;
            newButton.GetComponentInChildren<Text>().fontSize = buttonTextSize;
            newButton.GetComponentInChildren<RectTransform>().sizeDelta = new Vector2(25, 35);
           

            // Set the associated site.
            newButton.SetSite(site);

            // Determine the x position this button should go.
            float newXPos = siteButtonStartPos.x + (i * (newButton.buttonSize.x + horizontalBuffer));

            // Set the parent and position of the button.
            newButton.transform.SetParent(this.transform);
            newButton.transform.localPosition = new Vector3(newXPos, siteButtonStartPos.y, 0.0f);

            // Add this button to the list.
            siteButtons.Add(newButton);

            // Color the button unselected.
            newButton.SetButtonColor(buttonInactiveColor);


        }

        // If there are any buttons, select the first one (show a highlight).
        if (siteButtons.Count > 0)
        {
            siteButtons[selectedSiteIndex].SetButtonColor(buttonActiveColor);
        }
    }

    // Select a site button (i.e. "Luxor", "Mar Saba", etc.)
    public void SelectSiteButton(SiteButton siteButton)
    {
        // Initialize this list.
        siteElementButtons = new List<SiteElementButton>();

        // Get all the data types associated with that site.
        List<SiteElementSet> dataSets = siteButton.associatedSite.dataSets;

        siteButton.associatedSite.associatedPOI.SetSelected(true);

        // Create a button for each data type.
        for (int i = 0; i < dataSets.Count; i++)
        {
            
            // This specific data set.
            SiteElementSet dataSet = dataSets[i];

            // Create the new button from component. Add the SiteElementButton script and give it a name.
            SiteElementButton newButton = (GameObject.Instantiate(buttonPrefab) as GameObject).AddComponent<SiteElementButton>();
            newButton.gameObject.name = dataSet.setType;

            // Be sure to set the data.
            newButton.SetData(dataSet);

            // Determine this button's x and y position.
            float newXPos = i * (newButton.buttonSize.x + horizontalBuffer);
            float newYPos = -siteButton.buttonSize.y - verticalBuffer;

            // Set the parent and position of the button.
            newButton.transform.SetParent(siteButton.transform);
            newButton.transform.localPosition = new Vector3(newXPos, newYPos, 0.0f);

            // Add this button to the list.
            siteElementButtons.Add(newButton);

            // Set the button color to inactive.
            newButton.SetButtonColor(buttonInactiveColor);

        }

        // If there are any data types, highlight the first one.
        if (dataSets.Count > 0)
        {
            selectedElementIndex = 0;
            siteElementButtons[selectedElementIndex].SetButtonColor(buttonActiveColor);
        }


    }

    // Select a data type. Just call the activate function to load that data.
    public void SelectSiteSetButton(SiteElementButton siteElementButton)
    {
        siteElementButton.associatedElementSet.NextElement();
    }

    // Move the site buttons in a direction. Direction should be -1 or 1
    public void MoveSiteButtons(int direction)
    {

        // Make sure the new selection is within bounds. 
        if (selectedSiteIndex + direction >= 0 && selectedSiteIndex + direction <= siteButtons.Count - 1)
        {
            
            // Set the currently highlighted button to inactive.
            siteButtons[selectedSiteIndex].SetButtonColor(buttonInactiveColor);

            // Shift all buttons over.
            foreach (SiteBaseButton button in siteButtons)
            {

                button.MoveButtonHorizontally(-direction * (button.buttonSize.x + horizontalBuffer));

            }

            // Increase the index by direction (will be +1 if direction is positive, -1 otherwise).
            selectedSiteIndex += direction;

            // Set the new button to highlight.
            siteButtons[selectedSiteIndex].SetButtonColor(buttonActiveColor);
            siteButtons[selectedSiteIndex].associatedSite.associatedPOI.SetSelected(true);

        }
    }

    // Does the same stuff as the above function, but for element buttons. Could probably be the same function.
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

    // Clears all data type buttons for a site. Called when user presses "B" to go back to site selection.
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
