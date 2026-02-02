using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameEnums;
using static ActionHandler;

public class View_Error : View_Base
{
    [Space, Header("Text References")]
    [SerializeField] private TMP_Text errorHeadingTxt;
    [SerializeField] private TMP_Text errorMessageTxt;
    
    [Space, Header("Button References")]
    [SerializeField] private Button closeBtn;
    public override void OnScreenShow()
    {
        base.OnScreenShow();
        ErrorInvoke += OnErrorReceived;
    }
    
    public override void OnScreenHide()
    {
        base.OnScreenHide();
        ErrorInvoke -= OnErrorReceived;
        ResetUI();
    }

    public override void Init()
    {
        base.Init();
        InitView();
    }

    private void InitView()
    {
        closeBtn.onClick.AddListener(() =>
        {
            uiManager.HideOnlyCurrentView(View.Error);
        });
        ErrorInvoke += OnErrorReceived;
    }
    
    private void OnErrorReceived(ErrorType error, string message, bool isPopupRequired)
    {
        Debug.Log($"Error received in erro popup");
        if(!isPopupRequired) return;
        string errorHeading = GetHeadingBasedOnErrorType(error);
        errorHeadingTxt.text = errorHeading;
        errorMessageTxt.text = message;
    }
    
    private string GetHeadingBasedOnErrorType(ErrorType error)
    {
        return error switch
        {
            ErrorType.Network => "Network Error",
            ErrorType.Firebase => "Firebase Error",
            _ => "Unknown Error"
        };
    }
    
    private void ResetUI()
    {
        errorHeadingTxt.text = "";
        errorMessageTxt.text = "";
    }
    
}
