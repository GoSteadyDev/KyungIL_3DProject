using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class GameSaveData
{
    public int gold;
    public int waveIndex;
    public List<TowerSaveData> towers;
    public List<UnitSaveData> units;
}

[System.Serializable]
public struct UnitSaveData
{
    public string prefabId;
    public SerializableVector3 position;
    public float health;
}

[System.Serializable]
public struct TowerSaveData
{
    public TowerType type;           // Template의 towerType
    public int level;                // Template의 level
    public string pathCode;          // Template의 pathCode
    public SerializableVector3 pos;  // 위치
    public SerializableVector3 rot;  // 회전(Quaternion 대신 Vector3로)
}

[System.Serializable]
public struct SerializableVector3
{
    public float x,y,z;
    public static implicit operator Vector3(SerializableVector3 s)
        => new Vector3(s.x,s.y,s.z);
    public static implicit operator SerializableVector3(Vector3 v)
        => new SerializableVector3{ x=v.x, y=v.y, z=v.z };
}

public static class SaveSystem
{
    private static string path => Application.persistentDataPath + "/save.json";

    public static GameSaveData PendingLoad { get; set; }
    public static bool HasSave()
    {
        return File.Exists(path);
    }

    public static void Save(GameSaveData data)
    {
        string json = JsonUtility.ToJson(data, prettyPrint: true);
        File.WriteAllText(path, json);
    }

    public static void QueueLoad() 
    {
        PendingLoad = Load(); // JSON 파싱
    }
    
    public static GameSaveData Load()
    {
        if (!HasSave())
            throw new InvalidOperationException("저장 데이터가 없습니다.");
        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<GameSaveData>(json);
    }
}