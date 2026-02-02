using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameEnums;

public class View_Profile : View_Base
{
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private TMP_Text playerNamePlaceholder;
    [SerializeField] private ViewChangingBtn saveButton;
    [SerializeField] private ViewChangingBtn backButton;
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
        backButton.Initialize(uiManager);
        saveButton.Initialize(uiManager);
        
        saveButton.AddAdditionalClickListener(OnSaveClicked);
        if (playerNamePlaceholder != null)
        {
            playerNamePlaceholder.text = PlayerPrefs.GetString(Constants.PlayerName, "Enter Name...");
        }
    }
    
    private async void OnSaveClicked()
    {
        string newName = playerNameInput.text;

        if (string.IsNullOrEmpty(newName)) return;

        PlayerPrefs.SetString(Constants.PlayerName, newName);
        bool isProfileCreate = await firebaseManager.CreateOrUpdateUserProfile(newName);
        
        if (!isProfileCreate)
        {
            errorHandler.HandleError(ErrorType.Firebase, "Failed to create profile");
        }


    }
    
}
