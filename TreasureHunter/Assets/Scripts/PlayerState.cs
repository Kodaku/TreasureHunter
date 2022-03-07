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
    List<PlayerAction> actions = new List<PlayerAction>();
    private Action[] actionSpace = {Action.LEFT, Action.UP, Action.DOWN, Action.RIGHT};
    [NonSerialized]
    private HashedState hashedState;
    private float epsilon = 0.1f;

    public PlayerState(Vector3 _position, CellInfo _currentCellInfo, NeighboursInfo _neighboursCellInfo)
    {
        position = new float[]{_position.x, _position.y, _position.z};
        currentCellInfo = _currentCellInfo;
        neighboursCellsInfo = new CellInfo[]
                        {_neighboursCellInfo.Item1, _neighboursCellInfo.Item2, _neighboursCellInfo.Item3, _neighboursCellInfo.Item4};
        GenerateHashCode();
        GenerateBasicActions();
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
        foreach(Action action in actionSpace)
        {
            PlayerAction playerAction = new PlayerAction(action, 1.0f / totalActions);
            actions.Add(playerAction);
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
        Action bestAction = Action.IDLE;
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

    public void UpdateActionValue(Action currentAction, float maxQ, float reward)
    {
        foreach(PlayerAction playerAction in actions)
        {
            if(playerAction.name == currentAction)
            {
                playerAction.qValue += 0.01f * (reward + 1.0f * maxQ - playerAction.qValue);
            }
        }
    }

    public List<PlayerAction> GetPlayerActions()
    {
        return actions;
    }

    public HashedState GetHashedState()
    {
        return hashedState;
    }
}
