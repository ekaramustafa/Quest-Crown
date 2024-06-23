using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private Transform startButton;
    [SerializeField] private Transform exitButton;



    private void Awake()
    {
        startButton.GetComponent<Button_UI>().ClickFunc = () => {
            Loader.LoadLevelSelector();
        };
        startButton.GetComponent<Button_UI>().AddButtonSounds();
        exitButton.GetComponent<Button_UI>().ClickFunc = () => {
            Application.Quit();
        };
        exitButton.GetComponent<Button_UI>().AddButtonSounds();
    }
}
