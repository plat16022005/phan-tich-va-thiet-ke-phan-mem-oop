using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    [SerializeField] private AudioClip[] soundEffect;
    public AudioSource audioSource;
    public void ClickButton(int index)
    {
        audioSource.PlayOneShot(soundEffect[index]);
    }
}
