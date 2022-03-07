using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World
{
    //Key: Cell index; Value: state
    Dictionary<Vector3, Tuple<Vector3, CellInfo>> world;
    
    public void InitializeWorld(MyGrid grid, GameObject enemy, GameObject treasure)
    {
        world = new Dictionary<Vector3, Tuple<Vector3, CellInfo>>();
        foreach(Node node in grid.grid)
        {
            // Debug.Log(node.worldPosition);
            if(!world.ContainsKey(node.worldPosition))
                world.Add(node.worldPosition, Tuple.Create<Vector3, CellInfo>(node.worldPosition, CellInfo.FREE));
            else
                world[node.worldPosition] = Tuple.Create<Vector3, CellInfo>(node.worldPosition, CellInfo.FREE);
        }
        // Debug.Log(enemy.transform.position);
        world[enemy.transform.position] = Tuple.Create<Vector3, CellInfo>(enemy.transform.position, CellInfo.ENEMY);
        world[treasure.transform.position] = Tuple.Create<Vector3, CellInfo>(treasure.transform.position, CellInfo.TREASURE);
    }

    public void UpdateWorldEnemy(Vector3 oldPosition, Vector3 newPosition)
    {
        // Debug.Log(oldPosition);
        // Debug.Log(newPosition);
        world[oldPosition] = Tuple.Create<Vector3, CellInfo>(oldPosition, CellInfo.FREE);
        world[newPosition] = Tuple.Create<Vector3, CellInfo>(newPosition, CellInfo.ENEMY);
    }

    public void AddEnemy(Vector3 enemyPosition)
    {
        world[enemyPosition] = Tuple.Create<Vector3, CellInfo>(enemyPosition, CellInfo.ENEMY);
    }

    public void RemoveEnemy(Vector3 enemyPosition)
    {
        world[enemyPosition] = Tuple.Create<Vector3, CellInfo>(enemyPosition, CellInfo.FREE);
    }

    public void UpdateWorldTreasure(Vector3 oldPosition, Vector3 newPosition)
    {
        world[oldPosition] = Tuple.Create<Vector3, CellInfo>(oldPosition, CellInfo.FREE);
        world[newPosition] = Tuple.Create<Vector3, CellInfo>(newPosition, CellInfo.TREASURE);
    }

    public CellInfo GetWorldCellState(Vector3 worldPosition)
    {
        return world[worldPosition].Item2;
    }
}
