using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class Photographer : Person
{
    

    Camera cam;
    GameObject hud;

    private bool hudActive = false; //NEVER EDIT THIS
    public bool CameraLensActive //Edit this
    {
        get
        {
            return hudActive;
        }
        set
        {
            //Activate Camera HUD Based on this Value
            if(value)
            {
                hud.SetActive(true);
            }
            else
            {
                hud.SetActive(false);
            }
            hudActive = value;
        }
    }

    bool screenshotQueued = false;

    protected override void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        cam = Camera.main;
        hud = GameObject.Find("HUD").transform.Find("Camera").gameObject;

        OnPossession += ToggleHUD;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    //Camera
    bool canTakePhoto = true;
    public override void Ability()
    {
        if (!GameController._instance.paused && canTakePhoto)
        {
            Player.EnableControls(false);
            TakePhoto(Screen.width, Screen.height);
            StartCoroutine(CameraFlash());
            Player.EnableControls(true);
        }
    }

    public void TakePhoto(int width, int height)
    {
        cam.targetTexture = RenderTexture.GetTemporary(width, height, 16);
        screenshotQueued = true;
    }

    float flashTime = 1f;
    private IEnumerator CameraFlash()
    {
        canTakePhoto = false;

        float currentTime = 0;
        Image hudBackground = hud.GetComponent<Image>();
        Color baseColor = hudBackground.color;
        Color flashColor = Color.white;

        hudBackground.color = flashColor;
        yield return new WaitForSeconds(0.25f);

        while(hudBackground.color != baseColor)
        {
            hudBackground.color = Color.Lerp(flashColor, baseColor, currentTime / flashTime);
            currentTime += Time.deltaTime;
            yield return null;
        }

        canTakePhoto = true;
    }

    private void ToggleHUD(bool possessionActive)
    {
        CameraLensActive = possessionActive;
    }

    private void OnRenderObject()
    {
        if (screenshotQueued)
        {
            //Take Picture
            screenshotQueued = false;
            RenderTexture renderTexture = cam.targetTexture;
            Texture2D renderResult = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
            Rect rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
            renderResult.ReadPixels(rect, 0, 0);

            //Store Picture In Library
            PhotoLibrary.StorePhoto(renderResult);

            //Release camera to resume as normal
            RenderTexture.ReleaseTemporary(renderTexture);
            cam.targetTexture = null;
        }
    }

    private void OnDisable()
    {
        OnPossession -= ToggleHUD;
    }

}
