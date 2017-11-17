using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusText : MonoBehaviour {

    private static Text statusText;

    public void Awake()
    {
        if (statusText == null)
        {
            statusText = GetComponent<Text>();
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(transform.parent.gameObject);
            Hide();
        }
        else
        {
            GameObject.Destroy(this.gameObject);

        }

    }

    public static void SetText(string text)
    {
        if (statusText != null)
        {
            statusText.text = text;

        }
    }

    public static void Show()
    {
        if (statusText != null)
        {
            statusText.gameObject.SetActive(true);

        }
    }

    public static void Hide()
    {
        if (statusText != null)
        {
            statusText.gameObject.SetActive(false);
        }
    }
}
