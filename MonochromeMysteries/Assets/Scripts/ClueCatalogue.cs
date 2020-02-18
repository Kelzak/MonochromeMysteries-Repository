/* Name: ClueCatalogue.cs
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
    /// <returns>Clue struct that is associated with obj if found, null if not found</returns>
    public Clue? FindClue(GameObject obj)
    {
        foreach(Clue clue in clues)
        {
            if (clue.@object == obj)
                return clue;
        }
        return null;
    }

    /// <summary>
    /// Find a Clue in the clues array based on given string name
    /// </summary>
    /// <param name="name">The string name to search for</param>
    /// <returns>Clue struct that is associated with the name string if found, null if not found</returns>
    public Clue? FindClue(string name)
    {
        foreach(Clue clue in clues)
        {
            if (clue.name == name)
                return clue;
        }
        return null;
    }
}
