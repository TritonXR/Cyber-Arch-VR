using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleGamepadInput : MonoBehaviour {

	
	// Update is called once per frame
	void Update ()
    {

        CheckForInput();

	}

    public void CheckForInput()
    {

        if (GamepadInput.GetDown(InputOption.A_BUTTON))
        {
            Debug.Log("A button was pressed down!");
        }

        if (GamepadInput.Get(InputOption.A_BUTTON))
        {
            Debug.Log("A button is being held down!");
        }

        if (GamepadInput.GetUp(InputOption.A_BUTTON))
        {
            Debug.Log("A button was released!");
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

        if (GamepadInput.Get(InputOption.RIGHT_STICK_VERTICAL))
        {
            float stickValue = GamepadInput.GetInputValue(InputOption.RIGHT_STICK_VERTICAL);

            if (stickValue > 0)
            {
                Debug.Log("Right stick pushed up!");
            }
            else if (stickValue < 0)
            {
                Debug.Log("Right stick pushed down!");
            }
        }
    }
}
