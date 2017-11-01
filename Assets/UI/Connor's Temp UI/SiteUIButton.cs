using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


// Base class for UI buttons that work with Sites/Site elements.

[RequireComponent(typeof(Image))]
public class SiteBaseButton : MonoBehaviour {

    // Get the size of this button.
    public Vector2 buttonSize
    {
        get
        {
            float width = GetComponent<RectTransform>().rect.width;
            float height = GetComponent<RectTransform>().rect.height;

            return new Vector2(width, height);

        }
    }

    // Set the color of this button.
    public void SetButtonColor(Color buttonColor)
    {
        GetComponent<Image>().color = buttonColor;
    }

    // Move the button left/right.
    public void MoveButtonHorizontally(float delta)
    {

        this.transform.Translate(new Vector3(delta, 0.0f, 0.0f));

    }

}
