using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections;

public class ExportManager : MonoBehaviour
{
    public Texture2D s;
    public enum Setting
    {
        Selected,
        All
    }
    [SerializeField] public Setting setting = Setting.All;

    private int exportCount;
    private int objectCount;

    public void ClearFolders()
    {
        if (Directory.Exists("Assets/Exported")) { Directory.Delete("Assets/Exported", true); }
        Directory.CreateDirectory("Assets/Exported");

        if (Directory.Exists("Assets/Resources")) { Directory.Delete("Assets/Resources", true); }
        Directory.CreateDirectory("Assets/Resources");

        AssetDatabase.Refresh();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("Testing!");

            GameObject[] objectArray = GameObject.FindGameObjectsWithTag("Exportable");
            int count = 0;

            foreach (GameObject g in objectArray)
            {
                Texture2D itemBGTex = objectArray[0].GetComponentInChildren<Renderer>().material.mainTexture as Texture2D;
                itemBGTex.alphaIsTransparency = true;
                byte[] itemBGBytes = itemBGTex.EncodeToPNG();
                string texPath = "Assets/Resources/" + "Texture_" + count + ".png";
                File.WriteAllBytes(texPath, itemBGBytes);

                AssetDatabase.ImportAsset(texPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                //------------

                Material mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
                mat.SetFloat("_Surface", 1.0f);
                mat.renderQueue = 2999;

                var texture = Resources.Load<Texture2D>("Texture_" + count);
                Debug.Log(texture);
                mat.mainTexture = texture;

                string matPath = "Assets/Exported/" + "Material_" + count + ".mat";
                AssetDatabase.CreateAsset(mat, matPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                count++;
            }

        }

    }

    public void SavePrefabs()
    {
        GameObject[] objectArray = GameObject.FindGameObjectsWithTag("Exportable");
        if (setting == Setting.Selected)
            objectArray = Selection.gameObjects;

        //Assign all to root parent
        GameObject p = new GameObject("FigmaElements");

        foreach (GameObject g in objectArray)
        {
            g.transform.parent = p.transform;

            Texture2D itemBGTex = g.GetComponentInChildren<Renderer>().material.mainTexture as Texture2D;
            //Texture2D itemBGTex = s;
            itemBGTex.alphaIsTransparency = true;
            byte[] itemBGBytes = itemBGTex.EncodeToPNG();
            string texPath = "Assets/Exported/" + "Texture_" + objectCount + ".png";
            File.WriteAllBytes(texPath, itemBGBytes);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Material mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            mat.SetFloat("_Surface", 1.0f);
            mat.renderQueue = 2999;
           // mat.mainTexture = LoadPNG(texPath);
            string matPath = "Assets/Exported/" + "Material_" + objectCount + ".mat";
            AssetDatabase.CreateAsset(mat, matPath);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            g.GetComponentInChildren<Renderer>().material = mat;
            objectCount++;

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
