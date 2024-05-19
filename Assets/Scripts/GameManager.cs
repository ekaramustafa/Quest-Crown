using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private static GameManager instance;

    [SerializeField] private PolygonCollider2D confinerCollider;
    public int PlayerCount { get; set; }


    public static GameManager GetInstance()
    {
        return instance;
    }


    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this.gameObject);
        }
        instance = this;
    }

    public PolygonCollider2D GetConfinerCollider()
    {
        return confinerCollider;
    }



}
