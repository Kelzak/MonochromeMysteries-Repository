using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class PhotoLibrary : MonoBehaviour
{
    public static PhotoLibrary _instance;

    public static string directory;
    private List<Photo> scrapbook;
    private uint pictureCount = 0;

    public GameObject menu;
    public GameObject photoSlotPrefab;
    private GameObject noPhotosText;
    private int currentPage = 0;

    private delegate void ScrapbookEvent();
    //Called anytime a photo is taken/deleted/or page is changed
    private event ScrapbookEvent OnScrapbookChange;

    private struct Photo
    {
        public Photo(Sprite image, params GameObject[] cluesFeatured)
        {
            this.image = image;
            this.cluesFeatured = cluesFeatured;
        }
        
        public Photo(Sprite image)
        {
            this.image = image;
            cluesFeatured = new GameObject[0];
        }

        public void SetClues(params GameObject[] clues)
        {
            cluesFeatured = clues;
        }

        public Sprite image;
        public GameObject[] cluesFeatured;
    }

    // Start is called before the first frame update
    void Awake()
    {
        _instance = this;
        directory = Application.dataPath + "/_GameData/pictures";

        Directory.CreateDirectory(directory);

        scrapbook = new List<Photo>();

        OnScrapbookChange += UpdateUI;
    }

    private void Start()
    {
        noPhotosText = menu.transform.Find("Text").gameObject;
    }

    public int GetPhotoCount()
    {
        return scrapbook.Count;
    }

    public static void StorePhoto(Texture2D photo)
    {
        photo = _instance.CropPhoto(photo, 160 * 2, 150 * 2);

        _instance.pictureCount++;
        
        string fileName = string.Format("/Picture-{0}.png", _instance.pictureCount);
        
        System.IO.File.WriteAllBytes(PhotoLibrary.directory + fileName, photo.EncodeToPNG());
        Sprite newSprite = Sprite.Create(photo, new Rect(0, 0, photo.width, photo.height), Vector2.zero, 100f);
        newSprite.name = string.Format("Photo-{0}", _instance.pictureCount);
        newSprite.texture.Apply();
        _instance.scrapbook.Add(new Photo(newSprite));

        _instance.OnScrapbookChange?.Invoke();
        Debug.Log("Saved Camera Screenshot");
    }

    private Texture2D CropPhoto(Texture2D original, int targetWidth, int targetHeight)
    {
        int originalWidth = original.width;
        int originalHeight = original.height;
        float originalAspect = (float) originalWidth / originalHeight;
        float targetAspect = (float)targetWidth / targetHeight;
        int xOffset = 0;
        int yOffset = 0;
        float factor = 1;
        if(originalAspect > targetAspect) //Width Is Bigger, so it must be cropped
        {
            factor = (float) targetHeight / originalHeight;
            xOffset = (int)((originalWidth - originalHeight * targetAspect) / 2);
        }
        else //Height is bigger, so it must be croppped
        {
            factor = (float) targetWidth / originalWidth;
            yOffset = (int)((originalHeight - originalWidth * targetAspect) / 2);
        }
        Color32[] data = original.GetPixels32();
        Color32[] dataResult = new Color32[targetWidth * targetHeight];

        for(int y = 0; y < targetHeight; y++)
        {
            for(int x = 0; x < targetWidth; x++)
            {
                var p = new Vector2(Mathf.Clamp(xOffset + x / factor, 0, originalWidth - 1), Mathf.Clamp(yOffset + y / factor, 0, originalHeight - 1));
                var c11 = data[Mathf.FloorToInt(p.x) + originalWidth * (Mathf.FloorToInt(p.y))];
                var c12 = data[Mathf.FloorToInt(p.x) + originalWidth * (Mathf.CeilToInt(p.y))];
                var c21 = data[Mathf.CeilToInt(p.x) + originalWidth * (Mathf.FloorToInt(p.y))];
                var c22 = data[Mathf.CeilToInt(p.x) + originalWidth * (Mathf.CeilToInt(p.y))];
                var f = new Vector2(Mathf.Repeat(p.x, 1f), Mathf.Repeat(p.y, 1f));
                dataResult[x + y * targetWidth] = Color.Lerp(Color.Lerp(c11, c12, p.y), Color.Lerp(c21, c22, p.y), p.x);
            }
        }

        var textureResult = new Texture2D(targetWidth, targetHeight);
        textureResult.SetPixels32(dataResult);
        textureResult.Apply(true);
        return textureResult;
    }

    private void UpdateUI()
    {
        //Decide Whether to display "No Photos" Text
        if(scrapbook.Count > 0 && noPhotosText.activeSelf == true)
        {
            noPhotosText.SetActive(false);
        }
        else if (scrapbook.Count <= 0 && noPhotosText.activeSelf == false)
        {
            noPhotosText.SetActive(true);
        }

        //Update Picture Menu
        for(int i = 0; i < scrapbook.Count; i++)
        {
            if (i >= menu.transform.GetChild(0).childCount)
            {
                GameObject newSlot = Instantiate(photoSlotPrefab, menu.transform.GetChild(0));
                newSlot.transform.GetChild(1).GetComponent<Image>().sprite = _instance.scrapbook[i].image;
            }
        }
    }

    private void OnDisable()
    {
        OnScrapbookChange -= UpdateUI;
    }

    private void OnApplicationQuit()
    {
        Directory.Delete(directory, true);
    }
}
