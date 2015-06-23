using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AddNewListenersToButtons : MonoBehaviour {

	public Button save;
	public Button load;
	public Button exit;

	// Use this for initialization
	void Start () {
		save.GetComponent<Button> ().onClick.AddListener (() => DataContoller.control.Save ());
		load.GetComponent<Button> ().onClick.AddListener (() => DataContoller.control.Load ());
		exit.GetComponent<Button> ().onClick.AddListener (() => DataContoller.control.Quit ());
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
