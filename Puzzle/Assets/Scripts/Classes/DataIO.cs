
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class DataIO
{
    private static char SEP = Path.DirectorySeparatorChar;
    public static string RESOURCE_PATH = "Assets" + SEP + "Resources" + SEP;

    public static void WriteToJson<T>(string file_path, T data, bool append = false)
    {
        TextWriter writer = null;
        try
        {
            var json = JsonUtility.ToJson(data);
            writer = new StreamWriter(file_path, append);
            writer.Write(json);
        }
        finally
        {
            writer?.Close();
        }
    }

    public static T ReadFromJson<T>(string file_path)
    {
        TextReader reader = null;
        try
        {
            reader = new StreamReader(file_path);
            var json = reader.ReadToEnd();
            return JsonUtility.FromJson<T>(json);
        }
        finally
        {
            reader?.Close();
        }
    }
}
