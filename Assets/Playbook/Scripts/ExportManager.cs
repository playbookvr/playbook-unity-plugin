using System.IO;
using UnityEngine;
using UnityEditor;

public class ExportManager : MonoBehaviour
{
    public Sprite s;
    public enum Setting
    {
        Selected,
        All
    }
    [SerializeField] public bool ClearOnStart = true;
    [SerializeField] public Setting setting = Setting.All;

    private int exportCount;

    private void Awake()
    {
        if (ClearOnStart)
            ClearFolder();
    }

    public void ClearFolder()
    {
        if (Directory.Exists("Assets/Exported")) { Directory.Delete("Assets/Exported", true); }
        Directory.CreateDirectory("Assets/Exported");
    }

    public void SavePrefabs()
    {
        GameObject[] objectArray = GameObject.FindGameObjectsWithTag("Exportable");
        if (setting == Setting.Selected)
            objectArray = Selection.gameObjects;


        //-------------------------------

        Texture2D itemBGTex = s.texture;
        Debug.Log(itemBGTex);
        byte[] itemBGBytes = itemBGTex.EncodeToPNG();
        File.WriteAllBytes("Assets/Exported/" + "Background.png", itemBGBytes);

        Material mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        mat.SetFloat("_Surface", 1.0f);
        mat.mainTexture = itemBGTex;
        mat.renderQueue = 2999;
        string matPath = "Assets/Exported/" + itemBGTex.name + ".mat";
        AssetDatabase.CreateAsset(mat, matPath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        //-------------------------------

        //Assign all to root parent
        GameObject p = new GameObject("FigmaElements");

        foreach (GameObject g in objectArray)
        {
            g.transform.parent = p.transform;
            g.GetComponentInChildren<Renderer>().material = mat;

        }

        // Save grouped assets as prefab if assets have been imported
        if(objectArray.Length > 0)
        {
            if (!Directory.Exists("Assets/Exported"))
                AssetDatabase.CreateFolder("Assets", "Exported");

            string localPath = "Assets/Exported/" + p.name + "_" + exportCount + ".prefab";
            localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

            bool prefabSuccess;
            PrefabUtility.SaveAsPrefabAsset(p, localPath, out prefabSuccess);
            if (prefabSuccess == true)
                Debug.Log("Prefab was saved successfully");
            else
                Debug.Log("Prefab failed to save" + prefabSuccess);

            exportCount++;
        }
        

    }

}
