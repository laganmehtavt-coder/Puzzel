using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;
using Newtonsoft.Json;
using UnityEngine;
using static GameEnums;


public class FirebaseManager : MonoBehaviour
{
    //Script References
    private DependencyManager dependencyManager;
    private ErrorHandler errorHandler;

    private BuildConfiguration buildConfiguration;

    private DatabaseReference firebaseDB;
    private FirebaseAuth firebaseAuth;
    private FirebaseUser currentUser;
    private FSMManager fsmManager;
    private bool isFirebaseInitialized = false;

    public event Action<GameState> OnGameStateChanged;
    public event Action<string> OnGameDataReceived;

    // Internal Trackers
    private DatabaseReference _activeStateListener;
    private DatabaseReference _activeDataListener;
    public string CurrentRoomCode { get; private set; }
    public string CurrentUserId 
    {
        get 
        {
            if (currentUser != null) return currentUser.UserId;
            return SystemInfo.deviceUniqueIdentifier; 
        }
    }

    public string CurrentUserRole => PlayerPrefs.GetInt(Constants.PlayerRole, (int) PlayerRole.Host).ToString();

    #region Initialization

    public void Init()
    {
        dependencyManager = DependencyManager.Instance;
        errorHandler = dependencyManager.Resolve<ErrorHandler>();
        buildConfiguration = dependencyManager.Resolve<BuildConfiguration>();
        fsmManager = dependencyManager.Resolve<FSMManager>();
    }


    public async Task<bool> InitializeFirebase()
    {
        try
        {
            var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
            if (dependencyStatus == DependencyStatus.Available)
            {
                firebaseDB = FirebaseDatabase.GetInstance(Constants.DB_URL).RootReference;
                firebaseAuth = FirebaseAuth.DefaultInstance;
                if (firebaseAuth.CurrentUser == null)
                {
                    Debug.Log("Logging in anonymously...");
                    var authResult = await firebaseAuth.SignInAnonymouslyAsync();
                    currentUser = authResult.User;
                }
                else
                {
                    currentUser = firebaseAuth.CurrentUser;
                    Debug.Log("Already logged in.");
                }
                isFirebaseInitialized = true;
                Debug.Log("✅ Firebase Initialized.");
            }
            else
            {
                isFirebaseInitialized = false;
                errorHandler.HandleError(ErrorType.Firebase, "Firebase dependency issue");
                Debug.LogError($"❌ Firebase dependency issue: {dependencyStatus}");
            }
        }
        catch (Exception ex)
        {
            isFirebaseInitialized = false;
            Debug.LogError($"❌ Inside exception:  {ex.Message}");
            errorHandler.HandleError(ErrorType.Firebase, "Firebase initialization failed", true);
        }

        return isFirebaseInitialized;
    }

    #endregion

    #region Profile Management

    public async Task<bool> CreateOrUpdateUserProfile(string playerName)
    {
        if (!isFirebaseInitialized) return false;
        string userId = CurrentUserId;
        PlayerData player = new PlayerData { username = playerName, isReady = false, playerId = userId };
        string json = JsonConvert.SerializeObject(player);
        await GetRef(DBPaths.Users(userId)).SetRawJsonValueAsync(json);
        Debug.Log($"Profile created or updated for user: {userId}");
        return true;
    }

    public async Task<bool> UpdatePlayerRole(string userId, PlayerRole newRole)
    {
        if(!isFirebaseInitialized) return false;
        bool isHost = newRole == PlayerRole.Host;
        await GetRef(DBPaths.Users(userId)).Child(nameof(PlayerData.isHost)).SetValueAsync(isHost);
        PlayerPrefs.SetInt(Constants.PlayerRole, (int) newRole);
        return true;
    }

    #endregion

    #region Room Management

    
    public string CreateRoom(Action<bool> onComplete = null)
    {
        if (!isFirebaseInitialized) return null;
        string newRoomId = $"{Guid.NewGuid().ToString("N").Substring(0, 6)}";
        PlayerData hostPlayer = new PlayerData
        {
            username = PlayerPrefs.GetString(Constants.PlayerName, "Host"),
            isReady = true,
            playerId = CurrentUserId,
            isHost = true,
            version = Application.version,
            currentState = fsmManager != null ? fsmManager.CurrentState.ToString() : "Lobby",
            totalScore = 0
        };
        RoomSchema newRoom = new RoomSchema
        {
            roomId = newRoomId,
            currentState = "Lobby",
            version = Application.version,
            isPaused = false,
            timerValue = 10,
            currentRound = 1,
            hostData = hostPlayer,
            currentPlayers = new Dictionary<string, PlayerData>
            {
                { CurrentUserId, hostPlayer } 
            },
            waitingPlayers = new Dictionary<string, PlayerData>(),
            data_island = new Dictionary<string, IslandData>(),
            data_risks = new Dictionary<string, RisksData>(),
            data_luggage = new Dictionary<string, LuggageData>()
        };
        newRoom.currentState = GameState.Home.ToString();
        string json = JsonConvert.SerializeObject(newRoom);
        GetRef(DBPaths.Room(newRoom.roomId)).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("❌ Failed to create room: " + task.Exception);
                errorHandler.HandleError(ErrorType.Network, "Failed to create room.", true);
                onComplete?.Invoke(false);
            }
            else
            {
                Debug.Log($"✅ Room {newRoom.roomId} Created");
                CurrentRoomCode = newRoom.roomId;
                StartListeningToRoomState(newRoom.roomId);
                onComplete?.Invoke(true);
            }
        });
        return newRoomId;
    }

    public void JoinRoom(string roomCode, Action<bool> onComplete = null)
    {
        if (!isFirebaseInitialized) return;

        GetRef(DBPaths.Room(roomCode)).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || !task.Result.Exists)
            {
                Debug.LogError("❌ Room not found.");
                errorHandler.HandleError(ErrorType.Matchmaking, "Room Not Found. Please check the code and try again", true);
                onComplete?.Invoke(false);
                return;
            }

            PlayerData newPlayer = new PlayerData 
            { 
                username = currentUser.DisplayName, 
                isReady = true, 
                playerId = CurrentUserId, 
                isHost = false
            };
            string playerJson = JsonConvert.SerializeObject(newPlayer);
            GetRef(DBPaths.Player(roomCode, CurrentUserId)).SetRawJsonValueAsync(playerJson)
                .ContinueWithOnMainThread(joinTask =>
                {
                    if (joinTask.IsCompleted)
                    {
                        Debug.Log("✅ Joined Room Successfully");
                        CurrentRoomCode = roomCode;
                        StartListeningToRoomState(roomCode);
                        onComplete?.Invoke(true);
                    }
                    else
                    {
                        Debug.LogError("❌ Failed to write player data.");
                        onComplete?.Invoke(false);
                    }
                });
        });
    }

    #endregion

    #region Host Logic

    public void SetGameState(GameState newState)
    {
        if (string.IsNullOrEmpty(CurrentRoomCode)) return;
        GetRef(DBPaths.State(CurrentRoomCode)).SetValueAsync(newState.ToString())
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted) Debug.LogError("❌ Failed to set state");
            });
    }

    public void TriggerLuggageSwap()
    {
        if (string.IsNullOrEmpty(CurrentRoomCode)) return;

        GetRef(DBPaths.Players(CurrentRoomCode)).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (!task.Result.Exists) return;

            var players = JsonConvert.DeserializeObject<Dictionary<string, PlayerData>>(task.Result.GetRawJsonValue());
            List<string> ids = players.Keys.ToList();

            if (ids.Count < 2) return;

            Dictionary<string, object> updates = new Dictionary<string, object>();
            string root = GetRootPath();
            string statePath = $"{root}/{DBPaths.State(CurrentRoomCode)}";
            updates[statePath] = GameState.Luggage_Swap.ToString();
            for (int i = 0; i < ids.Count; i++)
            {
                string ownerId = ids[i];
                string viewerId = ids[(i + 1) % ids.Count];
                string lugPath = $"{root}/{DBPaths.DataLuggage(CurrentRoomCode)}/{ownerId}/assigned_viewer_id";
                updates[lugPath] = viewerId;
            }

            firebaseDB.UpdateChildrenAsync(updates);
        });
    }

    #endregion

    #region Listeners

    private void StartListeningToRoomState(string roomCode)
    {
        if (_activeStateListener != null) _activeStateListener.ValueChanged -= OnStateUpdated;
        _activeStateListener = GetRef(DBPaths.State(roomCode));
        _activeStateListener.ValueChanged += OnStateUpdated;
    }

    private void OnStateUpdated(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null || !args.Snapshot.Exists) return;
        Debug.Log("State Updated");
        string stateStr = args.Snapshot.Value.ToString();
        if (Enum.TryParse(stateStr, out GameState newState))
        {
            OnGameStateChanged?.Invoke(newState);
            Debug.Log($"State changed to : {newState}");
            // SwitchDataListeners(newState);
        }
    }

    // private void SwitchDataListeners(GameState newState)
    // {
    //     if (_activeDataListener != null)
    //     {
    //         _activeDataListener.ValueChanged -= OnDataUpdated;
    //         _activeDataListener = null;
    //     }
    //
    //     switch (newState)
    //     {
    //         case GameState.Island_Reveal:
    //             _activeDataListener = GetRef(DBPaths.DataIsland(CurrentRoomCode));
    //             break;
    //         case GameState.Risks_Reveal:
    //             _activeDataListener = GetRef(DBPaths.DataRisks(CurrentRoomCode));
    //             break;
    //         case GameState.Luggage_Swap:
    //             ListenForMyAssignedLuggage();
    //             return;
    //     }
    //
    //     if (_activeDataListener != null)
    //     {
    //         _activeDataListener.ValueChanged += OnDataUpdated;
    //     }
    // }

    private void ListenForMyAssignedLuggage()
    {
        var luggageRef = GetRef(DBPaths.DataLuggage(CurrentRoomCode));
        luggageRef.OrderByChild("assigned_viewer_id").EqualTo(CurrentUserId).ValueChanged += OnDataUpdated;
    }

    private void OnDataUpdated(object sender, ValueChangedEventArgs args)
    {
        if (args.Snapshot.Exists)
        {
            string json = args.Snapshot.GetRawJsonValue();
            OnGameDataReceived?.Invoke(json);
        }
    }

    public void StopAllListeners()
    {
        if (_activeStateListener != null) _activeStateListener.ValueChanged -= OnStateUpdated;
        if (_activeDataListener != null) _activeDataListener.ValueChanged -= OnDataUpdated;
    }

    #endregion

    #region Utils

    private string GetRootPath()
    {
        BuildType currentEnvironment = buildConfiguration.BuildType;
        switch (currentEnvironment)
        {
            case BuildType.Dev: return Constants.ROOT_DEV;
            case BuildType.Staging: return Constants.ROOT_STAGING;
            case BuildType.Production: return Constants.ROOT_PROD;
            default: return Constants.ROOT_DEV;
        }
    }

    public DatabaseReference GetRef(string nodeName)
    {
        if (!isFirebaseInitialized || firebaseDB == null)
        {
            Debug.LogError("Firebase not initialized!");
            return null;
        }

        string root = GetRootPath();
        return firebaseDB.Child(root).Child(nodeName);
    }

    #endregion

    #region Test Functions

    public void SendTestData()
    {
        if (!isFirebaseInitialized || firebaseDB == null)
        {
            Debug.LogError("❌ Cannot send data: Firebase is not initialized.");
            if (errorHandler != null)
                errorHandler.HandleError(ErrorType.Firebase, "Wait for initialization!", false);
            return;
        }

        string deviceId = SystemInfo.deviceUniqueIdentifier;
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        DatabaseReference testRef = GetRef("test_connection").Child(deviceId);
        Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "status", "Connected" },
            { "timestamp", timestamp },
            { "message", "Hello from Unity!" }
        };

        Debug.Log("⏳ Sending test data...");

        testRef.SetValueAsync(data).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError($"❌ Write Failed: {task.Exception}");
                errorHandler.HandleError(ErrorType.Network, "Failed to send test data.");
            }
            else if (task.IsCompleted)
            {
                Debug.Log($"✅ Success! Data sent to: test_connection/{deviceId}");
            }
        });
    }

    #endregion
}