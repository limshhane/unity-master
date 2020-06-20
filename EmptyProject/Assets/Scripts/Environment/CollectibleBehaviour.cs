using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;
using LIM_TRAN_HOUACINE_NGUYEN;

public class CollectibleBehaviour : SimpleGameStateObserver, IScore
{
    [SerializeField] int m_Score;
    public int Score { get { return m_Score; } }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.IsPlaying) return;
        transform.Rotate(new Vector3(15, 30, 45)*Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(name + " Collision with " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Player"))
        {
            EventManager.Instance.Raise(new ScoreItemEvent(){ eScore = this as IScore });
            Destroy(gameObject);
        }
    }

    #region Events subscription
    public override void SubscribeEvents()
    {
        base.SubscribeEvents();
        EventManager.Instance.AddListener<GoToNextLevelEvent>(GoToNextLevel);

    }

    public override void UnsubscribeEvents()
    {
        base.UnsubscribeEvents();
        EventManager.Instance.RemoveListener<GoToNextLevelEvent>(GoToNextLevel);
    }
    #endregion

    protected override void GameMenu(GameMenuEvent e)
    {
        Destroy(gameObject);
    }

    protected override void GameOver(GameOverEvent e)
    {
        Destroy(gameObject);
    }

    private void GoToNextLevel(GoToNextLevelEvent e)
    {
        Destroy(gameObject);
    }
}
