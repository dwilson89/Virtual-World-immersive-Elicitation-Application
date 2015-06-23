using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerActor : MonoBehaviour {

	public GameObject siteViewer;

	// Static globals
	static int exploration = 0;
	static int elicitation = 1;

	const float INITIAL_R = 0.5f;

	// Whether or not the use has movement and/or camera controls
	public bool hasMovementControls;
	public bool hasCameraControls;
	public bool hasOverallControls;

	public GameObject hoveredObject;

	public ObjectDataProperties objectProps;

	// Maximum user sight range
	public float maxSightDistance = 100;

	public static GameObject userAvatar;
	//public static PlayerActor userManager;

	//current mode exploration = 0, elicitation = 1
	public int currentMode;

	//public GameObject hoverControls;

	public GameObject ElicitorObj;

	public bool currentlyHovering;

	public float range;

	private Crosshair playerCross;

	public float interactionDistance = 5.0f;

	private CharacterController playerCont;

	private CustomFirstPersonController firstPcont;
	
	private GameObject currentElicitedObject;

	private bool elicitationInProgress;

	public OverlayController overlayCont;

	private bool isInspecting;

	public InspectionController inspectCont;

	public GameObject projector;

	private Projector elicitPro;

	public bool isPositioning;
	public bool isScaling;

	private Vector2 p_Input;
	private Vector3 m_MoveDir = Vector3.zero;
	private float speed = 0.1f;
	private float incrementR = 0.01f;

	private Vector3 currentElicPos;
	private float currentRadius;

	private float zoom = 10f;
	private float normal = 60f;
	private float smooth = 5f;
	private bool isZoomed = false;
	public float zoomFactor = 0.1f;
	public float currentZoom;
	public GameObject projectorController;

	public bool exitMode;

	//void Awake()
	//{

	//	if (userManager == null) {
	//		DontDestroyOnLoad (gameObject);
	//		userManager = this;
	//	} else if (userManager != this) {
	//		Destroy (gameObject);
	//	}
	//}

	// Use this for initialization
	void Start () {
		exitMode = false;
		currentRadius = 0.5f;
		currentMode = 0;
		userAvatar = gameObject;
		playerCross = GameObject.FindWithTag("Crosshair").GetComponent<Crosshair>();

		isInspecting = false;
		elicitationInProgress = false;
	
		firstPcont = userAvatar.GetComponent<CustomFirstPersonController> ();
		//EnableCharacterControl ();
		//EnableCameraControl ();
		//overlayCont = OverlayController.overlayController;
		overlayCont = GameObject.FindWithTag("GUIOverlay").GetComponent<OverlayController>();
		//inspectCont = InspectionController.inspectCont;
		inspectCont = GameObject.FindWithTag ("GUIOverlay").GetComponent<InspectionController> ();
		//overlayCont.EnableMainOverlay (true);
		projectorController = GameObject.FindGameObjectWithTag ("projector");
		projector = projectorController.transform.FindChild("Projector").gameObject;
		elicitPro = projector.GetComponent<Projector> ();
		siteViewer = GameObject.Find ("RICamera");
		currentZoom = normal;
	}

	private bool IsObjectInRange(float range){

		if (hoveredObject == null) {
			
			return false;
		} 
		
		return(Vector3.Distance (hoveredObject.transform.position, Camera.main.transform.position) <=
		       interactionDistance || Vector3.Distance (hoveredObject.transform.position, Camera.main.transform.position) < range);
		
	}

	public void SetExitMode(bool isOn){
		exitMode = isOn;
	}

	// Update is called once per frame
	void Update () {
	
		//playerCross = Crosshair.playerCrosshair;

		//disable updating of GUI if in inspection mode or elicitationInProgress
		if (!isInspecting && !elicitationInProgress && !exitMode) {
			//then have a if in elicitation mode .....

			// Perform a Raycast
			Vector3 v = new Vector3 (Camera.main.pixelWidth / 2.0f, Camera.main.pixelHeight / 2.0f, 0.0f);
			Ray ray = Camera.main.ScreenPointToRay (v); 
			RaycastHit hitInfo = new RaycastHit ();
			Physics.Raycast (ray, out hitInfo, maxSightDistance);
		
			// If the raycast hits an object, select the object
			if (hitInfo.collider != null && hitInfo.transform.tag == "Inspect Element") {
				ResetCrosshair();
				hoveredObject = hitInfo.collider.gameObject;
				objectProps = hoveredObject.GetComponent<ObjectDataProperties>();
				updateCrosshairOverlay (0);
			
			} else if (hitInfo.collider != null && hitInfo.transform.tag == "Reef") {
				hoveredObject = hitInfo.collider.gameObject;
				EnableProjector(false);
				ResetCrosshair();
				// Need to check if in elicitation mode
				if (GetCurrentMode () == 1) {
					updateCrosshairOverlay (1);
				}

			
			} else {

				ResetCrosshair();
				hoveredObject = null;
				SetCurrentlyHovering (false, hoveredObject);
				//hoverControls.SetActive(false);
			}

			//Check if the user has changed modes
			if (Input.GetKeyDown (KeyCode.Alpha1)) {
				SetCurrentMode (0);
			} else if (Input.GetKeyDown (KeyCode.Alpha2)) {
				SetCurrentMode (1);
			}

			if ((Input.GetKeyDown (KeyCode.R)) && currentlyHovering && (hoveredObject.tag == "Inspect Element")) {

				LoadInspector(hoveredObject);
			
			} else if ((Input.GetKeyDown (KeyCode.E)) && currentlyHovering && (hoveredObject.tag == "Reef")) {
				overlayCont.EnableElicitationOverlay (true);
				overlayCont.EnableMainOverlay(false);
				EnableElicitationMode (true);
				EnableControl (false, false);
				ResetCrosshair();
				Elicitation (hitInfo);
			}
		
		} else if (elicitationInProgress) {

			if(isScaling){
				GetScaleInput();
			} else if (isPositioning){
				GetPositionInput();

				if (Input.GetKeyDown (KeyCode.Return)) {
					EnablePositioning(false);

					overlayCont.EscapeOverlay();
				} if(Input.GetAxis ("Mouse ScrollWheel") > 0){
					isZoomed = true;
					ZoomCamera();
				} else if((Input.GetAxis ("Mouse ScrollWheel") < 0)){
					isZoomed = false;
					ZoomCamera();
				}

			}

		}

	}

	public void ZoomCamera(){
		
		if(isZoomed == true){
			
			//if(currentZoom > zoom){
			//	currentZoom -= zoomFactor;
				Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView,zoom,Time.deltaTime * smooth);
			//}
		} else {
			//if(currentZoom < normal){
			//	currentZoom += zoomFactor;
				Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView,normal,Time.deltaTime * smooth);
			//}
		}
	}

	public void UpdateCameraPosition(Transform camPos){
		Camera sV = siteViewer.GetComponent<Camera> ();
		//siteViewer.transform.position = camPos.position;

		Vector3 v = new Vector3 (sV.pixelWidth / 2.0f, sV.pixelHeight / 2.0f, 0.0f);
		Ray ray = sV.ScreenPointToRay (v); 
		RaycastHit hitInfo = new RaycastHit ();
		Physics.Raycast (ray, out hitInfo, 10.0f);
		
		siteViewer.transform.position = new Vector3(camPos.position.x, hitInfo.point.y + 1.5f, camPos.position.z);

	}

	private void SetCameraPosition(Vector3 pos){
		siteViewer.transform.position = new Vector3(pos.x, pos.y + 1.5f, pos.z);
	}

	public void ResetPosition(){
		PositionProjector(currentElicPos);
		Camera.main.fieldOfView = normal;
	}

	public void ResetScale(){
		elicitPro.orthographicSize = currentRadius;
	}

	private void GetScaleInput(){
		if (Input.GetKey (KeyCode.UpArrow) || (Input.GetAxis ("Mouse ScrollWheel") > 0)) {
			elicitPro.orthographicSize += incrementR;
		} else if (Input.GetKey(KeyCode.DownArrow) || (Input.GetAxis ("Mouse ScrollWheel") < 0)) {
			elicitPro.orthographicSize -= incrementR;
		} else if (Input.GetKeyDown (KeyCode.Return)) {
			EnableScaling(false);
			currentRadius = elicitPro.orthographicSize;
			overlayCont.EscapeOverlay();
		}

	}

	private void GetPositionInput(){
		// Read input
		float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
		float vertical = CrossPlatformInputManager.GetAxis("Vertical");

		p_Input = new Vector2(horizontal, vertical);
		
		m_MoveDir.x = p_Input.x * speed;
		m_MoveDir.z = p_Input.y * speed;

		projectorController.transform.Translate(m_MoveDir * Time.fixedDeltaTime, Camera.main.transform);
		UpdateCameraPosition (projectorController.transform);
	}

	public void EnableScaling(bool isOn){
		isScaling = isOn;
	}

	public void EnablePositioning (bool isOn){
		isPositioning = isOn;
	}

	public void ResetProjector(){
		EnableProjector (false);

		SetProjectorRadius (INITIAL_R);
	}

	public void LoadInspector(GameObject inspecObject){
		//disable all player controls, 
		EnableControl (false, false);
		//disable the main gui
		overlayCont.EnableMainOverlay (false);
		//enable the inspection window
		overlayCont.EnableInspectionOverlay (true);
		inspectCont.SetInspectionData (inspecObject);
		isInspecting = true;
	}

	public void EnableInspectionMode(bool isOn){
		isInspecting = isOn;
	}

	public void EnableElicitationMode(bool isOn){
		elicitationInProgress = isOn;
	}

	public void EnableControl(bool isCam, bool isMove){

		if (currentMode == 0) {
			EnableCamera (isCam);
			EnableMovement (isMove);
		} else if (currentMode == 1) {
			EnableCamera(isCam);
		}
	}

	public void updateCrosshairOverlay(int overlayType){
		
		if (IsObjectInRange (GetCurrentRange ())) {
			playerCross.EnablePopupOverlay (true);
			playerCross.setOverlay (overlayType);
			playerCross.SetCrosshairColor (Color.green);
			//userAvatar.GetComponent<PlayerActor>().SetCurrentlyHovering(true, hoveredObject);
			SetCurrentlyHovering (true, hoveredObject);
			//hoverControls.SetActive (true);
		} else {
			
			playerCross.SetCrosshairColor (Color.red);
			//hoverControls.SetActive (true);
		}

		if (overlayType == 0) {
			EnableProjector(true);
			siteViewer.SetActive (true);
			PositionProjector(hoveredObject.transform.position);
			SetProjectorRadius(objectProps.projectorRadius);
		}

	}

	private void EnableProjector(bool isOn){
		projectorController.SetActive(isOn); 
	}

	private void PositionProjector(Vector3 info){
		projectorController.transform.position = new Vector3 (info.x, 2.0f, info.z); 
	}

	private void SetProjectorRadius(float radius){
		elicitPro.orthographicSize = radius;
	}

	public void ResetCrosshair(){
		playerCross.EnablePopupOverlay (false);
		playerCross.SetCrosshairColor (Color.white);
		ResetProjector ();
		siteViewer.SetActive (false);
	}

	public float GetCurrentRange(){
		return range;
	}

	public int GetCurrentMode(){
		return currentMode;
	}

	public void SetCurrentMode(int mode){
		ResetCrosshair ();
		currentMode = mode;
		GameObject modeText = GameObject.FindWithTag("Mode Display");
		if (mode == exploration) {
			modeText.GetComponent<Text> ().text = "Game Mode: Exploration";
			range = 10.0f;
			EnableMovement(true);
		} else if (mode == elicitation){
			modeText.GetComponent<Text>().text = "Game Mode: Elicitation";
			range = 10.0f;
			EnableMovement(false);
		}
	}


	public void EnableCamera(bool isCameraMoving){
		hasCameraControls = isCameraMoving;
		firstPcont.isCameraMoving = hasCameraControls;
	}

	private void EnableMovement(bool isMoving){
		hasMovementControls = isMoving;
		firstPcont.isMoving = hasMovementControls;
		
	}

	public bool GetCurrentlyHovering(){
		return currentlyHovering;
	}

	public GameObject GetHoverObject(){

		return hoveredObject;
	}

	public void SetCurrentlyHovering(bool isHover, GameObject hoverObject){
		currentlyHovering = isHover;	
		hoveredObject = hoverObject;
	}

	//Function to bring up the inspector overlay, will disable all things
	public void InspectElement(){
		// Turn off player movement and camera controls
		EnableCamera (false);
		EnableMovement (false);

		//Enable inspection overlay

		//Disable main overlay
	}

	public void ResetMain(){

		//Enable main overlay

		//turn back on camera and/or player movement
		EnableCamera (true);

		//check mode to enable movement
		if (currentMode == exploration) {
			EnableMovement (true);
		} 

	}

	/*Elicitation Mode Functions*/

	public void Elicitation(RaycastHit hitInfo){

		Vector3 pos = new Vector3 (hitInfo.point.x, 2f, hitInfo.point.z);
		EnableProjector (true);
		siteViewer.SetActive (true);
		PositionProjector(pos);
		currentElicPos = new Vector3(projectorController.transform.position.x,projectorController.transform.position.y,projectorController.transform.position.z);
		SetCameraPosition (hitInfo.point);
	}
	

	//function to destroy object if canceled
	public void DestroyElicitationObject(){

		Destroy (currentElicitedObject,0.0f);
	}

	public void ConfirmElicitObjectValues(){

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
		currentObjectData.projectorRadius = projector.GetComponent<Projector> ().orthographicSize;

		//disable sphere, enable capsule

		EnableElicitationMode (false);

		//escape out of current overlay
		overlayCont.EscapeOverlay ();

		LoadInspector (capChild);
	
	}

	//Function to position the item
	private void SetElicitPosition(){
		//update position transform
	}

	//Function to scale
	private void SetElicitScale(){
		//update scale transform
	}
	public bool IsInspecting(){
		return isInspecting;
	}

	public bool IsElicitating(){
		return elicitationInProgress;
	}
}
