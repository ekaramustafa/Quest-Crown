using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
public static class Loader
{
    public const string MAX_LEVEL = "MAX_LEVEL";

    private static string currentLevel = "Level1";
    public static int currentLevelInteger = 1;
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
        currentLevelInteger = level;
        string levelString = "Level";
        levelString += level.ToString();
        currentLevel = levelString;
        SceneManager.LoadScene(levelString);
    }


    public static void LoadCurrentLevel()
    {
        SceneManager.LoadScene(currentLevel);
    }

    public static void LoadNextLevel()
    {
        int maxLevel = PlayerPrefs.GetInt(MAX_LEVEL);
        if(maxLevel == currentLevelInteger)
        {
            PlayerPrefs.SetInt(MAX_LEVEL, maxLevel + 1);
            PlayerPrefs.Save();
        }
        LoadLevel(currentLevelInteger + 1);
    }

    internal static void LoadLevelSelector()
    {
        currentLevelInteger = 1;
        Load(Scene.LevelSelectorMenu);
    }
}