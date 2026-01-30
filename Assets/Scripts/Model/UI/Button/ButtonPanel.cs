using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPanel : UIButton
{
    [SerializeField] private UIPanel panelTarget;
    public override void OnClick()
    {
        PanelController.Instance.Show(panelTarget.GetType());
    }
}
