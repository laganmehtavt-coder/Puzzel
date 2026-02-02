using UnityEngine;

public static class DBPaths
{
    public const string ROOMS = "rooms";
    public const string USERS = "users";
    
    // Path Builders
    public static string Room(string code) => $"{ROOMS}/{code}";
    public static string State(string code) => $"{ROOMS}/{code}/currentState";
    public static string Players(string code) => $"{ROOMS}/{code}/currentPlayers";
    public static string Player(string code, string uid) => $"{ROOMS}/{code}/currentPlayers/{uid}";

    public static string Users(string code) => $"{USERS}/{code}";
    
    public static string DataIsland(string code) => $"{ROOMS}/{code}/data_island";
    public static string DataRisks(string code) => $"{ROOMS}/{code}/data_risks";
    public static string DataLuggage(string code) => $"{ROOMS}/{code}/data_luggage";
    
}
