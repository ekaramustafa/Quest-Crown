using System;
using UnityEngine.SceneManagement;
public static class Loader
{
    public const string MAX_LEVEL = "MAX_LEVEL";

    private static string currentLevel = "";
    public enum Scene
    {
        MainMenu,
        LevelSelectorMenu,
        Loading,
    }

    private static Scene targetScene;
    public static void Load(Scene scene)
    {
        targetScene = scene;
        SceneManager.LoadScene(Scene.Loading.ToString());
    }

    public static void LoadTargetScene()
    {
        SceneManager.LoadScene(targetScene.ToString());
    }

    public static void LoadLevel(int level)
    {
        string levelString = "Level";
        levelString += level.ToString();
        currentLevel = levelString;
        SceneManager.LoadScene(levelString);
    }


    public static void LoadCurrentLevel()
    {
        SceneManager.LoadScene(currentLevel);
    }
}