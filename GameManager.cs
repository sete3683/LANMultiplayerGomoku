using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Net.Sockets;
using System.Net;
using Core;
using System.Collections.Concurrent;
using System;

public class GameManager : MonoBehaviour
{
    #region Singleton
    static GameManager _instance = null;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static GameManager Instance { get { return _instance; } }
    #endregion

    Board _board;
    SceneLoader _sceneLoader;

    Stone.Color _myColor;
    Stone.Color _currentTurn;

    Turn _turnText;
    ConcurrentQueue<Action> _jobQueue = new ConcurrentQueue<Action>();

    EndPopup _endPopup;
    UIKill _endUIKill;

    public Board Board { get { return _board; } set { _board = value; } }
    public SceneLoader SceneLoader { get { return _sceneLoader; } set { _sceneLoader = value; } }
    public Turn TurnText { get { return _turnText; } set { _turnText = value; } }
    public Stone.Color MyColor { get { return _myColor; } }
    public ConcurrentQueue<Action> JobQueue { get { return _jobQueue; } }
    public EndPopup EndPopup { get { return _endPopup; } set { _endPopup = value; } }
    public UIKill EndUIKill { set { _endUIKill = value; } }

    void Update()
    {
        while (_jobQueue.TryDequeue(out Action action))
            action.Invoke();
    }

    public void OnSearch(int color)
    {
        _myColor = (Stone.Color)color;
    }

    public void OnReady()
    {
        _sceneLoader.LoadScene(1);
    }

    public void StartGame()
    {
        StartPacket startPacket = new StartPacket();
        NetworkManager.Instance.SendPacket(startPacket);
    }

    public void OnStart()
    {
        _currentTurn = Stone.Color.Black;
        TryUnblock();
    }

    void TryUnblock()
    {
        if (_myColor == _currentTurn)
            _board.Unblock();
    }

    public void ProcessGame(string sendData)
    {
        _board.Block();

        ProcessPacket processPacket = new ProcessPacket() { data = sendData };
        NetworkManager.Instance.SendPacket(processPacket);

        MoveTurn();
    }

    public void OnProcess((int r, int c) pos)
    {
        if (_currentTurn == Stone.Color.Black)
            _board.board[pos.r, pos.c].PutBlackStone();
        else
            _board.board[pos.r, pos.c].PutWhiteStone();

        MoveTurn();
    }

    public void MoveTurn()
    {
        if (!CheckWin())
        {
            if (_currentTurn == Stone.Color.Black)
            {
                _currentTurn = Stone.Color.White;
                _turnText.SetWhite();
            }
            else
            {
                _currentTurn = Stone.Color.Black;
                _turnText.SetBlack();
            }

            TryUnblock();
        }
    }

    public bool CheckWin()
    {
        if (_board.CheckBoard())
        {
            DisconnectGame();
            return true;
        }

        return false;
    }
    public void DisconnectGame()
    {
        NetworkManager.Instance.Disconnect();
    }

    public void OnDisconnect()
    {
        _endUIKill.gameObject.SetActive(false);
        _endPopup.gameObject.SetActive(true);
        _endPopup.SetWinnerText(_currentTurn == Stone.Color.Black ? "Black" : "White");
    }

    public void ReturnMain()
    {
        _board.DestroyBoard();
        _board = null;
        _sceneLoader.LoadScene(0);
    }

    public void Log(string log)
    {
        Debug.Log(log);
    }
}
