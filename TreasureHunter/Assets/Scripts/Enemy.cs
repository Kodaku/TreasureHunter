using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Vector3 previousPosition;
    public void Move(MyGrid grid)
    {
        previousPosition = grid.NodeFromWorldPoint(transform.position).worldPosition;
        Vector3 tmpPosition = grid.NodeFromWorldPoint(transform.position).worldPosition;
        tmpPosition.x += 1.0f;
        transform.position = grid.NodeFromWorldPoint(tmpPosition).worldPosition;
    }

    public bool HasReachedDestination(MyGrid grid)
    {
        previousPosition = grid.NodeFromWorldPoint(previousPosition).worldPosition;
        return previousPosition == grid.NodeFromWorldPoint(transform.position).worldPosition;
    }

    public Vector3 GetPreviousPosition()
    {
        return previousPosition;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
}
