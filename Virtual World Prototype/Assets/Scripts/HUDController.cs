using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/*
**Author: Dustin Wilson
**Date: 18/05/2015
**Description: A script that that contains the functionality to update, enable/disable the different HUDs found in the application.
**Note: Remove the update function and attach the huds to one of the leap motion controller children
**/
public class HUDController : MonoBehaviour {

	//Huds
	public GameObject inspectionHUD;
	public GameObject EditHUD;
	public GameObject crosshairHUD;
	public GameObject ElicitationHUD;

	public List<GameObject> overlayArrayList;//Keeps track of the current progression of huds, to allow for easy backwards traversal
	public int currentIndex;

	//Controllers
	public VWPlayerActor userManager;
	public VWInspectionController inspectController;

	//Editing
	private bool isEdit;
	private InputField editField;

	//Player information
	public GameObject player;
	public GameObject playerDirection;

	//Positions
	private Vector3 front = new Vector3 (0.0f, 0f, 0f);
	private Vector3 back = new Vector3 (0.0f, 0f, -1f);

	// Use this for initialization
	void Start () {

		userManager = GameObject.FindWithTag ("Player").GetComponent<VWPlayerActor> ();
		overlayArrayList = new List <GameObject>();
		currentIndex = 0;
		overlayArrayList.Add (crosshairHUD);
		//inspectOver = gameObject.GetComponent<InspectionController> ();
		player = GameObject.FindWithTag ("PlayerController");
		EditHUD.SetActive (false);
		//DisableAllChildren (EditHUD, false);
		inspectionHUD.SetActive (false);
		//DisableAllChildren (inspectionHUD, false);
		PositionHUD(EditHUD, back);
		PositionHUD(inspectionHUD, back);
	}

	
	// Update is called once per frame
	void Update () {
	
		Vector3 newPos = player.transform.position + playerDirection.transform.forward*0.5f;
		newPos = new Vector3 (newPos.x, newPos.y - 0.25f, newPos.z);
		transform.position = newPos;
		transform.rotation = Quaternion.LookRotation (playerDirection.transform.forward);
	
	}

	/** Function: EnableCrossHair
	 ** Param: Boolean, true enable, false disable 
	 ** Purpose: Its purpose is turn on/off the main overlay (crosshair)
	 */
	public void EnableCrossHair(bool enable){
		EnableOverlay (crosshairHUD, enable);

	}

	/** Function: EnableEditHUD
	 ** Param: Boolean, true enable, false disable 
	 ** Purpose: Is to turn on/off the edit mode overlay
	 */
	public void EnableEditHUD(bool enable){
		EnableOverlay (EditHUD, enable);
		//PositionHUD(EditHUD, front);
		//DisableAllChildren (EditHUD, enable);

	}

	/** Function: EnableInspectionHUD
	 ** Param: Boolean, true enable, false disable 
	 ** Purpose: Is to turn on/off the cursor and the inspection overlay
	 */
	public void EnableInspectionHUD(bool enable){
		EnableOverlay (inspectionHUD, enable);
		//call to inspectioncontroller to set up the GUI elements
		//DisableAllChildren (EditHUD, enable);
	}

	/** Function: EnableElicitationHUD
	 ** Param: Boolean, true enable, false disable 
	 ** Purpose: Is to turn on/off the Elicitation overlay
	 */
	public void EnableElicitationHUD(bool enable){
		EnableOverlay (ElicitationHUD, enable);
	}

	/** Function: EnableOverlay
	 ** Param1: GameObject, the overlay to be enable/disable
	 ** Param2: Boolean, true enable, false disable 
	 ** Purpose: Is turn on/off the chosen overlay
	 */
	public void EnableOverlay(GameObject overlay, bool enable){
		
		overlay.SetActive (enable);
		//DisableAllChildren (overlay, enable);

		//if the edit or inspection hud
		if ((overlay.Equals (EditHUD) || overlay.Equals (inspectionHUD)) && enable == false) {
			PositionHUD(overlay, back);
		} else if ((overlay.Equals (EditHUD) || overlay.Equals (inspectionHUD)) && enable == true) {
			PositionHUD(overlay, front);
			Debug.Log ("Push to front");
		}

		//If enable and not the current overlay does not equal the selected overlay
		if (enable && !(overlayArrayList[currentIndex].Equals(overlay))) {
			//Add the overlay to the list to keep track of how far deep
			overlayArrayList.Add (overlay);
			currentIndex ++;
		} 
	}

	/** Function: RemoveElement
	 ** Purpose: Is the remove the current element from the array
	 */
	private void RemoveElement(){
		overlayArrayList.RemoveAt (currentIndex);
		currentIndex--;
	}

	/** Function: EscapeOverlay
	 ** Purpose: Function is to escape from the current overlay, and enable/disable controls and modes
	 */
	public void EscapeOverlay(){
		//Disable the current
		EnableOverlay (overlayArrayList[currentIndex], false);
		RemoveElement();
		EnableOverlay(overlayArrayList[currentIndex], true);

		//leave mode and bring up mainmode
		if (overlayArrayList [currentIndex] == crosshairHUD) {
			
			//userManager.EnableControl (true, true);
			//Disable all modes
			if (userManager.isInspecting) {
				//userManager.EnableInspectionMode (false);
				userManager.isInspecting = false;
				//Disable elicitationmode and destroy the object
			} else if (userManager.elicitationInProgress) {
				//userManager.EnableElicitationMode (false);
				userManager.elicitationInProgress = false;
				//userManager.DestroyElicitationObject();
				userManager.ResetProjector ();
			}
			
		} 
		
	}

	/** Function: PositionHUD
	 ** Param: The hud to be repositions
	 ** Param: The new position for the Hud
	 ** Purpose: Positions the given hud, to the given position
	 */
	private void PositionHUD(GameObject hud, Vector3 pos){
		//will need tweakin

		//transform.position = pos;
		hud.transform.localPosition = pos;
		//hud.transform.LookAt (playerDirection.transform, playerDirection.transform.up);//or player.transform
	}

	/** Function: SetEditFieldFocus
	 ** Purpose: Sets the edit field to be currently selected
	 */
	public void SetEditFieldFocus(){
	
		editField.Select ();
		editField.ActivateInputField ();
		//editField.MoveTextEnd (true);
	}

	/** Function: UpdateButton
	 ** Param: boolean, indicates which button is pressed, false its add, true its edit
	 ** Purpose: Calls for the edit mode overlay to be enable and for the inspection overlay to be turned off,
	 ** in addition to if isUpdate is true, the edit input field is filled with current data for editing
	 */
	public void UpdateButton(bool isOn){
		isEdit = isOn;
		EnableEditHUD (true);
		EnableInspectionHUD (false);
		editField = GameObject.FindWithTag ("Edit content").GetComponent<InputField> ();
		SetEditFieldFocus ();
		//set the text to be equal to the inspector text
		if (isEdit) {
			string info = inspectController.elicitedInfo;
			editField.text = info;
		} else {
			editField.text = "";
		}
	}

	/** Function: UpdateInformation
	 ** Param: Text, text component used by the input field
	 ** Purpose: Update the elicited Infomation obtained from the input field, for the inspection overlay
	 */
	public void UpdateInformation(string info){

		if (inspectController == null) {
			Debug.Log ("object is Null");
		} else if (inspectController.elicitedInfo == null) {
			Debug.Log ("string is Null");
		}

		if (info == null) {
			Debug.Log ("Text is Null");
		}

		if (isEdit || inspectController.elicitedInfo == "") {
			inspectController.elicitedInfo = info;
		} else {
			inspectController.elicitedInfo = inspectController.elicitedInfo +"\n\n" + info;
		}
		inspectController.UpdateOverlayText (inspectController.elicitedInfo);
	}
	
}
