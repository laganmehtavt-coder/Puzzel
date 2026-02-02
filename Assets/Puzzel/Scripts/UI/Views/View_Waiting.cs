using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class View_Waiting : View_Base
{
    
    [Space, Header("Text References")]
    [SerializeField] private TMP_Text waitingClientSideText;
    [SerializeField] private TMP_Text waitingHostSideText;
    [SerializeField] private TMP_Text roomCodeText;

    [Space, Header("Button References")] 
    [SerializeField] private Button startGameBtn;
    public override void OnScreenShow()
    {
        base.OnScreenShow();
        SetupView();
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
        startGameBtn.onClick.AddListener(OnStartGameButtonClick);
    }

    private void SetupView()
    {
        if (gameManager.currentPlayerRole == GameEnums.PlayerRole.Host)
        {
            startGameBtn.gameObject.SetActive(true);
            waitingHostSideText.gameObject.SetActive(true);
            waitingClientSideText.gameObject.SetActive(false);
            waitingHostSideText.text = $"Waiting for other players to join..";
        }
        else
        {
            waitingClientSideText.text = "Waiting for Host to start the game..";
        }
    }

    private void OnStartGameButtonClick()
    {
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        yield return new WaitForSeconds(5f);
        uiManager.ShowView(GameEnums.View.Island);
        fsmManager.ChangeState(GameEnums.GameState.Island);
        firebaseManager.SetGameState(GameEnums.GameState.Island);
        waitingHostSideText.text = "Stating in 5 seconds..";
    }

    private void ResetUI()
    {
        startGameBtn.gameObject.SetActive(false);
        waitingHostSideText.gameObject.SetActive(false);
        waitingClientSideText.gameObject.SetActive(true);
        
    }
    
    public void UpdateRoomCodeText(string roomCode) => roomCodeText.text = $"Room Code: {roomCode}";
}
