using System;
using System.Collections.Generic;
using UnityEngine;
using static GameEnums;

public class GameManager : MonoBehaviour
{
    [SerializeField] private View initialView;
    
    [Space, Header("Game Configs")]
    public Orientation orientation;
    public List<string> usernames = new List<string>();
    public PlayerRole currentPlayerRole;
    
    [Space, Header("Dev Mode Settings")]
    public bool isDevMode;
    public Platform currentPlatformDevMode;
    public PlayerRole currentPlayerDevRole;
    
    private DependencyManager dependencyManager;
    private UIManager uiManager;
    private FirebaseManager firebaseManager;
    private ErrorHandler errorHandler;
    private FSMManager fsmManager;

    private void Start()
    {
        GetReferences();
        SetOrientationType();
        InitScripts();
        StartGame();
    }

    private void GetReferences()
    {
        dependencyManager = DependencyManager.Instance;
        uiManager = dependencyManager.Resolve<UIManager>();
        firebaseManager = dependencyManager.Resolve<FirebaseManager>();
        errorHandler = dependencyManager.Resolve<ErrorHandler>();
        fsmManager = dependencyManager.Resolve<FSMManager>();
    }

    private void InitStates()
    {
        fsmManager.StateDictionary[GameState.Home].InitState(GameState.Home);
        fsmManager.StateDictionary[GameState.Island].InitState(GameState.Island);
        fsmManager.StateDictionary[GameState.EndGame].InitState(GameState.EndGame);
    }
    
    private void InitScripts()
    {
        uiManager.Init();
        firebaseManager.Init();
        errorHandler.Init();
        fsmManager.Init();
        InitStates();
    }
    
    private void StartGame()
    {
        uiManager.ShowView(initialView);
    }
    
    private void SetOrientationType()
    {
        if (currentPlatformDevMode == Platform.Android || currentPlatformDevMode == Platform.Ios)
            orientation = Orientation.Portrait;
        else
            orientation = Orientation.Landscape;
        
        Debug.Log($"Orientation logic applied. Mode: {orientation}");
    }

    public void GetRandomName()
    {
        if(PlayerPrefs.HasKey(Constants.PlayerName)) return;
        int randomIndex = UnityEngine.Random.Range(0, usernames.Count);
        PlayerPrefs.SetString(Constants.PlayerName, usernames[randomIndex]);
    }
}
