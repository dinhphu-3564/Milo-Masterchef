using UnityEngine;

public static class ItemData
{
    public static int GetItem(string key)
    {
        return PlayerPrefs.GetInt(key, 0);
    }

    public static void AddItem(string key, int amount)
    {
        int current = GetItem(key);
        PlayerPrefs.SetInt(key, current + amount);
        PlayerPrefs.Save();
    }
}
