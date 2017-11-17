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

        siteUI = SiteUI.instance;

        CreateLaser();
	}

    void CreateLaser()
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

        if (!SiteUI.instance)
        {
            Destroy(holder);
            Destroy(pointer);
        }
        else
        {
            if (!siteUI)
            {
                siteUI = SiteUI.instance;
            }

            if (!holder && !pointer)
            {
                CreateLaser();
            }

            if (selectedElementButton)
            {
                siteUI.ClearElementButtons();
                selectedElementButton = null;
                previousButton = null;
                selectedSiteButton = null;
            }



            SteamVR_TrackedController controller = GetComponent<SteamVR_TrackedController>();

            Ray raycast = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            int layerMask = LayerMask.GetMask("UI");
            bool bHit = Physics.Raycast(raycast, out hit, Mathf.Infinity, layerMask);


            if (bHit)
            {
                GameObject collidingObject = hit.collider.gameObject;
                SiteButton collidingSite = collidingObject.GetComponent<SiteButton>();
                SiteElementButton collidingElement = collidingObject.GetComponent<SiteElementButton>();

                if (collidingObject != selectedSiteButton)
                {
                    if (collidingSite)
                    {
                        collidingSite.SetButtonColor(Color.blue);

                    }
                    else if (collidingElement)
                    {
                        collidingElement.SetButtonColor(Color.blue);
                    }

                }



                if (previousButton && previousButton != collidingObject && (previousButton != selectedSiteButton && previousButton != selectedElementButton))
                {
                    SiteButton previousCollidingSite = previousButton.GetComponent<SiteButton>();
                    SiteElementButton previousCollidingElement = previousButton.GetComponent<SiteElementButton>();

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

                //if user was not pressing and now the trigger is pressed
                if (!isPressed && controller.triggerPressed)
                {
                    isPressed = true;
                    if (collidingSite)
                    {
                        Debug.LogWarning("colliding site " + collidingSite.name + " is green");
                        collidingSite.SetButtonColor(siteUI.buttonActiveColor);
                    }

                    int index;
                    if (collidingSite)
                    {

                        if (selectedSiteButton != collidingObject)
                        {
                            siteUI.ClearElementButtons();
                            selectedSiteButton = collidingObject;
                            index = collidingSite.siteIndex;
                            siteUI.LoopToSiteButton(index);
                            siteUI.SelectSiteButton(collidingSite);
                        }
                        

                    }
                    else if (collidingElement)
                    {
                        /*
                        selectedElementButton = collidingObject;
                        index = collidedElement.elementIndex;
                        siteUI.LoopToElementButton(index);
                        */
                        selectedElementButton = collidingObject;
                        siteUI.SelectSiteSetButton(collidingElement);
                    }

                }
                else if (isPressed && !controller.triggerPressed) //if user was pressing and now trigger is not pressed
                {
                    isPressed = false;
                }



            }
            else
            {
                if (previousButton && (previousButton != selectedSiteButton && previousButton != selectedElementButton))
                {
                    SiteButton previousCollidingSite = previousButton.GetComponent<SiteButton>();
                    SiteElementButton previousCollidingElement = previousButton.GetComponent<SiteElementButton>();

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

        }
        

    }
}
