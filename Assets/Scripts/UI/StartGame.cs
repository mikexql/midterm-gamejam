using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour, IInteractiveButton
{
    public void ButtonClick()
    {
        SceneManager.LoadScene("MainScene"); // name of your menu scene
    }
}