using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGrid : MonoBehaviour
{
    public Transform player;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public Node[,] grid;
    float nodeDiameter;
    int gridSizeX, gridSizeY;

    public void Initialize()
    {
        nodeDiameter = 2 * nodeRadius;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
        // print(grid.Length);
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;

        for(int x = 0; x < gridSizeX; x++)
        {
            for(int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius)
                                    + Vector3.up * (y * nodeDiameter + nodeRadius);
                // print(worldPoint);
                grid[x, y] = new Node(worldPoint);
            }
        }
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.y + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    public Node NodeFromGridPoint(int x, int y)
    {
        return grid[x, y];
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1.0f));

        // if(grid != null)
        // {
        //     // Node playerNode = NodeFromWorldPoint(player.position);
        //     foreach(Node node in grid)
        //     {
        //         Gizmos.color = Color.black;
        //         // if(playerNode == node)
        //         // {
        //         //     Gizmos.color = Color.green;
        //         // }
        //         Gizmos.DrawCube(node.worldPosition, Vector3.one * (nodeDiameter));
        //     }
        // }
    }

    public int columns
    {
        get { return Mathf.RoundToInt(grid.Length / gridSizeX); }
    }

    public int rows
    {
        get { return Mathf.RoundToInt(grid.Length / gridSizeY); }
    }
}
