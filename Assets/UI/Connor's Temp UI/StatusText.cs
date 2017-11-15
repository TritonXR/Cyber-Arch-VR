using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusText : MonoBehaviour {

    private static Text statusText;

    public void Awake()
    {

        statusText = GetComponent<Text>();

    }

    public static void SetText(string text)
    {
        if (statusText != null)
        {
            statusText.text = text;

        }

    }

}
