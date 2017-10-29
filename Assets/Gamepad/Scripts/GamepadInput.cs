using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InputOption
{
    LEFT_STICK_HORIZONTAL,
    RIGHT_STICK_HORIZONTAL,
    LEFT_STICK_VERTICAL,
    RIGHT_STICK_VERTICAL,
    A_BUTTON,
    B_BUTTON,
    LEFT_TRIGGER,
    RIGHT_TRIGGER,
    START_BUTTON,
    BACK_BUTTON
}

public class GamepadInput : MonoBehaviour {

    [SerializeField]
    private bool debug;

    public static float timeSinceLastInput = 0.0f;

    private static Dictionary<InputOption, string> inputOptions = new Dictionary<InputOption, string>() {

        { InputOption.LEFT_STICK_HORIZONTAL,  "Horizontal"           },
        { InputOption.RIGHT_STICK_HORIZONTAL, "RightStickHorizontal" },
        { InputOption.LEFT_STICK_VERTICAL,    "Vertical"             },
        { InputOption.RIGHT_STICK_VERTICAL,   "RightStickVertical"   },
        { InputOption.A_BUTTON,               "Xbox A"               },
        { InputOption.B_BUTTON,               "Xbox B"               },
        { InputOption.LEFT_TRIGGER,           "LeftTrigger"          },
        { InputOption.RIGHT_TRIGGER,          "RightTrigger"         },
        { InputOption.START_BUTTON,           "Xbox Start"           },
        { InputOption.BACK_BUTTON,            "Xbox Back"            }

    };

    public static List<InputOption> downInputs;
    public static List<InputOption> heldInputs;
    public static List<InputOption> releasedInputs;

    public static bool GetDown(InputOption option)
    {

        return downInputs.Contains(option);

    }

    public static bool Get(InputOption option)
    {

        return heldInputs.Contains(option);

    }

    public static bool GetUp(InputOption option)
    {

        return releasedInputs.Contains(option);

    }

    public static float GetInputValue(InputOption option)
    {

        return Input.GetAxis(inputOptions[option]);

    }

    private void Awake()
    {


        downInputs = new List<InputOption>();
        heldInputs = new List<InputOption>();
        releasedInputs = new List<InputOption>();

    }

    private void Start()
    {


    }

    private void Update()
    {

        UpdateInputStatus();

        foreach (InputOption key in inputOptions.Keys)
        {

            string value = inputOptions[key];

            if (Input.GetAxis(value) == 0)
            {

                InputRelease(key);

            }

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

    private void UpdateInputStatus()
    {

        while (downInputs.Count > 0)
        {
            InputHold(downInputs[0]);
        }

        releasedInputs.Clear();

    }
}
