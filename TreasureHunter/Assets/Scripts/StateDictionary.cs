using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HashedState = System.Tuple<UnityEngine.Vector3, CellInfo, System.Tuple<CellInfo, CellInfo, CellInfo, CellInfo>>;

public class StateDictionary
{
    private static Dictionary<HashedState, PlayerState> stateDictionary = new Dictionary<HashedState, PlayerState>();

    public static void AddPlayerState(PlayerState playerState)
    {
        playerState.GenerateHashCode();
        HashedState hashedState = playerState.GetHashedState();
        if(!stateDictionary.ContainsKey(hashedState))
        {
            stateDictionary.Add(hashedState, playerState);
        }
    }

    public static PlayerState GetPlayerState(HashedState hashedState)
    {
        if(stateDictionary.ContainsKey(hashedState))
        {
            return stateDictionary[hashedState];
        }
        else
        {
            return null;
        }
    }

    public static bool ContainsState(HashedState hashedState)
    {
        return stateDictionary.ContainsKey(hashedState);
    }
}
