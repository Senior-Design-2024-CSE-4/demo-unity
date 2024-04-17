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
        Resume();
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
        Debug.Log("Resuming");
        pauseUI.SetActive(false);
        Time.timeScale = 1f;
        PauseMenu.paused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Pause()
    {
        Debug.Log("Pausing");
        pauseUI.SetActive(true);
        Time.timeScale = 0f;
        PauseMenu.paused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
