using CodeMonkey.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectLevelUI : MonoBehaviour
{
    [SerializeField] private List<Transform> buttons;


    private List<TextMeshProUGUI> textMeshPros;

    private int maxLevel;

    private void Awake()
    {
        textMeshPros = new List<TextMeshProUGUI>();
        foreach(Transform button in buttons)
        {
            textMeshPros.Add(button.GetComponent<TextMeshProUGUI>());
        }
        VerifyLevelPref();
    }
    private void Start()
    {
        HideAllLevelTexts();
        maxLevel = PlayerPrefs.GetInt(Loader.MAX_LEVEL);
        for(int i=0;i< maxLevel; i++)
        {
            TextMeshProUGUI textMeshPro = textMeshPros[i];
            textMeshPro.overrideColorTags = true;
            Color currentColor = textMeshPro.color;
            textMeshPro.color = new Color(currentColor.r, currentColor.b, currentColor.g, 1f);
        }
        SetupClickDelegates();
    }


    private void HideAllLevelTexts()
    {
        foreach(TextMeshProUGUI textMeshPro in textMeshPros)
        {
            textMeshPro.overrideColorTags = true;
            Color currentColor = textMeshPro.color;
            textMeshPro.color = new Color(currentColor.r, currentColor.b, currentColor.g, currentColor.a / 2);
        }
    }

    public void VerifyLevelPref()
    {
        if (PlayerPrefs.HasKey(Loader.MAX_LEVEL)) return;
        PlayerPrefs.SetInt(Loader.MAX_LEVEL, 1);
        PlayerPrefs.Save();
    }

    private void SetupClickDelegates()
    {
        Debug.Log(maxLevel);
        for(int i=0;i<maxLevel; i++)
        {
            int levelIndex = i + 1;
            buttons[i].GetChild(0).GetComponent<Button_UI>().ClickFunc = () =>
            {
                Loader.LoadLevel(levelIndex);
            };
        }
    }



}
