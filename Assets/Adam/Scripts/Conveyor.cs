using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Conveyor : MonoBehaviour
{
    public Node[] nodes;
    private Point[] points;
    private float beltLength;

    public int amount;
    public GameObject clone;

    private void Start()
    {
        InitNodes();
    }
    private void Update()
    {
        for (int i = 0; i < points.Length; i++)
        {
            Node current = nodes[points[i].current];

            // ...
            float movement = Time.deltaTime * 1f;
            points[i].offset += movement;
            points[i].transform.position = current.position + (points[i].transform.forward * points[i].offset);

            if (points[i].offset > current.length)
            {
                var next = points[i].current + 1;
                points[i].current = nodes[next].isEnd == false ? next : 0;

                points[i].offset -= current.length;
                current = nodes[points[i].current];

                points[i].transform.position = current.position;
                points[i].transform.forward = current.forward;
                points[i].transform.rotation = Quaternion.LookRotation(current.forward);
            }
        }
    }

    public void InitNodes()
    {
        // ...
        beltLength = 0.0f;

        // ...
        for (int i = 0; i < nodes.Length; i++)
        {
            // ...
            if(i + 1 >= nodes.Length)
            {
                nodes[i].isEnd = true;
                break;
            }

            Node cur = nodes[i];
            Node next = nodes[i + 1];

            float distance = Vector3.Distance(next.position, cur.position);

            // ...
            nodes[i].position = cur.position;
            nodes[i].forward = next.position - cur.position;
            nodes[i].length = distance;

            beltLength += distance;
        }

        points = GeneratePoints().ToArray();
    }
    private IEnumerable<Point> GeneratePoints()
    {
        int index = 0;
        float offset = 0.0f;

        float increment = beltLength / amount;

        // Generate evenly spaced points (this was an absolute pain)
        for(int count = 1; count <= amount; count++)
        {
            // ...
            var node = nodes[index];

            // No space remains so switch to the next path
            if(offset > node.length)
            {
                // Completed point generation
                if(nodes[index + 1].isEnd)
                {
                    break;
                }

                // ...
                offset -= node.length;
                node = nodes[index + 1];
                index += 1;
            }

            // ...
            Vector3 position = node.position + (offset * node.forward.normalized);
            Vector3 forward = node.forward;
            Quaternion rotation = Quaternion.LookRotation(node.forward);

            // ...
            var point = new Point();
            if(Application.isPlaying)
            {
                point.gameObject = Instantiate(clone, transform);
                point.gameObject.active = true; // GameObjects spawned disabled?
                point.transform.position = position;
                point.transform.forward = forward;
                point.transform.rotation = rotation;
            }

            // ...
            point.offset = offset;
            point.current = index;

            // ...
            offset += increment;

            yield return point;
        }
    }
}

[Serializable]
public struct Node
{
    public Vector3 position;
    public Vector3 forward;

    public float length;
    public bool isEnd;
}

[Serializable]
public struct Point
{
    public GameObject gameObject;
    public Transform transform => gameObject.transform;

    public float offset;
    public int current;
}