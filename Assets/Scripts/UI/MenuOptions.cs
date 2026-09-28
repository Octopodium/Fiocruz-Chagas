using UnityEngine;

public class MenuOptions : MonoBehaviour
{
    public AmbientInfo firstAmbient;

    public void StartGame()
    {
        AmbientNavigation.StartAtAmbient(firstAmbient);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
