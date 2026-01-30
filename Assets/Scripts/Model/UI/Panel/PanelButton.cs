using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelButton : UIPanel
{
    protected override void OnOpen()
    {
        Debug.Log("PanelButton opened.");
    }
    protected override void OnClose()
    {
        Debug.Log("PanelButton closed.");
    }
}
