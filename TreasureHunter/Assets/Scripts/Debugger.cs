using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debugger : MonoBehaviour
{
    private float scaledQ;
    public GameObject valueUp;
    public GameObject valueDown;
    public GameObject valueLeft;
    public GameObject valueRight;
    private Dictionary<Vector3, GameObject> valueBreadcrumbs = new Dictionary<Vector3, GameObject>();

    public void SetColors(float currentQ, float minQValue, float maxQValue)
    {
        scaledQ = (currentQ - minQValue) / (maxQValue - minQValue + Mathf.Epsilon);
    }

    public void SpawnMove(Player player, MyGrid grid)
    {
        // print(currentQ);
        Action playerAction = player.GetCurrentAction();
        Node playerNode = grid.NodeFromWorldPoint(player.transform.position);
        switch(playerAction)
        {
            case Action.UP:
            {
                if(valueBreadcrumbs.ContainsKey(playerNode.worldPosition))
                {
                    Destroy(valueBreadcrumbs[playerNode.worldPosition]);
                    valueBreadcrumbs.Remove(playerNode.worldPosition);
                }
                GameObject newArrow = Instantiate(valueUp, playerNode.worldPosition, Quaternion.identity);
                newArrow.transform.parent = transform;
                SpriteRenderer spriteRenderer = newArrow.GetComponent<SpriteRenderer>();
                spriteRenderer.color = new Color(scaledQ, scaledQ, 0.0f);
                valueBreadcrumbs.Add(playerNode.worldPosition, newArrow);
                break;
            }
            case Action.DOWN:
            {
                if(valueBreadcrumbs.ContainsKey(playerNode.worldPosition))
                {
                    Destroy(valueBreadcrumbs[playerNode.worldPosition]);
                    valueBreadcrumbs.Remove(playerNode.worldPosition);
                }
                GameObject newArrow = Instantiate(valueDown, playerNode.worldPosition, Quaternion.identity);
                newArrow.transform.parent = transform;
                SpriteRenderer spriteRenderer = newArrow.GetComponent<SpriteRenderer>();
                spriteRenderer.color = new Color(scaledQ, scaledQ, 0.0f);
                valueBreadcrumbs.Add(playerNode.worldPosition, newArrow);
                break;
            }
            case Action.LEFT:
            {
                if(valueBreadcrumbs.ContainsKey(playerNode.worldPosition))
                {
                    Destroy(valueBreadcrumbs[playerNode.worldPosition]);
                    valueBreadcrumbs.Remove(playerNode.worldPosition);
                }
                GameObject newArrow = Instantiate(valueLeft, playerNode.worldPosition, Quaternion.identity);
                newArrow.transform.parent = transform;
                SpriteRenderer spriteRenderer = newArrow.GetComponent<SpriteRenderer>();
                spriteRenderer.color = new Color(scaledQ, scaledQ, 0.0f);
                valueBreadcrumbs.Add(playerNode.worldPosition, newArrow);
                break;
            }
            case Action.RIGHT:
            {
                if(valueBreadcrumbs.ContainsKey(playerNode.worldPosition))
                {
                    Destroy(valueBreadcrumbs[playerNode.worldPosition]);
                    valueBreadcrumbs.Remove(playerNode.worldPosition);
                }
                GameObject newArrow = Instantiate(valueRight, playerNode.worldPosition, Quaternion.identity);
                newArrow.transform.parent = transform;
                SpriteRenderer spriteRenderer = newArrow.GetComponent<SpriteRenderer>();
                spriteRenderer.color = new Color(scaledQ, scaledQ, 0.0f);
                valueBreadcrumbs.Add(playerNode.worldPosition, newArrow);
                break;
            }
        }
    }

    public void Reset()
    {
        foreach(Vector3 value in valueBreadcrumbs.Keys)
        {
            Destroy(valueBreadcrumbs[value]);
        }
        valueBreadcrumbs.Clear();
    }
}
