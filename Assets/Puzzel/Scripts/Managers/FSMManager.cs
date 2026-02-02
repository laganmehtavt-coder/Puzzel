using System.Collections.Generic;
using UnityEngine;
using static GameEnums;

public class FSMManager : MonoBehaviour
{
    [HideInInspector] private const GameState initialState = GameState.Home;
    [SerializeField] private State_Home state_Home;
    [SerializeField] private State_Island state_Island;
    [SerializeField] private State_GameEnd state_GameEnd;

    [SerializeField] private GameState currentState;

    private Dictionary<GameState, State_Base> stateDictionary;

    public GameState CurrentState => currentState;
    public Dictionary<GameState, State_Base> StateDictionary => stateDictionary;

    public void Init()
    {
        InitStateDict();

        currentState = initialState;
        stateDictionary[currentState].OnStateEnter();
    }

    public void ChangeState(GameState nextState)
    {
        if (stateDictionary != null && stateDictionary.Count > 0)
        {
            stateDictionary[currentState].OnStateExit();
            currentState = nextState;
            stateDictionary[currentState].OnStateEnter();
        }
    }
    

    private void Update()
    {
        if (stateDictionary == null) return;
        if (stateDictionary.Count == 0) return;

        stateDictionary[currentState].OnStateUpdate(Time.deltaTime);
    }

    private void InitStateDict()
    {
        stateDictionary = new Dictionary<GameState, State_Base>();
        stateDictionary.Add(GameState.Home, state_Home);
        stateDictionary.Add(GameState.Island, state_Island);
        stateDictionary.Add(GameState.EndGame, state_GameEnd);
    }
}
