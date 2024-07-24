using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public void Simulate()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Simulation");
    }

    public void About()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("About");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
