using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class RoomSchema
{
    public string roomId;
    public string currentState;
    public string version;
    public bool isPaused;
    public float timerValue;      
    public int currentRound;
    public PlayerData hostData;
    public Dictionary<string, PlayerData> currentPlayers;
    public Dictionary<string, PlayerData> waitingPlayers;
    public Dictionary<string, IslandData> data_island;
    public Dictionary<string, RisksData> data_risks;
    public Dictionary<string, LuggageData> data_luggage;
}


[Serializable]
public class PlayerData
{
    public string playerId;
    public string username;
    public bool isReady;
    public int totalScore;
    public bool isHost;
    public string currentState;
    public string version;
}


[Serializable]
public class IslandData
{
    public string fauna;  
    public string sauna;
    public string climate;
}

[Serializable]
public class RisksData
{
    public string riskText;
    public string benefitText;
}

[Serializable]
public class LuggageData
{
    public string itemMust;
    public string itemShould;
    public string itemCould;

    // Phase 2: The Swap Logic
    // If this is "User_B", it means User_B is currently looking at THIS luggage.
    // (Used by the Host to shuffle assignments)
    public string assignedViewerId; 
}