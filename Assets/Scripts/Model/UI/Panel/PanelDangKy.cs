using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelDangKy : UIPanel
{
    protected override void OnOpen()
    {
        Debug.Log("PanelDangKy opened.");
    }
    protected override void OnClose()
    {
        Debug.Log("PanelDangKy closed.");
    }
}
