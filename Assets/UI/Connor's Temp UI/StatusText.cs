using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Status text UI that the user can see when updated.
public class StatusText : MonoBehaviour {

    // Make this object static so the status text can be set from anywhere.
    private static Text statusText;
    private static GameObject parentCanvas;

    // Runs first, used here to set Singleton
    public void Awake()
    {
        // Set the status text object and ensure it doesn't destroy on load.
        if (statusText == null)
        {
            parentCanvas = GetComponentInParent<Canvas>().gameObject;

            statusText = GetComponent<Text>();
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(parentCanvas);
        }

        // If there's already a status text, destroy it.
        else
        {
            GameObject.Destroy(this.gameObject);

        }

    }

    // Hide the Text by default.
    public void Start()
    {
        Hide();

    }

    // Set the text that will display.
    public static void SetText(string text)
    {
        if (statusText != null)
        {
            // Show the text if it exists.
            StatusText.Show();
            statusText.text = text;

        }
    }

    // Shows the text to the user.
    public static void Show()
    {
        if (statusText != null)
        {
            // Just activate the GameObject.
            statusText.gameObject.SetActive(true);

        }
    }

    // Hide the text so that the user can't see it.
    public static void Hide()
    {
        if (statusText != null)
        {
            // Just set the object as inactive.
            statusText.gameObject.SetActive(false);
        }
    }
}
