using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataNetworkService : MonoBehaviour
{
    public static DataNetworkService Instance { get; private set;}
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    public CustomData CreateCustomData(int hair, int eyes, int nose, int mouth, int race, int @class)
    {
        CustomData data = new CustomData();
        data.hair = hair;
        data.eyes = eyes;
        data.nose = nose;
        data.race = race;
        data.mouth = mouth;
        data.@class = @class;
        return data;
    }
}
