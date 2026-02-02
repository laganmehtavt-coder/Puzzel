using UnityEngine;

public class GameEnums
{
    public enum ViewType
    {
        FullScreen, 
        Popup
    }

    public enum View
    {
        Splash,
        Home,
        Error,
        RoleChoose,
        Profile,
        Matchmaking,
        Waiting,
        Island
    }

    public enum Platform
    {
        Unknown,
        Android,
        Ios,
        Windows,
        MacOS,
        TvOS,
    }
    
    public enum BuildType
    {
        Dev,
        Staging,
        Production
    }

    public enum PlayerRole
    {
        Host,
        Client
    }
    
    public enum Orientation
    {
        Portrait,
        Landscape
    }

    public enum ErrorType
    {
        Unknown,
        Network,
        Firebase,
        Matchmaking
    }
    
    public enum GameState
    {
        Home,
        Waiting,
        Island,
        // Island_Reveal,
        Risks,
        // Risks_Reveal,
        Luggage,
        Luggage_Swap,
        Results,
        EndGame
    }
}
