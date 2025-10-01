using Assets.Game.Scripts.Handlers;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridCellHandler))]
public class GridCellHandlerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GridCellHandler gridCellHandler = (GridCellHandler)target;

        DrawDefaultInspector();

        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Current Item Info is available only in Play Mode.", MessageType.Info);
            return;
        }

        if (gridCellHandler.CurrentItem != null)
            EditorGUILayout.LabelField($"Current Item Info: {(gridCellHandler.CurrentItem as MonoBehaviour).name}", EditorStyles.boldLabel);
        else
            EditorGUILayout.LabelField("Cell doesn't have any item!", EditorStyles.boldLabel);
    }
}
