using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// All the different supported Xbox controls.
public enum InputOption
{
    LEFT_STICK_HORIZONTAL,
    RIGHT_STICK_HORIZONTAL,
    LEFT_STICK_VERTICAL,
    RIGHT_STICK_VERTICAL,
    A_BUTTON,
    B_BUTTON,
    X_BUTTON,
    Y_BUTTON,
    LEFT_TRIGGER,
    RIGHT_TRIGGER,
    START_BUTTON,
    BACK_BUTTON
}

// Class for input from an Xbox controller. 
public class GamepadInput : MonoBehaviour {

    // Enables/disables debug mode & debug print statements.
    [SerializeField] private bool debug = false;

    private static bool inputLocked = false;

    // Checks the time since there was last input.
    public static float timeSinceLastInput = 0.0f;

    // All the possible input options, mapped with an enum to the corresponding input string.
    //   NOTE: To add a new input, first create a new option in the enum, then...
    //   go to Edit -> Project Settings -> Input and set up the new input there. Then,
    //   add the exact string from that new input to the dictionary below, with the enum as a key. 
    private static Dictionary<InputOption, string> inputOptions = new Dictionary<InputOption, string>() {

        { InputOption.LEFT_STICK_HORIZONTAL,  "Horizontal"           },
        { InputOption.RIGHT_STICK_HORIZONTAL, "RightStickHorizontal" },
        { InputOption.LEFT_STICK_VERTICAL,    "Vertical"             },
        { InputOption.RIGHT_STICK_VERTICAL,   "RightStickVertical"   },
        { InputOption.A_BUTTON,               "Xbox A"               },
        { InputOption.B_BUTTON,               "Xbox B"               },
        { InputOption.X_BUTTON,               "Xbox X"               },
        { InputOption.Y_BUTTON,               "Xbox Y"               },
        { InputOption.LEFT_TRIGGER,           "LeftTrigger"          },
        { InputOption.RIGHT_TRIGGER,          "RightTrigger"         },
        { InputOption.START_BUTTON,           "Xbox Start"           },
        { InputOption.BACK_BUTTON,            "Xbox Back"            },
        
    };

    // Lists to keep track of active inputs.
    public static List<InputOption> downInputs;
    public static List<InputOption> heldInputs;
    public static List<InputOption> releasedInputs;

    /// <summary>
    /// Returns true if specified input was pressed down in this frame.
    /// </summary>
    /// <param name="option">The input option to check.</param>
    /// <returns>True if pressed down, false if not pressed down.</returns>
    public static bool GetDown(InputOption option)
    {

        if (inputLocked)
        {
            return false;
        }

        return downInputs.Contains(option);

    }

    /// <summary>
    /// Returns true if specified input is being held down this frame.
    /// </summary>
    /// <param name="option">The input option to check.</param>
    /// <returns>True if input is being held, false otherwise.</returns>
    public static bool Get(InputOption option)
    {

        if (inputLocked)
        {
            return false;
        }

        return heldInputs.Contains(option);
    }

    /// <summary>
    /// Returns true if specified input was released this frame.
    /// </summary>
    /// <param name="option">The input option to check.</param>
    /// <returns>True if input was released, false otherwise.</returns>
    public static bool GetUp(InputOption option)
    {

        if (inputLocked)
        {
            return false;
        }

        return releasedInputs.Contains(option);
    }

    /// <summary>
    /// Returns the value of the specified input. Non-zero means the input is active.
    /// This is especially useful for analog sticks, since the value determines which
    /// direction they're being held, and how far they're being held.
    /// </summary>
    /// <param name="option">The input option to check.</param>
    /// <returns>A float value representing the input value this frame. </returns>
    public static float GetInputValue(InputOption option)
    {

        if (inputLocked)
        {
            return 0.0f;
        }

        return Input.GetAxis(inputOptions[option]);
    }

    private void Awake()
    {
        downInputs = new List<InputOption>();
        heldInputs = new List<InputOption>();
        releasedInputs = new List<InputOption>();
    }

    private void Update()
    {
        // Update all the inputs to their correct positions.
        UpdateInputStatus();

        // Iterate through each option and check it's status this frame.
        foreach (InputOption key in inputOptions.Keys)
        {

            string value = inputOptions[key];

            // If the input isn't being held, try to release it.
            if (Input.GetAxis(value) == 0)
            {
                // Releases the option if the input was active.
                InputRelease(key);

            }

            // If the input is being used, activate it.
            if (Input.GetAxis(value) != 0)
            {
                InputDown(key);
            }
        }

        if (downInputs.Count == 0 && heldInputs.Count == 0)
        {
            timeSinceLastInput += Time.deltaTime;

            if (debug)
            {
                // Commenting this one out for now. It's a tad annoying.
                // Debug.LogFormat("Time since last input: {0}", timeSinceLastInput);
            }
        }

    }

    // Adds specified input to list of down inputs.
    // Only works if input is not already active.
    private void InputDown(InputOption input)
    {

        if (!heldInputs.Contains(input))
        {

            timeSinceLastInput = 0.0f;

            downInputs.Add(input);
            heldInputs.Add(input);

            if (debug)
            {
                Debug.LogFormat("Input {0} down", input.ToString());
            }

        }
    }

    // Adds specified input to list of held inputs.
    // Only works if input was in down inputs previous frame.
    private void InputHold(InputOption input)
    {
        if (downInputs.Contains(input))
        {

            downInputs.Remove(input);

            if (debug)
            {
                Debug.LogFormat("Input {0} hold", input.ToString());
            }

        }
    }

    // Adds specified input to list of released inputs.
    // Only works if input was being held last frame.
    private void InputRelease(InputOption input)
    {

        if (heldInputs.Contains(input))
        {
            heldInputs.Remove(input);
            releasedInputs.Add(input);


            if (debug)
            {
                Debug.LogFormat("Input {0} release", input.ToString());
            }
        }

    }

    // Updates the status of all inputs, making them held and clearing released inputs.
    private void UpdateInputStatus()
    {

        while (downInputs.Count > 0)
        {
            InputHold(downInputs[0]);
        }

        releasedInputs.Clear();

    }

    public static void LockInput(bool locked)
    {
        inputLocked = locked;
    }
}
