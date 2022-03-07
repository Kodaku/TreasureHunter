using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HashedState = System.Tuple<UnityEngine.Vector3, CellInfo, System.Tuple<CellInfo, CellInfo, CellInfo, CellInfo>>;

[System.Serializable]
public class Q
{
    List<PlayerState> QFuncion = new List<PlayerState>();

    public bool ContainsState(HashedState hashedState)
    {
        return StateDictionary.ContainsState(hashedState);
    }

    public void AddPlayerState(PlayerState playerState)
    {
        QFuncion.Add(playerState);
        StateDictionary.AddPlayerState(playerState);
    }

    public void InitializeStateDictionary()
    {
        foreach(PlayerState playerState in QFuncion)
        {
            StateDictionary.AddPlayerState(playerState);
        }
    }

    public Action GetBestAction(HashedState hashedState)
    {
        PlayerState playerState = StateDictionary.GetPlayerState(hashedState);
        return playerState.GetBestAction();
    }

    public float GetMaxQ(HashedState hashedState)
    {
        PlayerState playerState = StateDictionary.GetPlayerState(hashedState);
        return playerState.GetMaxQ();
    }

    public void Update(HashedState hashedState, Action currentAction, float maxQ, float reward)
    {
        PlayerState playerState = StateDictionary.GetPlayerState(hashedState);
        playerState.UpdateActionValue(currentAction, maxQ, reward);
    }
}
