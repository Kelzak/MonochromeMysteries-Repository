using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveSystem
{
    private const int MAX_SAVE_SLOTS = 3;
    [Range(1,MAX_SAVE_SLOTS)]
    public static int currentSaveSlot = 1;
    public static bool existingSaveData = false;
    public static bool loading = false;

    private static string saveDataPath = Path.Combine(Application.persistentDataPath, "_GameData");
    private static string fileExtension = ".david";

    private static BinaryFormatter formatter;

    public static void Save(int saveSlot)
    {
        
        formatter = new BinaryFormatter();
        //Game Save
        using (var stream = new FileStream(Path.Combine(saveDataPath, string.Format("save{0}", saveSlot) + fileExtension), FileMode.Create))
        {
            //SavePlayer((Player)GameObject.FindObjectOfType(typeof(Player)), saveSlot);
            Data.GameData data = new Data.GameData((Player)GameObject.FindObjectOfType(typeof(Player)),
                                            (MainMenu)GameObject.FindObjectOfType(typeof(MainMenu)));
            formatter.Serialize(stream, data);
        }

        //Save Save
        using (var stream = new FileStream(Path.Combine(saveDataPath, "saveInfo" + fileExtension), FileMode.Create))
        {
            Data.SaveData data = new Data.SaveData();
            formatter.Serialize(stream, data);
        }

        Log.AddEntry("Save Completed");
    }

    public static void Load()
    {
        loading = true;
        string path;
        if (!Directory.Exists(Path.Combine(saveDataPath, "_GameData")))
        {
            Directory.CreateDirectory(Path.Combine(saveDataPath, "_GameData"));
        }

        if (File.Exists(path = Path.Combine(saveDataPath, "saveInfo" + fileExtension)))
        {
            Data.SaveData data;
            using (var stream = new FileStream(path, FileMode.Open))
            {
                if(stream.Length == 0)
                {
                    return;
                }
                formatter = new BinaryFormatter();
                data = formatter.Deserialize(stream) as Data.SaveData;
                //Load General Save information
                SaveSystem.existingSaveData = data.existingSave;
                SaveSystem.currentSaveSlot = data.mostRecentSaveSlot;
            }

            Load(data.mostRecentSaveSlot);
        }
        else
        {
            File.Create(Path.Combine(saveDataPath, "saveInfo" + fileExtension)).Dispose();
            loading = false;
        }
    }

    public static void Load(int saveSlot)
    {
        loading = true;
        string path;
        if (File.Exists(path = Path.Combine(saveDataPath, string.Format("save{0}", saveSlot) + fileExtension)))
        {
            Data.GameData data;
            using (var stream = new FileStream(path, FileMode.Open))
            {
                if (stream.Length == 0)
                {
                    return;
                }
                formatter = new BinaryFormatter();
                data = formatter.Deserialize(stream) as Data.GameData;
            }

            //MAIN MENU
            MainMenu menu = (MainMenu)GameObject.FindObjectOfType(typeof(MainMenu));
            Data.MainMenuData mainMenuData = data.mainMenuData;

            var tvPos = new Vector3(mainMenuData.currentTV_pos[0], mainMenuData.currentTV_pos[1], mainMenuData.currentTV_pos[2]);
            Collider[] hit = Physics.OverlapSphere(tvPos, 1f);
            Television result = new Television();
            foreach (Collider x in hit)
            {
                if (x.GetComponent<Television>())
                    result = x.GetComponent<Television>();

            }
            if (result.screen != null)
            {
                MainMenu._instance.SetCurrentTV(result);
                GameController._instance.playerSpawn.transform.position = result.transform.Find("CamPoint").position;
                GameController._instance.playerSpawn.transform.rotation = result.transform.Find("CamPoint").rotation;
            }

            //PLAYER
            Player player = (Player)GameObject.FindObjectOfType(typeof(Player));
            player.cam = Camera.main.gameObject;
            Data.PlayerData playerData = data.playerData;

                //Find what player was possessing on save
            GameObject target = GameObject.Find(playerData.playerName);
                //Move the player
            target.transform.position = player.transform.position = GameController._instance.playerSpawn.transform.position;
            target.transform.rotation = player.transform.rotation = GameController._instance.playerSpawn.transform.rotation;;
            if(target.name != "Player")
                player.ForcePossession(target);

            //Copy attributes
            ////Component comp = GameObject.Find(playerData.playerName).AddComponent(typeof(Player));
            ////System.Reflection.FieldInfo[] fields = typeof(Player).GetFields();
            ////foreach (System.Reflection.FieldInfo field in fields)
            ////{
            ////    field.SetValue(comp, field.GetValue(player));
            ////}
            //////Put camera in possessed player
            ////comp.Poss
            ////Camera.main.transform.parent = comp.transform;
            ////Transform targetTransform = comp.transform.Find("CamPoint") ? comp.transform.Find("CamPoint") : comp.transform;
            ////Camera.main.transform.position = targetTransform.position;
            ////Camera.main.transform.rotation = targetTransform.rotation;
            ////comp.transform.position = GameController._instance.playerSpawn.transform.position;

            //PHOTO LIBRARY
            Data.PhotoLibraryData library = data.libraryData;
            PhotoLibrary._instance.photoCount = library.photoCount;
            string imagePath;
            for (int i = 0; i < library.photoImgPaths.Length; i++)
            {
                imagePath = library.photoImgPaths[i];
                PhotoLibrary.photoPaths.Add(imagePath);


                Texture2D photo;
                byte[] bytes = System.IO.File.ReadAllBytes(imagePath);
                photo = new Texture2D(160 * 2, 150 * 2);
                photo.LoadImage(bytes);
                Sprite newSprite = Sprite.Create(photo, new Rect(0, 0, photo.width, photo.height), Vector2.zero, 100f);
                newSprite.texture.Apply();
                PhotoLibrary.AddToPhotoInfo(new PhotoLibrary.PhotoInfo(newSprite, library.cluesFeatured[i]));
            }

            PhotoLibrary.TriggerOnScrapbookChange();

               
        }
        else
        {
            Debug.Log("Save Data for Slot " + saveSlot + " Not Found.");
        }
        loading = false;
    }
}
