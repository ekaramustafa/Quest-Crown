using UnityEngine.SceneManagement;
public static class Loader
{

    public enum Scene
    {
        MainMenu,
        Loading,
        GameScene,

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
}