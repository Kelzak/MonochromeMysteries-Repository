//Made by matt kirchoff

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeAnim : MonoBehaviour
{
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenSafe(GameObject safe)
    {
        animator = safe.transform.Find("Hinge").GetComponent<Animator>();
        animator.SetBool("open", true);

    }
}
