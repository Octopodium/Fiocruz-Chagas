using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "NewAmbient", menuName = "Scriptable Objects/Ambient", order = 1)]
public class AmbientInfo : ScriptableObject {
    public static string folderPath = "Ambients/";
    public string ambientName;

    #if UNITY_EDITOR
    // Allows drag and drop in the Unity editor window
    public UnityEditor.SceneAsset sceneAsset; 
    #endif

    [HideInInspector] public string sceneName;

    // Updates the scene name automatically if changed in Editor
    private void OnValidate()
    {
        #if UNITY_EDITOR
        if (sceneAsset != null)
        {
            sceneName = sceneAsset.name;
        }
        #endif
    }


    public static AmbientInfo GetAmbient(string ambientName) {
        return Resources.Load<AmbientInfo>(folderPath + ambientName);
    }

    public static AmbientInfo[] GetAmbients() {
        return Resources.LoadAll<AmbientInfo>(folderPath);
    }
}
