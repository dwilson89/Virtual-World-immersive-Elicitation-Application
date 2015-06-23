using UnityEngine;
using System.Collections;
using VRWidgets;

public class SliderDemoBasicHandle : SliderHandleBase 
{
  public GameObject activeBar = null;

  private void UpdateActiveBar()
  {
    if (activeBar)
    {
      activeBar.transform.localPosition = (transform.localPosition + lowerLimit.transform.localPosition) / 2.0f;
      Vector3 activeBarScale = activeBar.transform.localScale;
      activeBarScale.x = Mathf.Abs(transform.localPosition.x - lowerLimit.transform.localPosition.x);
      activeBar.transform.localScale = activeBarScale;
    }
  }

  public override void UpdatePosition(Vector3 displacement)
  {
    base.UpdatePosition(displacement);
    UpdateActiveBar();
  }


  public override void Awake()
  {
    base.Awake();
    UpdateActiveBar();
  }
}
