using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour {

    private GameObject deltaSelected;

    // Use this for initialization
    void Start () {
		
	}

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(deltaSelected);
        }
    }

    void LateUpdate()
    {
        deltaSelected = EventSystem.current.currentSelectedGameObject;
    }

    public void Load3DModelScene()
    {
        SceneManager.LoadScene("3DModel_Scene");
    }

    public void Load360ImageScene()
    {
        SceneManager.LoadScene("360Image_Scene");
    }

    public void LoadVideoScene()
    {
        SceneManager.LoadScene("Video_Scene");
    }
}
