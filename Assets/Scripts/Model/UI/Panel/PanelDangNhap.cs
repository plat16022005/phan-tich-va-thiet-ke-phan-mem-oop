using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelDangNhap : UIPanel
{
    protected override void OnOpen()
    {
        Debug.Log("PanelDangNhap opened.");
    }
    protected override void OnClose()
    {
        Debug.Log("PanelDangNhap closed.");
    }
}
