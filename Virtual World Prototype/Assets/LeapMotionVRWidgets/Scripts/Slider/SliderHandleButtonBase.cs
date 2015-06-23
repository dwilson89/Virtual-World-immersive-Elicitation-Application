using UnityEngine;
using System.Collections;

namespace VRWidgets
{
  public abstract class SliderHandleButtonBase : ButtonBase
  {
    public HandDetector handDetector;
    public SliderHandleBase sliderHandle;

    private Vector3 target_pivot_ = Vector3.zero;

    public abstract void HandlePressed();
    public abstract void HandleReleased();

    public override void ButtonPressed()
    {
      if (handDetector.target)
      {
        sliderHandle.ResetPivot();
        target_pivot_ = handDetector.target.transform.position;
      }
      HandlePressed();
    }

    public override void ButtonReleased()
    {
      handDetector.ResetTarget();
      HandleReleased();
    }

    // Update is called once per frame
    public override void Update()
    {
      base.Update();
      if (is_pressed_)
      {
        if (handDetector.target)
        {
          sliderHandle.UpdatePosition(handDetector.target.transform.position - target_pivot_);
        }
      }
    }
  }
}

