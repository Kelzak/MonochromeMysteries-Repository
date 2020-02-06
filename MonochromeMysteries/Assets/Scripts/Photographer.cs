using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Photographer : Person
{
    
    uint pictureCount = 0;


    Camera cam;
    bool screenshotQueued = false;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;

    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }

    //Camera
    public override void Ability()
    {
        GetComponent<Player>().EnableControls(false);
        TakePhoto(Screen.width, Screen.height);
        GetComponent<Player>().EnableControls(true);
    }

    public void TakePhoto(int width, int height)
    {
        cam.targetTexture = RenderTexture.GetTemporary(width, height, 16);
        screenshotQueued = true;
    }

    private void OnRenderObject()
    {
        if (screenshotQueued)
        {
            screenshotQueued = false;
            RenderTexture renderTexture = cam.targetTexture;
            Texture2D renderResult = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
            Rect rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
            renderResult.ReadPixels(rect, 0, 0);

            byte[] byteArray = renderResult.EncodeToPNG();
            pictureCount++;
            string fileName = string.Format("/Picture-{0}.png", pictureCount);
            System.IO.File.WriteAllBytes(PhotoLibrary.directory + fileName, byteArray);
            Debug.Log("Saved Camera Screenshot");

            RenderTexture.ReleaseTemporary(renderTexture);
            cam.targetTexture = null;
        }
    }
}
