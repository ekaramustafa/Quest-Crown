using System.Collections;
using UnityEngine;

public class LevelFinish : MonoBehaviour
{

    private CoinManager coinManager;

    [SerializeField] private LayerMask playerLayerMask;

    private PlayerController player;
    private Collider2D col;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        col.enabled = false;
    }
    private void Start()
    {
        coinManager = CoinManager.GetInstance();
        coinManager.OnCoinCollected += OnCoinCollected;
    }

    private void OnCoinCollected(object sender, System.EventArgs e)
    {
        col.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((playerLayerMask.value & (1 << collision.gameObject.layer)) != 0)
        {
            if (col.enabled)
            {            
                player = collision.gameObject.GetComponent<PlayerController>();
                StartCoroutine(NextLevelCoroutine());
            }
            else
            {
                //Give some feedback to the player
            }

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
