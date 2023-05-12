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
        if(clone == null)
        {
            Debug.LogError("Provide a reference gameobject in the conveyor script. Inspector -> Debug Mode");
        }
    }
    private void Update()
    {
        // Iterate through each point to update transform
        for(int i = 0; i < points.Length; i++)
        {
            // Helper
            Node current = nodes[points[i].current];

            // Increment offset
            points[i].offset += Time.deltaTime * 0.1f;

            // Calculate position
            points[i].transform.position = (current.forward * points[i].offset) + current.position;

            // Modify current node
            if(points[i].offset > current.length)
            {
                int next = points[i].current + 1;
                points[i].offset -= current.length;

                // SHOWCASE CODE
                if(nodes[next].isEnd && points[i].transform.childCount > 0)
                {
                    Destroy(points[i].transform.GetChild(0).gameObject);
                }

                points[i].current = nodes[next].isEnd ? 0 : next;
                current = nodes[points[i].current];

                // ...
                points[i].transform.position = current.position;
                points[i].transform.forward = current.forward;
                points[i].transform.rotation = current.rotation;
            }
        }
    }

    public void InitNodes()
    {
        // Runtime edits
        if(Application.isPlaying)
        {
            // Destroy the children 😈
            for(int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }

            // ...
            beltLength = 0.0f;
        }

        // ...
        for(int i = 0; i < nodes.Length; i++)
        {
            // Mark endpoint
            if(i + 1 >= nodes.Length)
            {
                nodes[i].isEnd = true;
                break;
            }

            // Helpers
            Node current = nodes[i];
            Node next = nodes[i + 1];
            float distance = Vector3.Distance(next.position, current.position);
            Vector3 forward = (next.position - current.position).normalized;

            // ...
            nodes[i].position = current.position;
            nodes[i].forward = forward;
            nodes[i].rotation = Quaternion.LookRotation(forward);
            nodes[i].length = distance;

            // Increment total length
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
            var point = new Point();
            if(Application.isPlaying)
            {
                point.gameObject = Instantiate(clone, transform);
                point.gameObject.SetActive(true); // GameObjects spawned disabled?
                point.transform.position = (node.forward * offset) + node.position;
                point.transform.forward = node.forward;
                point.transform.rotation = node.rotation;
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
    public Quaternion rotation;

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