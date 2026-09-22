/// <summary>
/// Interface for allowing a class to be saved/loaded. Said class should call GameManager.instance.save.AddSaveable(saveable) on Awake and set to itself.
/// </summary>
public interface ISaveable {
    /// <summary>
    /// Called on game save. Works as one in a chain, receiving a save data, adding it's own information and returning it with the saved info.
    /// You should always consider that the PlayerData received by parameter is a new blank PlayerData. You should only modify the fields related with your system.
    /// </summary>
    /// <param name="data">Save data where you must save your systems informations. Always consider it as a new blank PlayerData. Must be returned by the end of the method.</param>
    /// <returns>The data passed on parameter, with this ISaveable information added to it.</returns>
    PlayerData Save(PlayerData data);

    /// <summary>
    /// Called on game load. Receives the loaded PlayerData. 
    /// </summary>
    /// <param name="data"></param>
    void Load(PlayerData data);
}
