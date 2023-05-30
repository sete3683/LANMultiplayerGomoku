using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndPopup : MonoBehaviour
{
    public TextMeshProUGUI winnerText;

    void Start()
    {
        GameManager.Instance.EndPopup = this;
        gameObject.SetActive(false);
    }

    public void SetWinnerText(string winner)
    {
        winnerText.text = winner;
    }

    public void ExitGame()
    {
        GameManager.Instance.ReturnMain();
    }
}