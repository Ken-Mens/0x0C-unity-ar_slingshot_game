using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasM: MonoBehaviour
{
    // Start is called before the first frame update
    public void Restart()
    {
        SceneManager.LoadScene("ARSlingshotGame");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
