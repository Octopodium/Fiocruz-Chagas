using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Essa classe não está acabada, favor não utilizar por hora!
/// </summary>
public class CameraController : MonoBehaviour {
    public Camera mainCamera;


    #region Check Under Mouse
    /// <summary>
    /// Cache for optimatization. Used by 'CheckUnderMouse()' to store the last gameObject under the mouse.
    /// </summary>
    public GameObject lastUnderMouseGameObject {get; private set;} = null;
    public IUnderMouse lastUnderMouseInterface {get; private set;} = null;

    
    /// <summary>
    /// This function can be called to check for GameObject and IUnderMouse under the current mouse position.
    /// Called every frame by Player
    /// </summary>
    /// <param name="underMouse">Outs the IUnderMouse if it was found, if not, will be null.</param>
    /// <returns></returns>
    public GameObject CheckUnderMouse(out IUnderMouse underMouse) {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;

        underMouse = null;

        if (Physics.Raycast(ray, out hit)) {
            GameObject under = hit.transform.gameObject;

            if (under == lastUnderMouseGameObject) {
                underMouse = lastUnderMouseInterface;
                if (underMouse != null) underMouse = underMouse.CanBeFound() ? underMouse : null;

                return lastUnderMouseGameObject;
            }

            underMouse = under.GetComponent<IUnderMouse>();

            lastUnderMouseGameObject = under;
            lastUnderMouseInterface = underMouse;

            if (underMouse != null) underMouse = underMouse.CanBeFound() ? underMouse : null;

        } else {
            lastUnderMouseGameObject = null;
            lastUnderMouseInterface = null;
        }

        return lastUnderMouseGameObject;
    }
    
    #endregion
}
