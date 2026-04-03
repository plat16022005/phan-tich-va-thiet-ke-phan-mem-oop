using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CollectItem : MonoBehaviour
{
    [SerializeField] private TextMeshPro text;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            text.gameObject.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            text.gameObject.SetActive(false);
        }
    }
}
