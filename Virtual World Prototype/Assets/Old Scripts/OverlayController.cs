using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class OverlayController : MonoBehaviour {

	public GameObject InspectionOverlay;
	public GameObject MainGuiOverlay;
	public GameObject EditModeOverlay;

	//public GameObject ModeGuiOverlay;

	public GameObject MainMenuOverlay; // not yet implemented

	//Elicitation overlays
	public GameObject ElicitationMainOverlay;
	public GameObject ElicitationScaleOverlay;
	public GameObject EliciationPositionOverlay;

	//public GameObject userAvatar;
	public PlayerActor userManager;

	public OverlayController overlayController;

	public GameObject currentOverlay;
	public GameObject previousOverlay;

	public List<GameObject> overlayArrayList;
	public int currentIndex;

	public InspectionController inspectOver;

	private bool isEdit;

	private InputField editField;

	//void Awake()
	//{
	//	if (overlayController == null) {
	//		DontDestroyOnLoad (gameObject);
	//		overlayController = this;
	//	} else if (overlayController != this) {
	//		Destroy (gameObject);
	//	}
	//	userManager = GameObject.FindWithTag ("Player").GetComponent<PlayerActor> ();
	//}


	// Use this for initialization
	void Start () {
		//userManager = PlayerActor.userManager;
		userManager = GameObject.FindWithTag ("Player").GetComponent<PlayerActor> ();
		currentOverlay = null;
		//overlayController = this;
		previousOverlay = null;
		overlayArrayList = new List <GameObject>();
		currentIndex = 0;
		overlayArrayList.Add (MainGuiOverlay);
		inspectOver = gameObject.GetComponent<InspectionController> ();
	}
	
	// Update is called once per frame
	void Update () {

		//userManager = PlayerActor.userManager;

		//need an value to stop escaping when index is 0.

		if (Input.GetKeyDown (KeyCode.Escape)) {
			//if in one of the modes
			if(currentIndex>0){
				EscapeOverlay();
				userManager.exitMode = false;
			} else {
				EnableExitMenu(true);
				userManager.exitMode = true;
				EnableMainOverlay(false);
				userManager.EnableControl(false,false);
			}
		}
	}

	public void EnableInspectionOverlay(bool enable){

		EnableOverlay (InspectionOverlay, enable);
	}

	public void EnableMainOverlay(bool enable){
		
		EnableOverlay (MainGuiOverlay, enable);
	}

	public void EnableEditModeOverlay(bool enable){

		EnableOverlay (EditModeOverlay, enable);
	}

	public void EnableElicitationOverlay(bool enable){
		
		EnableOverlay (ElicitationMainOverlay, enable);
	}

	public void EnableElicitationPositionOverlay(bool enable){
		
		EnableOverlay (EliciationPositionOverlay, enable);
	}

	public void EnableElicitationScaleOverlay(bool enable){

		EnableOverlay (ElicitationScaleOverlay, enable);
		
	}

	public void EnableExitMenu(bool enable){
		
		EnableOverlay (MainMenuOverlay, enable);
		
	}


	public void EnableOverlay(GameObject overlay, bool enable){

		overlay.SetActive (enable);

		if (enable && !(overlayArrayList[currentIndex].Equals(overlay))) {
			overlayArrayList.Add (overlay);
			currentIndex ++;
		} 

	}

	private void RemoveElement(){
		overlayArrayList.RemoveAt (currentIndex);
		currentIndex--;
	}

	//public void EscapeOverlay(){
	//	userManager.EscapeOverlay ();
	//}

	public void EscapeOverlay(){
		//Disable the current
		EnableOverlay (overlayArrayList[currentIndex], false);
		RemoveElement();
		EnableOverlay(overlayArrayList[currentIndex], true);

		//leave mode and bring up mainmode
		if (overlayArrayList [currentIndex] == MainGuiOverlay) {

			userManager.EnableControl (true, true);
			//Disable all modes
			if (userManager.IsInspecting ()) {
				userManager.EnableInspectionMode (false);
				//Disable elicitationmode and destroy the object
			} else if (userManager.IsElicitating ()) {
				userManager.EnableElicitationMode (false);
				//userManager.DestroyElicitationObject();
				userManager.ResetProjector ();
			}
		
		} else if (overlayArrayList [currentIndex] == ElicitationMainOverlay) {
			Debug.Log("Back at elicitoverlay");
			userManager.EnableCamera(false);
			if(userManager.isPositioning){
				Debug.Log("Reset Position");
				userManager.ResetPosition();
			} else if(userManager.isScaling){
				userManager.ResetScale ();
			}
			userManager.EnablePositioning(false);
			userManager.EnableScaling(false);
		}

	}

	public void UpdateButton(bool isUpdate){
		isEdit = isUpdate;
		EnableEditModeOverlay (true);
		EnableInspectionOverlay (false);
		//set the text to be equal to the inspector text
		if (isEdit) {
			string info = inspectOver.objectInformation;
			editField = GameObject.FindWithTag ("Edit content").GetComponent<InputField> ();
			editField.text = info;
		}

	}

	public void UpdateInformation(Text info){

		if (isEdit || inspectOver.objectInformation == "") {
			inspectOver.objectInformation = info.text;
		} else {
			inspectOver.objectInformation = inspectOver.objectInformation +"\n\n" + info.text;
		}
		inspectOver.UpdateOverlayText (inspectOver.objectInformation);
	}
	

}
