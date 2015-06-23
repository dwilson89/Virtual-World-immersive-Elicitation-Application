using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour {
	
	// Whether or not the use has movement and/or camera controls
	public bool hasMovementControls;
	public bool hasCameraControls;
	public bool hasOverallControls;
	
	public GameObject hoveredObject;
	
	public ObjectDataProperties objectProps;
	
	// Maximum user sight range
	public float maxSightDistance = 100;
	
	//public GameObject userAvatar;
	public PlayerActor userManager;
	
	private Image crosshair;
	
	//public GameObject hoverControls;

	public GameObject popUpOverlay;

	public float interactionDistance = 5.0f;

	//public static Crosshair playerCrosshair;  

	//void Awake()
	//{
	//	if(playerCrosshair != null)
	//		GameObject.Destroy(playerCrosshair);
	//	else
	//		playerCrosshair = this;
		
	//	DontDestroyOnLoad(this);
	//}

	// Use this for initialization
	void Start () {

		crosshair = GetComponent<Image> ();
		//userAvatar = GameObject.FindGameObjectWithTag ("Player");
		//userManager = PlayerActor.userManager;
	
	}

	
	// Update is called once per frame
	void Update () {

		//userManager = PlayerActor.userManager;
		
	}

	public void ResetCrosshair(){

		crosshair.color = Color.white;
		popUpOverlay.SetActive(false);
	}


	public void SetCrosshairColor (Color col){
		crosshair.color = col;

	}

	public void EnablePopupOverlay(bool status){
		popUpOverlay.SetActive (status);
	}


	public void setOverlay(int choice){
	
		string text = "";

		if (choice == 0) {
			text = "Press \"R\" to Inspect";
		} else if (choice == 1) {
			text = "Press \"E\" to Elicitate";
		}

		popUpOverlay.GetComponentInChildren<Text> ().text = text;
	}
	
}