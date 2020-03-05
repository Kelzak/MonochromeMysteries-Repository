using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Television : MonoBehaviour
{
    public GameObject screen;
    public GameObject mainMenu, tvStatic;
    public Animation staticAnim;

    private MeshRenderer mesh;
    private Vector3 boxCenter;
    private Collider[] hit;

    // Start is called before the first frame update
    void Start()
    {
        screen = transform.Find("Screen").gameObject;
        mainMenu = screen.transform.Find("MainMenu").gameObject;
        tvStatic = screen.transform.Find("Static").gameObject;


        mesh = GetComponent<MeshRenderer>();
        boxCenter = transform.forward + mesh.bounds.max;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool CheckForPlayerInRange()
    {
        //Check area in front of TV for player
        hit = Physics.OverlapBox(boxCenter, mesh.bounds.size, Quaternion.identity);
        bool found = false;
        foreach (Collider x in hit)
        {
            if (x.gameObject.GetComponent<Player>())
            {
                found = true;
            }
        }

        return found;
    }

   
}
