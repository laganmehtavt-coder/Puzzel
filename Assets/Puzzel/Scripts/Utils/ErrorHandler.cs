using UnityEngine;
using static GameEnums;

public class ErrorHandler : MonoBehaviour
{
    private DependencyManager dependencyManager;
    private UIManager uiManager;
    
    public void Init()
    {
        dependencyManager = DependencyManager.Instance;
        uiManager = dependencyManager.Resolve<UIManager>();
    }
    
    public void HandleError(ErrorType errorType, string message, bool isPopupRequired = false)
    {
        ActionHandler.ErrorInvoke?.Invoke(errorType, message, isPopupRequired);
        if (!isPopupRequired)
        {
            Debug.LogError($"Error type: {errorType}. Message: {message}");
            return;
        }
        uiManager.ShowView(View.Error);
    }
}
