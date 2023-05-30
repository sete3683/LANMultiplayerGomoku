using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stone : MonoBehaviour
{
    public enum Color
    {
        None,
        White,
        Black
    }

    [SerializeField] GameObject _whiteStone;
    [SerializeField] GameObject _blackStone;

    GameObject _stone;
    Color _color = Color.None;
    
    public bool HasStone()
    {
        return _color != Color.None;
    }

    public bool HasWhite()
    {
        return _color == Color.White;
    }

    public bool HasBlack()
    {
        return _color == Color.Black;
    }

    public void PutMyStone()
    {
        if (GameManager.Instance.MyColor == Color.White)
            PutWhiteStone();
        else
            PutBlackStone();

        GameManager.Instance.ProcessGame(name);
    }

    public void PutWhiteStone()
    {
        _stone = Instantiate(_whiteStone, transform, true);
        _stone.transform.localPosition = Vector3.zero;
        GetComponent<Button>().enabled = false;
        _color = Color.White;
    }

    public void PutBlackStone()
    {
        _stone = Instantiate(_blackStone, transform, true);
        _stone.transform.localPosition = Vector3.zero;
        GetComponent<Button>().enabled = false;
        _color = Color.Black;
    }

    public void DestroyStone()
    {
        Destroy(_stone);
    }
}
