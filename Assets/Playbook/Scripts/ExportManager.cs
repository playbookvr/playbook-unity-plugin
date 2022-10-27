using System.IO;
using UnityEngine;
using UnityEditor;

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

    public void ClearFolder()
    {
        if (Directory.Exists("Assets/Exported")) { Directory.Delete("Assets/Exported", true); }
        Directory.CreateDirectory("Assets/Exported");
        AssetDatabase.Refresh();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("Texture");
            GameObject[] objectArray = GameObject.FindGameObjectsWithTag("Exportable");
            int count = 0;
            foreach (GameObject g in objectArray)
            {
                count++;
                Texture2D itemBGTex = g.GetComponentInChildren<Renderer>().material.mainTexture as Texture2D;
                itemBGTex.alphaIsTransparency = true;
                byte[] itemBGBytes = itemBGTex.EncodeToPNG();
                string texPath = "Assets/Exported/" + "Texture_" + count + ".png";
                File.WriteAllBytes(texPath, itemBGBytes);
                AssetDatabase.ImportAsset(texPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

            }

        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("Material");
            GameObject[] objectArray = GameObject.FindGameObjectsWithTag("Exportable");
            int count = 0;
            string texPath = "Assets/Exported/" + "Texture_" + count + ".png";
            foreach (GameObject g in objectArray)
            {
                count++;
                Material mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
                mat.SetFloat("_Surface", 1.0f);
                mat.renderQueue = 2999;
                mat.mainTexture = LoadPNG(texPath);
                string matPath = "Assets/Exported/" + "Material_" + count + ".mat";
                AssetDatabase.CreateAsset(mat, matPath);

                AssetDatabase.SaveAssets();
            }
        }
    }

    public void SavePrefabs()
    {
        GameObject[] objectArray = GameObject.FindGameObjectsWithTag("Exportable");
        if (setting == Setting.Selected)
            objectArray = Selection.gameObjects;


        //-------------------------------

        //Texture2D itemBGTex = s.texture;
        //Debug.Log(itemBGTex);
        //byte[] itemBGBytes = itemBGTex.EncodeToPNG();
        //File.WriteAllBytes("Assets/Exported/" + "Background.png", itemBGBytes);

        //Material mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        //mat.SetFloat("_Surface", 1.0f);
        //mat.mainTexture = itemBGTex;
        //mat.renderQueue = 2999;
        //string matPath = "Assets/Exported/" + itemBGTex.name + ".mat";
        //AssetDatabase.CreateAsset(mat, matPath);

        //AssetDatabase.SaveAssets();
        //AssetDatabase.Refresh();

        //-------------------------------

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
            mat.mainTexture = LoadPNG(texPath);
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

    public static Texture2D LoadPNG(string filePath)
    {

        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        else
            Debug.Log("No asset at path!");
        return tex;
    }

}
