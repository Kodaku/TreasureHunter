using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

using State = System.Tuple<UnityEngine.Vector3, CellInfo, System.Tuple<CellInfo, CellInfo, CellInfo, CellInfo>>;
using NextCellInfo = System.Tuple<CellInfo, CellInfo, CellInfo, CellInfo>;
public class Player : MonoBehaviour
{
    private State currentState  = null;
    private State nextState = null;
    private Q actionValueFuction;
    private Policy policy;
    private Model model;
    private Action[] actionSpace = {Action.LEFT, Action.UP, Action.DOWN, Action.RIGHT};
    private bool hasPickedTreasure = false;
    private bool hasHitEnemy = false;
    // private float currentReward = 0.0f;
    private float kappa = 0.001f;
    private Action currentAction;

    private State GetState(World world, MyGrid grid)
    {
        Vector3 currentPos = transform.position;

        CellInfo currentCellInfo = world.GetWorldCellState(currentPos);

        Vector3 upCell = grid.NodeFromWorldPoint(new Vector3(currentPos.x, currentPos.y + 1.0f, currentPos.z)).worldPosition;
        Vector3 leftCell = grid.NodeFromWorldPoint(new Vector3(currentPos.x - 1.0f, currentPos.y, currentPos.z)).worldPosition;
        Vector3 downCell = grid.NodeFromWorldPoint(new Vector3(currentPos.x, currentPos.y - 1.0f, currentPos.z)).worldPosition;
        Vector3 rightCell = grid.NodeFromWorldPoint(new Vector3(currentPos.x + 1.0f, currentPos.y, currentPos.z)).worldPosition;

        NextCellInfo neighsStates = Tuple.Create(world.GetWorldCellState(upCell), world.GetWorldCellState(leftCell),
                                                    world.GetWorldCellState(downCell), world.GetWorldCellState(rightCell));

        return Tuple.Create(currentPos, currentCellInfo, neighsStates);
    }
    public void SetCurrentState(World world, MyGrid grid)
    {
        currentState = GetState(world, grid);
    }

    public void SetNextState(World world, MyGrid grid)
    {
        nextState = GetState(world, grid);
    }

    public void SelectAction()
    {
        Action bestAction = GetBestAction();
        policy.EpsilonGreedyUpdate(currentState, bestAction);
        currentAction = policy.ChooseAction(currentState);
    }

    private Action GetBestAction()
    {
        if(actionValueFuction.ContainsState(currentState))
        {
            return actionValueFuction.GetBestAction(currentState);
        }
        else
        {
            PlayerState playerState = new PlayerState(currentState.Item1, currentState.Item2, currentState.Item3);
            actionValueFuction.AddPlayerState(playerState);
            policy.AddPlayerState(playerState);
            model.AddPlayerState(playerState);
            return GetBestAction();
        }
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

    public void UpdateAllTau()
    {
        model.IncreaseAllTau();
        model.ResetTau(currentState, currentAction);
    }

    public void QUpdate(float reward, bool isEndState)
    {
        if(!isEndState)
        {
            float maxQ = GetMaxQ(nextState);
            // Q[currentState][currentAction] += 0.01f * (reward + 1.0f * maxQ - Q[currentState][currentAction]);
            actionValueFuction.Update(currentState, currentAction, maxQ, reward, isEndState);
        }
        else
            // Q[currentState][currentAction] += 0.01f * (reward - Q[currentState][currentAction]);
            actionValueFuction.Update(currentState, currentAction, 0.0f, reward, isEndState);
        // currentState = nextState;
    }

    public void UpdateModel(float reward)
    {
        model.Update(currentState, currentAction, nextState, reward);
        currentState = nextState;
    }

    public void RunSimulation()
    {
        Tuple<PlayerState, PlayerState, float, float> simulationResult = model.GetRandomPlayerStateAndReward();
        if(simulationResult != null)
        {
            State currentSimulatedState = simulationResult.Item1.GetHashedState();
            State nextSimulatedState = simulationResult.Item2.GetHashedState();
            // print(currentSimulatedState);
            // print(nextSimulatedState);
            float simulationTau = simulationResult.Item4;
            float simulationReward = simulationResult.Item3 + kappa * Mathf.Sqrt(simulationTau);
            if(!simulationResult.Item1.isTerminal)
            {
                float maxQ = GetMaxQ(nextSimulatedState);
                actionValueFuction.Update(currentSimulatedState, currentAction, maxQ, simulationReward, simulationResult.Item1.isTerminal);
            }
            else
                actionValueFuction.Update(currentSimulatedState, currentAction, 0.0f, simulationReward, simulationResult.Item1.isTerminal);
        }
    }

    private float GetMaxQ(State nextState)
    {
        if(actionValueFuction.ContainsState(nextState))
        {
            return actionValueFuction.GetMaxQ(nextState);
        }
        else
        {
            PlayerState playerState = new PlayerState(nextState.Item1, nextState.Item2, nextState.Item3);
            policy.AddPlayerState(playerState);
            actionValueFuction.AddPlayerState(playerState);
            model.AddPlayerState(playerState);
            return GetMaxQ(nextState);
        }
    }

    public float GetQ()
    {
        return actionValueFuction.GetQ(currentState, currentAction);
    }

    public Action GetCurrentAction()
    {
        return currentAction;
    }

    public void SaveData()
    {
        Serializer.WriteToBinaryFile<Policy>("Assets/Resources/policyDyna.txt", policy);
        Serializer.WriteToBinaryFile<Q>("Assets/Resources/QDyna.txt", actionValueFuction);
        Serializer.WriteToBinaryFile<Model>("Assets/Resources/modelDyna.txt", model);
    }

    public void LoadData()
    {
        if(System.IO.File.Exists("Assets/Resources/QDyna.txt"))
        {
            //Load Q and Policy and initialize the StateDictionary
            policy = Serializer.ReadFromBinaryFile<Policy>("Assets/Resources/policyDyna.txt");
            actionValueFuction = Serializer.ReadFromBinaryFile<Q>("Assets/Resources/QDyna.txt");
            model = Serializer.ReadFromBinaryFile<Model>("Assets/Resources/modelDyna.txt");
            policy.InitializeStateDictionary();
            actionValueFuction.InitializeStateDictionary();
            model.InitializeStateDictionary();
        }
        else
        {
            policy = new Policy();
            actionValueFuction = new Q();
            model = new Model();
        }
    }
}
