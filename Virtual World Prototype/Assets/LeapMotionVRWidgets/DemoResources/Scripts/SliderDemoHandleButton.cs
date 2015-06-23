using UnityEngine;
using System.Collections;
using VRWidgets;

public class SliderDemoHandleButton : SliderHandleButtonBase 
{
  public SliderDemoGraphics topLayer;
  public SliderDemoGraphics midLayer;
  public SliderDemoGraphics botLayer;

  public override void HandlePressed()
  {
    PressedGraphics();
  }

  public override void HandleReleased()
  {
    ReleasedGraphics();
  }

  private void PressedGraphics()
  {
    topLayer.SetBloomGain(5.0f);
    botLayer.SetBloomGain(4.0f);
    botLayer.SetColor(new Color(0.0f, 1.0f, 1.0f, 1.0f));
  }

  private void ReleasedGraphics()
  {
    topLayer.SetBloomGain(2.0f);
    botLayer.SetBloomGain(2.0f);
    botLayer.SetColor(new Color(0.0f, 0.25f, 0.25f, 0.5f));
  }

  private void UpdateGraphics()
  {
    Vector3 position = GetPosition();
    position.z -= (triggerDistance + 0.01f);

    topLayer.transform.localPosition = position - new Vector3(0.0f, 0.0f, 0.01f + 0.25f * (1 - GetPercent()));
    botLayer.transform.localPosition = position;
    midLayer.transform.localPosition = (topLayer.transform.localPosition + botLayer.transform.localPosition) / 2.0f;
  }

  public override void Awake()
  {
    base.Awake();
    ReleasedGraphics();
  }

  public override void Update()
  {
    base.Update();
    UpdateGraphics();
  }
}
