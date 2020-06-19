using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;
using LIM_TRAN_HOUACINE_NGUYEN;


public class BulletBehaviour : SimpleGameStateObserver
{
    Rigidbody m_Rigidbody;
    Transform m_Transform;


    protected override void Awake()
    {
        base.Awake();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Transform = GetComponent<Transform>();

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))Destroy(gameObject);
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
