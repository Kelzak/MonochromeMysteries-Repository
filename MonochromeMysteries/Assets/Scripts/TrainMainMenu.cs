using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TrainMainMenu : MonoBehaviour
{
    public GameObject HUD;
    public Image fadeToBlackScreen;
    public GameObject darkBackground;
    public GameObject pressTabtoClose;
    public GameObject controlsMenu;
    public GameObject creditsMenu;
    public GameObject optionsMenu;

    // Start is called before the first frame update
    void Start()
    {
        HUD = GameObject.Find("HUD");
        darkBackground = HUD.transform.Find("DarkBackground").gameObject;
        controlsMenu = HUD.transform.Find("ControlsMenu").gameObject;
        creditsMenu = HUD.transform.Find("CreditsMenu").gameObject;
        optionsMenu = HUD.transform.Find("OptionsMenu").gameObject;
        fadeToBlackScreen = HUD.transform.Find("FadeToBlackScreen").gameObject.GetComponent<Image>();
        pressTabtoClose = HUD.transform.Find("PressTabToClose").gameObject;
        
        StartCoroutine(TitleScreenIntro());
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            ExitControls();
        }
    }

    public IEnumerator TitleScreenIntro()
    {
        fadeToBlackScreen.canvasRenderer.SetAlpha(1f);
        fadeToBlackScreen.gameObject.SetActive(true);
        fadeToBlackScreen.CrossFadeAlpha(0, 2, false);
        yield return new WaitForSeconds(3f);
        fadeToBlackScreen.gameObject.SetActive(false);

    }

    public void Play()
    {
        StartCoroutine(StartPlay());
    }

    public IEnumerator StartPlay()
    {
        fadeToBlackScreen.canvasRenderer.SetAlpha(0.0f);
        Debug.Log("play");
        fadeToBlackScreen.gameObject.SetActive(true);
        fadeToBlackScreen.CrossFadeAlpha(1, 2, false);
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("matt train");
    }

    private void FadeToBlackScreen()
    {

    }

    public void Controls()
    {
        controlsMenu.SetActive(true);
        darkBackground.SetActive(true);
        pressTabtoClose.SetActive(true);
        Debug.Log("Controls");
    }

    public void Credits()
    {
        Debug.Log("credits");
        creditsMenu.SetActive(true);
        darkBackground.SetActive(true);
        pressTabtoClose.SetActive(true);
    }

    public void ExitControls()
    {
        controlsMenu.SetActive(false);
        optionsMenu.SetActive(false);
        darkBackground.SetActive(false);
        creditsMenu.SetActive(false);
        pressTabtoClose.SetActive(false);
    }

    public void OptionsMenu()
    {
        Debug.Log("options");
        optionsMenu.SetActive(true);
        darkBackground.SetActive(true);
        pressTabtoClose.SetActive(true);
    }

    public void Quit()
    {
        Debug.Log("quit");
        Application.Quit();
    }
}
