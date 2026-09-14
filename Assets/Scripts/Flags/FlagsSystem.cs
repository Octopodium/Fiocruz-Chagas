using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The flag control system. Stores every setted flag value. Used for setting and getting flags.
/// </summary>
public class FlagsSystem : MonoBehaviour {
    public static FlagsSystem instance => GameManager.instance.flags;
    FlagsRegister register;
    Dictionary<string, object> flags = new Dictionary<string, object>();

    public Action<string, object> OnFlagChanged;


    void Awake() {
        register = FlagsRegister.instance;
    }

    
    /// <summary>
    /// Sets a flag with a value.
    /// </summary>
    /// <param name="flagName">The flag's name. It must be stored in the FlagsRegister.</param>
    /// <param name="value">The value to be setted. Even though FlagsDescriptor suggests a type, the system never checks it, this may change in the future.</param>
    /// <returns>Returns true if the flag was setted. If the 'flagName' is invalid, will return false.</returns>
    public bool SetFlag(string flagName, object value) {
        if (!flags.ContainsKey(flagName) && !IsValidFlag(flagName)) return false;
        flags[flagName] = value;

        OnFlagChanged?.Invoke(flagName, value);
        return true;
    }

    /// <summary>
    /// Gets a setted flag's value (regardless of the type).
    /// </summary>
    /// <param name="flagName">The flag's name</param>
    /// <returns>Returns the flag's value as an object type. You can cast it to the correct type if you know it. Returns null if flag not setted.</returns>
    public object GetFlag(string flagName) {
        if (!flags.ContainsKey(flagName)) return null;
        return flags[flagName];
    }
    
    /// <summary>
    /// Gets a setted flag's int value.
    /// </summary>
    /// <param name="flagName">The flag's name</param>
    /// <returns>Returns the flag's int value. If flag's not an INT type or is not setted, returns the default value for an int.</returns>
    public int GetFlagInt(string flagName) {
        if (!flags.ContainsKey(flagName) || GetFlagType(flagName) != FlagType.INT) return default;
        return (int) flags[flagName];
    }

    /// <summary>
    /// Gets a setted flag's float value.
    /// </summary>
    /// <param name="flagName">The flag's name</param>
    /// <returns>Returns the flag's float value. If flag's not a FLOAT type or is not setted, returns the default value for a float.</returns>
    public float GetFlagFloat(string flagName) {
        if (!flags.ContainsKey(flagName) || GetFlagType(flagName) != FlagType.FLOAT) return default;
        return (float) flags[flagName];
    }

    // <summary>
    /// Gets any setted flag's string value.
    /// </summary>
    /// <param name="flagName">The flag's name</param>
    /// <returns>Returns the flag's string value. If flag's not a STRING type or is not setted, returns the default value for a string.</returns>
    public string GetFlagString(string flagName) {
        if (!flags.ContainsKey(flagName) || GetFlagType(flagName) != FlagType.STRING) return default;
        return (string) flags[flagName];
    }

    // <summary>
    /// Gets any setted flag's bool value.
    /// </summary>
    /// <param name="flagName">The flag's name</param>
    /// <returns>Returns the flag's bool value. If flag's not a BOOL type or is not setted, returns the default value for a bool.</returns>
    public bool GetFlagBool(string flagName) {
        if (!flags.ContainsKey(flagName) || GetFlagType(flagName) != FlagType.BOOL) return default;
        return (bool) flags[flagName];
    }


    /// <summary>
    /// Checks if a flag is setted (has any value stored).
    /// </summary>
    /// <param name="flagName">The flag's name.</param>
    /// <returns>True if setted, false if not.</returns>
    public bool IsFlagSetted(string flagName) {
        return flags.ContainsKey(flagName);
    }


    #region Utils
    /// <summary>
    /// Checks if a certain flag's name is present on the FlagsRegister.
    /// </summary>
    /// <param name="flagName">The flag's name.</param>
    /// <returns>True if flagName found in the FlagsRegister.</returns>
    public bool IsValidFlag(string flagName) {
        foreach (FlagDescriptor descriptor in register.availableFlags) {
            if (descriptor.name == flagName) return true;
        }

        return false;
    }

    /// <summary>
    /// Gets the FlagType enum related with the flagName. Can be used to check if the flagName is valid aswell.
    /// </summary>
    /// <param name="flagName">The flag's name.</param>
    /// <returns>Returns the associated FlagType on the FlagsRegister. If not found, returns null.</returns>
    public FlagType? GetFlagType(string flagName) {
        foreach (FlagDescriptor descriptor in register.availableFlags) {
            if (descriptor.name == flagName) return descriptor.type;
        }

        return null;
    }
    #endregion

}
