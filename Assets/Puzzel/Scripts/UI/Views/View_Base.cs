using System;
using System.Linq;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static GameEnums;

public class View_Base : MonoBehaviour
{
    [Serializable]
    public class ViewChangingBtn
    {
        [SerializeField] public Button btn;
        [SerializeField] public View nextView;
        
        [SerializeField, Space] public bool waitAsync;
        [SerializeField] private TMP_Text tmp_Text;
        [SerializeField] private GameObject obj_Loading;

        private UIManager uiManager;

        public void Initialize(UIManager uiMgr, bool removeListners = true)
        {
            uiManager = uiMgr;

            if(removeListners) btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => 
            {
                var popupView = uiManager.AllActiveViews.FirstOrDefault(s => s.viewType == ViewType.Popup);
                bool containsNextView = uiManager.AllActiveViews.Any(s => s.view == nextView);

                if (popupView != null)
                {
                    uiManager.HideView(popupView.view);

                    if (uiManager.IsPopup(nextView))
                    {
                        uiManager.ShowView(nextView);
                    }
                }
                if (!containsNextView) uiManager.ShowView(nextView);
            });
        }

        public void AddAdditionalClickListener(UnityAction additionalAction)
        {
            btn.onClick.AddListener(additionalAction);
        }

        public void StartLoading()
        {
            if (!waitAsync) return;

            btn.interactable = false;
            if (tmp_Text != null) tmp_Text.gameObject.SetActive(false);
            if (obj_Loading != null) obj_Loading.SetActive(true);
        }

        public void StopLoading()
        {
            if (!waitAsync) return;

            btn.interactable = true;
            if (tmp_Text != null) tmp_Text.gameObject.SetActive(true);
            if (obj_Loading != null) obj_Loading.SetActive(false);
        }
    }
    
    [Serializable]
    public class BufferButton
    {
        [SerializeField] public Button btn;
        [SerializeField] private TMP_Text tmp_Text;
        [SerializeField] private GameObject obj_loading;

        public void StartLoading()
        {
            btn.interactable = false;
            if (tmp_Text != null) tmp_Text.gameObject.SetActive(false);
            if (obj_loading != null) obj_loading.SetActive(true);
        }

        public void StopLoading()
        {
            btn.interactable = true;
            if (tmp_Text != null) tmp_Text.gameObject.SetActive(true);
            if (obj_loading != null) obj_loading.SetActive(false);
        }
    }



    [ReadOnly, SerializeField] private bool isViewVisible;

    [SerializeField] private GameObject mainPanel;

    protected DependencyManager dependencyManager;
    protected UIManager uiManager;
    protected GameManager gameManager;
    protected FirebaseManager firebaseManager;
    protected ErrorHandler errorHandler;
    protected FSMManager fsmManager;

    

    public bool ViewVisible => isViewVisible;

    public virtual void OnScreenShow()
    {
        isViewVisible = true;
        ShowView();
    }

    public virtual void OnScreenHide()
    {
        isViewVisible = false;
        HideView();
    }

    public virtual void Init()
    {
        GetRefs();
        isViewVisible = mainPanel.activeInHierarchy;
    }

    private void GetRefs()
    {
        dependencyManager = DependencyManager.Instance;
        uiManager = dependencyManager.Resolve<UIManager>();
        gameManager = dependencyManager.Resolve<GameManager>();
        firebaseManager = dependencyManager.Resolve<FirebaseManager>();
        errorHandler = dependencyManager.Resolve<ErrorHandler>();
        fsmManager = dependencyManager.Resolve<FSMManager>();
    }

    private void ShowView()
    {
        mainPanel.SetActive(true);
    }

    private void HideView()
    {
        mainPanel.SetActive(false);
    }
}
