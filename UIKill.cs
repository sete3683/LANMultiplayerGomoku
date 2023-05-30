using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIKill : MonoBehaviour
{
    void Start()
    {
        GameManager.Instance.EndUIKill = this;
    }
}