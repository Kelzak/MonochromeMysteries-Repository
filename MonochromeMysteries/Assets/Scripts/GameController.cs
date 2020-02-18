/* Name: GameController.cs
 * Author: Zackary Seiple
 * Description: Handles the basic functions of the game including pulling up menus and pausing
 * Last Updated: 2/18/2020 (Zackary Seiple)
 * Changes: Added Header
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController _instance;

    public bool paused = false;

    // Start is called before the first frame update
    void Start()
    {
        _instance = this;
    }

    // Update is called once per frame
    /// <summary>
    /// Checks each frame for inputs to open the photograph collection menu and to exit the game
    /// </summary>
    void Update()
    {
        //Photo Library Menu
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            TogglePause();
            PhotoLibrary._instance.menu.SetActive(paused);
            if (PhotoLibrary._instance.GetPhotoCount() > 3)
            {
                Transform grid = PhotoLibrary._instance.menu.transform.GetChild(0);
                Vector3 topPos = grid.position;
                topPos.y = 225;
                grid.position = topPos;

            }
        }

        //Quitting the game
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    /// <summary>
    /// Toggles the Game Pausing
    /// </summary>
    public static void TogglePause()
    {
        _instance.paused = !_instance.paused;

            if (_instance.paused)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Player.EnableControls(false);
                Time.timeScale = 0;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Player.EnableControls(true);
                Time.timeScale = 1;
            }

    }
}
