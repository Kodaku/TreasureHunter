using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using NumSharp;

using State = System.Tuple<UnityEngine.Vector3, CellInfo, System.Tuple<CellInfo, CellInfo, CellInfo, CellInfo>>;
using NextCellInfo = System.Tuple<CellInfo, CellInfo, CellInfo, CellInfo>;
using ActionQ = System.Collections.Generic.Dictionary<Action, float>;
using ActionProbability = System.Collections.Generic.Dictionary<Action, float>;
public class Player : MonoBehaviour
{
    private State currentState  = null;
    private State nextState = null;
    private Q actionValueFuction;
    private Policy policy;
    // private Dictionary<State, ActionQ> Q = new Dictionary<State, ActionQ>();
    // private Dictionary<State, ActionProbability> policy = new Dictionary<State, ActionProbability>();
    private Action[] actionSpace = {Action.LEFT, Action.UP, Action.DOWN, Action.RIGHT};
    private bool hasPickedTreasure = false;
    private bool hasHitEnemy = false;
    private float currentReward = 0.0f;
    private Action currentAction;
    private float epsilon = 0.1f;
    public void SetCurrentState(World world, MyGrid grid)
    {
        Vector3 currentPos = transform.position;

        CellInfo currentCellInfo = world.GetWorldCellState(currentPos);

        Vector3 upCell = grid.NodeFromWorldPoint(new Vector3(currentPos.x, currentPos.y + 1.0f, currentPos.z)).worldPosition;
        Vector3 leftCell = grid.NodeFromWorldPoint(new Vector3(currentPos.x - 1.0f, currentPos.y, currentPos.z)).worldPosition;
        Vector3 downCell = grid.NodeFromWorldPoint(new Vector3(currentPos.x, currentPos.y - 1.0f, currentPos.z)).worldPosition;
        Vector3 rightCell = grid.NodeFromWorldPoint(new Vector3(currentPos.x + 1.0f, currentPos.y, currentPos.z)).worldPosition;

        NextCellInfo neighsStates = Tuple.Create(world.GetWorldCellState(upCell), world.GetWorldCellState(leftCell),
                                                    world.GetWorldCellState(downCell), world.GetWorldCellState(rightCell));

        // nextState = currentState;

        currentState = Tuple.Create(currentPos, currentCellInfo, neighsStates);
        // print(currentState);

        // if(nextState == null)
        // {
        //     nextState = currentState;
        // }
    }

    public void SetNextState(World world, MyGrid grid)
    {
        Vector3 currentPos = transform.position;

        CellInfo currentCellInfo = world.GetWorldCellState(currentPos);

        Vector3 upCell = grid.NodeFromWorldPoint(new Vector3(currentPos.x, currentPos.y + 1.0f, currentPos.z)).worldPosition;
        Vector3 leftCell = grid.NodeFromWorldPoint(new Vector3(currentPos.x - 1.0f, currentPos.y, currentPos.z)).worldPosition;
        Vector3 downCell = grid.NodeFromWorldPoint(new Vector3(currentPos.x, currentPos.y - 1.0f, currentPos.z)).worldPosition;
        Vector3 rightCell = grid.NodeFromWorldPoint(new Vector3(currentPos.x + 1.0f, currentPos.y, currentPos.z)).worldPosition;

        NextCellInfo neighsStates = Tuple.Create(world.GetWorldCellState(upCell), world.GetWorldCellState(leftCell),
                                                    world.GetWorldCellState(downCell), world.GetWorldCellState(rightCell));

        // nextState = currentState;

        nextState = Tuple.Create(currentPos, currentCellInfo, neighsStates);
    }

    public void SelectAction()
    {
        // Action bestAction = GetBestAction();
        Action bestAction = GetBestAction();
        // policy[currentState] = GetEpsilonGreedy(bestAction);
        policy.EpsilonGreedyUpdate(currentState, bestAction);
        // currentAction = ChooseAction();
        currentAction = policy.ChooseAction(currentState);
    }

    private Action GetBestAction()
    {
        // if(Q.ContainsKey(currentState))
        // {
        //     ActionQ actionQ = Q[currentState];
        //     Action bestAction = Action.IDLE;
        //     float maxActionValue = Mathf.NegativeInfinity;
        //     foreach(Action action in actionQ.Keys)
        //     {
        //         float actionValue = actionQ[action];
        //         if(actionValue > maxActionValue)
        //         {
        //             maxActionValue = actionValue;
        //             bestAction = action;
        //         }
        //     }
        //     return bestAction;
        // }
        // else
        // {
        //     AddActionQ(currentState);
        //     AddActionProbabilities(currentState);
        //     return GetBestAction();
        // }
        if(actionValueFuction.ContainsState(currentState))
        {
            return actionValueFuction.GetBestAction(currentState);
        }
        else
        {
            PlayerState playerState = new PlayerState(currentState.Item1, currentState.Item2, currentState.Item3);
            actionValueFuction.AddPlayerState(playerState);
            policy.AddPlayerState(playerState);
            return GetBestAction();
        }
    }

    private ActionProbability GetEpsilonGreedy(Action bestAction)
    {
        ActionProbability actionProbabilities = new ActionProbability();
        int totalActions = actionSpace.Length;
        foreach(Action action in actionSpace)
        {
            if(action == bestAction)
            {
                actionProbabilities.Add(action, 1.0f - epsilon + (epsilon / totalActions));
            }
            else
            {
                actionProbabilities.Add(action, epsilon / totalActions);
            }
        }

        return actionProbabilities;
    }

    private Action ChooseAction()
    {
        // double sum = 0.0f;
        // ActionProbability actionProbabilities = policy[currentState];
        // List<double> distribution = new List<double>();
        // List<Action> actions = new List<Action>();
        // foreach(Action action in actionProbabilities.Keys)
        // {
        //     actions.Add(action);
        //     distribution.Add(actionProbabilities[action]);
        // }

        // // print(np.random.choice(actionSpace, p:distribution.ToArray()));
        // NDArray test = np.zeros(actions.Count);
        // NDArray test2 = np.zeros(distribution.Count);
        // // print(test);
        // for(int i = 0; i < actions.Count; i++)
        // {
        //     test[i] = (int)actions[i];
        //     test2[i] = distribution[i];
        // }

        // // print(test);
        // // print(test2);
        // // print(np.random.choice(test, probabilities:distribution.ToArray()));

        // List<double> cumulative = distribution.Select(c => {
        //     var result = sum + c;
        //     sum += c;
        //     return result;
        // }).ToList();

        // float r = UnityEngine.Random.value;
        // int idx = cumulative.BinarySearch(r);
        // if(idx < 0)
        // {
        //     idx = ~idx;
        // }
        // if(idx > cumulative.Count - 1)
        // {
        //     idx = cumulative.Count - 1;
        // }

        // return actions[idx];
        return Action.IDLE;
    }

    private void AddActionQ(State state)
    {
        // ActionQ actionQ = new ActionQ();
        // foreach(Action action in actionSpace)
        // {
        //     actionQ.Add(action, 0.0f);
        // }
        // Q.Add(state, actionQ);
    }

    private void AddActionProbabilities(State state)
    {
        // int totalActions = actionSpace.Length;
        // ActionProbability actionProbabilities = new ActionProbability();
        // foreach(Action action in actionSpace)
        // {
        //     actionProbabilities.Add(action, (1.0f / totalActions));
        // }
        // policy.Add(state, actionProbabilities);
    }

    public void Move(MyGrid grid)
    {
        Vector3 tmpPosition = grid.NodeFromWorldPoint(transform.position).worldPosition;
        if(currentAction == Action.UP)
        {
            tmpPosition.y += 1.0f;
        }
        else if(currentAction == Action.LEFT)
        {
            tmpPosition.x -= 1.0f;
        }
        else if(currentAction == Action.DOWN)
        {
            tmpPosition.y -= 1.0f;
        }
        else if(currentAction == Action.RIGHT)
        {
            tmpPosition.x += 1.0f;
        }
        transform.position = grid.NodeFromWorldPoint(tmpPosition).worldPosition;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        // print(collider.gameObject.tag);
        if(collider.gameObject.CompareTag("Treasure"))
        {
            hasPickedTreasure = true;
        }
        else if(collider.gameObject.CompareTag("Enemy"))
        {
            hasHitEnemy = true;
        }
    }

    public bool HasPickedTreasure()
    {
        return hasPickedTreasure;
    }

    public void ResetPickTreasure()
    {
        hasPickedTreasure = false;
    }

    public bool HasHitEnemy()
    {
        return hasHitEnemy;
    }

    public void ResetHitEnemy()
    {
        hasHitEnemy = false;
    }

    public bool IsStuck()
    {
        return currentState == nextState;
    }

    public void QUpdate(float reward, bool isEndState)
    {
        if(!isEndState)
        {
            float maxQ = GetMaxQ();
            // Q[currentState][currentAction] += 0.01f * (reward + 1.0f * maxQ - Q[currentState][currentAction]);
            actionValueFuction.Update(currentState, currentAction, maxQ, reward);
        }
        else
            // Q[currentState][currentAction] += 0.01f * (reward - Q[currentState][currentAction]);
            actionValueFuction.Update(currentState, currentAction, 0.0f, reward);
        currentState = nextState;
    }

    private float GetMaxQ()
    {
        // if(Q.ContainsKey(nextState))
        // {
        //     float maxQ = Mathf.NegativeInfinity;
        //     ActionQ actionQ = Q[nextState];
        //     foreach(Action action in actionQ.Keys)
        //     {
        //         float qValue = actionQ[action];
        //         if(qValue > maxQ)
        //         {
        //             maxQ = qValue;
        //         }
        //     }

        //     return maxQ;
        // }
        // else
        // {
        //     AddActionQ(nextState);
        //     AddActionProbabilities(nextState);
        //     return GetMaxQ();
        // }
        if(actionValueFuction.ContainsState(nextState))
        {
            return actionValueFuction.GetMaxQ(nextState);
        }
        else
        {
            PlayerState playerState = new PlayerState(nextState.Item1, nextState.Item2, nextState.Item3);
            policy.AddPlayerState(playerState);
            actionValueFuction.AddPlayerState(playerState);
            return GetMaxQ();
        }
    }

    public void SaveData()
    {
        Serializer.WriteToBinaryFile<Policy>("Assets/Resources/policy.txt", policy);
        Serializer.WriteToBinaryFile<Q>("Assets/Resources/Q.txt", actionValueFuction);
    }

    public void LoadData()
    {
        if(System.IO.File.Exists("Assets/Resources/Q.txt"))
        {
            //Load Q and Policy and initialize the StateDictionary
            policy = Serializer.ReadFromBinaryFile<Policy>("Assets/Resources/policy.txt");
            actionValueFuction = Serializer.ReadFromBinaryFile<Q>("Assets/Resources/Q.txt");
            policy.InitializeStateDictionary();
            actionValueFuction.InitializeStateDictionary();
        }
        else
        {
            policy = new Policy();
            actionValueFuction = new Q();
        }
        // Q = Serializer.ReadFromJsonFile<Dictionary<State, ActionQ>>("Assets/Resources/Q.txt");
        // policy = Serializer.ReadFromJsonFile<Dictionary<State, ActionProbability>>("Assets/Resources/policy.txt");
    }
}
