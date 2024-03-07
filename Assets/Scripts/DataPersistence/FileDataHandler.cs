using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using System.IO;

public class FileDataHandler
{
    private string filePath;
    private string fileName;
    private bool useEncryption;
    private readonly string encryptionCodeWord = "galleta826e034602ghsjai";

    public FileDataHandler(string filePath, string fileName, bool useEncryption)
    {
        this.filePath = filePath;
        this.fileName = fileName;
        this.useEncryption = useEncryption;
    }

    /// <summary>
    /// Loads the data from the save file to a Game Data object.
    /// </summary>
    public GameData LoadData(PlayerControllerData playerControllerData)
    {
        string fullpath = Path.Combine(filePath, fileName);
        GameData loadedData = new GameData(playerControllerData);
        try
        {
            if (File.Exists(fullpath))
            {
                var dataToLoad = string.Empty;
                using (FileStream stream = new FileStream(fullpath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                //optionally decrypt the data
                if (useEncryption)
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }

                //desirialize the data from JSON to a GameData object
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
                //loadedData = JsonConvert.DeserializeObject<GameData>(dataToLoad);
                Debug.Log(loadedData);
            }
            else
            {
                loadedData = null;
            }
        }
        catch (Exception e)
        {
            Debug.Log("Error ocurred while trying to load data from file: " + fullpath + "\n" + e);
        }

        return loadedData;
    }

    /// <summary>
    /// Saves the Game Data to a file.
    /// </summary>
    /// <param name="gameData">The game data to be saved.</param>
    public void SaveData(GameData gameData)
    {
        string fullpath = Path.Combine(filePath, fileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullpath));

            string dataToStore = JsonUtility.ToJson(gameData, true);
            //string dataToStore = JsonConvert.SerializeObject(gameData, Formatting.Indented);

            //optionally encrypt the data
            if (useEncryption)
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }

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

    //It is encrypted by using the XOR operator
    private string EncryptDecrypt(string textToEncrypt)
    {
        string result = string.Empty;
        for (int i = 0; i < textToEncrypt.Length; i++)
        {
            result += (char)(textToEncrypt[i] ^ encryptionCodeWord[i % encryptionCodeWord.Length]);
        }
        return result;
    }
}
