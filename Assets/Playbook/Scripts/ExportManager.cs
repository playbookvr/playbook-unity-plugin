using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class ExportManager : MonoBehaviour
{
    public enum Setting
    {
        Selected,
        All
    }
    [SerializeField] public Setting setting = Setting.All;

    private int exportCount;

    public void ClearFolders()
    {
        if (Directory.Exists("Assets/Exported")) { Directory.Delete("Assets/Exported", true); }
        Directory.CreateDirectory("Assets/Exported");

        if (Directory.Exists("Assets/Resources")) { Directory.Delete("Assets/Resources", true); }
        Directory.CreateDirectory("Assets/Resources");

        AssetDatabase.Refresh();
    }

    public void SavePrefabs()
    {
        if (!Directory.Exists("Assets/Exported"))
            AssetDatabase.CreateFolder("Assets", "Exported");

        if (!Directory.Exists("Assets/Resources"))
            AssetDatabase.CreateFolder("Assets", "Resources");

        if (!Directory.Exists("Assets/Exported/" + "Export_" + exportCount))
            AssetDatabase.CreateFolder("Assets/Exported", "Export_" + exportCount);

        if (!Directory.Exists("Assets/Resources/" + "Export_" + exportCount))
            AssetDatabase.CreateFolder("Assets/Resources", "Export_" + exportCount);

        GameObject[] objectArray = GameObject.FindGameObjectsWithTag("Exportable");
        if (setting == Setting.Selected)
            objectArray = Selection.gameObjects;

        //Assign all to root parent
        GameObject p = new GameObject("FigmaElements");
        List<GameObject> tempObjectsArray = new List<GameObject>();
        int count = 0;

        foreach (GameObject o in objectArray)
        {
            GameObject g = Instantiate(o);
            tempObjectsArray.Add(g);
            g.transform.parent = p.transform;

            Texture2D itemBGTex = g.GetComponentInChildren<Renderer>().material.mainTexture as Texture2D;
            itemBGTex.alphaIsTransparency = true;
            byte[] itemBGBytes = itemBGTex.EncodeToPNG();
            string texPath = "Assets/Resources/Export_" + exportCount + "/" + "Texture_" + count + ".png";
            File.WriteAllBytes(texPath, itemBGBytes);

            AssetDatabase.ImportAsset(texPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            //------------

            Material mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            mat.SetFloat("_Surface", 1.0f);
            mat.renderQueue = 2999;
            var texture = Resources.Load<Texture2D>("Export_" + exportCount +"/Texture_" + count);
            mat.mainTexture = texture;

            g.GetComponentInChildren<Renderer>().material = mat;

            string matPath = "Assets/Exported/Export_" + exportCount + "/" + "Material_" + count + ".mat";
            AssetDatabase.CreateAsset(mat, matPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            count++;
        }

        // Save grouped assets as prefab if assets have been imported
        if (p.transform.childCount > 0)
        {
            string localPath = "Assets/Exported/Export_" + exportCount + "/" + p.name + "_" + exportCount + ".prefab";
            localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

            bool prefabSuccess;
            PrefabUtility.SaveAsPrefabAsset(p, localPath, out prefabSuccess);
            if (prefabSuccess == true)
                Debug.Log("Prefab was saved successfully");
            else
                Debug.Log("Prefab failed to save" + prefabSuccess);

            exportCount++;
        }

        //Cleanup temp objects
        tempObjectsArray.Clear();
        Destroy(p);
        

    }

}
