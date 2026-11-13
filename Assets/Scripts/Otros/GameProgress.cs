using System;
using System.Collections.Generic;
using UnityEngine;

public static class GameProgress
{
    public const int TotalRequired = 9;
    public const int PerWorld = 3;
    private const string SaveKey = "RelatosOscuros_Progreso";

    [Serializable]
    public class SaveData
    {
        public List<string> ids = new List<string>();
        public List<int> worlds = new List<int>();
    }

    private static SaveData data = new SaveData();
    private static bool loaded;

    public static event Action<int> OnCollectedChanged;
    public static event Action OnAllCollected;

    public static int Total { get { EnsureLoaded(); return data.ids.Count; } }
    public static bool AllCollected { get { return Total >= TotalRequired; } }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        loaded = false;
        data = new SaveData();
        OnCollectedChanged = null;
        OnAllCollected = null;
    }

    private static void EnsureLoaded()
    {
        if (loaded) return;
        loaded = true;
        string json = PlayerPrefs.GetString(SaveKey, "");
        if (string.IsNullOrEmpty(json)) return;
        SaveData d = JsonUtility.FromJson<SaveData>(json);
        if (d != null && d.ids != null && d.worlds != null && d.ids.Count == d.worlds.Count) data = d;
    }

    public static bool IsCollected(string id)
    {
        EnsureLoaded();
        return data.ids.Contains(id);
    }

    public static int CountInWorld(int world)
    {
        EnsureLoaded();
        int n = 0;
        foreach (int w in data.worlds)
            if (w == world) n++;
        return n;
    }

    public static bool Collect(string id, int world)
    {
        EnsureLoaded();
        if (data.ids.Contains(id)) return false;
        data.ids.Add(id);
        data.worlds.Add(world);
        Save();
        if (OnCollectedChanged != null) OnCollectedChanged(data.ids.Count);
        if (data.ids.Count >= TotalRequired && OnAllCollected != null) OnAllCollected();
        return true;
    }

    public static void Save()
    {
        PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(data));
        PlayerPrefs.Save();
    }

    public static void ResetAll()
    {
        data = new SaveData();
        loaded = true;
        PlayerPrefs.DeleteKey(SaveKey);
        PlayerPrefs.Save();
        if (OnCollectedChanged != null) OnCollectedChanged(0);
    }
}