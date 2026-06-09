using UnityEngine;

public static class MinigameProgress
{
    public const string GiovanniId = "Giovanni";
    public const string VictorId = "Victor";
    public const string MarinoId = "Marino";
    public const string JorgeId = "Jorge";
    public const string NicteId = "Nicte";

    public static string GetBeatenKey(string minigameId)
    {
        int userId = PlayerPrefs.GetInt("UserId", 1);
        return $"MinigameBeaten_{userId}_{minigameId}";
    }

    public static bool IsBeaten(string minigameId)
    {
        return PlayerPrefs.GetInt(GetBeatenKey(minigameId), 0) == 1;
    }

    public static void MarkBeaten(string minigameId)
    {
        PlayerPrefs.SetInt(GetBeatenKey(minigameId), 1);
        PlayerPrefs.Save();
    }
}