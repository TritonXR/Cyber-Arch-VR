using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ViveLaser : MonoBehaviour
{
    public Color color;
    public float thickness = 0.002f;
    public GameObject holder;
    public GameObject pointer;

    private bool isColliding = false;
    private bool isPressed = false;

    private GameObject previousButton;
    private GameObject previousSiteContact;
    private GameObject previousElementContact;
    private GameObject selectedSiteButton;
    private GameObject selectedElementButton;

    public SiteUI siteUI;

	// Use this for initialization
	void Start ()
    {
        if (holder == null)
        {
            holder = new GameObject();
        }
        
        holder.transform.parent = this.transform;
        holder.transform.localPosition = Vector3.zero;
		holder.transform.localRotation = Quaternion.identity;

        if (pointer == null)
        {
            pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
        }
		
        pointer.transform.parent = holder.transform;
        pointer.transform.localScale = new Vector3(thickness, thickness, 100f);
        pointer.transform.localPosition = new Vector3(0f, 0f, 50f);
		pointer.transform.localRotation = Quaternion.identity;
		BoxCollider collider = pointer.GetComponent<BoxCollider>();
        if (collider)
        {
            Object.Destroy(collider);
        }

        Material newMaterial = new Material(Shader.Find("Unlit/Color"));
        newMaterial.SetColor("_Color", color);
        pointer.GetComponent<MeshRenderer>().material = newMaterial;
	}


    // Update is called once per frame
	void Update ()
    {

        SteamVR_TrackedController controller = GetComponent<SteamVR_TrackedController>();

        Ray raycast = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        int layerMask = LayerMask.GetMask("UI");
        bool bHit = Physics.Raycast(raycast, out hit, Mathf.Infinity,layerMask);

        SiteButton previousCollidingSite;
        SiteElementButton previousCollidingElement;

        if (bHit)
        {
            GameObject collidingObject = hit.collider.gameObject;
            SiteButton collidingSite = collidingObject.GetComponent<SiteButton>();
            SiteElementButton collidingElement = collidingObject.GetComponent<SiteElementButton>();
            

            if (collidingObject != selectedSiteButton && collidingObject != selectedElementButton) //if the user pointed at a non-selected button, then turn that button blue and deselect a previous button
            {
                if (collidingSite)
                {
                    collidingSite.SetButtonColor(Color.blue);
                }
                else if (collidingElement)
                {
                    collidingElement.SetButtonColor(Color.blue);
                }


                if (previousButton && previousButton != collidingObject)
                {
                    previousCollidingSite = previousButton.GetComponent<SiteButton>();
                    previousCollidingElement = previousButton.GetComponent<SiteElementButton>();

                    if (previousCollidingSite)
                    {
                        previousCollidingSite.SetButtonColor(siteUI.buttonInactiveColor);
                    }
                    else if (previousCollidingElement)
                    {
                        previousCollidingElement.SetButtonColor(siteUI.buttonInactiveColor);
                    }
                }
               
                previousButton = collidingObject;

            }

            //if user was not pressing and now the trigger is pressed
            if (!isPressed && controller.triggerPressed)
            {
                isPressed = true;
                if (collidingSite)
                {
                    collidingSite.SetButtonColor(siteUI.buttonActiveColor);
                } else if (collidingElement)
                {
                    collidingElement.SetButtonColor(siteUI.buttonActiveColor);
                }

                int index;
                if (collidingSite)
                {
                    selectedSiteButton = collidingObject;
                    index = collidingSite.siteIndex;
                    siteUI.LoopToSiteButton(index);
                    siteUI.SelectSiteButton(collidingSite);

                    /*
                    if (selectedElementButton)
                    {
                        selectedElementButton.GetComponent<Image>().color = siteUI.buttonInactiveColor;
                        selectedElementButton = null;
                        siteUI.ClearElementButtons();
                    }
                    */

                }/*
                else if (collidedElement)
                {
                    selectedElementButton = collidingObject;
                    index = collidedElement.elementIndex;
                    siteUI.LoopToElementButton(index);
                    //siteUI.SelectSiteSetButton(collidedElement);
                }*/

            }
            else if (isPressed && !controller.triggerPressed) //if user was pressing and now trigger is not pressed
            {
                isPressed = false;
            }



        } else
        {
            if (previousButton)
            {
                previousCollidingSite = previousButton.GetComponent<SiteButton>();
                previousCollidingElement = previousButton.GetComponent<SiteElementButton>();

                if (previousCollidingSite)
                {
                    previousCollidingSite.SetButtonColor(siteUI.buttonInactiveColor);
                }
                else if (previousCollidingElement)
                {
                    previousCollidingElement.SetButtonColor(siteUI.buttonInactiveColor);
                }
            }

            previousButton = null;
        }








            /*
            if (previousSiteContact != null && previousElementContact != null) //if the user pointed at a site and element button earlier...
            {
                if (previousSiteContact != selectedSiteButton && previousElementContact != selectedElementButton) //and that button earlier was not a button they already selected
                {
                    Debug.Log("Block A");

                    //unhighlight the earlier button and highlight the new button being pointed at
                    previousSiteContact.GetComponent<Image>().color = siteUI.buttonInactiveColor;
                    previousElementContact.GetComponent<Image>().color = siteUI.buttonInactiveColor;

                    if (collidingObject != selectedSiteButton && collidingObject != selectedElementButton) //as long as it is not the button that is being selected
                    {
                        collidingObject.GetComponent<Image>().color = Color.blue;
                    }
                }
                else if (previousSiteContact == selectedSiteButton || previousElementContact == selectedElementButton)
                {
                    if (selectedSiteButton)
                    {
                        Debug.Log("Block B with " + previousSiteContact.name + " equals selected " + selectedSiteButton.name);
                    }
                    else if (selectedElementButton)
                    {
                        Debug.Log("Block B with " + previousElementContact.name + " equals selected " + selectedElementButton.name);
                    }
                    if (collidingObject != selectedSiteButton && collidingObject != selectedElementButton)
                    {
                        Debug.Log("Changing colliding " + collidingObject.name + " to blue");
                        collidingObject.GetComponent<Image>().color = Color.blue;
                    }
                }
            } else if (previousSiteContact != null && previousElementContact == null) //user is pointing between a site and another site
            {
                Debug.Log("Block D");
                //unhighlight the earlier button and highlight the new button being pointed at
                previousSiteContact.GetComponent<Image>().color = siteUI.buttonInactiveColor;
                if (collidingObject != selectedSiteButton && collidingObject != selectedElementButton) //as long as it is not the button that is being selected
                {
                    collidingObject.GetComponent<Image>().color = Color.blue;
                }
            } else if (previousElementContact != null && previousSiteContact == null) //user is pointing between an element and another element
            {
                Debug.Log("Block E");
                //unhighlight the earlier button and highlight the new button being pointed at
                previousElementContact.GetComponent<Image>().color = siteUI.buttonInactiveColor;
                if (collidingObject != selectedSiteButton && collidingObject != selectedElementButton) //as long as it is not the button that is being selected
                {
                    collidingObject.GetComponent<Image>().color = Color.blue;
                }
            } else //if the user didn't point at a site earlier...
            {
                Debug.Log("Block C");
                if (collidingObject != selectedSiteButton && collidingObject != selectedElementButton) //as long as it is not the button that is being selected
                {
                    collidingObject.GetComponent<Image>().color = Color.blue;
                }
            }
            */


            /*
            if (previousContact != null && previousContact != selectedButton)
            {
                previousContact.GetComponent<Image>().color = siteUI.buttonInactiveColor;
                if (collidingObject != selectedButton)
                {
                    collidingObject.GetComponent<Image>().color = Color.blue;
                }
            } else if (previousContact != null && previousContact == selectedButton)
            {
                if (collidingObject != selectedButton)
                {
                    collidingObject.GetComponent<Image>().color = Color.blue;
                }
            }
            */

            /*
            SiteButton collidedSite = collidingObject.GetComponent<SiteButton>();
            SiteElementButton collidedElement = collidingObject.GetComponent<SiteElementButton>();

            if (collidedSite)
            {
                previousSiteContact = collidingObject;
            } else if (collidedElement)
            {
                previousElementContact = collidingObject;
            }

            //if user was not pressing and now the trigger is pressed
            if (!isPressed && controller.triggerPressed)
            {
                isPressed = true;
                collidingObject.GetComponent<Image>().color = siteUI.buttonActiveColor;

                int index;
                if (collidedSite)
                {
                    selectedSiteButton = collidingObject;
                    index = collidedSite.siteIndex;
                    siteUI.LoopToSiteButton(index);
                    siteUI.SelectSiteButton(collidedSite);

                    if (selectedElementButton)
                    {
                        selectedElementButton.GetComponent<Image>().color = siteUI.buttonInactiveColor;
                        selectedElementButton = null;
                        siteUI.ClearElementButtons();
                    }
                    
                } else if (collidedElement)
                {
                    selectedElementButton = collidingObject;
                    index = collidedElement.elementIndex;
                    siteUI.LoopToElementButton(index);
                    //siteUI.SelectSiteSetButton(collidedElement);
                }
 
            } 
            else if (isPressed && !controller.triggerPressed) //if user was pressing and now trigger is not pressed
            {
                isPressed = false;
            }

        } else //if the user is not pointing at a button
        {
            if (previousSiteContact != null || previousElementContact != null) {
                if (previousSiteContact != selectedSiteButton)
                {
                    previousSiteContact.GetComponent<Image>().color = siteUI.buttonInactiveColor;
                }
                if (previousElementContact != selectedElementButton)
                {
                    previousElementContact.GetComponent<Image>().color = siteUI.buttonInactiveColor;
                }
            }

            if (!isPressed && controller.triggerPressed)
            {
                isPressed = true;
                if (selectedSiteButton)
                {
                    selectedSiteButton.GetComponent<Image>().color = siteUI.buttonInactiveColor;
                    selectedSiteButton = null;
                }
                if (selectedElementButton)
                {
                    selectedElementButton.GetComponent<Image>().color = siteUI.buttonInactiveColor;
                    selectedElementButton = null;
                }
                siteUI.ClearElementButtons();

            } else if (isPressed && !controller.triggerPressed)
            {
                isPressed = false;
            }
        }
        */
        
    }
}
