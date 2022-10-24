using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ExportManager))]
public class ExportEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ExportManager em = (ExportManager)target;
        if (GUILayout.Button("Save Prefabs"))
        {
            em.SavePrefabs();
        }
    }
}
