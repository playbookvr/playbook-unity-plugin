using UnityEditor;
using UnityEngine;

namespace Playbook.Scripts.Figma
{
    [CustomEditor(typeof(FigmaSyncManager))]
    public class FigmaSyncEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            FigmaSyncManager fs = (FigmaSyncManager)target;
            if (GUILayout.Button("Import from Figma"))
            {
                fs.SyncFigmaFile(-1);
                Debug.Log("Import!");
            }
        }
    }
}
