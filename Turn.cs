using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Turn : MonoBehaviour
{
    TextMeshProUGUI _turn;

    void Awake()
    {
        _turn = GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        GameManager.Instance.TurnText = this;
    }

    public void SetWhite()
    {
        _turn.text = "White";
        _turn.color = Color.white;
    }

    public void SetBlack()
    {
        _turn.text = "Black";
        _turn.color = Color.black;
    }
}
