using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameEnums;

public class View_Splash : View_Base
{
    [Header("Loading References")]
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private TMP_Text loadingPercentageText;
    [SerializeField] private int loadingDuration = 2;
    
    private bool isFirebaseDone = false;
    private bool isMinTimeDone = false;
    private bool hasError = false;
    
    public override void OnScreenShow()
    {
        base.OnScreenShow();
        isFirebaseDone = false;
        hasError = false;
        bool isInternetActive = CheckForInternetConnection();
        if (isInternetActive)
        {
            Debug.Log("Internet Activate");
            StartCoroutine(StartLoading());
            InitializeFirebaseAsync();
        }
        else
        {
            hasError = true;
        }
    }
    
    public override void OnScreenHide()
    {
        base.OnScreenHide();
    }

    public override void Init()
    {
        base.Init();
    }
    private void Start() 
    {
        Debug.Log("Enter5ing in script");
        
    }
    private bool CheckForInternetConnection()
    {
        if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
        {
            Debug.Log("Internet is active");
            return true;
        }
        Debug.LogError("Internet is not active");
        hasError = true;
        errorHandler.HandleError(ErrorType.Network, "Please check your internet connection and try again.", true);
        return false;
    }
    
    private async void InitializeFirebaseAsync()
    {
        bool success = await firebaseManager.InitializeFirebase();
        isFirebaseDone = success;
        CreatOrUpdateUserProfile();
        if (!success)
        {
            hasError = true;
            Debug.LogError("Firebase failed to initialize");
        }
    }
    
    private IEnumerator StartLoading()
    {
        Debug.Log("⏳ Game Starting...");
        float elapsed = 0f;
        while (elapsed < loadingDuration && !hasError)
        {
            elapsed += Time.deltaTime;
            UpdateUI(elapsed / loadingDuration);
            yield return null;
        }
        if (hasError) yield break;
        while (!isFirebaseDone && !hasError)
        {
            UpdateUI(1f); 
            yield return null;
        }
        if (!hasError && isFirebaseDone)
        {
            Debug.Log("Loading Complete!");
            uiManager.ShowView(View.Home);
        }
        else
        {
            Debug.Log("Loading Aborted due to error.");
        }
    }
    
    private void UpdateUI(float progress)
    {
        progress = Mathf.Clamp01(progress);

        if (loadingSlider != null)
            loadingSlider.value = progress;

        if (loadingPercentageText != null)
            loadingPercentageText.text = $"{Mathf.RoundToInt(progress * 100)}%";
    }

    private async void CreatOrUpdateUserProfile()
    {
        gameManager.GetRandomName();
        bool isProfileCreated = await firebaseManager.CreateOrUpdateUserProfile(PlayerPrefs.GetString(Constants.PlayerName));
        if (isProfileCreated)
        {
            // Update on UI
            ((View_Home)uiManager.GetView(View.Home)).UpdateUserProfile();
        }
    }
}
