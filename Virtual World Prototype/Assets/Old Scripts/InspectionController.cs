using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InspectionController : MonoBehaviour {

	//public static InspectionController inspectCont;

	public GameObject siteViewer;
	
	public string objectInformation;

	// The elicited transform information
	public Transform TransformInfo;

	public GameObject currentObject;

	private ObjectDataProperties currentObjectData;

	private GameObject modeText;

	//void Awake()
	//{
	//	if (inspectCont == null) {
	//		DontDestroyOnLoad (gameObject);
	//		inspectCont = this;
	//	} else if (inspectCont != this) {
	//		Destroy (gameObject);
	//	}

	//}

	// Use this for initialization
	void Start () {
		//inspectCont = this;

	}
	
	// Update is called once per frame
	void Update () {
		//inspectCont = this;
	}

	//function that takes an object and populates the insdie data
	public void SetInspectionData(GameObject inspectElement){

		currentObjectData = inspectElement.GetComponent<ObjectDataProperties> ();
		TransformInfo = currentObjectData.elicitedTransformInfo;
		objectInformation = currentObjectData.elicitedInformation;
		currentObject = inspectElement;
		modeText = GameObject.FindWithTag("inspect content");
		UpdateCameraPosition (TransformInfo);
		UpdateOverlayText (objectInformation);

	}

	public void UpdateOverlayText(string text){

		modeText.GetComponent<Text> ().text = text;
		objectInformation = text;

		currentObjectData.elicitedInformation = text;
	}

	public void UpdateCameraPosition(Transform camPos){
		siteViewer = GameObject.Find ("RICamera");
		siteViewer.transform.position = camPos.position;

		//siteViewer.transform.Rotate (new Vector3 (-90f,0f,0f));

		TransformInfo = siteViewer.transform;

	}

}
