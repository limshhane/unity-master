using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;
using LIM_TRAN_HOUACINE_NGUYEN;

public class EnemyWallBehaviour : SimpleGameStateObserver
{
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] int timeBetweenEnemySpawn;
    [SerializeField] int maxEnemy;

    int enemyCounter;
    bool canSpawnEnemy;

    protected override void Awake()
    {
        base.Awake();
        transform.Rotate(new Vector3(1, 0, 0), -90);
        enemyCounter = 0;
        canSpawnEnemy = true;

    }


    private void Spawnenemy()
    {

        float randomX = Random.Range(-this.GetComponent<Renderer>().bounds.size.x/2 , this.GetComponent<Renderer>().bounds.size.x/2);
        float randomY = Random.Range(-this.GetComponent<Renderer>().bounds.size.y / 2, this.GetComponent<Renderer>().bounds.size.y / 2);
        Vector3 v = new Vector3(randomX, randomY, 0);
        GameObject enemy = Instantiate(enemyPrefab, this.transform.position+v, Quaternion.identity);
        enemy.transform.SetParent(this.transform);
    }

    private void Update()
    {
        if (!GameManager.Instance.IsPlaying) return;
        if (enemyCounter < maxEnemy && canSpawnEnemy)
        {
            Spawnenemy();
            enemyCounter++;
            StartCoroutine(SpawnEnemyCountdownCoroutine());
        }
    }

    private IEnumerator SpawnEnemyCountdownCoroutine()
    {
        canSpawnEnemy = false;
        yield return new WaitForSeconds(timeBetweenEnemySpawn);
        canSpawnEnemy = true;
    }

    #region Events subscription
    public override void SubscribeEvents()
    {
        base.SubscribeEvents();
        EventManager.Instance.AddListener<EnemyHasBeenHitEvent>(EnemyHasBeenHit);

        //Level
        EventManager.Instance.AddListener<GoToNextLevelEvent>(GoToNextLevel);

    }

    public override void UnsubscribeEvents()
    {
        base.UnsubscribeEvents();
        EventManager.Instance.RemoveListener<EnemyHasBeenHitEvent>(EnemyHasBeenHit);
        EventManager.Instance.RemoveListener<GoToNextLevelEvent>(GoToNextLevel);

    }
    #endregion


    protected override void GameOver(GameOverEvent e)
    {
        Destroy(gameObject);
    }

    protected override void GameMenu(GameMenuEvent e)
    {
        Destroy(gameObject);
    }

    private void EnemyHasBeenHit(EnemyHasBeenHitEvent e)
    {
        enemyCounter--;
    }

    private void GoToNextLevel(GoToNextLevelEvent e)
    {
        Destroy(gameObject);
    }
}
