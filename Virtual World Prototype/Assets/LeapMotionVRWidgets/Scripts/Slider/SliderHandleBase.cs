using UnityEngine;
using System.Collections;

namespace VRWidgets
{
  public class SliderHandleBase : MonoBehaviour
  {
    public GameObject upperLimit;
    public GameObject lowerLimit;

    protected Vector3 constraint_direction_;
    protected Vector3 pivot_;

    public float GetPercent()
    {
      float length = Vector3.Distance(upperLimit.transform.position, lowerLimit.transform.position);
      float value = Vector3.Distance(transform.position, lowerLimit.transform.position);
      float percent = (length > 0.0f) ? value / length : 0.0f;
      return percent * 100.0f;
    }

    public void ResetPivot()
    {
      pivot_ = transform.position;
    }

    public virtual void UpdatePosition(Vector3 displacement)
    {
      transform.position = Vector3.Project(displacement, constraint_direction_) + pivot_;
      Vector3 sliderHandlePosition = transform.localPosition;
      if (sliderHandlePosition.x < lowerLimit.transform.localPosition.x)
      {
        sliderHandlePosition.x = lowerLimit.transform.localPosition.x;
      }

      if (sliderHandlePosition.x > upperLimit.transform.localPosition.x)
      {
        sliderHandlePosition.x = upperLimit.transform.localPosition.x;
      }

      transform.localPosition = sliderHandlePosition;
    }

    // Use this for initialization
    public virtual void Awake()
    {
      pivot_ = transform.position;
      constraint_direction_ = (upperLimit.transform.position - lowerLimit.transform.position) / 2.0f;
    }
  }
}

