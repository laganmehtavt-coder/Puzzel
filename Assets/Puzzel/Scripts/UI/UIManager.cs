using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GameEnums;

public class UIManager : MonoBehaviour
{
    [Serializable]
    public class SceneViewSetup
    {
        [HideInInspector] public string name;
        [HideInInspector] public View view;

        [SerializeField] public ViewType viewType;
        [SerializeField] public View_Base viewRef;

        public Action OnScreenShow;
        public Action OnScreenHide;
    }
    [SerializeField] private SceneViewSetup[] sceneViews;

    [SerializeField] private GameObject popupParent;

    public IEnumerable<SceneViewSetup> AllActiveViews => sceneViews.Where(vs => vs.viewRef.ViewVisible);
    

    private void OnValidate()
    {
        var enumValues = Enum.GetValues(typeof(View));
        int enumCount = enumValues.Length;

        if (sceneViews == null || sceneViews.Length != enumCount)
        {
            Array.Resize(ref sceneViews, enumCount);
        }

        for (int i = 0; i < enumCount; i++)
        {
            if (sceneViews[i] == null) sceneViews[i] = new SceneViewSetup();

            sceneViews[i].view = (View)enumValues.GetValue(i);
            sceneViews[i].name = sceneViews[i].view.ToString();
        }
    }


    private void OnDestroy()
    {
        UnsubscribeViewEvents();
    }

    public void Init()
    {
        InitSceneViews();

        foreach (var view in sceneViews)
        {
            view.viewRef.Init();
        }
    }

    public View_Base GetView(View type)
    {
        foreach (var setup in sceneViews)
        {
            if (setup.view == type)
                return setup.viewRef;
        }
        return null;
    }

    public T GetView<T>() where T : View_Base => sceneViews.Select(viewSetup => viewSetup.viewRef).OfType<T>().FirstOrDefault();

    public void ShowView<T>() where T : View_Base
    {
        var setup = sceneViews.FirstOrDefault(vs => vs.viewRef is T);
        if (setup == null) return;

        if (setup.viewType != ViewType.Popup)
        {
            foreach (var vs in sceneViews)
            {
                if (vs != setup && vs.viewRef.ViewVisible)
                    vs.OnScreenHide?.Invoke();

                if (vs.viewType == ViewType.Popup) popupParent.SetActive(false);
            }
        }

        if (!setup.viewRef.ViewVisible) 
        {
            if (setup.viewType == ViewType.Popup) popupParent.SetActive(true);
            setup.OnScreenShow?.Invoke();
        }
    }

    public void ShowView(View view) => ShowView(GetView(view));

    public void ShowView(View_Base view)
    {
        if (view == null) return;
        var setup = sceneViews.FirstOrDefault(vs => vs.viewRef == view);
        if (setup == null) return;

        if (setup.viewType != ViewType.Popup)
        {
            foreach (var vs in sceneViews)
            {
                if (vs != setup && vs.viewRef.ViewVisible)
                    vs.OnScreenHide?.Invoke();

                if (vs.viewType == ViewType.Popup) popupParent.SetActive(false);
            }
        }

        if (!setup.viewRef.ViewVisible)
        {
            if (setup.viewType == ViewType.Popup) popupParent.SetActive(true);
            setup.OnScreenShow?.Invoke();
        }
    }

    public void HideView<T>() where T : View_Base
    {
        var setup = sceneViews.FirstOrDefault(s => s.viewRef is T && s.viewRef.ViewVisible);
        if (setup == null) return;

        setup.OnScreenHide?.Invoke();
        if (setup.viewType == ViewType.Popup) popupParent.SetActive(false);
    }

    public void HideView(View view) => HideView(GetView(view));

    public void HideView(View_Base view)
    {
        if (view == null) return;
        var setup = sceneViews.FirstOrDefault(s => s.viewRef == view && s.viewRef.ViewVisible);
        if (setup == null) return;

        setup.OnScreenHide?.Invoke();
        if (setup.viewType == ViewType.Popup) popupParent.SetActive(false);
    }

    public void HideAllViews()
    {
        popupParent.SetActive(false);
        foreach (var viewSetup in sceneViews.Where(viewSetup => viewSetup.viewRef.ViewVisible))
        {
            viewSetup.OnScreenHide?.Invoke();
        }
    }

    public void HideOnlyCurrentView(View view)
    {
        var setup = sceneViews.FirstOrDefault(s => s.view == view && s.viewRef.ViewVisible);
        if (setup == null)
        {
            Debug.LogWarning($"Could not hide view '{view}'. It might not exist or is already hidden.");
            return;
        }
        setup.OnScreenHide?.Invoke();
    }   

    public bool IsPopup(View view)
    {
        var setup = sceneViews.FirstOrDefault(vs => vs.view == view);
        return setup != null && setup.viewType == ViewType.Popup;
    }

    private void InitSceneViews()
    {
        SubscribeViewEvents();
    }

    private void SubscribeViewEvents()
    {
        foreach (var viewSetup in sceneViews)
        {
            viewSetup.OnScreenShow += viewSetup.viewRef.OnScreenShow;
            viewSetup.OnScreenHide += viewSetup.viewRef.OnScreenHide;
        }
    }

    private void UnsubscribeViewEvents()
    {
        foreach (var viewSetup in sceneViews)
        {
            viewSetup.OnScreenShow -= viewSetup.viewRef.OnScreenShow;
            viewSetup.OnScreenHide -= viewSetup.viewRef.OnScreenHide;
        }
    }
}
