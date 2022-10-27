using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ExportManager))]
public class ExportEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ExportManager em = (ExportManager)target;

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Save Prefabs"))
            em.SavePrefabs();

        if (GUILayout.Button("Clear Folders"))
            em.ClearFolders();

        GUILayout.EndHorizontal();
    }
}
