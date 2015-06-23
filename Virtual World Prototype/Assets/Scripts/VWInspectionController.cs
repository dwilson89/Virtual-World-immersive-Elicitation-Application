using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/*
**Author: Dustin Wilson
**Date: 18/05/2015
**Description: A script that Sets up the data displayed on the inspection overlay
**Note: Remove this script and add its functionality to the OverlayController Script
**/
public class VWInspectionController : MonoBehaviour {

	public GameObject siteViewer;// The Reef inspector Camera
	
	public string elicitedInfo;
	
	// The elicited transform information
	public Transform TransformInfo;
	
	public GameObject currentObject;
	
	private ObjectDataProperties currentObjectData;
	
	private GameObject modeText;

	// Use this for initialization
	void Start () {
		elicitedInfo = "";
	}

	/** Function: SetInspectionData
	 ** Param: GameObject, element being inspected 
	 ** Purpose: Set up the inspection overlay, with data obtained from the element to be inspected
	 */
	public void SetInspectionData(GameObject inspectElement){
		
		currentObjectData = inspectElement.GetComponent<ObjectDataProperties> ();
		TransformInfo = currentObjectData.elicitedTransformInfo;
		elicitedInfo = currentObjectData.elicitedInformation;
		currentObject = inspectElement;
		modeText = GameObject.FindWithTag("scroll text");
		UpdateCameraPosition (TransformInfo);
		UpdateOverlayText (elicitedInfo);
		
	}

	/** Function: UpdateOverlayText
	 ** Param: string, updated elicited data 
	 ** Purpose: Update the text displayed, with the edit/added information
	 */
	public void UpdateOverlayText(string text){
		
		modeText.GetComponent<TextMesh> ().text = text;
		elicitedInfo = text;
		
		currentObjectData.elicitedInformation = text;
	}

	/** Function: UpdateCameraPosition
	 ** Param: The new transform for the reef inspector camera
	 ** Purpose: Update the reef inspection camera, to the current objects position
	 */
	public void UpdateCameraPosition(Transform camPos){
		siteViewer = GameObject.Find ("RICamera");
		siteViewer.transform.position = camPos.position;
		
		//siteViewer.transform.Rotate (new Vector3 (-90f,0f,0f));
		
		TransformInfo = siteViewer.transform;
		
	}
}
