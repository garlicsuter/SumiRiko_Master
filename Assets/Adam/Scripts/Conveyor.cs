using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Conveyor : MonoBehaviour
{
    private Node[] nodes;
    private float length;

    public int amount;

    private void OnValidate()
    {
        // Reset value
        length = 0.0f;

        // Init nodes
        nodes = new Node[transform.childCount];
        for(int i = 0; i < transform.childCount; i++)
        {
            // Fetch current & next child
            Transform child = transform.GetChild(i);
            Transform next = ((i + 1) < transform.childCount) ? transform.GetChild(i + 1) : null;

            // Calculate distance between children
            float dist = next != null ? Vector3.Distance(child.position, next.position) : 0.0f;

            // Set values
            nodes[i].position = child.position;
            nodes[i].length = dist;
            length += dist;
        }
    }
    private void OnDrawGizmos()
    {
        for(int i = 0; i < nodes.Length; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(nodes[i].position, 0.1f);

            // Does this node continue?
            if(nodes[i].isEnd == false)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(nodes[i].position, nodes[i + 1].position);
            }
        }

        Gizmos.color = Color.green;

        int index = 0;

        float accumulation = 0.0f;
        float increment = length / (amount + 1);

        for(int count = 1; count <= amount; count++)
        {
            float offset = increment * count - accumulation;

            if(nodes[index].length < offset)
            {
                accumulation += nodes[index].length + (offset - nodes[index].length);
                index += 1;
            }

            if(nodes[index].isEnd == false)
            {
                Vector3 dir = (nodes[index + 1].position - nodes[index].position).normalized;
                Gizmos.DrawWireSphere(nodes[index].position + (dir * offset), 0.05f);
            }
            else
            {
                print($"[FINISHED] - [Index: {index}] [Count: {count}]");
                break;
            };

            print($"[Index: {index}] [Count: {count}] [Offset: {offset}]");
        }
    }
}

[Serializable]
internal struct Node
{
    internal Vector3 position;
    internal float length;
    internal bool isEnd => length == 0;
}