using UnityEngine;
using System.Collections;

public class VWPlayerController: MonoBehaviour {

	public static VWPlayerController userManager;

	void Awake()
	{
	
		if (userManager == null) {
			DontDestroyOnLoad (gameObject);
			userManager = this;
		} else if (userManager != this) {
			Destroy (gameObject);
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
