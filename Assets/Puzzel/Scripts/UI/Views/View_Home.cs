using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameEnums;

public class View_Home : View_Base
{
    [Space, Header("Button References")]
    [SerializeField] private ViewChangingBtn profileBtn;
    
    [Space, Header("Text References")]
    [SerializeField] private TMP_Text userNameText; 
    
    [Space, Header("View Setup")]
    [SerializeField] private GameObject desktopView;
    [SerializeField] private GameObject phoneView;
    
    [Space, Header("Testing Setup")]
    [SerializeField] private Button startBtn;
    public override void OnScreenShow()
    {
        base.OnScreenShow();
        if(!PlayerPrefs.HasKey(Constants.PlayerRole)) uiManager.ShowView(View.RoleChoose);
    }
    
    public override void OnScreenHide()
    {
        base.OnScreenHide();
        ResetView();
    }

    public override void Init()
    {
        base.Init();
        SetUpView();
    }

    private void SetUpView()
    {
        profileBtn.Initialize(uiManager);
        userNameText.text = PlayerPrefs.GetString(Constants.PlayerName);
        startBtn.onClick.AddListener(() =>
        {
            uiManager.ShowView(View.Matchmaking);
        });
    }

    private void RoomCreationStatus(bool isRoomCreated)
    {
        if(isRoomCreated) Debug.Log("Room created successfully!");
    }

    private void ResetView()
    {
        phoneView.SetActive(false);
        desktopView.SetActive(false);
    }

    public void UpdateUserProfile()
    {
        userNameText.text = PlayerPrefs.GetString(Constants.PlayerName);
    }
    
}
