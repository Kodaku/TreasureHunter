using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

using State = System.Tuple<UnityEngine.Vector3, CellInfo, System.Tuple<CellInfo, CellInfo, CellInfo, CellInfo>>;
using ActionQ = System.Collections.Generic.Dictionary<Action, float>;
using ActionProbability = System.Collections.Generic.Dictionary<Action, float>;

[Serializable]
public class DataKeeper
{
    private Dictionary<State, ActionQ> Q = new Dictionary<State, ActionQ>();
    private Dictionary<State, ActionProbability> policy = new Dictionary<State, ActionProbability>();

    public void SetQ(Dictionary<State, ActionQ> newQ)
    {
        Q = newQ;
    }

    public Dictionary<State, ActionQ> GetQ()
    {
        return Q;
    }

    public void SetPolicy(Dictionary<State, ActionProbability> newPolicy)
    {
        policy = newPolicy;
    }

    public Dictionary<State, ActionProbability> GetPolicy()
    {
        return policy;
    }
}
