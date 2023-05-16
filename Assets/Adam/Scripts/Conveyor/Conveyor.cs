using System;
using System.Collections;
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

    private float increment;

    private SpawnParts[] partSpawner;

    private void Start()
    {
        InitNodes();
        if (clone == null)
        {
            Debug.LogError("Provide a reference gameobject in the conveyor script. Inspector -> Debug Mode");
        }

        partSpawner = GameObject.Find("SpawnParts").GetComponents<SpawnParts>();

        StartCoroutine("Move");
    }
    private IEnumerator Move()
    {
        foreach (var comp in partSpawner)
        {
            comp.Spawn();
        }

        yield return new WaitForSecondsRealtime(5f);

        StartCoroutine("Move");

        // Iterate through each point to update transform
        for (int i = 0; i < points.Length; i++)
        {
            // Helper
            Point point = points[i];
            Node node = nodes[point.node];

            // Increment offset
            points[i].offset += increment;

            // Calculate position
            points[i].transform.position = (node.forward * point.offset) + node.position;

            // Modify current node
            if (point.offset > node.length)
            {
                int next = points[i].node + 1;
                points[i].offset -= node.length;

                if (nodes[next].isEnd)
                {
                    if (point.transform.childCount != 0)
                    {
                        Destroy(point.transform.GetChild(0).gameObject);
                    }
                }

                bool end = nodes[next].isEnd;

                points[i].node = end ? 0 : next;
                node = nodes[point.node];

                // ...
                points[i].transform.position = node.position;
                points[i].transform.forward = node.forward;
                points[i].transform.rotation = node.rotation;
            }
        }
    }

    public void InitNodes()
    {
        // Runtime edits
        if (Application.isPlaying)
        {
            // Destroy the children 😈
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }

            // ...
            beltLength = 0.0f;
        }

        // ...
        for (int i = 0; i < nodes.Length; i++)
        {
            // Mark endpoint
            if (i + 1 >= nodes.Length)
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

        increment = beltLength / amount;

        points = GeneratePoints().ToArray();
    }
    private IEnumerable<Point> GeneratePoints()
    {
        int index = 0;
        float offset = 0.0f;

        // Generate evenly spaced points (this was an absolute pain)
        for (int count = 1; count <= amount; count++)
        {
            // ...
            var node = nodes[index];

            // No space remains so switch to the next path
            if (offset > node.length)
            {
                // Completed point generation
                if (nodes[index + 1].isEnd)
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
            if (Application.isPlaying)
            {
                point.gameObject = Instantiate(clone, transform);
                point.gameObject.SetActive(true); // GameObjects spawned disabled?
                point.transform.position = (node.forward * offset) + node.position;
                point.transform.forward = node.forward;
                point.transform.rotation = node.rotation;
            }

            // ...
            point.offset = offset;
            point.node = index;

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
    public int node;
}