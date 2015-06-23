using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Leap;

/*
**Author: Dustin Wilson
**Date: 09/06/2015
**Class: VWPlayerActor
**Version: 1.5
**Description: A script that contains the main functionality for the user, and the objects in the scene
**/
public class VWPlayerActor : MonoBehaviour {

	//Class constants
	const float MAX_SIGHT_DISTANCE = 100f;// Maximum user sight range
	const float INITIAL_R = 0.5f;
	const float MAX_ACTION_TIME = 1.0f;
	const float INTERACTION_DISTANCE = 5.0f;
	const float RANGE = 10.0f;
	const bool EXPLORATION = false;
	const bool ELICITATION = true;

	//Hoverobject variables
	public GameObject hoveredObject;
	private ObjectDataProperties objectProps;

	public GameObject mainUI;
	public GameObject siteViewer;
	public VWCrosshair crosshair;

	//States of operation
	public bool isInspecting = false;
	public bool elicitationInProgress = false;
	private bool currentlyHovering = false;

	//Projector Variables
	public Projector elicitPro;
	public GameObject projectorController;

	//Oculus and Leap Motion Variables
	public GameObject centerEye;
	public GameObject leapMotionOVRController = null;
	public HandController handController = null;
	public CharacterController leapCharCont;

	//HUD controllers
	public VWInspectionController inspectCont;
	public HUDController HUDCont;

	public GameObject ElicitorObj;

	private RaycastHit hitInfo;

	//Delay timers for gestures
	private float actionTime;
	private float turnOffTime;

	//Size and position value for scaling
	private float previousDistance;
	private float currentSize;
	private bool isScaling;

	private bool hasDetectedGesture;
	private bool isPositioning = false;
	private bool userMode;
	
	// Use this for initialization
	void Start () {

		crosshair = mainUI.transform.FindChild ("CrosshairImage").gameObject.GetComponent<VWCrosshair> ();
		userMode = ELICITATION;
		leapCharCont = leapMotionOVRController.GetComponent<CharacterController> ();
		actionTime = 0.0f;
		turnOffTime = 0.0f;
		previousDistance = 0.0f;
		isScaling = false;
		isPositioning = false;
		hasDetectedGesture = false;
		currentSize = INITIAL_R;
		//handController.IgnoreCollisionsWithHands (GameObject.FindGameObjectWithTag("floor"), true);
	}

	/** Function: IsObjectInRange
	 ** Purpose: Is to check, if the hoverObject object does exist, is it in range of the user for interaction
	 */
	private bool IsObjectInRange(float range){
		
		if (hoveredObject == null) {
			
			return false;
		} 
		
		return(Vector3.Distance (hoveredObject.transform.position, centerEye.transform.position) <=
		       INTERACTION_DISTANCE || Vector3.Distance (hoveredObject.transform.position, centerEye.transform.position) < range);
		
	}
	
	// Update is called once per frame
	void Update () {
		
		//disable updating of GUI if in inspection mode or elicitationInProgress
		if (!isInspecting && !elicitationInProgress) {
			//then have a if in elicitation mode .....

			// Perform a Raycast
			//Vector3 v = new Vector3 (leftEyeCamera.pixelWidth / 2.0f, leftEyeCamera.pixelHeight / 2.0f, 0.0f);
			//Ray ray = leftEyeCamera.ScreenPointToRay (v); 

			hitInfo = new RaycastHit ();
			Physics.Raycast (centerEye.transform.position, centerEye.transform.forward, out hitInfo, MAX_SIGHT_DISTANCE);

			//Debugging purposes.
			//Debug.DrawRay(centerEye.transform.position, centerEye.transform.forward * 10.0f, Color.green);

			// If the raycast hits an object, select the object
			if (hitInfo.collider != null && hitInfo.transform.tag == "Inspect Element") {
				ResetCrosshair();
				hoveredObject = hitInfo.collider.gameObject;
				objectProps = hoveredObject.GetComponent<ObjectDataProperties>();
				updateCrosshairOverlay (0);
			
			//if the raycast hits the reef
			} else if (hitInfo.collider != null && hitInfo.transform.tag == "Reef") {
				hoveredObject = hitInfo.collider.gameObject;
				EnableProjector(false);
				ResetCrosshair();

				// Need to check if in elicitation mode
				if (userMode == ELICITATION) {
					updateCrosshairOverlay (1);
				}
				
			//if it doesnt hit anything
			} else {
				ResetCrosshair();
				hoveredObject = null;
				SetCurrentlyHovering (false, hoveredObject);
			}
		}	
	}

	// LateUpdate is called once per frame - used for leap motion controller 
	void LateUpdate () {

		//Don't do anything if one is null
		if (leapMotionOVRController == null || handController == null)
			return;
		
		// Move forward if both palms are facing outwards! Whoot!
		HandModel[] hands = handController.GetAllGraphicsHands();

		//Frame currentFrame = handController.GetFrame ();

		//disable updating of GUI if in inspection mode or elicitationInProgress
		if (!isInspecting && !elicitationInProgress) {

			CheckForMovement (hands);

			CheckForUserIndication (hands);

		} else if (elicitationInProgress) {

			CheckForScaling (hands);

			//check for hands in scene if no scaling
			if (hands.Length >= 1 && !isScaling) {

				bool isComfirm = false;
				bool isPos = false;

				//Note: This whole section this need to be refactored its similar code to the CheckForUserIndication

				//Note: In future refactor this (parametres would be hands and finger types)
				foreach(HandModel hand in hands){
					if(isFingerPointing (hand,Finger.FingerType.TYPE_INDEX)){
						isPos = true;
						hasDetectedGesture = true;
					}else if (isFingerPointing(hand, Finger.FingerType.TYPE_THUMB)){
						isComfirm = true;
						hasDetectedGesture = true;
					} else {
						hasDetectedGesture = false;
					}
				}

				//if a gesture has been detected, which one
				if(hasDetectedGesture){
				
					if(actionTime < MAX_ACTION_TIME && !isPositioning){
						actionTime += Time.deltaTime; 
					} else {
						actionTime = 0.0f;
						if(isComfirm && !isPos && !isScaling){
							ConfirmElicitObjectValues();
						} else if(isPos){
							ProjectorColorChange(Color.yellow);
							PositionElicitation();
							isPositioning = true;
						} else {
							isPositioning = false;
							ProjectorColorChange(Color.green);
						}
					}
				}
			}
		}

		//if not gesture incremet reset timer
		if (!hasDetectedGesture) {
			turnOffTime +=Time.deltaTime;
		} else {
			turnOffTime = 0f;
		}

		//if reset timer has hit max, reset timers
		if (turnOffTime > MAX_ACTION_TIME) {
			actionTime = 0.0f;
			turnOffTime = 0f;
		}
	}


	/** Function: CheckForScaling
	 ** Param: Array of hands in the scene 
	 ** Purpose: Is to check if the user has gesture to scale the selected area, and then scale accordingly
	 */
	private void CheckForScaling (HandModel[] hands)
	{
		bool isScale = false;
		//check for scaling
		if (hands.Length > 1) {
			//now check if both hands are open
			if (IsHandOpen (hands [0]) && IsHandOpen (hands [1])) {
				//get the scale factor
				Vector3 hand1 = hands [0].GetPalmPosition ();
				Vector3 handnormal0 = hands [0].GetPalmNormal ().normalized;
				Vector3 hand2 = hands [1].GetPalmPosition ();
				Vector3 handnormal1 = hands [1].GetPalmNormal ().normalized;
				float cosAng = Vector3.Dot (handnormal0, handnormal1);
				if (cosAng < -0.8f) {
					float currentDistance = Mathf.Sqrt (Mathf.Pow (hand1.x - hand2.x, 2f) + Mathf.Pow (hand1.y - hand2.y, 2f) + Mathf.Pow (hand1.z - hand2.z, 2f));
					if (!isScaling) {
						isScaling = true;
						previousDistance = currentDistance;
						currentSize = elicitPro.orthographicSize;
					}
					//calculate scale factor
					float scaleF = currentDistance / previousDistance;
					//float scaleF = currentFrame.ScaleFactor(previousFrame); 
					//set the scale
					SetProjectorRadius ((currentSize) * scaleF);
					ProjectorColorChange (Color.red);
					isScale = true;
				}
			} 
		}

		//if no scale has been detected
		if(!isScale){
			isScaling = false;
			hasDetectedGesture = false;
			ProjectorColorChange (Color.green);
		}
	}

	/** Function: CheckForUserIndication
	 ** Param: Array of hands in the scene 
	 ** Purpose: Is to check if the user has gesture to interact with objects in the virtual world, and then acts accordingly
	 */
	private void CheckForUserIndication (HandModel[] hands)
	{
		//Want to check for a pointing index (would originally be one hand, but a second hand can sometime be in frame)
		if (hands.Length >= 1) {
			bool isElic = false;
			bool isInspec = false;
			//check each hand for a pointing index - used for checking "one-hand" commands
			foreach (HandModel hand in hands) {
				//check for a pointing finger
				if (isFingerPointing (hand, Finger.FingerType.TYPE_INDEX) && currentlyHovering && (hoveredObject.tag == "Inspect Element")) {
					isInspec = true;
					hasDetectedGesture = true;
				}
				else if (isFingerPointing (hand, Finger.FingerType.TYPE_INDEX) && currentlyHovering && (hoveredObject.tag == "Reef")) {
					isElic = true;
					hasDetectedGesture = true;
				} else {
					hasDetectedGesture = false;
				}
			}
			//if a gesture has been detected
			if (hasDetectedGesture) {
				//check for the actiontime
				if (actionTime < MAX_ACTION_TIME) {
					actionTime += Time.deltaTime;
				}else {
					actionTime = 0.0f;
					if (isElic) {//if elicitation
						Elicitation (hitInfo);
						elicitationInProgress = true;
					} else if (isInspec) {//if inspection
						LoadInspector (hoveredObject);
					}
				}
			}
		}
	}

	/** Function: CheckForMovement
	 ** Param: Array of hands in the scene 
	 ** Purpose: Is to check if the user has gestured to move, and then moves the user in the looked at direction
	 */
	private void CheckForMovement (HandModel[] hands)
	{
		//Checks if there are two hands in view
		if (hands.Length > 1) {
			Vector3 direction0 = (hands [0].GetPalmPosition () - handController.transform.position).normalized;
			Vector3 normal0 = hands [0].GetPalmNormal ().normalized;
			Vector3 direction1 = (hands [1].GetPalmPosition () - handController.transform.position).normalized;
			Vector3 normal1 = hands [1].GetPalmNormal ().normalized;
			if (Vector3.Dot (direction0, normal0) > direction0.sqrMagnitude * 0.5f && Vector3.Dot (direction1, normal1) > direction1.sqrMagnitude * 0.5f) {
				// always move along the camera forward as it is the direction that it being aimed at
				Vector3 desiredMove = centerEye.transform.forward;
				desiredMove.y = 0f;
				leapCharCont.Move (desiredMove * Time.fixedDeltaTime);
			}
		}
	}

	
	/** Function: PositionElicitation()
	 ** Purpose: Is to update the position of the elicitation area, via the use of raycasting
	 ** Note: Should be refactored as this is duplicated code from the update function 
	 */
	private void PositionElicitation(){

		RaycastHit elicitHitInfo = new RaycastHit ();
		Physics.Raycast (centerEye.transform.position, centerEye.transform.forward, out elicitHitInfo, MAX_SIGHT_DISTANCE);
		
		//get the current position
		Vector3 curPos = projectorController.transform.position;
		
		//if it collided with the reef, update the position of the 
		if (elicitHitInfo.collider != null && elicitHitInfo.transform.tag == "Reef") {
			//update current position
			curPos = Vector3.MoveTowards(curPos, elicitHitInfo.point, 5*Time.deltaTime);
			projectorController.transform.position = curPos;
			UpdateCameraPosition (projectorController.transform.position);
		}
	}

	/** Function: UpdateCameraHeight
	 ** Param: Vector3, the position to be used for the camera
	 ** Purpose: Is to update the Reef Inspection camera position, in addition to keep it at a desired height
	 ** Note: Should possibly be refactored, and could use linecast
	 */
	private void UpdateCameraPosition(Vector3 pos){
		
		//Perform a raycast
		Camera sV = siteViewer.GetComponent<Camera> ();
		
		Vector3 v = new Vector3 (sV.pixelWidth / 2.0f, sV.pixelHeight / 2.0f, 0.0f);
		Ray ray = sV.ScreenPointToRay (v); 
		RaycastHit hitInfo = new RaycastHit ();
		Physics.Raycast (ray, out hitInfo, 10.0f);
		//Update the height with an offset of 1.5
		siteViewer.transform.position = new Vector3(pos.x, hitInfo.point.y + 1.5f, pos.z);
		
	}

	/** Function: LoadInspector
	 ** Param: GameObject, the GameObject holding the information to be used for inspection
	 ** Purpose: Is to set up the application inspection mode, by setting control variables, enabling/disabling the specific overlays
	 ** and the setting of the inspection data.
	 */
	public void LoadInspector(GameObject inspecObject){
		hoveredObject = null;
		currentlyHovering = false;
		//disable the main gui
		HUDCont.EnableCrossHair(false);
		//enable the inspection window
		HUDCont.EnableInspectionHUD (true);
		//load the stored data
		inspectCont.SetInspectionData (inspecObject);
		isInspecting = true;
	}

	/** Function: IsHandOpen
	 ** Param: The hand model for the user 
	 ** Purpose: Its purpose is the check if the hand is fully closed or opened, by checking each fingers extension
	 */
	private bool IsHandOpen(HandModel hand){

		for (int f = 0; f < hand.fingers.Length; f++)
		{
			Finger digit = hand.fingers[f].GetLeapFinger();
			if(!digit.IsExtended){//if digit is not extended, its not fully open

				return false;
			}
		}
		return true;
	}
	
	/** Function: isFingerPointing
	 ** Param: The hand model for the user 
	 ** Param: the finger type of the pointing finger
	 ** Purpose: Its purpose is the check if only the designated finger is extended
	 */
	public bool isFingerPointing(HandModel hand, Finger.FingerType type){
		
		bool isFinger = false;
		bool isHandClosed = true;
		
		for (int f = 0; f < hand.fingers.Length; f++)
		{
			Finger digit = hand.fingers[f].GetLeapFinger();
			if(digit.IsExtended){
				if(digit.Type() == type){
					isFinger = true;
				} else {
					return false;
				}
			}
		}
		return isFinger;
	}

	/** Function: ResetProjector()
	 ** Purpose: Is to reset the projector back default, which entails disabling it, changing 
	 ** its color back to green and setting the radius back to the intial value 
	 */
	public void ResetProjector(){
		EnableProjector (false);
		
		SetProjectorRadius (INITIAL_R);
	}

	/** Function: EnableProjector
	 ** Param: boolean, the current active state for the projector
	 ** Purpose: Is to enable/disable, the current active state for the projector.
	 */
	private void EnableProjector(bool isOn){
		projectorController.SetActive(isOn); 
	}

	/** Function: updateCrosshairOverlay
	 ** Param: integer, the overlay to be loaded (0 - exploration, 1 - elicitation)
	 ** Purpose: Updates the crosshair overlay accordingly to whether it is in range or not,
	 ** by enabling a popup overlay and changing the crosshair color, also setting the variables to 
	 ** indicate a current hovered object.
	 */
	public void updateCrosshairOverlay(int overlayType){
		
		if (IsObjectInRange (RANGE)) {
			crosshair.EnablePopupOverlay (true);
			crosshair.setOverlay (overlayType);
			crosshair.SetCrosshairColor (Color.green);
			SetCurrentlyHovering (true, hoveredObject);
		} else {
			
			crosshair.SetCrosshairColor (Color.red);
		}
		
		if (overlayType == 0) {
			EnableProjector(true);
			siteViewer.SetActive (true);
			PositionProjector(hoveredObject.transform.position);
			SetProjectorRadius(objectProps.projectorRadius);
			SetCameraPosition(hoveredObject.transform.position);
		}
		
	}

	/** Function: ResetCrosshair
	 ** Purpose: Is to reset the crosshair, by setting it to its default color and disabling the pop-up overlay.
	 */
	public void ResetCrosshair(){
		crosshair.EnablePopupOverlay (false);
		crosshair.SetCrosshairColor (Color.white);
		ResetProjector ();
		siteViewer.SetActive (false);
	}

	/** Function: SetCurrentlyHovering
	 ** Param1: boolean, indicates the hovered state
	 ** Param2: GameObject, the game object being hovered over
	 ** Purpose: Is to set the current hovered object, and the currently hovering state
	 */
	public void SetCurrentlyHovering(bool isHover, GameObject hoverObject){
		currentlyHovering = isHover;	
		hoveredObject = hoverObject;
	}

	/** Function: SetCurrentMode
	 ** Param: integer, the mode of operation 0 - exploration, 1 - elicitation
	 ** Purpose: Is to set up the current mode indicated, by setting the label up the top left-hand corner
	 ** and turning off/on the controls used in that mode
	 */
	public void SetCurrentMode(){
		ResetCrosshair ();
		userMode = !userMode;
		GameObject modeText = GameObject.FindWithTag("Mode Display");
		if (userMode == EXPLORATION) {
			modeText.GetComponent<Text> ().text = "Mode: Exploration";
		} else if (userMode == ELICITATION){
			modeText.GetComponent<Text>().text = "Mode: Elicitation";
		}
	}

	/** Function: PositionProjector
	 ** Param: Vector3, the position to be used to update the projector
	 ** Purpose: Is to set the projector to the new position, with a height of 2.
	 ** Note: Height may need to be changed in the future to account for higher meshes
	 */
	private void PositionProjector(Vector3 info){
		projectorController.transform.position = new Vector3 (info.x, 2.0f, info.z); 
	}

	/** Function: SetProjectorRadius
	 ** Param: float, the value of the radius to be set
	 ** Purpose: Is to set the projector orthographic size with the new radius value.
	 */
	private void SetProjectorRadius(float radius){
		elicitPro.orthographicSize = radius;
	}

	/** Function: Elicitation
	 ** Param1: RaycastHit, the current ray cast hit info
	 ** Purpose: Is to set up the Elicitation mode, by setting up the projector and reef inspection camera
	 */
	public void Elicitation(RaycastHit hitInfo){
		ResetCrosshair ();
		HUDCont.EnableElicitationHUD (true);
		Vector3 pos = new Vector3 (hitInfo.point.x, 2f, hitInfo.point.z);
		EnableProjector (true);
		siteViewer.SetActive (true);
		PositionProjector(pos);
		SetCameraPosition (hitInfo.point);
		ProjectorColorChange(Color.green);
	}

	/** Function: ProjectorColorChange
	 ** Param: Color, the new color for the projector
	 ** Purpose: Is to change the color of the projector
	 */
	public void ProjectorColorChange(Color col){
		elicitPro.material.color = col;
	}

	private void SetCameraPosition(Vector3 pos){
		siteViewer.transform.position = new Vector3(pos.x, pos.y + 1.5f, pos.z);
	}

	/** Function: ConfirmElicitObjectValues
	 ** Purpose: Is to create an Elicitation object, assign the gather values to, disable the current mode, and load the inspector
	 ** for the purpose of adding reef information to the newly created object
	 */
	public void ConfirmElicitObjectValues(){

		GameObject currentElicitedObject;
		ProjectorColorChange(Color.green);
		
		Vector3 pos = new Vector3 (projectorController.transform.position.x, 1.0f, projectorController.transform.position.z); 
		
		//create an item at the position obtained via hitInfo
		currentElicitedObject = Instantiate (ElicitorObj, pos, Quaternion.identity) as GameObject;
		
		//u get the object
		GameObject capChild = currentElicitedObject.transform.FindChild ("Capsule").gameObject;
		
		capChild.SetActive (true);
		
		//grab the script
		ObjectDataProperties currentObjectData = capChild.GetComponent<ObjectDataProperties> ();
		
		// Set the transform information for each child
		currentObjectData.elicitedAreaTransform = projectorController.transform; 
		currentObjectData.elicitedTransformInfo = capChild.transform;
		currentObjectData.projectorRadius = elicitPro.orthographicSize;
		
		//disable sphere, enable capsule
		
		elicitationInProgress = false;
		
		//escape out of current overlay
		HUDCont.EscapeOverlay ();

		LoadInspector (capChild);
		
	}

}
