using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PadlockPuzzle : MonoBehaviour
{
    [SerializeField]
    public static string correctCode1 = "12345";
    public static string correctCode2 = "54321";
    public static string correctCode3 = "67890";
    public static string enteredCode1;
    public static string enteredCode2;
    public static string enteredCode3;

    public Text inputCodeText;

    public GameObject safe1;
    public GameObject safe2;
    public GameObject safe3;

    public int totalInputs = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DetermineNumberPressed();
        CheckifCodeisCorrect();

    }

    private void OnMouseDown()
    {
        //enteredCode1 += gameObject.name;
        //totalInputs += 1;
    }

    public void DetermineNumberPressed()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (Player.isAtTheFirstSafe)
            {
                totalInputs += 1;
                enteredCode1 += "1";
            }
            if (Player.isAtTheSecondSafe)
            {
                totalInputs += 1;
                enteredCode2 += "1";
            }
            if (Player.isAtTheThirdSafe)
            {
                totalInputs += 1;
                enteredCode3 += "1";
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (Player.isAtTheFirstSafe)
            {
                totalInputs += 1;
                enteredCode1 += "2";
            }
            if (Player.isAtTheSecondSafe)
            {
                totalInputs += 1;
                enteredCode2 += "2";
            }
            if (Player.isAtTheThirdSafe)
            {
                totalInputs += 1;
                enteredCode3 += "2";
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (Player.isAtTheFirstSafe)
            {
                totalInputs += 1;
                enteredCode1 += "3";
            }
            if (Player.isAtTheSecondSafe)
            {
                totalInputs += 1;
                enteredCode2 += "3";
            }
            if (Player.isAtTheThirdSafe)
            {
                totalInputs += 1;
                enteredCode3 += "3";
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (Player.isAtTheFirstSafe)
            {
                totalInputs += 1;
                enteredCode1 += "4";
            }
            if (Player.isAtTheSecondSafe)
            {
                totalInputs += 1;
                enteredCode2 += "4";
            }
            if (Player.isAtTheThirdSafe)
            {
                totalInputs += 1;
                enteredCode3 += "4";
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (Player.isAtTheFirstSafe)
            {
                totalInputs += 1;
                enteredCode1 += "5";
            }
            if (Player.isAtTheSecondSafe)
            {
                totalInputs += 1;
                enteredCode2 += "5";
            }
            if (Player.isAtTheThirdSafe)
            {
                totalInputs += 1;
                enteredCode3 += "5";
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            if (Player.isAtTheFirstSafe)
            {
                totalInputs += 1;
                enteredCode1 += "6";
            }
            if (Player.isAtTheSecondSafe)
            {
                totalInputs += 1;
                enteredCode2 += "6";
            }
            if (Player.isAtTheThirdSafe)
            {
                totalInputs += 1;
                enteredCode3 += "6";
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            if (Player.isAtTheFirstSafe)
            {
                totalInputs += 1;
                enteredCode1 += "7";
            }
            if (Player.isAtTheSecondSafe)
            {
                totalInputs += 1;
                enteredCode2 += "7";
            }
            if (Player.isAtTheThirdSafe)
            {
                totalInputs += 1;
                enteredCode3 += "7";
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            if (Player.isAtTheFirstSafe)
            {
                totalInputs += 1;
                enteredCode1 += "8";
            }
            if (Player.isAtTheSecondSafe)
            {
                totalInputs += 1;
                enteredCode2 += "8";
            }
            if (Player.isAtTheThirdSafe)
            {
                totalInputs += 1;
                enteredCode3 += "8";
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            if (Player.isAtTheFirstSafe)
            {
                totalInputs += 1;
                enteredCode1 += "9";
            }
            if (Player.isAtTheSecondSafe)
            {
                totalInputs += 1;
                enteredCode2 += "9";
            }
            if (Player.isAtTheThirdSafe)
            {
                totalInputs += 1;
                enteredCode3 += "9";
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            if (Player.isAtTheFirstSafe)
            {
                totalInputs += 1;
                enteredCode1 += "0";
            }
            if (Player.isAtTheSecondSafe)
            {
                totalInputs += 1;
                enteredCode2 += "0";
            }
            if (Player.isAtTheThirdSafe)
            {
                totalInputs += 1;
                enteredCode3 += "0";
            }
        }
    }

    private void CheckifCodeisCorrect()
    {
        if (totalInputs == 5)
        {
            if (Player.isAtTheFirstSafe && enteredCode1 == correctCode1)
            {
                Debug.Log("Correct!");
                enteredCode1 = "";
                totalInputs = 0;
                Destroy(safe1);
            }
            else if (Player.isAtTheSecondSafe && enteredCode2 == correctCode2)
            {
                Debug.Log("Correct!");
                enteredCode2 = "";
                totalInputs = 0;
                Destroy(safe2);
            }
            else if (Player.isAtTheThirdSafe && enteredCode3 == correctCode3)
            {
                Debug.Log("Correct!");
                enteredCode3 = "";
                totalInputs = 0;
                Destroy(safe3);
            }
            else
            {
                Debug.Log("Incorrect");
                enteredCode1 = "";
                enteredCode2 = "";
                enteredCode3 = "";
                totalInputs = 0;
            }
        }
    }
}
