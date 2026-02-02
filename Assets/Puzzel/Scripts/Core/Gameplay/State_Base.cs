using UnityEngine;
using static GameEnums;

public class State_Base : MonoBehaviour
{
    protected GameState myState;
    protected DependencyManager dependencyManager;
    protected FirebaseManager firebaseManager;
    protected FSMManager fsmManager;
    protected GameManager gameManager;
    protected UIManager uiManager;

    public virtual void InitState(GameState _state)
    {
        myState = _state;
        dependencyManager = DependencyManager.Instance;
        gameManager = DependencyManager.Instance.Resolve<GameManager>();
        fsmManager = DependencyManager.Instance.Resolve<FSMManager>();
        uiManager = DependencyManager.Instance.Resolve<UIManager>();
    }

    public virtual void OnStateEnter()
    {

    }

    public virtual void OnStateUpdate(float deltaTime)
    {

    }

    public virtual void OnStateExit()
    {

    }
}