using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Vector3 worldPosition;
    private CellInfo m_cellInfo;

    public Node(Vector3 _worldPosition)
    {
        worldPosition = _worldPosition;
    }

    public CellInfo cellInfo
    {
        get { return m_cellInfo; }
        set { m_cellInfo = value; }
    }
}
