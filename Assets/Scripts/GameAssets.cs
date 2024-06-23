using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    [Header("Transforms&Prefabs")]
    [SerializeField] private Transform arrow;

    [Space(5)]
    [Header("Sound Clips")]
    public List<SoundAudioClip> audioClips;
    [Serializable]
    public class SoundAudioClip
    {
        public SoundManager.Sound sound;
        public AudioClip audioClip;

    }
    public Transform GetArrow()
    {
        return arrow;
    }
}
