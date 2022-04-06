using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

using HashedState = System.Tuple<UnityEngine.Vector3, CellInfo, System.Tuple<CellInfo, CellInfo, CellInfo, CellInfo>>;
using NeighboursInfo = System.Tuple<CellInfo, CellInfo, CellInfo, CellInfo>;

[Serializable]
public class PlayerState
{
    private float[] position;
    private CellInfo currentCellInfo;
    private CellInfo[] neighboursCellsInfo;
    private PlayerAction[] actions;
    [NonSerialized]
    private PlayerState[] nextStates;
    private Action[] actionSpace = {Action.LEFT, Action.UP, Action.DOWN, Action.RIGHT};
    [NonSerialized]
    private HashedState hashedState;
    private float[] tau;
    private float epsilon = 0.1f;
    private bool m_isTerminal = false;

    public PlayerState(Vector3 _position, CellInfo _currentCellInfo, NeighboursInfo _neighboursCellInfo)
    {
        position = new float[]{_position.x, _position.y, _position.z};
        currentCellInfo = _currentCellInfo;
        neighboursCellsInfo = new CellInfo[]
                        {_neighboursCellInfo.Item1, _neighboursCellInfo.Item2, _neighboursCellInfo.Item3, _neighboursCellInfo.Item4};
        GenerateHashCode();
        GenerateBasicActions();
        // InitializeNextStates();
        InitializeTau();
    }

    public bool isTerminal
    {
        get { return m_isTerminal; }
        set { m_isTerminal = value; }
    }

    public void GenerateHashCode()
    {
        Vector3 statePosition = new Vector3(position[0], position[1], position[2]);
        NeighboursInfo neighboursInfo = 
                Tuple.Create(neighboursCellsInfo[0], neighboursCellsInfo[1], neighboursCellsInfo[2], neighboursCellsInfo[3]);
        hashedState = Tuple.Create(statePosition, currentCellInfo, neighboursInfo);
    }

    private void GenerateBasicActions()
    {
        int totalActions = actionSpace.Length;
        actions = new PlayerAction[totalActions];
        foreach(Action action in actionSpace)
        {
            PlayerAction playerAction = new PlayerAction(action, 1.0f / totalActions);
            actions[(int)action] = playerAction;
        }
    }

    public void InitializeNextStates()
    {
        int totalActions = actionSpace.Length;
        nextStates = new PlayerState[totalActions];
        foreach(Action action in actionSpace)
        {
            nextStates[(int)action] = null;
        }
    }

    private void InitializeTau()
    {
        int totalActions = actionSpace.Length;
        tau = new float[totalActions];
        foreach(Action action in actionSpace)
        {
            tau[(int)action] = 0.0f;
        }
    }

    public void UpdateProbabilities(Action bestAction)
    {
        int totalActions = actionSpace.Length;
        foreach(PlayerAction playerAction in actions)
        {
            if(playerAction.name == bestAction)
            {
                playerAction.probability = 1.0f - epsilon + (epsilon / totalActions);
            }
            else
            {
                playerAction.probability = (epsilon / totalActions);
            }
        }
    }

    public Action ChooseAction()
    {
        float sum = 0.0f;
        List<float> distribution = new List<float>();
        foreach(PlayerAction playerAction in actions)
        {
            distribution.Add(playerAction.probability);
        }

        List<float> cumulative = distribution.Select(c => {
            var result = sum + c;
            sum += c;
            return result;
        }).ToList();

        float r = UnityEngine.Random.value;
        int idx = cumulative.BinarySearch(r);
        if(idx < 0)
        {
            idx = ~idx;
        }
        if(idx > cumulative.Count - 1)
        {
            idx = cumulative.Count - 1;
        }

        return actions[idx].name;
    }

    public Action GetBestAction()
    {
        Action bestAction = Action.LEFT;
        float maxActionValue = Mathf.NegativeInfinity;
        foreach(PlayerAction playerAction in actions)
        {
            float actionValue = playerAction.qValue;
            if(actionValue > maxActionValue)
            {
                maxActionValue = actionValue;
                bestAction = playerAction.name;
            }
        }
        return bestAction;
    }

    public void SetNextStateAndReward(Action currentAction, PlayerState nextState, float reward)
    {
        nextStates[(int)currentAction] = nextState;
        PlayerAction currentPlayerActionInstance = actions[(int)currentAction];
        currentPlayerActionInstance.reward = reward;
    }

    public Tuple<PlayerState, PlayerState, float, float> GetRandomNextStateAndReward()
    {
        List<Action> validActions = new List<Action>();
        foreach(Action action in actionSpace)
        {
            if(nextStates[(int)action] != null)
            {
                validActions.Add(action);
            }
        }
        int totalActions = validActions.Count;
        // Debug.Log(totalActions);
        if(totalActions > 0)
        {
            int index = UnityEngine.Random.Range(0, totalActions);
            Action randomAction = validActions[index];
            PlayerAction randomTakenAction = actions[(int)randomAction];
            // Debug.Log(randomAction + ", " + nextStates.Length);
            // Debug.Log(nextStates[0] + ", " + nextStates[1] + ", " + nextStates[2] + ", " + nextStates[3]);
            return Tuple.Create(this, nextStates[(int)randomAction], randomTakenAction.reward, tau[(int)randomAction]);
        }
        return null;
    }

    public float GetMaxQ()
    {
        float maxQ = Mathf.NegativeInfinity;
        foreach(PlayerAction playerAction in actions)
        {
            float qValue = playerAction.qValue;
            if(qValue > maxQ)
            {
                maxQ = qValue;
            }
        }

        return maxQ;
    }

    public float GetQ(Action currentAction)
    {
        return actions[(int)currentAction].qValue;
    }

    public void UpdateActionValue(Action currentAction, float maxQ, float reward, bool isEndState)
    {
        m_isTerminal = isEndState;
        PlayerAction playerAction = actions[(int)currentAction];
        actions[(int)currentAction].qValue += 0.01f * (reward + 1.0f * maxQ - playerAction.qValue);
        // foreach(PlayerAction playerAction in actions)
        // {
        //     if(playerAction.name == currentAction)
        //     {
        //         playerAction.qValue += 0.01f * (reward + 1.0f * maxQ - playerAction.qValue);
        //     }
        // }
    }

    public void IncreaseTau()
    {
        foreach(Action action in actionSpace)
        {
            tau[(int)action] += 1.0f;
        }
    }

    public void ResetTau(Action currentAction)
    {
        tau[(int)currentAction] = 0.0f;
    }

    public HashedState GetHashedState()
    {
        return hashedState;
    }
}
