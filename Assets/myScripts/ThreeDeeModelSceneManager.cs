using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ThreeDeeModelSceneManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    void Update()
    {

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

        if (GamepadInput.Get(InputOption.LEFT_STICK_HORIZONTAL))
        {
            float stickValue = GamepadInput.GetInputValue(InputOption.LEFT_STICK_HORIZONTAL);

            if (stickValue > 0)
            {
                Debug.Log("Left analog stick pushed to the right!");
            }
            else if (stickValue < 0)
            {
                Debug.Log("Left analog stick pushed to the left!");
            }

        }

    }
}
