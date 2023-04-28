using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Conveyor))]
public class ConveyorEditor : Editor
{
    private int selected = -1;

    public override void OnInspectorGUI()
    {
        var c_target = target as Conveyor;

        EditorGUI.BeginChangeCheck();

        c_target.amount = EditorGUILayout.IntField("Amount", c_target.amount);

        if(EditorGUI.EndChangeCheck())
        {
            c_target.InitNodes();
        }
    }

    public void OnSceneGUI()
    {
        Event gui_event = Event.current;
        var c_target = target as Conveyor;
        Vector2 mousePos = HandleUtility.GUIPointToWorldRay(gui_event.mousePosition).origin;

        Handles.CapFunction cap = Handles.CircleHandleCap;

        for(int i = 0; i < c_target.nodes.Length; i++)
        {
            //c_target.nodes[i].position = Handles.PositionHandle(c_target.nodes[i].position, Quaternion.identity);

            Handles.color = Color.white;
            Handles.DrawWireCube(c_target.nodes[i].position, Vector3.one * 0.1f);

            Handles.Label(c_target.nodes[i].position + Vector3.up * 0.1f, i.ToString());

            Handles.color = Color.cyan;
            if(i + 1 < c_target.nodes.Length)
            {
                Handles.DrawDottedLine(c_target.nodes[i].position, c_target.nodes[i + 1].position, 5f);
            }
        }
    }
}