using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Readables : MonoBehaviour
{
    public Photographer photographer;
    public Endings endingManager;

    public GameObject passwordLetter1;
    public GameObject journal;
    public GameObject redRoomCode;
    public GameObject loveNote;
    public GameObject darkBackground;
    public GameObject pressCToCloseText;
    public GameObject[] diary;
    public GameObject[] love;

    private bool onDiary;
    private bool onLove;

    private int pageIndex;
    public static bool isReadingLetter;
    public static bool readtime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FlipPage();
        //ReadLetter();
        StopReadingLetter();
    }

    RaycastHit letterHit;
    public void ReadLetter()
    {
        Ray letterRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(letterRay, out letterHit))
        {
            if (letterHit.collider.tag == "letter" && letterHit.distance < Player.reticleDist)
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    readtime = false;
                    StartCoroutine(ReadTime());
                    isReadingLetter = true;
                    GameController.TogglePause();
                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.visible = false;
                    photographer.CameraLensActive = false;

                    if (letterHit.collider.gameObject.name == "Manager's Safe Code")
                    {
                        passwordLetter1.SetActive(true);
                    }
                    else if (letterHit.collider.gameObject.name == "Mechanic's Diary")
                    {
                        onDiary = true;
                        Debug.Log("got diary");
                        pageIndex = 0;
                        diary[0].SetActive(true);
                    }
                    else if (letterHit.collider.gameObject.name == "Love Letters")
                    {
                        onLove = true;
                        pageIndex = 0;
                        love[0].SetActive(true);
                    }
                    else if (letterHit.collider.gameObject.name == "Love Note")
                    {
                        loveNote.SetActive(true);
                    }
                    else if (letterHit.collider.gameObject.name == "Manager's Journal")
                    {
                        journal.SetActive(true);
                    }
                    else if (letterHit.collider.gameObject.name == "Photographer's Safe Code")
                    {
                        redRoomCode.SetActive(true);
                    }

                    Player.EnableControls(false);
                   // pressCToCloseText.SetActive(true);
                    darkBackground.SetActive(true);
                    StartCoroutine(ReadTime());
                }
            }
        }
    }

    public void StopReadingLetter()
    {
        if (isReadingLetter && (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.F) && readtime))
        {

            GameController.TogglePause();
            if (photographer.GetComponent<Player>())
            {
                photographer.CameraLensActive = true;
                photographer.canTakePhoto = true;
            }
            isReadingLetter = false;
            endingManager.knifeInstructions.SetActive(false);
            passwordLetter1.SetActive(false);
           // pressCToCloseText.SetActive(false);
            darkBackground.SetActive(false);
            redRoomCode.SetActive(false);
            loveNote.SetActive(false);
            journal.SetActive(false);
            foreach (GameObject page in diary)
            {

                page.SetActive(false);
            }
            foreach (GameObject page in love)
            {

                page.SetActive(false);
            }
            onDiary = false;
            onLove = false;
            readtime = false;
        }
    }

    public IEnumerator ReadTime()
    {
        yield return new WaitForSecondsRealtime(.1f);
        readtime = true;
    }

    public void FlipPage()
    {
        //flip pages of letters / books
        if (onDiary)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                diary[pageIndex].SetActive(false);
                if (pageIndex >= diary.Length - 1)
                {
                    Debug.Log("Flip Page");
                    foreach (GameObject page in diary)
                    {
                        page.SetActive(false);
                    }
                    pageIndex = diary.Length - 1;
                    diary[pageIndex].SetActive(true);
                }
                else
                {
                    pageIndex++;
                    diary[pageIndex].SetActive(true);
                }
            }
            if (Input.GetMouseButtonDown(1))
            {
                diary[pageIndex].SetActive(false);
                if (pageIndex <= 0)
                {
                    Debug.Log("Flip Page");
                    foreach (GameObject page in diary)
                    {
                        page.SetActive(false);
                    }
                    pageIndex = 0;
                    diary[pageIndex].SetActive(true);
                }
                else
                {
                    pageIndex--;
                    diary[pageIndex].SetActive(true);
                }
            }
        }
        if (onLove)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                love[pageIndex].SetActive(false);
                if (pageIndex >= love.Length - 1)
                {
                    Debug.Log("Flip Page");
                    foreach (GameObject page in love)
                    {
                        page.SetActive(false);
                    }
                    pageIndex = love.Length - 1;
                    love[pageIndex].SetActive(true);
                }
                else
                {
                    pageIndex++;
                    love[pageIndex].SetActive(true);
                }
            }
            if (Input.GetMouseButtonDown(1))
            {
                love[pageIndex].SetActive(false);

                if (pageIndex <= 0)
                {
                    Debug.Log("Flip Page");
                    foreach (GameObject page in love)
                    {
                        page.SetActive(false);
                    }
                    pageIndex = 0;
                    love[pageIndex].SetActive(true);
                }
                else
                {
                    pageIndex--;
                    love[pageIndex].SetActive(true);
                }
            }
        }
    }

}
