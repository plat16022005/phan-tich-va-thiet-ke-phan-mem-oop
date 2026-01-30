using System;
using System.Collections.Generic;
using UnityEngine;

public class PanelController : MonoBehaviour
{
    public static PanelController Instance;

    private Dictionary<Type, UIPanel> panels = new Dictionary<Type, UIPanel>();
    private UIPanel currentPanel;

    [Header("Default Panel")]
    [SerializeField] private UIPanel defaultPanel;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        UIPanel[] panelList = GetComponentsInChildren<UIPanel>(true);

        foreach (var panel in panelList)
        {
            panels.Add(panel.GetType(), panel);
            panel.hide(); // Ẩn toàn bộ trước
        }
    }

    private void Start()
    {
        if (defaultPanel != null)
        {
            Show(defaultPanel.GetType());
        }
        else
        {
            Debug.LogWarning("PanelController: Default Panel is not assigned!");
        }
    }

    // ===== API dùng cho Button / code khác =====
    public void Show<T>() where T : UIPanel
    {
        Show(typeof(T));
    }

    public void Show(Type panelType)
    {
        if (currentPanel != null)
            currentPanel.hide();

        if (panels.TryGetValue(panelType, out UIPanel panel))
        {
            panel.show();
            currentPanel = panel;
        }
        else
        {
            Debug.LogError($"Panel {panelType.Name} not found!");
        }
    }
}
