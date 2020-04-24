using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Data
{
    //Save Settings, Independent of Game
    [System.Serializable]
    public class SaveData
    {
        public bool existingSave;
        public int mostRecentSaveSlot;

        public SaveData()
        {
            existingSave = SaveSystem.existingSaveData;
            mostRecentSaveSlot = SaveSystem.currentSaveSlot;
        }
    }

    //Game State Data
    /// <summary>
    /// The Combination of all the different instances of Game State Data
    /// </summary>
    [System.Serializable]
    public class GameData
    {
        public PlayerData playerData;
        public MainMenuData mainMenuData;
        public PhotoLibraryData libraryData;

        public GameData(Player player, MainMenu menu)
        {
            playerData = new PlayerData(player);
            mainMenuData = new MainMenuData(menu);
           libraryData = new PhotoLibraryData();
        }
    }

    [System.Serializable]
    public class PlayerData
    {
        //Name of what player is in when saving
        public string playerName;

        public PlayerData(Player player)
        {
            playerName = player.name;
        }
    }

    [System.Serializable]
    public class MainMenuData
    {
        public float[] currentTV_pos;

        public MainMenuData(MainMenu menu)
        {
            //Position of TV
            currentTV_pos = new float[3];
            Television tempTV = menu.GetCurrentTV();
            currentTV_pos[0] = tempTV.transform.position.x;
            currentTV_pos[1] = tempTV.transform.position.y;
            currentTV_pos[2] = tempTV.transform.position.z;
        }
    }

    [System.Serializable]
    public class PhotoLibraryData
    {
        public string[] photoImgPaths;
        public int[][] cluesFeatured;
        public string[] labelTexts;
        public string[] detailTexts;
        public uint photoCount;

        public PhotoLibraryData()
        {
            //Get image paths
            photoImgPaths = PhotoLibrary.photoPaths.ToArray();
            photoCount = PhotoLibrary._instance.photoCount;

            List<PhotoLibrary.PhotoInfo> tempInfo = PhotoLibrary.GetPhotoInfo();

            //Set Clues Featured,Label Texts, and detail texts for photos
            cluesFeatured = new int[tempInfo.Count][];
            for(int i = 0; i < tempInfo.Count; i++)
            {
                cluesFeatured[i] = new int[tempInfo[i].cluesFeatured.Length];

                //Set Clues Featured
                for (int j = 0; j < cluesFeatured[i].Length; j++)
                {
                    cluesFeatured[i][j] = tempInfo[i].cluesFeatured[j];
                }

            }

        }
    }

    [System.Serializable]
    public class TutorialData
    {
        public bool tutorialCompleted;
    }
}
