using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMain : MonoBehaviour, IInteractiveButton
{
    public void ButtonClick()
    {
        SceneManager.LoadScene("MainMenu"); // name of your menu scene
    }
}