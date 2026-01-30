using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GetData : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    public void onClick()
    {
        if (inputField != null)
        {
            Debug.Log(inputField.text);
        }
    }
}
