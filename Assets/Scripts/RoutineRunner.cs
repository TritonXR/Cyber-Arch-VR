using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoutineRunner : MonoBehaviour {

    public static RoutineRunner instance;

    private void Awake()
    {

        if (instance != null)
        {
            GameObject.Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
            
    }
}
