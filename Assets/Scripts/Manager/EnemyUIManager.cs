using UnityEngine;
using TMPro;
using System.Collections;

public class EnemyUIManager : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI hpText;
    private Coroutine hideCoroutine;
    public void ShowEnemyInfo(string name, int hpcurrent, int hpmax)
    {
        panel.SetActive(true);
        nameText.text = name;
        hpText.text = $"{hpcurrent}/{hpmax}";
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }

        hideCoroutine = StartCoroutine(HideAfterDelay());
    }
    public void HideEnemyInfo()
    {
        panel.SetActive(false);
    }
    IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(5f);

        panel.SetActive(false);
    }
}