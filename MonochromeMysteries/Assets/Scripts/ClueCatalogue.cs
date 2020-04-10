﻿/* Name: ClueCatalogue.cs
 * Author: Zackary Seiple
 * Description: Contains a reference to all the clues in the game and information about them
 * Last Updated: 2/18/2020 (Zackary Seiple)
 * Changes: Initial Creation
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClueCatalogue : MonoBehaviour
{
    public static ClueCatalogue _instance;
    //The array of clues
    public Clue[] clues;

    /// <summary>
    /// The structure of a clue, contains the (string) name, (string) description, (GameObject) object and (bool) relevance to the case
    /// </summary>
    [System.Serializable]
    public struct Clue
    {
        //The name of the clue
        public string name;
        //The description of the clue that will be shown by the photo
        public string description;
        //The Object that represents this clue
        public GameObject @object;
        //Determines whether this clue actually pertains to the case
        public bool relevant;
    }

    // Start is called before the first frame update
    void Start()
    {
        _instance = this;

    }

    /// <summary>
    /// Find a Clue in the clues array based on the given GameObject
    /// </summary>
    /// <param name="obj">The GameObject to search for</param>
    /// <returns>Index that is associated with obj if found, -1 if not found</returns>
    public int FindClue(GameObject obj)
    {
        for(int i = 0; i < clues.Length; i++)
        {
            if (clues[i].@object == obj)
                return i;
        }
        return -1;
    }

    /// <summary>
    /// Find a Clue in the clues array based on given string name
    /// </summary>
    /// <param name="name">The string name to search for</param>
    /// <returns>index that is associated with the name string if found, -1 if not found</returns>
    public int FindClue(string name)
    {
        for(int i = 0; i < clues.Length; i++)
        {
            if (clues[i].name == name)
                return i;
        }
        return -1;
    }

    public int[] DetectCluesOnScreen(float posX = 0, float posY = 0, float width = 1920, float height = 1080, float detectionDistance = 15)
    {

        //Create List of indexes
        List<int> cluesDetected = new List<int>();

        //Selecting only camera part of the screen
        Rect screenSpace = new Rect(posX, posY, width, height);

        RaycastHit[] hit;
        GameObject currObj;
        Vector3 targetPoint;
        Vector3 objScreenPoint = Vector3.zero;

        //Check if clue is in area of screen, if it's close enough, and if it's visible
        for (int i = 0; i < clues.Length; i++)
        {
            currObj = clues[i].@object;
            targetPoint = currObj.transform.Find("TargetPoint") != null ? currObj.transform.Find("TargetPoint").position : currObj.transform.position;
            if ( screenSpace.Contains(objScreenPoint = Camera.main.WorldToScreenPoint(targetPoint)) &&
                objScreenPoint.z < detectionDistance &&
                objScreenPoint.z > 0)
            {
                hit = Physics.RaycastAll(Camera.main.transform.position, targetPoint - Camera.main.transform.position, Vector3.Distance(Camera.main.transform.position, targetPoint), ~0, QueryTriggerInteraction.Ignore);
                bool safe = true;
                foreach(RaycastHit x in hit)
                {
                    safe = x.collider.gameObject == currObj || x.collider.gameObject.GetComponent<Player>();
                    if (safe == false)
                        break;
                }
                if(safe)
                { 
                    Debug.Log("Detected: " + currObj.name);
                    cluesDetected.Add(i);
                }
            }
        }

        //Debug.Log(objScreenPoint.ToString() + " clues detected: " + cluesDetected.Count);
        //Debug.Log("clues detected: " + cluesDetected.Count /*+ " name:" + hit.collider.gameObject.name*/);
        return cluesDetected.ToArray();
    }
}
