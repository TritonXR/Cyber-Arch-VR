using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logo : MonoBehaviour {
    public static GameObject logo;
    
    private void Awake()
    {
        logo = this.gameObject;
        DontDestroyOnLoad(logo);
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public static void Hide()
    {
        logo.gameObject.SetActive(false);
    }

    public static void Show()
    {
        logo.gameObject.SetActive(true);
    }
}
