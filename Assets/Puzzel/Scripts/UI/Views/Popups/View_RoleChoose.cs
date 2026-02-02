using UnityEngine;
using UnityEngine.UI;
using static GameEnums;

public class View_RoleChoose : View_Base
{
    [SerializeField] private Button clientModeBtn;
    [SerializeField] private Button hostModeBtn;
    public override void OnScreenShow()
    {
        base.OnScreenShow();
    }
    
    public override void OnScreenHide()
    {
        base.OnScreenHide();
    }

    public override void Init()
    {
        base.Init();
        InitView();
    }

    private void InitView()
    {
        clientModeBtn.onClick.AddListener(() =>
        {
            Debug.Log("Client mode set");
            SetPlayerRole(PlayerRole.Client);
            uiManager.HideOnlyCurrentView(View.RoleChoose);
        });
        hostModeBtn.onClick.AddListener(() =>
        {
            Debug.Log("Host mode set");
            SetPlayerRole(PlayerRole.Host);
            uiManager.HideOnlyCurrentView(View.RoleChoose);
        });
    }
    
    private async void SetPlayerRole(PlayerRole currentPlayerRole)
    {
        if (gameManager.isDevMode)
        {
            PlayerPrefs.SetInt(Constants.PlayerRole, (int) gameManager.currentPlayerDevRole);
            Debug.Log($"Dev mode is on. Player role is set to {gameManager.currentPlayerDevRole}");
            return;
        }
        Debug.Log($"Player role is {currentPlayerRole}");
        gameManager.currentPlayerRole = currentPlayerRole;
        PlayerPrefs.SetInt(Constants.PlayerRole, (int) currentPlayerRole);
        bool isPlayerRoleUpdated = await firebaseManager.UpdatePlayerRole(firebaseManager.CurrentUserId, currentPlayerRole);
        if (!isPlayerRoleUpdated) Debug.LogError("Failed to update player role");
    }
}
