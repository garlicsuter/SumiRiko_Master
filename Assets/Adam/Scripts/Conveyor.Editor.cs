using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(Conveyor))]
public class ConveyorEditor : Editor
{
    private bool cloned = false;

    private bool shiftDown;

    private int selected = -1;

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
        Event evt = Event.current;
        var c_target = target as Conveyor;

        ResetShift(evt);

        for (int i = 0; i < c_target.nodes.Length; i++)
        {
            // Style text
            var style = new GUIStyle();
            style.normal.textColor = Color.black;

            Handles.Label(c_target.nodes[i].position + (Vector3.up * 0.25f), $"{i}", style);

            Handles.color = Color.cyan;
            if (c_target.nodes.Length > i + 1)
            {
                Handles.DrawDottedLine(c_target.nodes[i].position, c_target.nodes[i + 1].position, 5f);
            }

            // Use a button to select elements
            if (selected != i)
            {
                Handles.color = Color.white;

                bool pressed = Handles.Button(c_target.nodes[i].position, Quaternion.identity, 0.125f, 0.125f, Handles.SphereHandleCap);
                selected = pressed ? i : selected;
            }
        }

        if (selected != -1)
        {
            c_target.nodes[selected].position = Handles.PositionHandle(c_target.nodes[selected].position, Quaternion.identity);

            // Mouse events are unreliable so exclude them altogether...
            if (evt.modifiers == EventModifiers.Shift && cloned == false)
            {
                // ...
                var list = new List<Node>(c_target.nodes);
                list.Insert(selected + 1, c_target.nodes[selected]);
                c_target.nodes = list.ToArray();

                cloned = true;
                selected += 1;
            }
        }
    }

    private void ResetShift(Event evt)
    {
        if (evt.modifiers == EventModifiers.Shift)
        {
            shiftDown = true;
        }
        else if (shiftDown == true)
        {
            shiftDown = false;
            cloned = false;
        }
    }
}