using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void PlayWhite()
    {
        GameManager.Instance.OnSearch(1);
        NetworkManager.Instance.StartServer();
    }

    public void PlayBlack()
    {
        GameManager.Instance.OnSearch(2);
        NetworkManager.Instance.StartClient();
    }
}
