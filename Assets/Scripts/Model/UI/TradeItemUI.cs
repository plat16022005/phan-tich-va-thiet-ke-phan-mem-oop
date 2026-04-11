using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradeItemUI : MonoBehaviour
{
    public TextMeshProUGUI txtName;
    private Characters characterData;

    public void Init(Characters character, ulong clientId)
    {
        characterData = character;
        txtName.text = character.nickname;

        GetComponent<Button>().onClick.AddListener(() => GameUI.instance.TradeWithPlayer(clientId, characterData));
    }
}
