using UnityEngine;

public static class PersistenceManager
{
    public static int GetCurrentLevelIndex()
    {
        return PlayerPrefs.GetInt("CurrentLevelIndex", 0);
    }
    
    public static void SetCurrentLevelIndex(int value)
    {
        PlayerPrefs.SetInt("CurrentLevelIndex", value);
    }
}
