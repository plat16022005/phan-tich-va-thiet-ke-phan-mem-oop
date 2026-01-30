using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIPanel : MonoBehaviour
{
    public virtual void show()
    {
        gameObject.SetActive(true);
        OnOpen();
    }
    public virtual void hide()
    {
        OnClose();
        gameObject.SetActive(false);
    }
    protected virtual void OnOpen() {}
    protected virtual void OnClose() {}
}
