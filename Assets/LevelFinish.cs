using System.Collections;
using UnityEngine;

public class LevelFinish : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayerMask;

    private PlayerController player;
    private Collider2D col;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((playerLayerMask.value & (1 << collision.gameObject.layer)) != 0)
        {
            player = collision.gameObject.GetComponent<PlayerController>();
            StartCoroutine(NextLevelCoroutine());
        }
    }

    private IEnumerator NextLevelCoroutine()
    {
        player.SetCanMove(false);
        player.StopPlayer();
        yield return new WaitForSeconds(2f);
        Loader.LoadNextLevel();
    }
}
