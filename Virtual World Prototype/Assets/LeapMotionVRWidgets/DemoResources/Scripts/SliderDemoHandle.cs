using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VRWidgets;

public class SliderDemoHandle : SliderHandleBase 
{
  public GameObject activeBar = null;
  public GameObject dot = null;
  public int numberOfDots = 0;

  private List<GameObject> dots = new List<GameObject>();

  private void UpdateActiveBar()
  {
    activeBar.transform.localPosition = (transform.localPosition + lowerLimit.transform.localPosition) / 2.0f;
    Vector3 activeBarScale = activeBar.transform.localScale;
    activeBarScale.x = Mathf.Abs(transform.localPosition.x - lowerLimit.transform.localPosition.x);
    activeBar.transform.localScale = activeBarScale;
    Renderer[] renderers = activeBar.GetComponentsInChildren<Renderer>();
    foreach (Renderer renderer in renderers)
    {
      renderer.material.SetFloat("_Gain", 3.0f);
    }

    if (GetPercent() > 99.0f)
    {
      Renderer[] upper_limit_renderers = upperLimit.GetComponentsInChildren<Renderer>();
      foreach (Renderer renderer in upper_limit_renderers)
      {
        renderer.enabled = true;
      }
    }
    else
    {
      Renderer[] upper_limit_renderers = upperLimit.GetComponentsInChildren<Renderer>();
      foreach (Renderer renderer in upper_limit_renderers)
      {
        renderer.enabled = false;
      }
    }
  }

  private void UpdateDots()
  {
    for (int i = 0; i < dots.Count; ++i)
    {
      if (dots[i].transform.localPosition.x < transform.localPosition.x)
      {
        Renderer[] renderers = dots[i].GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
          renderer.material.color = new Color(0.0f, 1.0f, 1.0f, 1.0f);
          renderer.material.SetFloat("_Gain", 3.0f);
        }
      }
      else
      {
        Renderer[] renderers = dots[i].GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
          renderer.material.color = new Color(0.0f, 0.25f, 0.25f, 0.5f);
          renderer.material.SetFloat("_Gain", 1.0f);
        }
      }
    }
  }

  public override void UpdatePosition(Vector3 displacement)
  {
    base.UpdatePosition(displacement);
    if (activeBar)
    {
      UpdateActiveBar();
    }
    if (numberOfDots > 0) {
      UpdateDots();
    }
  }

  public override void Awake()
  {
    base.Awake();
    if (numberOfDots > 0)
    {
      float lower_limit = lowerLimit.transform.localPosition.x;
      float upper_limit = upperLimit.transform.localPosition.x;
      float length = upper_limit - lower_limit;
      float increments = length / numberOfDots;

      for (float x = lower_limit + increments / 2.0f; x < upper_limit; x += increments)
      {
        GameObject new_dot = Instantiate(dot) as GameObject;
        new_dot.transform.parent = transform;
        new_dot.transform.localPosition = new Vector3(x, 1.0f, -0.1f);
        new_dot.transform.localScale = Vector3.one;
        new_dot.transform.parent = transform.parent;
        dots.Add(new_dot);
      }
      Destroy(dot);
      UpdateDots();
    }
    if (activeBar)
    {
      UpdateActiveBar();
    }
  }
}
