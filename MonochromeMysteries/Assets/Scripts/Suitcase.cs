using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Suitcase : ItemAbs
{
    public int slot1;
    public TMP_Text slot1Text;
    public int slot2;
    public TMP_Text slot2Text;
    public int slot3;
    public TMP_Text slot3Text;
    private static int correctNum1 = 6;
    private static int correctNum2 = 2;
    private static int correctNum3 = 4;
    //private static string inputtedCode = "";

    public static bool puzzleActivated;

    public Button number1Up;
    public Button number1Down;
    public Button number2Up;
    public Button number2Down;
    public Button number3Up;
    public Button number3Down;

    public GameObject suitcaseUI;

    public bool puzzleComplete;

    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        puzzleActivated = false;
        puzzleComplete = false;
        animator.SetBool("Open", false);

    }

    // Update is called once per frame
    void Update()
    {
        slot1Text.text = slot1.ToString();
        slot2Text.text = slot2.ToString();
        slot3Text.text = slot3.ToString();

        if (puzzleActivated)
        {
            if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab))
            {
                HidePadlock();
            }
            
            if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                CheckIfCodeisCorrect();
            }
        }
    }

    public void NumberUp(string buttonName)
    {
        if (buttonName.Contains("1"))
        {
            if (slot1 != 9)
                slot1 += 1;
            else
                slot1 = 0;
        }
        if (buttonName.Contains("2"))
        {
            if (slot2 != 9)
                slot2 += 1;
            else
                slot2 = 0;
        }
        if (buttonName.Contains("3"))
        {
            if (slot3 != 9)
                slot3 += 1;
            else
                slot3 = 0;
        }
    }

    public void NumberDown(string buttonName)
    {
        if (buttonName.Contains("1"))
        {
            if (slot1 != 0)
                slot1 -= 1;
            else
                slot1 = 9;
        }
        if (buttonName.Contains("2"))
        {
            if (slot2 != 0)
                slot2 -= 1;
            else
                slot2 = 9;
        }
        if (buttonName.Contains("3"))
        {
            if (slot3 != 0)
                slot3 -= 1;
            else
                slot3 = 9;
        }
    }

    public void CheckIfCodeisCorrect()
    {
        if(correctNum1 == slot1 && correctNum2 == slot2 && correctNum3 == slot3)
        {
            Debug.Log("suitcase opened");
            puzzleComplete = true;
            HidePadlock();
            animator.SetBool("Open", true);
            Destroy(gameObject.GetComponent<BoxCollider>());
            Destroy(gameObject.GetComponent<Outline>());
            Destroy(gameObject.GetComponent<Item>());
            //gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
        }
        else
        {
            Debug.Log("Wrong code try again");
            slot1 = 0;
            slot2 = 0;
            slot3 = 0;
            //ADD WRONG CODE SOUND EFFECT HERE
        }
    }

    public override void Activate()
    {
        if(!puzzleComplete)
        {
            ShowPadlock();
        }
    }

    private void ShowPadlock()
    {
        suitcaseUI.SetActive(true);
        Time.timeScale = 0;
        Player.canMove = false;
        Player.canLook = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        puzzleActivated = true;
    }

    private void HidePadlock()
    {
        suitcaseUI.SetActive(false);
        Time.timeScale = 1;
        Player.canMove = true;
        Player.canLook = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        puzzleActivated = false;
    }

    public override void SetItemUI()
    {
        if (StateChecker.isGhost)
        {
            GetComponent<Item>().SetUI(null, null, null, "Spirit can't use", true);
        }
        else
        {
            GetComponent<Item>().SetUI(null, null, null, "Press F to unlock suitcase", true);
        }
    }
}
