using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;
    public int CurrentQuest;
    public string State;
    public Dictionary<int, string> location = new Dictionary<int, string>
    {
        {1, "Vùng đất bằng phẳng"},
        {2, "Tận cùng bản đồ bên trái"}
    };
    public Dictionary<int, int> rewards = new Dictionary<int, int>
    {
        {1, 10},
        {2, 15}  
    };
    public Dictionary<ulong, int> playerProcess = new Dictionary<ulong, int>();
    private void Awake()
    {
        instance = this;
    }
    public void AddProcess(ulong clientId, int amount)
    {
        if (!playerProcess.ContainsKey(clientId))
            playerProcess[clientId] = 0;

        playerProcess[clientId] += amount;
    }

    // public int GetProcess(ulong clientId)
    // {
    //     return playerProcess.ContainsKey(clientId) ? playerProcess[clientId] : 0;
    // }
}
