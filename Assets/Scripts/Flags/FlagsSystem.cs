using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// The flag control system. Stores every setted flag value. Used for setting and getting flags.
/// </summary>
public class FlagsSystem : MonoBehaviour, ISaveable {
    public static FlagsSystem instance => GameManager.instance.flags;
    FlagsRegister register;
    Dictionary<string, object> flags = new Dictionary<string, object>();

    public Action<string, object> OnFlagChanged;


    void Awake() {
        register = FlagsRegister.instance;
        SetAutoFlags();

        GameManager.instance.saveManager.AddSaveable(this);
    }

    void OnDestroy() {
        UnsetAutoFlags();

        GameManager.instance?.saveManager.RemoveSaveable(this);
    }

    void FixedUpdate() {
        flagsAutoSettedThisFrame.Clear();
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
    /// Sets a bool flag as true.
    /// </summary>
    /// <param name="flagName">The flag's name. It must be stored in the FlagsRegister.</param>
    /// <returns>Returns true if the flag was setted. If the 'flagName' is invalid, will return false.</returns>
    public bool CheckFlag(string flagName) {
        if (GetFlagType(flagName) != FlagType.BOOL) return false;
        flags[flagName] = true;
        OnFlagChanged?.Invoke(flagName, true);
        return true;
    }

    /// <summary>
    /// Sets a bool flag as false.
    /// </summary>
    /// <param name="flagName">The flag's name. It must be stored in the FlagsRegister.</param>
    /// <returns>Returns true if the flag was setted. If the 'flagName' is invalid, will return false.</returns>
    public bool UncheckFlag(string flagName) {
        if (GetFlagType(flagName) != FlagType.BOOL) return false;
        flags[flagName] = false;
        OnFlagChanged?.Invoke(flagName, false);
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
    /// Gets a setted flag's value (regardless of the type).
    /// </summary>
    /// <param name="flagName">The flag's name</param>
    /// <param name="getDefaultValueInstead">Optional parameter. If true, when flag not set, will return default value of same type, else returns null.</param>
    /// <returns>Returns the flag's value as an object type. </returns>
    public object GetFlag(string flagName, bool getDefaultValueInstead) {
        object result = GetFlag(flagName);
        if (result != null) return result;
        if (!getDefaultValueInstead) return null;
        FlagType? flagType = GetFlagType(flagName);
        return flagType == null ? null : GetDefaultTypeOf(FlagDescriptor.GetTypeByFlagTypes((FlagType) flagType));
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

    object GetDefaultTypeOf(Type t) {
        if (t.IsValueType)
            return Activator.CreateInstance(t);
        return null;
    }

    #endregion

    /// <summary>
    /// Returns the save data. Used for saving the current flag state.
    /// </summary>
    /// <returns></returns>
    public PlayerData Save(PlayerData data) {
        Dictionary<string, bool> boolDictionary = new Dictionary<string, bool>();
        Dictionary<string, int> intDictionary = new Dictionary<string, int>();
        Dictionary<string, float> floatDictionary = new Dictionary<string, float>();
        Dictionary<string, string> stringDictionary = new Dictionary<string, string>();

        foreach (string flagName in flags.Keys) {
            FlagType? type = GetFlagType(flagName);
            switch (type) {
                case FlagType.BOOL:
                    boolDictionary[flagName] = (bool) flags[flagName];
                    break;
                case FlagType.INT:
                    intDictionary[flagName] = (int) flags[flagName];
                    break;
                case FlagType.FLOAT:
                    floatDictionary[flagName] = (float) flags[flagName];
                    break;
                case FlagType.STRING:
                    stringDictionary[flagName] = (string) flags[flagName];
                    break;
            }
        }

        FlagsSaveData flagsData = new FlagsSaveData();
        flagsData.SetBool(boolDictionary.Keys.ToArray(), boolDictionary.Values.ToArray());
        flagsData.SetInt(intDictionary.Keys.ToArray(), intDictionary.Values.ToArray());
        flagsData.SetFloat(floatDictionary.Keys.ToArray(), floatDictionary.Values.ToArray());
        flagsData.SetString(stringDictionary.Keys.ToArray(), stringDictionary.Values.ToArray());

        data.flags = flagsData;
        return data;

    }

    /// <summary>
    /// Clears the current setted flags and sets flags from save. Used to Load saved flags state from 'Save' method.
    /// </summary>
    public void Load(PlayerData data) {
        flags.Clear();

        FlagsSaveData flagsData = data.flags;

        foreach (KeyValuePair<string, bool> values in flagsData.GetBool()) {
            SetFlag(values.Key, values.Value);
        }

        foreach (KeyValuePair<string, int> values in flagsData.GetInt()) {
            SetFlag(values.Key, values.Value);
        }

        foreach (KeyValuePair<string, float> values in flagsData.GetFloat()) {
            SetFlag(values.Key, values.Value);
        }

        foreach (KeyValuePair<string, string> values in flagsData.GetString()) {
            SetFlag(values.Key, values.Value);
        }
    }


    #region Auto Flag Setters

    IDisposable autoFlagDialogueDisposable;
    HashSet<string> flagsAutoSettedThisFrame = new HashSet<string>();

    /// <summary>
    /// Setup of events that sets auto-flags on the system. Called on awake (as it only sets event calls and doesn't actually changes the state of anything).
    /// </summary>
    void SetAutoFlags() {
        GameManager.instance.player.inventory.OnAddToInventory += HandleCollectableAdded;
        GameManager.instance.player.inventory.OnRemoveFromInventory += HandleCollectableRemoved;

        autoFlagDialogueDisposable = GameManager.instance.dialogue.VariableStorage.AddChangeListener(HandleDialogueVariableChange);
        OnFlagChanged += HandleFlagChanged;

    }

    /// <summary>
    /// Unset auto-flags events setted by SetAutoFlags. Called on destroy.
    /// </summary>
    void UnsetAutoFlags() {
        GameManager.instance.player.inventory.OnAddToInventory -= HandleCollectableAdded;
        GameManager.instance.player.inventory.OnRemoveFromInventory -= HandleCollectableRemoved;

        if (autoFlagDialogueDisposable != null) {
            autoFlagDialogueDisposable.Dispose();
            autoFlagDialogueDisposable = null;
        }

        OnFlagChanged -= HandleFlagChanged;
    }

    /// <summary>
    /// Called everytime a Collectable is added to the inventory. If Collectable has a related flag, will set it as true if valid.
    /// </summary>
    /// <param name="collectable">Collectable just added on inventory.</param>
    void HandleCollectableAdded(Collectable collectable) {
        string flag = collectable.GetRelatedFlag();
        if (string.IsNullOrEmpty(flag)) return;
        if (!IsValidFlag(flag)) {
            Debug.LogWarning("Flag \"" + flag + "\" not setted on FlagsRegister!");
            return;
        }

        SetFlag(flag, true);
    }

    /// <summary>
    /// Called everytime a Collectable is removed from the inventory.
    /// If Collectable has a related flag, and there's no more of the same Collectable on the inventory, will set the flag as false if valid.
    /// </summary>
    /// <param name="collectable"></param>
    void HandleCollectableRemoved(Collectable collectable) {
        string flag = collectable.GetRelatedFlag();
        if (string.IsNullOrEmpty(flag)) return;
        if (!IsValidFlag(flag)) {
            Debug.LogWarning("Flag \"" + flag + "\" not setted on FlagsRegister!");
            return;
        }

        if (GameManager.instance.player.inventory.InventoryContainsCollectable(collectable)) return;

        SetFlag(flag, false);
    }

    /// <summary>
    /// Called everytime a flag is changed. This method is used to update the Yarn variable storage to share the same value as the flags.
    /// </summary>
    /// <param name="name">Flag name</param>
    /// <param name="value">Flag value</param>
    void HandleFlagChanged(string name, object value) {
        string dialogueName = "$" + name;

        if (flagsAutoSettedThisFrame.Contains(name)) return;
        flagsAutoSettedThisFrame.Add(name);

        if (value.GetType() == typeof(bool)) GameManager.instance.dialogue.VariableStorage.SetValue(dialogueName, (bool) value);
        else if (value.GetType() == typeof(int) || value.GetType() == typeof(float)) GameManager.instance.dialogue.VariableStorage.SetValue(dialogueName,  Convert.ToSingle(value));
        else if (value.GetType() == typeof(string)) GameManager.instance.dialogue.VariableStorage.SetValue(dialogueName, (string) value);
    }

    /// <summary>
    /// Called everytime a Yarn variable is changed. This method is used to update a related flag (if valid) with it's value in the Yarn variable storage.
    /// </summary>
    /// <param name="name">Yarn variable name</param>
    /// <param name="value">Yarn variable value</param>
    void HandleDialogueVariableChange(string name, object value) {
        string flagName = name.StartsWith("$") ? name.Substring(1) : name;
        if (IsValidFlag(flagName)) SetFlag(flagName, value);
    }

    #endregion

}

[Serializable]
public struct FlagsSaveData {
    public string[] boolFlags;
    public bool[] boolValues;

    public string[] intFlags;
    public int[] intValues;

    public string[] floatFlags;
    public float[] floatValues;

    public string[] stringFlags;
    public string[] stringValues;

    public void SetBool(string[] flagNames, bool[] values) {
        boolFlags = flagNames;
        boolValues = values;
    }

    public void SetInt(string[] flagNames, int[] values) {
        intFlags = flagNames;
        intValues = values;
    }

    public void SetFloat(string[] flagNames, float[] values) {
        floatFlags = flagNames;
        floatValues = values;
    }

    public void SetString(string[] flagNames, string[] values) {
        stringFlags = flagNames;
        stringValues = values;
    }

    IEnumerable<KeyValuePair<T0, T1>> IterateArrays<T0,T1>(T0[] arr1, T1[] arr2) {
        for (int i = 0; i < arr1.Length; i++) {
            yield return KeyValuePair.Create(arr1[i], arr2[i]);
        }
    }

    public IEnumerable<KeyValuePair<string, bool>> GetBool() => IterateArrays(boolFlags, boolValues);
    public IEnumerable<KeyValuePair<string, int>> GetInt() => IterateArrays(intFlags, intValues);
    public IEnumerable<KeyValuePair<string, float>> GetFloat() => IterateArrays(floatFlags, floatValues);
    public IEnumerable<KeyValuePair<string, string>> GetString() => IterateArrays(stringFlags, stringValues);
}