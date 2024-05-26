using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    [SerializeField] private Transform arrow;


    public Transform GetArrow()
    {
        return arrow;
    }
}
