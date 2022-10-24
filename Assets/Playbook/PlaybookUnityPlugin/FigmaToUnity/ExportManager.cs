using System.IO;
using UnityEngine;
using UnityEditor;

public class ExportManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SavePrefabs()
    {
        string path = "Assets/Playbook/Prefabs";
        if (Directory.Exists(path)) { Directory.Delete(path, true); }
        Directory.CreateDirectory(path);

        // Keep track of the currently selected GameObject(s)
        GameObject[] objectArray = Selection.gameObjects;

        // Loop through every GameObject in the array above
        foreach (GameObject gameObject in objectArray)
        {
            // Create folder Prefabs and set the path as within the Prefabs folder,
            // and name it as the GameObject's name with the .Prefab format
            //if (Directory.Exists("Assets/Playbook/Prefabs"))
            //{
            //    Directory.Delete("Assets/Playbook/Prefabs");
            //    AssetDatabase.CreateFolder("Assets/Playbook", "Prefabs");
            //}   else 
            //    AssetDatabase.CreateFolder("Assets/Playbook", "Prefabs");
            string localPath = path + gameObject.name + ".prefab";

            // Make sure the file name is unique, in case an existing Prefab has the same name.
            localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

            // Create the new Prefab and log whether Prefab was saved successfully.
            bool prefabSuccess;
            PrefabUtility.SaveAsPrefabAsset(gameObject, localPath, out prefabSuccess);
            if (prefabSuccess == true)
                Debug.Log("Prefab was saved successfully");
            else
                Debug.Log("Prefab failed to save" + prefabSuccess);
        }
    }
}
