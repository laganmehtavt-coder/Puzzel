using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameEnums;

public class View_MatchMaking : View_Base
{
    [Space, Header("Button References")]
    [SerializeField] private Button createRoomBtn;
    [SerializeField] private Button joinRoomBtn;
    
    [Space, Header("Text References")]
    [SerializeField] private TMP_Text roomCodeTxt;
    
    [Space, Header("Input Field")]
    [SerializeField] private TMP_InputField roomCodeInputField;
    
    [Space, Header("Game Object References")]
    [SerializeField] private GameObject createRoomPanel;
    [SerializeField] private GameObject joinRoomPanel;
    
    public override void OnScreenShow()
    {
        base.OnScreenShow();
        SetUpView();
    }
    
    public override void OnScreenHide()
    {
        base.OnScreenHide();
        ResetUI();
    }
    
    public override void Init()
    {
        base.Init();
        InitView();
    }

    private void InitView()
    {
        createRoomBtn.onClick.AddListener(OnCreateRoomButtonClicked);
        joinRoomBtn.onClick.AddListener(OnJoinRoomButtonClicked);
    }

    private void SetUpView()
    {
        if (gameManager.currentPlayerRole == PlayerRole.Host)
        {
            createRoomPanel.SetActive(true);
            joinRoomPanel.SetActive(false);
        }
        else
        {
            joinRoomPanel.SetActive(true);
            createRoomPanel.SetActive(false);
        }
    }
    
    private void ResetUI()
    {
        roomCodeInputField.text = "";
        createRoomPanel.SetActive(false);
        joinRoomPanel.SetActive(false);
    }

    private void OnCreateRoomButtonClicked()
    {
        string roomId = firebaseManager.CreateRoom(OnRoomCreated);
        roomCodeTxt.text = roomId;
    }
    private void OnRoomCreated(bool isRoomCreated)
    {
        if (!isRoomCreated) errorHandler.HandleError(ErrorType.Matchmaking, "Failed to create room");
        ((View_Waiting)uiManager.GetView(View.Waiting)).UpdateRoomCodeText(roomCodeTxt.text);
        uiManager.ShowView(View.Waiting);
    }
    
    private void OnJoinRoomButtonClicked()
    {
        string roomId = roomCodeInputField.text.Trim();
        if (string.IsNullOrEmpty(roomId))
        {
            Debug.LogError("Room code is empty");
            errorHandler.HandleError(ErrorType.Matchmaking, "Please enter a room code", true);
            return;
        }
        firebaseManager.JoinRoom(roomId, OnRoomJoined);
    }
    private void OnRoomJoined(bool isRoomJoined)
    {
        if (!isRoomJoined) errorHandler.HandleError(ErrorType.Matchmaking, "Failed to join room");
        uiManager.ShowView(View.Waiting);
    }
}
