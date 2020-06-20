using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LIM_TRAN_HOUACINE_NGUYEN;
using SDD.Events;

public class EnemyBehaviour : SimpleGameStateObserver, IScore
{
    [Header("Score")]
    [SerializeField] private int m_Score;
    public int Score { get { return m_Score; } }
    [Header("Time Alive")]
    [SerializeField] private float m_Lifetime;

    [Header("Tire")]
    [SerializeField] private GameObject m_BulletPrefab;
    [SerializeField] private float m_ShootPeriod;
    private float m_NextShootTime;
    [SerializeField] private Transform m_BulletSpawnPoint;
    [SerializeField] private float m_LifeDuration;
    [SerializeField] private float m_BulletSpeed;

    Rigidbody m_Rigidbody;
    Transform m_Transform;
    bool alive;
    bool canShoot;

    protected override void Awake()
    {
        base.Awake();

        m_Rigidbody = GetComponent<Rigidbody>();
        m_Transform = GetComponent<Transform>();
        alive = true;
        canShoot = true;
        StartCoroutine(EnemyLifeCountdownCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        if (!alive)
        {
            Destroy(this.gameObject);
            EventManager.Instance.Raise(new EnemyHasBeenHitEvent() { });
        }
        if (canShoot)
        {
            ShootBullet();
            StartCoroutine(ShootCoroutine());
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            EventManager.Instance.Raise(new EnemyHasBeenHitEvent() { });
            EventManager.Instance.Raise(new ScoreItemEvent() { eScore = this as IScore });
            Destroy(this.gameObject);
        }
    }

    void ShootBullet()
    {
        GameObject bulletGO = Instantiate(m_BulletPrefab, m_BulletSpawnPoint.position, Quaternion.identity);

        Destroy(bulletGO, m_LifeDuration);
    }

    IEnumerator ShootCoroutine()
    {
        canShoot = false;
        yield return new WaitForSeconds(m_ShootPeriod);
        canShoot = true;
    }


    private IEnumerator EnemyLifeCountdownCoroutine()
    {
        yield return new WaitForSeconds(m_Lifetime);
        alive = false;
    }
}
