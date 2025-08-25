using UnityEngine;

public class TutorialSaveUtil
{
    private const string TutorialKey = "TutorialCompleted";

    public static bool IsTutorialCompleted()
    {
        return PlayerPrefs.GetInt(TutorialKey, 0) == 1;
    }

    public static void MarkTutorialComplete()
    {
        PlayerPrefs.SetInt(TutorialKey, 1);
        PlayerPrefs.Save();
    }

    public static void ResetTutorialProgress() // 테스트용
    {
        PlayerPrefs.DeleteKey(TutorialKey);
    }
}
