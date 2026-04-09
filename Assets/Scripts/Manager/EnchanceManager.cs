using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnchanceManager : MonoBehaviour
{
    public static EnchanceManager instance;
    public int RequiredQuantity = 10;
    private void Awake()
    {
        instance = this;
    }
}
