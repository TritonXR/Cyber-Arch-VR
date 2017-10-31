using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(Image))]
public class SiteBaseButton : MonoBehaviour {


    public Vector2 buttonSize
    {
        get
        {
            float width = GetComponent<RectTransform>().rect.width;
            float height = GetComponent<RectTransform>().rect.height;

            return new Vector2(width, height);

        }
    }

    public void SetButtonColor(Color buttonColor)
    {
        GetComponent<Image>().color = buttonColor;
    }

    public void MoveButtonHorizontally(float delta)
    {

        this.transform.Translate(new Vector3(delta, 0.0f, 0.0f));

    }

}
