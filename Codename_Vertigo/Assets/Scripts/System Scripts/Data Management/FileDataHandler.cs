using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileDataHandler
{
    //Stores the name of the directory where the file/folder is saved
    private string dataDirPath = "";
    //Store the name of the data file
    private string dataFileName = "";

    //Whether or not encryption is being used (This is configured by the persistence manager)
    private bool useEncryption = false;
    //Code word used for encrypting data
    private readonly string encryptionCodeword = "Gamma";

    //File extension for back-up data file
    private readonly string backupExtensions = ".bak";

    //Constructor called from the persistence manager
    public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
        this.useEncryption = useEncryption;
    }

    public GameData Load(string profileId, bool allowRestoreFromBack = true)
    {
        //Base case - if the id is null, return right away
        if(profileId == null)
        {
            return null;
        }

        //Use path.combine to account for different OS's and path separators
        string fullPath = Path.Combine(dataDirPath, profileId,dataFileName);
        GameData loadedData = null;
        if (File.Exists(fullPath))
        {
            try
            {
                //Load the serialized data from the file
                string dataToLoad = "";
                //Open a file stream to connect to the file and open it
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    //Create a filestream reader to enable reading from the file
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        //Set the string by reading all the way to the end of the file
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                if (useEncryption)
                {
                    //Set dataToLoad by decrypting it using XOR encryption and the codeword
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }

                //deserialize data from Json back into game data object
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);

            }
            //If the file failed to load the first time
            catch (Exception e)
            {
                //If the file is allowed to restore from backup (Which it will do once, by default)
                if (allowRestoreFromBack)
                {
                    //Issue a warning, then attempt to roll back the file
                    Debug.LogWarning("Failed to load data file. Attempting to roll back.\n" + e);
                    bool rollbackSuccess = AttemptRollbackOfData(fullPath);
                    //If rollback was successful, attempt the load again
                    if (rollbackSuccess)
                    {
                        loadedData = Load(profileId, false);
                    }
                }
                else
                {
                    Debug.LogError("Error occurred when trying to load file at path " + fullPath
                        + " and backup failed to work.\n" + e);
                }
            }
        }

        //Return the loaded Data
        return loadedData;
    }

    //Used to save the data
    public void Save(GameData data, string profileId)
    {
        //Return if the ID is null
        if(profileId == null)
        {
            return;
        }
        //Use path.combine to account for different OS's and path separators
        string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
        //Backup file path is created by adding a backup extension to the existing file path
        string backupFilePath = fullPath + backupExtensions;
        try
        {
            //Create directory the file will be written to if it doesn't already exist
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            //Serialize c# object into Json
            string dataToStore = JsonUtility.ToJson(data, true);

            if (useEncryption)
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }

            //write serialized data to file
            //Using blocks ensure the connection to the file is closed when finished reading/writing
            //Neglecting to do this will leave the file in a permanent state of being open, resulting in it
            //Becoming inaccessible
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                //Creates a streamwriter, responsible for writing data to the file
                using(StreamWriter writer = new StreamWriter(stream))
                {
                    //Write data to file
                    writer.Write(dataToStore);
                }
            }

            //Verify that the newly saved file can be loaded successfully
            GameData verifiedGameData = Load(profileId);

            //If the verified data isn't null, copy the file to the backup path, and overwrite the file that is currently there if it exists
            if(verifiedGameData != null)
            {
                File.Copy(fullPath, backupFilePath, true);
            }
            //Otherwise, throw an exception
            else
            {
                throw new Exception("Save file couldn't be verified and back-up could not be created");
            }


        } catch(Exception e)
        {
            Debug.LogError("Error occuured when trying to save data to file: " + fullPath + "\n" + e);
        }
    }

    //Called when deleting data from a save slot
    public void Delete(string profileId)
    {
        //Base case - if id is null, return as it needs to exist first
        if(profileId == null)
        {
            return;
        }


        string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
        try
        {
            if (File.Exists(fullPath))
            {
                //Delete the profile folder and everything within it
                Directory.Delete(Path.GetDirectoryName(fullPath), true);
            }
            else
            {
                Debug.LogWarning("Tried to delete profile data, but data was not found at: " + fullPath);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to delete data for profileId: " + profileId + " at path: " + fullPath + "\n" + e);
        }
    }

    public Dictionary<string, GameData> LoadAllProfiles()
    {
        Dictionary<string, GameData> profileDictionary = new Dictionary<string, GameData>();

        //Loop all directory names in the directory path
        IEnumerable<DirectoryInfo> dirInfos = new DirectoryInfo(dataDirPath).EnumerateDirectories();
        
        foreach(DirectoryInfo dirInfo in dirInfos)
        {
            string profileId = dirInfo.Name;

            //Defensive programming - check if it exists
            //If it doesn't, then this folder isn't a save file and should be skipped
            string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
            if (!File.Exists(fullPath))
            {
                continue;
            }

            //Load the game for the profile and save it within the dictionary
            GameData profileData = Load(profileId);
            //Ensure the data isn't null
            if(profileData != null)
            {
                profileDictionary.Add(profileId, profileData);
            }
            else
            {
                Debug.LogError("Tried to load profile but something went wrong.");
            }
        }

        return profileDictionary;
    }

    public string GetDataLastScene(string selectedID)
    {
        GameData dataToCheck = Load(selectedID);

        return dataToCheck.lastSceneSaved;
    }

    public string GetMostRecentlyUpdatedProfile()
    {
        string mostRecentProfile = null;
        Dictionary<string, GameData> profilesData = LoadAllProfiles();
        foreach(KeyValuePair<string, GameData> pair in profilesData)
        {
            string profileId = pair.Key;
            GameData gameData = pair.Value;

            if(gameData == null)
            {
                continue;
            }

            if(mostRecentProfile == null)
            {
                mostRecentProfile = profileId;
            }
            else
            {
                DateTime mostRecentDateTime = DateTime.FromBinary(profilesData[mostRecentProfile].lastSaved);
                DateTime newDateTime = DateTime.FromBinary(gameData.lastSaved);

                if(newDateTime > mostRecentDateTime)
                {
                    mostRecentProfile = profileId;
                }

            }
        }

        return mostRecentProfile;
    }

    //Method responsible for encrypting and decrypting data if the manager is configured to encrypt data
    private string EncryptDecrypt(string data)
    {
        //Set modified data to blank
        string modifiedData = "";
        //For each of the characters in the data, encrypt it using a random word from the encryption codeword
        //This is done using XOR encryption
        for(int i = 0; i < data.Length; i++)
        {
            modifiedData += (char)(data[i] ^ encryptionCodeword[i % encryptionCodeword.Length]);
        }

        //Return the modified data to write to the JSON file
        return modifiedData;
    }

    private bool AttemptRollbackOfData(string fullPath)
    {
        bool success = false;
        string backupFilePath = fullPath + backupExtensions;

        try
        {
            if (File.Exists(backupFilePath))
            {
                File.Copy(backupFilePath, fullPath, true);
                success = true;
                Debug.LogWarning("Data had to be rolled back to backup file at: " + backupFilePath);
            }
            else
            {
                throw new Exception("Tried to roll back data, but no backup file exists to roll back to");
            }
        }
        catch(Exception e)
        {
            Debug.Log("Error occurred while trying to roll back to backup file at: " + backupFilePath + "\n" + e);
        }

        return success;
    }

}
