using UnityEngine;
using System.Collections;


[ExecuteInEditMode]
public class Path : MonoBehaviour
{
    public Color pathColor = Color.green;
    public float nodeGizmoDiameter = 0.2f;

    public Transform[] nodes;


    void Start()
    {
        int numNodes = transform.childCount;
        if (numNodes > 1)
        {
            // Take all children and make them nodes
            nodes = new Transform[numNodes];
            for (int i = 0; i < numNodes; i++)
            {
                nodes[i] = transform.GetChild(i);
            }
        }
    }

    void OnDrawGizmos()
    {
        if (nodes != null)
        {
            Gizmos.color = pathColor;
            for (int i = 0; i < nodes.Length; i++)
            {
                if (i + 1 < nodes.Length)
                {
                    Gizmos.DrawLine(nodes[i].position, nodes[i + 1].position);
                }
            }
            for (int i = 0; i < nodes.Length; i++)
            {
                Gizmos.DrawSphere(nodes[i].position, nodeGizmoDiameter);
            }
        }
    }

}
