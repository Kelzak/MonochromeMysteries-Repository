/* Name: PhotoLibrary.cs
 * Author: Zackary Seiple
 * Description: Contains a library of all the photos taken by the Photographer. Also controls the menu containing all the photos
 * Last Updated: 2/25/2020 (Zackary Seiple)
 * Changes: Added Photo Examining
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PhotoLibrary : MonoBehaviour
{
    public static PhotoLibrary _instance;

    private List<Photo> scrapbook;
    private uint pictureCount = 0;

    private GameObject selectedSlot;
    [HideInInspector]
    public GameObject SelectedSlot
    {
        get { return selectedSlot; }
        set
        {
            //Handle highlights
            if (selectedSlot != null)
                Destroy(selectedSlot.GetComponent<Outline>());
            selectedSlot = value;
            selectedSlot.AddComponent<Outline>().OutlineColor = Color.yellow;
        }
    }

    public GameObject photoCollectionMenu;
    public GameObject photoSlotPrefab;
    private GameObject examinePhotoMenu;
    private GameObject photoGrid;
    private GameObject noPhotosText;
    private int currentPage = 0;

    private delegate void ScrapbookEvent();
    //Called anytime a photo is taken/deleted/or page is changed
    private event ScrapbookEvent OnScrapbookChange;

    /// <summary>
    /// The photo structure that contains the image associated with a photo as well as the clues featured in the photo
    /// </summary>
    public class Photo
    {

        [Header("Photo Elements")]
        public GameObject photoSlot;
        public Sprite image;
        public int[] cluesFeatured;
        public string labelText, detailsText;

        [Header("Submenu")]
        public RectTransform subMenu;
        private bool subMenu_Active = false;
        private float subMenu_TransitionTime = 0.25f;

        public Photo(GameObject photoSlot, Sprite image, params int[] cluesFeatured)
        {
            this.image = image;
            this.cluesFeatured = cluesFeatured;
            this.photoSlot = photoSlot;

            this.subMenu = photoSlot.transform.Find("Submenu").GetComponent<RectTransform>();

            //Prepare Examine and Delete Buttons
            photoSlot.transform.Find("Submenu").Find("ExamineButton").GetComponent<Button>().onClick.AddListener(
                () => { _instance.ExaminePhoto(_instance.FindPhotoFromObject(photoSlot)); });
            photoSlot.transform.Find("Submenu").Find("DeleteButton").GetComponent<Button>().onClick.AddListener(
                () => { _instance.RemovePhoto(_instance.FindPhotoFromObject(photoSlot)); });

            //OnPointerEnter
            EventTrigger.Entry onPointerEnterEntry = new EventTrigger.Entry();
            onPointerEnterEntry.eventID = EventTriggerType.PointerEnter;
            onPointerEnterEntry.callback.AddListener((eventData) => { _instance.selectedSlot = eventData.selectedObject; _instance.StartCoroutine(ToggleSubMenu(true)); });
            photoSlot.GetComponent<EventTrigger>().triggers.Add(onPointerEnterEntry);
            //OnPointerExit
            EventTrigger.Entry onPointerExitEntry = new EventTrigger.Entry();
            onPointerExitEntry.eventID = EventTriggerType.PointerExit;
            onPointerExitEntry.callback.AddListener((eventData) => { _instance.selectedSlot = null; _instance.StartCoroutine(ToggleSubMenu(false)); });
            photoSlot.GetComponent<EventTrigger>().triggers.Add(onPointerExitEntry);


            labelText = detailsText = "";
            foreach(int x in cluesFeatured)
            {
                if (labelText != "")
                    labelText += ", ";
                labelText += ClueCatalogue._instance.clues[x].name;
                detailsText += ClueCatalogue._instance.clues[x].name + ":\n";
                detailsText += ClueCatalogue._instance.clues[x].description + "\n\n";
            }
        }

        public void Deconstruct()
        {
            Destroy(photoSlot);
            Destroy(image);
        }

        public void SetClues(params int[] clues)
        {
            cluesFeatured = clues;
        }

        public void SetObject(GameObject photoSlot)
        {
            this.photoSlot = photoSlot;
        }

        public IEnumerator ToggleSubMenu(bool active)
        {
            if (subMenu_Active == active)
                yield break;

            Vector3 startPos = Vector3.zero;
            Vector3 endPos = startPos;
            float shiftValue = subMenu.sizeDelta.x + photoSlot.GetComponentInParent<GridLayoutGroup>().cellSize.x / 2;
            Debug.Log(subMenu_Active);
            if (!subMenu_Active)
                endPos.x += shiftValue * GameController.mainHUD.GetComponent<CanvasScaler>().scaleFactor;
            else
            {
                startPos.x += shiftValue * GameController.mainHUD.GetComponent<CanvasScaler>().scaleFactor;
            }

            subMenu_Active = !subMenu_Active;
            bool startBool = subMenu_Active;
            

            float currentTime = 0;
            while(startPos != endPos && subMenu_Active == startBool)
            {
                subMenu.anchoredPosition = Vector3.Lerp(startPos, endPos, currentTime / subMenu_TransitionTime);
                currentTime += Time.unscaledDeltaTime;
                yield return null;
            }
            subMenu.anchoredPosition = endPos;
        }

        

    }

    // Start is called before the first frame update
    void Awake()
    {
        _instance = this;

        scrapbook = new List<Photo>();

        OnScrapbookChange += UpdateUI;
    }

    private void Start()
    {
        noPhotosText = photoCollectionMenu.transform.Find("NoPhotosText").gameObject;
        examinePhotoMenu = photoCollectionMenu.transform.Find("ExamineMenu").gameObject;
        photoGrid = photoCollectionMenu.transform.Find("Grid").gameObject;
    }

    bool examining = false;
    /// <summary>
    /// Toggle the examining of a specific photo
    /// </summary>
    /// <param name="photo">The Photo in scrapbook to be examined</param>
    private void ExaminePhoto(Photo photo)
    {
        Debug.Log(examining);
        //If not examining, start examining
        if (examining == false)
        {
            examining = true;

            //Set Description and Label
            Transform examinePhotoSlot = examinePhotoMenu.transform.Find("PhotoSlot");
            examinePhotoSlot.Find("Image").GetComponent<Image>().sprite = photo.image;
            examinePhotoSlot.Find("Label").GetComponent<Text>().text = photo.labelText;
            examinePhotoMenu.transform.Find("Description").GetComponent<Text>().text = photo.detailsText;

            //If no details, then there are no clues featured, no need to have description so just center the picture
            if(photo.detailsText == "")
            {
                examinePhotoMenu.transform.Find("Description").GetComponent<LayoutElement>().ignoreLayout = true;
            }

            examinePhotoMenu.SetActive(true);
            photoGrid.SetActive(false);

            //Let the photo be clicked to exit examining mode
            examinePhotoMenu.transform.Find("PhotoSlot").GetComponent<Button>().onClick.AddListener(() => { ExaminePhoto(photo); });

        }
        //If examining, stop examining
        else if(examining == true)
        {
            examining = false;

            //No need to have the menu be clickable
            examinePhotoMenu.transform.Find("PhotoSlot").GetComponent<Button>().onClick.RemoveAllListeners();

            //if the description was removed, put it back
            if (photo.detailsText == "")
            {
                examinePhotoMenu.transform.Find("Description").GetComponent<LayoutElement>().ignoreLayout = false;
            }
            examinePhotoMenu.SetActive(false);
            photoGrid.SetActive(true);
        }

    }

    /// <summary>
    /// Getter for the number of photos currently possessed by the player
    /// </summary>
    /// <returns>An integer representing the number of photos currently in the scrapbook</returns>
    public int GetPhotoCount()
    {
        return scrapbook.Count;
    }

    /// <summary>
    /// Crops and turns the photo from the Photographer into a sprite to be put into an image and placed in scrapbook
    /// </summary>
    /// <param name="photo">The Texture2D photo taken by the Photographer</param>
    /// <param name="clues">An array of ints representing the indexes of clues in ClueCatalogue</param>
    public static void StorePhoto(Texture2D photo, params int[] clues)
    {
        photo = _instance.CropPhoto(photo, 160 * 2, 150 * 2);

        _instance.pictureCount++;

        //Create Image
        Sprite newSprite = Sprite.Create(photo, new Rect(0, 0, photo.width, photo.height), Vector2.zero, 100f);
        newSprite.name = string.Format("Photo-{0}", _instance.pictureCount);
        newSprite.texture.Apply();

        //Create PhotoSlot
        GameObject newSlot = Instantiate(_instance.photoSlotPrefab, _instance.photoCollectionMenu.transform.GetChild(0));

        _instance.scrapbook.Add(new Photo(newSlot, newSprite, clues));

        _instance.OnScrapbookChange?.Invoke();
    }

    public static void StorePhoto(Texture2D photo)
    {
        StorePhoto(photo, new int[0]);
    }

    /// <summary>
    /// Remove a photo from the scrapbook
    /// </summary>
    /// <param name="photo">Photo struct to be removed</param>
    public void RemovePhoto(Photo photo)
    {
       int indexToRemove = _instance.scrapbook.FindIndex((Photo photoI) => { return photoI.image == photo.image && 
                                                                 photoI.labelText == photo.labelText &&
                                                                 photoI.detailsText == photo.detailsText; });
        if (indexToRemove != -1)
        {
            //Destroy Photo Slot GameObject
            _instance.scrapbook[indexToRemove].Deconstruct();
            //Remove from Scrapbook
            _instance.scrapbook.RemoveAt(indexToRemove);
        }

        _instance.OnScrapbookChange?.Invoke();
    }

    /// <summary>
    /// Remove a photo from the scrapbook
    /// </summary>
    /// <param name="photoSlot">The photo slot GameObject to be removed</param>
    public void RemovePhoto(GameObject photoSlot)
    {
        RemovePhoto(_instance.FindPhotoFromObject(photoSlot));
    }

    /// <summary>
    /// Crops the photo from the center while preserving image quality
    /// </summary>
    /// <param name="original">The original photo to be cropped</param>
    /// <param name="targetWidth">The target width in pixels to crop the photo to</param>
    /// <param name="targetHeight">The target height in pixels to crop the photo to</param>
    /// <returns>A Texture2D consisting of the cropped photo</returns>
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

    /// <summary>
    /// Updates the UI Menu to visually reflect the number of photos contained in the scrapbook
    /// </summary>
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
            GameObject photoSlot = scrapbook[i].photoSlot;
            photoSlot.transform.Find("Image").GetComponent<Image>().sprite = scrapbook[i].image;
            photoSlot.transform.Find("Label").GetComponent<Text>().text = scrapbook[i].labelText;
            
        }
    }

    private Photo FindPhotoFromObject(GameObject obj)
    {
        for (int i = 0; i < scrapbook.Count; i++)
        {
            if (scrapbook[i].photoSlot == obj)
                return scrapbook[i];
        }

        return null;
    }

    private void OnDisable()
    {
        OnScrapbookChange -= UpdateUI;
    }
}
