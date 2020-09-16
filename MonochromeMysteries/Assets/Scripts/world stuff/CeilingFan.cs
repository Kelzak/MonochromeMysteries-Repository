using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CeilingFan : MonoBehaviour
{
    private IEnumerator coroutine;
    public float spinSpeed;
    public float spinTimer;
    private float defaultSpinSpeed;
    public float slowSpinSpeed;
    public float fastSpinSpeed;
    public bool onSwitch = true;
    private bool toggle;

    // Start is called before the first frame update
    void Start()
    {
        defaultSpinSpeed = spinSpeed;
        //StartCoroutine("Spin1", spinTimer);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * Time.deltaTime * spinSpeed);

        if (onSwitch)
        {

            //StartCoroutine("Spin1", spinTimer);
        }
        else
        {
            spinSpeed = Mathf.Lerp(spinSpeed, 0, spinTimer * Time.deltaTime);
            return;
        }

        if(toggle)
        {
            spinSpeed = Mathf.Lerp(spinSpeed, slowSpinSpeed, spinTimer * Time.deltaTime);
            toggle = false;
        }
        else
        {
            spinSpeed = Mathf.Lerp(spinSpeed, fastSpinSpeed, spinTimer * Time.deltaTime);
            toggle = true;
        }
    }


/*
    private IEnumerator Spin1(float spinTime)
    {
        spinSpeed = Mathf.Lerp(fastSpinSpeed, slowSpinSpeed, spinTime * Time.deltaTime);
        yield return new WaitForSeconds(spinTime);
        StartCoroutine("Spin2", spinTime);


    }
    private IEnumerator Spin2(float spinTime)
    {
        spinSpeed = Mathf.Lerp(slowSpinSpeed, fastSpinSpeed, spinTime * Time.deltaTime);
        yield return new WaitForSeconds(spinTime);
        StartCoroutine("Spin1", spinTime);

    }
*/

    public void Activate()
    {
        onSwitch = !onSwitch;
    }
}
