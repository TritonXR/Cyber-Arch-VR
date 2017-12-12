using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KioskUI : MonoBehaviour {

    public static KioskUI instance;

    public void Awake()
    {

        instance = this;

    }

    public void Show(bool show)
    {
        if (show)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }


    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
