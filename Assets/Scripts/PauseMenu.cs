using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public bool isPaused = false;
    public bool isDisconnecting = false;
    public AudioSource winSound;
    public AudioSource looseSound;
    bool play = false;
    private void Start()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
    }
    public void Paused()
    {
        if (!isDisconnecting)
        {
            if (!isPaused)
            {
                transform.GetChild(0).gameObject.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                isPaused = true;
            }
            else
            {
                Resume();
            }
        }
    }    
    
    public void Resume()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isPaused = false;
    }

    public void Lose()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isPaused = true;
        if (!play)
        {
            looseSound.Play();
            play = true;
        }
    }

    public void Win()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isPaused = true;
        if (!play)
        {
            winSound.Play();
            play = true;
        }
    }

    public void Quit()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
        isDisconnecting = true;
        GameManager.Instance.LeaveRoom();
    }
}


