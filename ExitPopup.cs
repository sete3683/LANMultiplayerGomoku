using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitPopup : MonoBehaviour
{
    public void ExitGame()
    {
        GameManager.Instance.DisconnectGame();
        GameManager.Instance.ReturnMain();
    }
}