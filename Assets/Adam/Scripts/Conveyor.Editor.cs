using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(Conveyor))]
public class ConveyorEditor : Editor
{
    private bool cloned = false;
    private int selected;

    public override void OnInspectorGUI()
    {
        Event gui_event = Event.current;
        var c_target = target as Conveyor;

        EditorGUI.BeginChangeCheck();

        c_target.amount = EditorGUILayout.IntField("Amount", c_target.amount);

        for (int i = 0; i < c_target.nodes.Length; i++)
        {
            var old = c_target.nodes[i].position;
            c_target.nodes[i].position = EditorGUILayout.Vector3Field($"Node: {i + 1}", old);
        }

        if (EditorGUI.EndChangeCheck())
        {
            c_target.InitNodes();
        }
    }

    public void OnSceneGUI()
    {
        Event gui_event = Event.current;
        var c_target = target as Conveyor;

        Debug.Log(gui_event.button == 0);

        if (gui_event.type == EventType.KeyUp) // Scuffed
        {
            cloned = false;
        }

        for (int i = 0; i < c_target.nodes.Length; i++)
        {
            var style = new GUIStyle();
            style.normal.textColor = Color.black;

            Handles.Label(c_target.nodes[i].position + (Vector3.up * 0.25f), $"{i}", style);

            Handles.color = Color.cyan;
            if (c_target.nodes.Length > i + 1)
            {
                Handles.DrawDottedLine(c_target.nodes[i].position, c_target.nodes[i + 1].position, 5f);
            }

            float dist = HandleUtility.DistanceToCircle(c_target.nodes[i].position, 0.1f);
            if (selected == -1 && dist == 0)
            {
                selected = i;
            }

            if (i == selected)
            {
                c_target.nodes[i].position = Handles.PositionHandle(c_target.nodes[i].position, Quaternion.identity);

                if (gui_event.button == 0 && gui_event.modifiers == EventModifiers.Shift && cloned == false)
                {
                    cloned = true;
                    Debug.Log(cloned);

                    var list = new List<Node>(c_target.nodes);
                    list.Insert(i + 1, new Node());
                    c_target.nodes = list.ToArray();
                }
            }
            else
            {
                Handles.color = Color.white;
                Handles.DrawWireCube(c_target.nodes[i].position, Vector3.one * 0.1f);
            }
        }

        selected = -1;
    }
}