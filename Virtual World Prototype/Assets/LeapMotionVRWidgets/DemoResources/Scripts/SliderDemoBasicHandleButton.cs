using UnityEngine;
using System.Collections;
using VRWidgets;

public class SliderDemoBasicHandleButton : SliderHandleButtonBase 
{
  public GameObject graphics;

  public override void HandlePressed()
  {
    Debug.Log("HandleReleased");
  }

  public override void HandleReleased()
  {
    Debug.Log("HandlePressed");
  }
	
	public override void Update () 
  {
    base.Update();
    graphics.transform.localPosition = GetPosition();
	}
}
