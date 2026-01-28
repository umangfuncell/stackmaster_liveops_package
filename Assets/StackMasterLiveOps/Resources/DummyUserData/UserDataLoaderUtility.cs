using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public static class UserDataLoaderUtility
{
    // Define the constant filename here, relative to the Resources folder
    // The path includes the subfolder "dummy user data/" and the file name "UserData"
    private const string CsvFilePath = "DummyUserData/profileData";
    private const string ProfilePicturesPath = "DummyUserData/ProfilePictures/";

    /// <summary>
    /// Data structure to hold a row of user data.
    /// </summary>

    // Helper method to load the TextAsset once
    private static TextAsset LoadCsvFile()
    {
        TextAsset csvFile = Resources.Load<TextAsset>(CsvFilePath);
        if (csvFile == null)
        {
            Debug.LogError("CSV file not found at path: " + CsvFilePath);
        }
        return csvFile;
    }

    /// <summary>
    /// Gets one random user data entry from the CSV file.
    /// </summary>
    public static UserData GetRandomUser()
    {
        TextAsset csvFile = LoadCsvFile();
        if (csvFile == null) return null;

        string[] lines = csvFile.text.Split('\n');

        // Filter out empty lines and the header (use Skip(1))
        var dataLines = lines.Skip(1).Where(line => !string.IsNullOrWhiteSpace(line)).ToList();

        if (dataLines.Count == 0)
        {
            Debug.LogError("No data rows found in the CSV file.");
            return null;
        }

        // Select a random line using Unity's Random.Range
        int randomIndex = Random.Range(0, dataLines.Count);
        string randomLine = dataLines[randomIndex];

        string[] fields = randomLine.Trim().Split(',');

        if (fields.Length >= 4)
        {
            return new UserData
            {
                ID = int.Parse(fields[0]),
                Name = fields[1].Trim(),
                ImageFileName = fields[2].Trim(),
            };
        }
        else
        {
            Debug.LogError("Error parsing random line: not enough fields.");
            return null;
        }
    }

    /// <summary>
    /// Loads a Sprite dynamically from the ProfilePictures subfolder in Resources.
    /// </summary>
    public static Sprite LoadSpriteFromResources(string imageFileName)
    {
        // Remove the file extension for Resources.Load
        string spriteNameWithoutExtension = Path.GetFileNameWithoutExtension(imageFileName);

        // The path is relative to any "Resources" folder
        string fullPath = ProfilePicturesPath + spriteNameWithoutExtension;

        Sprite loadedSprite = Resources.Load<Sprite>(fullPath);

        if (loadedSprite == null)
        {
            Debug.LogError($"Failed to load sprite at path: {fullPath}");
        }

        return loadedSprite;
    }

    // You could also update GetUserDataByName to not require a filename parameter
    // public static UserData GetUserDataByName(string targetName) { ... }
}
public class UserData
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string ImageFileName { get; set; }
}