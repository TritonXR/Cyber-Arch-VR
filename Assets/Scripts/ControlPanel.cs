using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlPanel : MonoBehaviour {

    public static Sprite homePanel;
    public static Sprite caveCamPanel;
    public Sprite homePanelImage;
    public Sprite caveCamPanelImage;
    static Image imageComponent;

	// Use this for initialization
	void Start () {
      imageComponent = this.GetComponent<Image>();

        homePanel = homePanelImage;
        caveCamPanel = caveCamPanelImage;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public static void SetHomePanel()
    {
        imageComponent.sprite = homePanel;
    }

    public static void SetCaveCamPanel()
    {
        imageComponent.sprite = caveCamPanel;
    }

    
}
