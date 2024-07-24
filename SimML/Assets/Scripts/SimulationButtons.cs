using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationButtons : MonoBehaviour
{
    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Simulation");
    }
    public void Menu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu");
    }
}
