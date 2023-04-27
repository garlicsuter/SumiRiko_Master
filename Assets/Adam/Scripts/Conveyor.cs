using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Conveyor : MonoBehaviour
{
    private _Transform[] points;
    private Node[] nodes;
    private float length;

    public int amount;
    public GameObject clone;

    private void Start()
    {
        InitNodes();

        foreach(var point in points)
        {
            var obj = Instantiate(clone, transform);
            obj.transform.position = point.position;
            obj.transform.rotation = point.rotation;
        }
    }
    private void Update()
    {

    }
    private void OnValidate()
    {
        if(Application.isPlaying == false)
            InitNodes();
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

        for(int i = 0; i < points.Length; i++)
        {
            Handles.DrawWireCube(points[i].position, Vector3.one * 0.05f);
            Handles.Label(points[i].position, (i + 1).ToString());
        }
    }

    private void InitNodes()
    {
        // Reinitialize value just in case
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

        points = GeneratePoints().ToArray();
    }
    private IEnumerable<_Transform> GeneratePoints()
    {
        int index = 0;
        float offset = 0.0f;

        float increment = length / amount;

        // Generate evenly spaced points (this was an absolute pain)
        for(int count = 1; count <= amount; count++)
        {
            // No space remains so switch to the next path
            if(offset > nodes[index].length)
            {
                offset -= nodes[index].length;
                index += 1;

                // Completed point generation
                if(nodes[index].isEnd)
                {
                    break;
                }
            }

            // Calculate location
            Vector3 direction = nodes[index + 1].position - nodes[index].position;
            Vector3 position = nodes[index].position + (offset * direction.normalized);

            // ...
            offset += increment;

            yield return new _Transform(position, Quaternion.LookRotation(direction));
        }
    }
}

[Serializable]
internal struct Node
{
    internal Vector3 position;
    internal float length;
    internal bool isEnd
    {
        get => length <= 0;
    }
}

[Serializable]
internal struct _Transform
{
    internal Vector3 position;
    internal Quaternion rotation;

    internal _Transform(Vector3 pos, Quaternion rot)
    {
        position = pos;
        rotation = rot;
    }
}