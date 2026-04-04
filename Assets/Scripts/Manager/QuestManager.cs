using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;
    public int CurrentQuest;
    public string State;
    private Dictionary<int, string> location = new Dictionary<int, string>
    {
        {1, "Vùng đất bằng phẳng"},
        {2, "Tận cùng bản đồ bên trái"}
    };
    private void Awake()
    {
        instance = this;
    }
    public string GetLocation(int id)
    {
        return location[id];
    }
}
