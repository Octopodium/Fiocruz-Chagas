using System;
using UnityEngine;

/// <summary>
/// Scriptable Object that registers all valid flags. Should only be one on the project.
/// </summary>
[CreateAssetMenu(fileName = "FlagsDescriptor", menuName = "Scriptable Objects/Flags Descriptor", order = 1)]
public class FlagsRegister : ScriptableObject {
    public static string mainRegisterResourcePath = "FlagsRegister";
    public static FlagsRegister instance => GetRegister();


    public FlagDescriptor[] availableFlags;


    /// <summary>
    /// Returns a FlagDescriptor with the same name as parameter.
    /// </summary>
    /// <param name="name">The FlagDescriptor's name</param>
    /// <returns>The FlagDescriptor with the same name as the parameter, or null if not found.</returns>
    public FlagDescriptor GetDescriptorByName(string name) {
        foreach (FlagDescriptor descriptor in availableFlags) {
            if (descriptor.name == name) return descriptor;
        }

        return null;
    }
    
    /// <summary>
    /// Gets the project's Flags Register using Resources.Load on the 'mainRegisterResourcePath'.
    /// </summary>
    /// <returns>The project's Flags Register</returns>
    public static FlagsRegister GetRegister() {
        return Resources.Load<FlagsRegister>(mainRegisterResourcePath);
    }

    /// <summary>
    /// Gets the project's Flags Register using Resources.LoadAsync on the 'mainRegisterResourcePath'. Will call the callback when finished.
    /// </summary>
    /// <param name="callback">Action to be called when FlagsRegister is loaded</param>
    public static void GetRegisterCallback(Action<FlagsRegister> callback) {
        Resources.LoadAsync<FlagsRegister>(mainRegisterResourcePath).completed += (op) => {
            ResourceRequest req = (ResourceRequest) op;
            callback?.Invoke((FlagsRegister) req.asset);
        };
    }

}
