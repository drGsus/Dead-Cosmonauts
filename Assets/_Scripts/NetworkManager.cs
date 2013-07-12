﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public interface IPlayer
    {
        void Move(Vector3 moveAmount);
        void Look(float direction);
        void Shoot(float direction);
    }
    public interface IJoinGameHandler
    {
        IPlayer OnPlayerJoined(NetworkPlayer player);
    }

    class PlayerData
    {
        public IPlayer player;
        public int lastCommunicationsTurn;
    }

    public IJoinGameHandler joinGameHandler;
    public int communicationsTurn;
    public int simulationTurn;
    public int simulationDelay = 2;
    public float tickrate = 0.03333333333f;
    public float timeSinceLastEvaluation = 0.0f;
    public List<GameEvent> events = new List<GameEvent>();

    readonly Dictionary<string, PlayerData> _playerDatas = new Dictionary<string, PlayerData>();
    public string gameTypeName = "Dead-Cosmonauts";
    public string gameName = "Singleton";

    public class GameEvent
    {
        public string playerGuid;
        public int turn;
        public bool processed = false;
    }

    public class MoveEvent : GameEvent
    {
        public Vector3 move;
    }

    void Awake()
    {
        networkView.group = 1;
    }

    void Start()
    {
        StartCoroutine(InitializeNetwork());
    }

    IEnumerator InitializeNetwork()
    {
        MasterServer.RequestHostList(gameTypeName);
        yield return new WaitForEndOfFrame();
        HostData[] hosts = MasterServer.PollHostList();
        
        if (hosts == null || hosts.Length == 0)
        {
            CreateGame();
        }
        else
        {
            JoinGame(hosts[0]);
        }
    }

    private void CreateGame()
    {
        Network.InitializeServer(32, 25001, !Network.HavePublicAddress());
    }

    private void JoinGame(HostData host)
    {
        Network.Connect(host);
    }

    private void OnServerInitialized()
    {
        MasterServer.RegisterHost(gameTypeName, gameName);
    }

    private void OnMasterServerEvent(MasterServerEvent e)
    {
        if (e == MasterServerEvent.RegistrationFailedNoServer ||
            e == MasterServerEvent.RegistrationFailedGameType ||
            e == MasterServerEvent.RegistrationFailedGameName)
        {
            CreateGame();
        }
    }

    private void OnConnectedToServer()
    {
        networkView.RPC("JoinedGame",RPCMode.OthersBuffered,networkView.owner);
    }

    public void MoveMe(Vector3 move, bool shouldPredict = false)
    {
        networkView.RPC("MovePlayer",RPCMode.OthersBuffered,communicationsTurn,move);
        // Predict me immediately.
        if (shouldPredict)
        {
            _playerDatas[networkView.owner.guid].player.Move(move);
        }
    }

    public void FireMe(float direction, bool shouldPredict = false)
    {
        networkView.RPC("FirePlayer",RPCMode.OthersBuffered,communicationsTurn,direction);
        if (shouldPredict)
        {
            _playerDatas[networkView.owner.guid].player.Shoot(direction);
        }
    }

    [RPC]
    public void JoinedGame(NetworkPlayer joinedPlayer)
    {
        _playerDatas[joinedPlayer.guid] = new PlayerData()
            {
                lastCommunicationsTurn = 0,
                player = joinGameHandler.OnPlayerJoined(joinedPlayer)
            };
    }

    [RPC]
    public void StartingGameData(int oldestCommunicationsTurn)
    {
        if (communicationsTurn < oldestCommunicationsTurn)
        {
            communicationsTurn = oldestCommunicationsTurn;
        }
    }

    [RPC]
    public void MovePlayer(NetworkPlayer inPlayer, int inTurn, Vector3 inMove)
    {
        events.Add(new MoveEvent()
            {
                move = inMove,
                playerGuid = inPlayer.guid,
                turn = inTurn
            });
    }

    [RPC]
    public void FirePlayer(NetworkPlayer inPlayer, int inTurn, float direction)
    {
        events.Add(new FireEvent()
            {
                turn = inTurn,
                playerGuid = inPlayer.guid,
                direction = direction
            });
    }

    public class FireEvent : GameEvent
    {
        public float direction;
    }

    [RPC]
    public void ReachedTurn(NetworkPlayer inPlayer, int reachedTurn)
    {
        _playerDatas[inPlayer.guid].lastCommunicationsTurn = reachedTurn;
        float playerTickrate = (float)Network.GetAveragePing(inPlayer) * 0.001f;
        float connections = (float)Network.connections.Length;
        tickrate = tickrate*(connections - 1.0f)/(connections) + playerTickrate*(1.0f/connections);
    }

    void Update()
    {
        timeSinceLastEvaluation += Time.smoothDeltaTime;
        int lastTurn = communicationsTurn;

        if (timeSinceLastEvaluation >= tickrate)
        {
            communicationsTurn++;
            timeSinceLastEvaluation -= tickrate;
        }
        if (communicationsTurn != lastTurn)
        {
            networkView.RPC("ReachedTurn",RPCMode.OthersBuffered,networkView.owner,communicationsTurn);
        }

        bool playersUpToSpeed = true;
        foreach (PlayerData player in _playerDatas.Values)
        {
            if (player.lastCommunicationsTurn < communicationsTurn - simulationDelay)
            {
                playersUpToSpeed = false;
                break;
            }
        }

        if (playersUpToSpeed && simulationTurn < communicationsTurn - simulationDelay)
        {
            Evaluate();
        }
    }

    public void Evaluate()
    {
        List<GameEvent> eventsToEvaluate = events.FindAll(e => e.turn <= communicationsTurn - simulationDelay);

        int latestProcessed = -1;

        foreach (GameEvent e in eventsToEvaluate)
        {
            MoveEvent moveEvent = e as MoveEvent;
            if (moveEvent != null && _playerDatas.ContainsKey(moveEvent.playerGuid))
            {
                _playerDatas[moveEvent.playerGuid].player.Move(moveEvent.move);
                moveEvent.processed = true;
                if (latestProcessed < moveEvent.turn)
                {
                    latestProcessed = moveEvent.turn;
                }
            }
        }
        
        if (latestProcessed > simulationTurn)
        {
            simulationTurn = latestProcessed;
        }

        events.RemoveAll(e => e.processed = true);
    }
}