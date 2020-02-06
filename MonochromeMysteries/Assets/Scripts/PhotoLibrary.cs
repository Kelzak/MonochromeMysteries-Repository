using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PhotoLibrary : MonoBehaviour
{
    PhotoLibrary instance;

    public static string directory;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        directory = Application.dataPath + "/_GameData/pictures";

        Directory.CreateDirectory(directory);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnApplicationQuit()
    {
        Directory.Delete(directory, true);
    }
}
