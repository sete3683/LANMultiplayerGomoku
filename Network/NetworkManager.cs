using Core;
using Client;
using Server;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Concurrent;
using System;

public class NetworkManager : MonoBehaviour
{
    #region Singleton
    static NetworkManager _instance = null;

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

    public static NetworkManager Instance { get { return _instance; } }
    #endregion

    public enum Status
    {
        None,
        Client,
        Server
    }

    public int searchPort;
    public int listenPort;

    ClientHost _client;
    ServerHost _server;

    Status _myStatus = Status.None;
    ConcurrentQueue<Action> _jobQueue = new ConcurrentQueue<Action>();

    public ConcurrentQueue<Action> JobQueue { get { return _jobQueue; } }

    void Update()
    {
        while (_jobQueue.TryDequeue(out Action action))
            action.Invoke();
    }

    public void StartClient()
    {
        _client = new ClientHost();
        _client.Init(searchPort);
        _myStatus = Status.Client;
    }

    public void StopSearchClient()
    {
        _client.connector.StopSearch();
    }

    public void StartServer()
    {
        _server = new ServerHost();
        _server.Init(searchPort, listenPort);
        _myStatus = Status.Server;
    }

    public void StopSearchServer()
    {
        _server.listener.StopSearch();
    }

    public void StopListenServer()
    {
        _server.listener.StopListen();
    }

    public void Disconnect()
    {
        switch (_myStatus)
        {
            case Status.Client:
                DisconnectClient();
                break;

            case Status.Server:
                DisconnectServer();
                break;

            default:
                break;
        }
    }

    void DisconnectClient()
    {
        _client.session.Disconnect();
        _myStatus = Status.None;
    }

    void DisconnectServer()
    {
        _server.room.Clear();
        _myStatus = Status.None;
    }

    public void SendPacket(Packet packet)
    {
        switch (_myStatus)
        {
            case Status.Client:
                SendUnicastPacketClient(packet);
                break;

            case Status.Server:
                SendBroacastPacketServer(packet);
                break;

            default:
                break;
        }
    }

    public void SendUnicastPacketClient(Packet packet)
    {
        _client.session.SendPacket(packet);
    }

    public void SendBroacastPacketServer(Packet packet)
    {
        _server.room.Broadcast(packet);
    }
}
