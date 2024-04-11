using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool paused;

    public GameObject pauseUI;
    
    // Start is called before the first frame update
    void Start()
    {
        PauseMenu.paused = false;
        pauseUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PauseMenu.paused)
            {
                Resume();
            } else {
                Pause();
            }
        }
    }
    
    public void Restart()
    {
        Debug.Log("Restarting maze...");
        SceneManager.LoadScene("Maze");
    }


    public void Menu()
    {
        Debug.Log("Returning to menu...");
        SceneManager.LoadScene("MainMenu");
    }

    public void Resume()
    {
        pauseUI.SetActive(false);
        Time.timeScale = 1f;
        PauseMenu.paused = false;
    }

    public void Pause()
    {
        pauseUI.SetActive(true);
        Time.timeScale = 0f;
        PauseMenu.paused = true;
    }
}
