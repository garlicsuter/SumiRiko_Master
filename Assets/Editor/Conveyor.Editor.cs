using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(Conveyor))]
public class ConveyorEditor : Editor
{
    private Conveyor c_target => target as Conveyor;

    private int selection = -1;

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        // Inspector values
        EditorGUILayout.PropertyField(serializedObject.FindProperty("nodes"));
        c_target.amount = EditorGUILayout.IntField("Amount", c_target.amount);

        // Will at least one node into existance
        if(c_target.nodes.Length == 0)
        {
            c_target.nodes = new Node[1];
        }

        if(EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            OnNodeChange();
        }
    }

    public void OnSceneGUI()
    {
        // ...
        Event evt = Event.current;

        // Handle input modifiers
        if(evt.isKey && evt.type == EventType.KeyUp)
        {
            switch(evt.keyCode)
            {
                case KeyCode.RightShift:
                case KeyCode.LeftShift:
                    InsertNode();
                    break;

                case KeyCode.RightAlt:
                case KeyCode.LeftAlt:
                    DeleteNode();
                    break;
            }
        }

        // Handle selection and draw nodes
        for(int i = 0; i < c_target.nodes.Length; i++)
        {
            // Draw styled text
            var style = new GUIStyle();
            style.normal.textColor = Color.black;
            Handles.Label(c_target.nodes[i].position + (Vector3.up * 0.25f), $"{i}", style);

            // Draw dotted lines
            Handles.color = Color.cyan;
            if(c_target.nodes.Length > i + 1)
            {
                Handles.DrawDottedLine(c_target.nodes[i].position, c_target.nodes[i + 1].position, 5f);
            }

            // Select the specific node that the user clicked on
            if(selection != i)
            {
                Handles.color = Color.white;

                bool pressed = Handles.Button(c_target.nodes[i].position, Quaternion.identity, 0.125f, 0.125f, Handles.SphereHandleCap);
                selection = pressed ? i : selection;
            }
        }

        // Something is selected so deal with it
        if(selection != -1)
        {
            Vector3 newPos = Handles.PositionHandle(c_target.nodes[selection].position, Quaternion.identity);

            if(newPos != c_target.nodes[selection].position)
            {
                c_target.nodes[selection].position = newPos;
                OnNodeChange();
            }
        }
    }

    /// <summary>
    /// Insert a node after the current selection
    /// </summary>
    private void InsertNode()
    {
        // Design decision requires the use of this method
        var list = new List<Node>(c_target.nodes);
        list.Insert(selection + 1, c_target.nodes[selection]);
        c_target.nodes = list.ToArray();

        // Modify endpoint
        if(c_target.nodes[selection].isEnd)
        {
            c_target.nodes[selection].isEnd = false;
            c_target.nodes[selection + 1].isEnd = true;
        }

        selection += 1;

        OnNodeChange();
    }

    /// <summary>
    /// Delete the selection
    /// </summary>
    private void DeleteNode()
    {
        // Ignore the first node
        if(selection > 0)
        {
            // Modify endpoint
            if(c_target.nodes[selection].isEnd)
            {
                c_target.nodes[selection - 1].isEnd = true;
            }

            // Design decision requires the use of this method
            var list = new List<Node>(c_target.nodes);
            list.RemoveAt(selection);
            c_target.nodes = list.ToArray();

            selection -= 1;

            OnNodeChange();
        }
    }

    private void OnNodeChange() => c_target.InitNodes();
}