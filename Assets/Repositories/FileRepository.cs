using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

[System.Serializable]
public class UserData
{
    public string username;
    public string password;
    public int wins;
}

[System.Serializable]
public class DatabaseWrapper
{
    public List<UserData> users = new List<UserData>();
}


public class FileRepository : IPlayerRepository
{
    private string savePath;

    public FileRepository()
    {
        savePath = Path.Combine(Application.persistentDataPath, "game_users_v2.json");
    }

    private DatabaseWrapper LoadDatabase()
    {
        if (File.Exists(savePath))
        {
            return JsonUtility.FromJson<DatabaseWrapper>(File.ReadAllText(savePath));
        }
        return new DatabaseWrapper();
    }

    private void SaveDatabase(DatabaseWrapper db)
    {
        File.WriteAllText(savePath, JsonUtility.ToJson(db, true));
    }

    

    public bool RegisterUser(string username, string password)
    {
        var db = LoadDatabase();
        if (db.users.Any(u => u.username == username)) return false;

        db.users.Add(new UserData { username = username, password = password, wins = 0 });
        SaveDatabase(db);
        return true;
    }

    public bool LoginUser(string username, string password)
    {
        var db = LoadDatabase();
        return db.users.Any(u => u.username == username && u.password == password);
    }

   
    public void SaveWin()
    {
        string currentName = SessionManager.Instance.playerName;
        if (string.IsNullOrEmpty(currentName)) return;

        var db = LoadDatabase();
        var user = db.users.FirstOrDefault(u => u.username == currentName);
        
        if (user != null)
        {
            user.wins++;
            SaveDatabase(db);
            Debug.Log(currentName + " kazandı! Toplam zafer: " + user.wins);
        }
    }

    public int GetHighScore()
    {
        var db = LoadDatabase();
        if (db.users.Count == 0) return 0;
        return db.users.Max(u => u.wins);
    }
}