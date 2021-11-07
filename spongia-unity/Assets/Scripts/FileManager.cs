using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public static class FileManager
{
    public static bool WriteToFile(string a_FileName, string a_FileContents, string path = null)
    {
        if (path == null)
        {
            path = Application.persistentDataPath;
        }

        var fullPath = Path.Combine(path, a_FileName);

        try
        {
            File.WriteAllText(fullPath, a_FileContents);
            Debug.Log("Saved settings");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to write to {fullPath} with exception {e}");
            return false;
        }
    }

    public static bool LoadFromFile(string a_FileName, out string result, string path = null)
    {
        if (path == null)
        {
            path = Application.persistentDataPath;
        }

        var fullPath = Path.Combine(path, a_FileName);
        Debug.Log($"Load path: {fullPath}");

        try
        {
            result = File.ReadAllText(fullPath);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to read from {fullPath} with exception {e}");
            result = "";
            return false;
        }
    }
}