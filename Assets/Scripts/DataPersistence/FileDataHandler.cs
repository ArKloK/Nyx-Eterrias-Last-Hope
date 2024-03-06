using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileDataHandler
{
    private string filePath;
    private string fileName;

    public FileDataHandler(string filePath, string fileName)
    {
        this.filePath = filePath;
        this.fileName = fileName;
    }

    public GameData LoadData()
    {
        string fullpath = Path.Combine(filePath, fileName);
        GameData loadedData = null;
        if (File.Exists(fullpath))
        {
            try
            {
                string dataToLoad;
                using (FileStream stream = new FileStream(fullpath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                //desirialize the data from JSON to a GameData object
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.Log("Error ocurred while trying to load data from file: " + fullpath + "\n" + e);
            }
        }
        return loadedData;
    }

    public void SaveData(GameData gameData)
    {
        string fullpath = Path.Combine(filePath, fileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullpath));

            string dataToStore = JsonUtility.ToJson(gameData, true);

            using (FileStream stream = new FileStream(fullpath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("Error ocurred while trying to save data to file: " + fullpath + "\n" + e);
        }
    }
}
