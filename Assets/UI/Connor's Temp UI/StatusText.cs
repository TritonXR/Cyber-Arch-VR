using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusText : MonoBehaviour {

    private static Text statusText;
    private static GameObject parentCanvas;
    private static Vector3 positionFromCameraToCanvas;

    public void Awake()
    {
        if (statusText == null)
        {
            parentCanvas = GetComponentInParent<Canvas>().gameObject;
            positionFromCameraToCanvas = parentCanvas.transform.position - Camera.main.transform.position;

            statusText = GetComponent<Text>();
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(parentCanvas);
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
            StatusText.Show();
            statusText.text = text;

        }
    }

    public static void Show()
    {
        if (statusText != null)
        {
            UpdateCanvasPosition();
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

    public static void UpdateCanvasPosition()
    {

        parentCanvas.transform.position = Camera.main.transform.position + positionFromCameraToCanvas;

    }

    public void Update()
    {
        if (parentCanvas.transform.position != Camera.main.transform.position + positionFromCameraToCanvas)
        {
            UpdateCanvasPosition();
        }
    }
}
