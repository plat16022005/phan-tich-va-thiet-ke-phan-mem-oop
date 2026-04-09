using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsManager : MonoBehaviour
{
    public static ItemsManager instance;
    public List<string> NameItems;
    public List<Sprite> ImageItems;
    private void Awake()
    {
        instance = this;
    }
}
