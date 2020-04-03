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
        enteredCode1 += gameObject.name;
        totalInputs += 1;
    }

    private void DetermineNumberPressed()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            enteredCode1 += "1";
            totalInputs += 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            enteredCode1 += "2";
            totalInputs += 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            enteredCode1 += "3";
            totalInputs += 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            enteredCode1 += "4";
            totalInputs += 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            enteredCode1 += "5";
            totalInputs += 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            enteredCode1 += "6";
            totalInputs += 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            enteredCode1 += "7";
            totalInputs += 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            enteredCode1 += "8";
            totalInputs += 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            enteredCode1 += "9";
            totalInputs += 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            enteredCode1 += "0";
            totalInputs += 1;
        }

        Debug.Log(enteredCode1);

    }

    private void CheckifCodeisCorrect()
    {
        if (totalInputs == 5)
        {
            if (enteredCode1 == correctCode1)
            {
                Debug.Log("Correct!");
                totalInputs = 0;
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Incorrect");
                enteredCode1 = "";
                totalInputs = 0;
            }
        }
    }
}
