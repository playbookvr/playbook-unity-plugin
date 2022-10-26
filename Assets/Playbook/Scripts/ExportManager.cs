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
    public Setting setting = Setting.All;

    public void SavePrefabs()
    {
        GameObject[] objectArray = GameObject.FindGameObjectsWithTag("Exportable");
        if (setting == Setting.Selected)
            objectArray = Selection.gameObjects;

        ////mat.renderQueue = 2999; // We change the renderQueue to always have images render on top of panels

        //Assign all to root parent
        //GameObject p = new GameObject("FigmaElements");
        foreach (GameObject gameObject in objectArray)
        {
            //gameObject.transform.parent = p.transform;
            //Texture tex = gameObject.GetComponentInChildren<Renderer>().material.mainTexture;
            //Texture2D texture = new Texture2D(tex.width, tex.height, TextureFormat.RGB24, false);
            //byte[] bytes = texture.EncodeToPNG();
            //File.WriteAllBytes("Assets/Exported" + "image1" + ".png", bytes);
            //AssetDatabase.SaveAssets();
            //AssetDatabase.Refresh();

            Texture2D itemBGTex = s.texture;
            byte[] itemBGBytes = itemBGTex.EncodeToPNG();
            File.WriteAllBytes("Assets/Exported" + "Background.png", itemBGBytes);
            AssetDatabase.Refresh();

            //Material mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            //mat.SetFloat("_Surface", 1.0f);
            //mat.mainTexture = tex;
            //string matPath = "Assets/Exported/"+ tex.name + ".mat";
            //AssetDatabase.CreateAsset(mat, matPath);


        }

        // Save prefab ---------------------------------
        //if (!Directory.Exists("Assets/Exported"))
        //    AssetDatabase.CreateFolder("Assets", "Exported");
        //string localPath = "Assets/Exported/" + gameObject.name + ".prefab";

        //// Make sure the file name is unique, in case an existing Prefab has the same name.
        //localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

        //// Create the new Prefab and log whether Prefab was saved successfully.
        //bool prefabSuccess;
        //PrefabUtility.SaveAsPrefabAsset(gameObject, localPath, out prefabSuccess);
        //if (prefabSuccess == true)
        //    Debug.Log("Prefab was saved successfully");
        //else
        //    Debug.Log("Prefab failed to save" + prefabSuccess);
    }

}
