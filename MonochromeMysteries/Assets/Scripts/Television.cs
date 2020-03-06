using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Television : MonoBehaviour
{
    public GameObject screen;
    public GameObject mainMenu, tvStatic, options;
    public Animation staticAnim;

    public enum ButtonName { NewGame, Continue, Resume, Options, Quit };
    private Button[] buttons;

    private MeshRenderer mesh;
    private Vector3 boxCenter;
    private Collider[] hit;

    // Start is called before the first frame update
    void Start()
    {
        screen = transform.Find("Screen").gameObject;
        mainMenu = screen.transform.Find("MainMenu").gameObject;
        options = screen.transform.Find("Options").gameObject;
        tvStatic = screen.transform.Find("Static").gameObject;

        Transform menuOptions = transform.Find("Screen").Find("MainMenu").Find("MenuOptions");
        buttons = new Button[] { menuOptions.Find("New Game").GetComponent<Button>(),
                                     menuOptions.Find("Continue").GetComponent<Button>(),
                                     menuOptions.Find("Resume").GetComponent<Button>(),
                                     menuOptions.Find("Options").GetComponent<Button>(),
                                     menuOptions.Find("Quit").GetComponent<Button>() };

        //Add Listeners
        GameController x = FindObjectOfType<GameController>();
        buttons[0].onClick.AddListener(() => x.TriggerMainMenu());
        buttons[0].onClick.AddListener(() => MainMenu._instance.ChangeFromInitialOptions());
        buttons[1].onClick.AddListener(() => x.TriggerMainMenu());
        buttons[2].onClick.AddListener(() => x.TriggerMainMenu());
        buttons[3].onClick.AddListener(() => MainMenu._instance.TriggerSwitchMenu("Options"));
        buttons[4].onClick.AddListener(() => x.QuitGame());

        //Set the right buttons at start
        SwapButtons(true, ButtonName.NewGame, ButtonName.Options, ButtonName.Quit, ButtonName.Continue);
        SwapButtons(false, ButtonName.Resume);

        mainMenu.SetActive(false);
        options.SetActive(false);
        


        mesh = GetComponent<MeshRenderer>();
        boxCenter = transform.forward + mesh.bounds.max;

    }

    public void SwapButtons(bool active, params ButtonName[] indexesToToggle)
    {
        foreach(int x in indexesToToggle)
        {
            buttons[x].gameObject.SetActive(active);
        }
    }

    public bool CheckForPlayerInRange()
    {
        //Check area in front of TV for player
        hit = Physics.OverlapBox(boxCenter, mesh.bounds.size, Quaternion.identity);
        bool found = false;
        foreach (Collider x in hit)
        {
            if (x.gameObject.GetComponent<Player>())
            {
                found = true;
            }
        }

        return found;
    }

   
}
