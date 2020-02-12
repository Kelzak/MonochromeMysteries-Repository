using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    static GameController _instance;

    bool paused = false;

    // Start is called before the first frame update
    void Start()
    {
        _instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        //Photo Library Menu
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            TogglePause();
            PhotoLibrary._instance.menu.SetActive(paused);
        }
    }


    public static void TogglePause()
    {
        _instance.paused = !_instance.paused;

            if (_instance.paused)
            {
                Cursor.lockState = CursorLockMode.Confined;
                Player.EnableControls(false);
                Time.timeScale = 0;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Player.EnableControls(true);
                Time.timeScale = 1;
            }

    }
}
