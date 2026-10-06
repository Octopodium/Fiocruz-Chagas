using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls creating and event handling of interactable markers.
/// </summary>
public class IndicatorManager : MonoBehaviour {
    public GameObject indicatorPrefab;
    Dictionary<GameObject, InteractableMarker> spawnedIndicators = new Dictionary<GameObject, InteractableMarker>();
    

    void Awake() {
        SetupRefreshEvents();
    }

    void Start() {
        CreateAmbientIndicators();
    }

    #region Creation Methods

    /// <summary>
    /// Creates a interactable indicator for a gameObject that contains at least one IUnderMouse.
    /// Every IUnderMouse of a single gameObject (as you can have more than one in the same object) will share the same indicator.
    /// This is called internally by CreateAmbientIndicators.
    /// </summary>
    /// <param name="interactableHolder">GameObject that contains the IUnderMouses.</param>
    public void CreateIndicatorFor(GameObject interactableHolder) {
        if (HasIndicator(interactableHolder)) return;

        GameObject instance = Instantiate(indicatorPrefab);
        InteractableMarker indicator = instance.GetComponent<InteractableMarker>();


        MarkerSettings settings = interactableHolder.GetComponent<MarkerSettings>();
        instance.transform.localScale = Vector3.one * (settings != null ? settings.scale : 1);

        instance.transform.SetParent(interactableHolder.transform);

        instance.transform.localPosition = settings != null ? settings.positionOffset : Vector3.zero;

        spawnedIndicators[interactableHolder] = indicator;
        indicator.SetupInteractables();
    }

    /// <summary>
    /// Destroy every Indicator created in this scene and clears the lookup table.
    /// This is called internally everytime AmbientNavigation is about to switch scenes (aka onBeforeChangingAmbient).
    /// </summary>
    public void ClearAllIndicators() {
        foreach (KeyValuePair<GameObject, InteractableMarker> pair in spawnedIndicators) {
            if (pair.Value != null && pair.Value.gameObject != null) Destroy(pair.Value.gameObject);
        }

        spawnedIndicators.Clear();
    }

    /// <summary>
    /// Removes and destroys a indicator for a certain gameObject that had a indicator. Also removes it from the lookup table.
    /// </summary>
    /// <param name="interactableHolder">GameObject that had an Indicator.</param>
    public void RemoveIndicatorOf(GameObject interactableHolder) {
        if (!HasIndicator(interactableHolder)) return;

        InteractableMarker indicator = spawnedIndicators[interactableHolder];
        if (indicator != null) Destroy(indicator.gameObject);

        spawnedIndicators.Remove(interactableHolder);
    }

    /// <summary>
    /// Checks if a gameObject has an indicator on the lookup table.
    /// </summary>
    /// <param name="interactableHolder">GameObject to check.</param>
    /// <returns>Returns true if the gameObject is present in the lookup table (meaning it has a indicator)</returns>
    public bool HasIndicator(GameObject interactableHolder) => spawnedIndicators.ContainsKey(interactableHolder);

    /// <summary>
    /// Gets the indicator (InteractableMarker) of a certain gameObject, or null if it doesn't have one.
    /// </summary>
    /// <param name="interactableHolder">GameObject to get the indicator.</param>
    /// <returns>Returns the indicator associated with the gameObject if present, else returns null.</returns>
    public InteractableMarker GetIndicator(GameObject interactableHolder) => spawnedIndicators.ContainsKey(interactableHolder) ? spawnedIndicators[interactableHolder] : null;

    #endregion


    /// <summary>
    /// Called internally on Awake, setups all events that changes the game state to refresh the state of an indicator.
    /// Normally condition checks happens every frame when mouse over, to make it less heavy, will only check when something change in the game.
    /// Also setups ambient events to clear indicators from a previous scene and fetch indicators from a new scene.
    /// </summary>
    void SetupRefreshEvents() {
        // É babado.
        Player player = GameManager.instance.player;
        player.onCollectableHeldChanged += held => RefreshIndicators();
        player.inventory.OnAddToInventory += item => RefreshIndicators();
        player.inventory.OnRemoveFromInventory += item => RefreshIndicators();
        player.quest.OnQuestChanged += (quest, step, state) => RefreshIndicators();
        GameManager.instance.flags.OnFlagChanged += (flag, value) => RefreshIndicators();
        GameManager.instance.cam.onCurrentCameraAreaChange += area => RefreshIndicators();
        GameManager.instance.inspectator.OnInspectingChanged += inspectable => RefreshIndicators();


        GameManager.instance.navigation.onBeforeChangingAmbient += ambient => ClearAllIndicators();
        GameManager.instance.navigation.onAfterChangingAmbient += ambient => CreateAmbientIndicators();
    }


    List<GameObject> removedIndicators = new List<GameObject>();

    /// <summary>
    /// Checks every indicator to refresh its visibility. Also clears indicators from the lookup table that where removed.
    /// Called internally by a lot of events (see SetupRefreshEvents)
    /// </summary>
    void RefreshIndicators() {
        Player player = GameManager.instance.player;
        bool isInteractables = player.collectableHeld == null;

        removedIndicators.Clear();

        foreach (KeyValuePair<GameObject, InteractableMarker> pair in spawnedIndicators) {
            InteractableMarker indicator = pair.Value;

            if (indicator == null || indicator.gameObject == null) {
                removedIndicators.Add(pair.Key);
                continue;
            }

            indicator.RefreshVisibility(isInteractables);
        }

        if (removedIndicators.Count > 0) {
            foreach (GameObject obj in removedIndicators) {
                spawnedIndicators.Remove(obj);
            }
        }
    }

    /// <summary>
    /// Fetch every IUnderMouse of the scene and tries to create a indicator for each gameObject that contains it (only once per gameObject).
    /// Called internally on AmbientNavigation's onAfterChangingAmbient, everytime the current ambient changes.
    /// </summary>
    void CreateAmbientIndicators() {
        IUnderMouse[] interactables = FindObjectsByType<IUnderMouse>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        HashSet<GameObject> objects = new HashSet<GameObject>();
        
        foreach (IUnderMouse interactable in interactables) {
            GameObject interactableHolder = interactable.gameObject;
            if (objects.Contains(interactableHolder)) continue;

            objects.Add(interactableHolder);
            CreateIndicatorFor(interactableHolder);
        }

        RefreshIndicators();
    }

}
