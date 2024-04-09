using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public static class SaveData {

    public static Game savedGame;

    public static void Save()
    {
        savedGame = Game.current;
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/arkangelsavefile.gd");
        bf.Serialize(file, SaveData.savedGame);
        file.Close();
    }

    public static void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/arkangelsavefile.gd"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/arkangelsavefile.gd", FileMode.Open);
            SaveData.savedGame = (Game)bf.Deserialize(file);
            file.Close();
        }
    }

    public static void DeleteFile()
    {
        File.Delete(Application.persistentDataPath + "/arkangelsavefile.gd");
    }
}
