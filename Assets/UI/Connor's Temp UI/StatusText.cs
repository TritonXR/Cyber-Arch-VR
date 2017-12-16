using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusText : MonoBehaviour {

    private static Text statusText;
    private static GameObject parentCanvas;

    public void Awake()
    {
        if (statusText == null)
        {
            parentCanvas = GetComponentInParent<Canvas>().gameObject;

            statusText = GetComponent<Text>();
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(parentCanvas);
        }
        else
        {
            GameObject.Destroy(this.gameObject);

        }

    }

    public void Start()
    {
        Hide();

    }

    public static void SetText(string text)
    {
        if (statusText != null)
        {
            StatusText.Show();
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
