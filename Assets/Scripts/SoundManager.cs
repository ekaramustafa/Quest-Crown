using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static GameAssets;

public static class SoundManager
{

    //cache vars
    private static AudioSource walkAudio;

    private static float delayBeforeDestroy = 3f;


    public enum Sound
    {
        Jump,
        Death,
        ButtonOver,
        ButtonClicked,
        CoinCollect,
        Rejection,
        ArrowShooting,
        Success
    }

    public static void PlaySound(Sound sound)
    {
        GameObject gameObject = new GameObject("Sound", typeof(AudioSource));
        AudioSource audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.PlayOneShot(GetAudioClip(sound));
        GameObject.Destroy(gameObject, delayBeforeDestroy);

    }

    public static void PlaySound(Sound sound, float delay)
    {
        GameObject gameObject = new GameObject("Sound", typeof(AudioSource));
        AudioSource audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.PlayOneShot(GetAudioClip(sound));
        GameObject.Destroy(gameObject, delay);

    }

    public static AudioClip GetAudioClip(Sound sound)
    {
        foreach (SoundAudioClip soundAudioClip in GameManager.GetInstance().GetComponent<GameAssets>().audioClips)
        {
            if (soundAudioClip.sound == sound)
            {
                return soundAudioClip.audioClip;
            }
        }

        Debug.LogError("Specified Sound cannot be found : " + sound.ToString());
        return null;
    }


    public static void AddButtonSounds(this Button_UI button_UI)
    {
        button_UI.MouseOverOnceFunc += () => PlaySound(Sound.ButtonOver);
        button_UI.ClickFunc += () => PlaySound(Sound.ButtonClicked);
    }


}