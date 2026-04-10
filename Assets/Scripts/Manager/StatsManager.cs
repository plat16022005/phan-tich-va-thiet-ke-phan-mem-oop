using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public static StatsManager instance;
    public int potentialPoint;
    public int[] stats;
    private void Awake()
    {
        instance = this;
    }
}
