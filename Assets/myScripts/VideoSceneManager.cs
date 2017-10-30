using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VideoSceneManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update() {

        CheckForInput();

    }

    public void CheckForInput()
    {
        if (GamepadInput.GetDown(InputOption.B_BUTTON))
        {
            Debug.Log("B button was pressed down! Going back one level.");

            SceneManager.LoadScene("Main");
        }

        if (GamepadInput.GetDown(InputOption.START_BUTTON))
        {
            Debug.Log("The start button was pressed down! Going back to Main Menu");

            SceneManager.LoadScene("Main");
        }
    }
}
