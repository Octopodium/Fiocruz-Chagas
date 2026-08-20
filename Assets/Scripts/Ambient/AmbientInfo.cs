using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "NewAmbient", menuName = "Game Data/Ambient", order = 1)]
public class AmbientInfo : ScriptableObject {
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
}
