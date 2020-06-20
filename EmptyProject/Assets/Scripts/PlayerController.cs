using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LIM_TRAN_HOUACINE_NGUYEN;
using SDD.Events;

public class PlayerController : SimpleGameStateObserver, IScore
{
    Rigidbody m_Rigidbody;
    Renderer m_Renderer;
    Color m_Color;

    [Header("Deplacement")]
    [Tooltip("Vitesse en m.s-1")]
    [SerializeField] float m_MaxSpeed;
    [SerializeField] float m_ForwardSpeed;
    public float Speed { get { return m_ForwardSpeed; } }

    [SerializeField] float m_TranslationSpeed;
    [SerializeField] float m_Acceleration;
    [SerializeField] float m_TimeBetweenAcceleration;
    float timeBeforeAcceleration;

    [Header("Tire")]
    [SerializeField] private GameObject m_BulletPrefab;
    [SerializeField] private float m_ShootPeriod;
    bool canShoot;
    [SerializeField] private Transform m_BulletSpawnPoint;
    [SerializeField] private float m_LifeDuration;
    [SerializeField] private float m_BulletSpeed;
    bool canJump;

    [Header("Score")]
    [SerializeField] private int m_Score;
    public int Score { get { return m_Score; } }

    [Header("Invulnerability")]
    [SerializeField] private int invulnerabilityTime;

    [Header("Enemy")]
    [SerializeField] private float distanceFromPlayer;
    [SerializeField] public GameObject enemyWallPrefab;
    [SerializeField] public GameObject crosshairPrefab;
    private GameObject enemyWall;
    private GameObject crosshair;

    private GameObject follower;
    float timer;
    bool invincible;
    bool canScore;

    protected override void Awake()
    {
        base.Awake();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Renderer = GetComponent<Renderer>();
        m_Color = m_Renderer.material.color;
        timeBeforeAcceleration = m_TimeBetweenAcceleration;
        canJump = true;
        canShoot = true;
        invincible = false;
        canScore = true;
        enemyWall = Instantiate(enemyWallPrefab, new Vector3(0, enemyWallPrefab.GetComponent<Renderer>().bounds.size.y/2, distanceFromPlayer), Quaternion.identity);
        crosshair = Instantiate(crosshairPrefab, new Vector3(this.transform.position.x, m_BulletSpawnPoint.transform.position.y, enemyWall.transform.position.z-5),Quaternion.identity);
        crosshair.transform.SetParent(this.transform);
    }


    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.IsPlaying) return;
        if (m_Rigidbody.transform.position.y < 0)
        {
            EventManager.Instance.Raise(new GameOverEvent());
        }
        Debug.Log("Is playing : " + GameManager.Instance.IsPlaying);
        timeBeforeAcceleration -= Time.deltaTime;
        
        if (m_ForwardSpeed < m_MaxSpeed && timeBeforeAcceleration < 0)
        {

            m_ForwardSpeed += m_Acceleration;
            timeBeforeAcceleration = m_TimeBetweenAcceleration;
        }
        if (Input.GetButton("Fire1") && canShoot)
        {
            ShootBullet();
            StartCoroutine(ShootCoroutine());
        }
        if (canScore)
        {
            EventManager.Instance.Raise(new ScoreItemEvent() { eScore = this as IScore });
            StartCoroutine(ScoreCoroutine());
        }
    }

    IEnumerator ScoreCoroutine()
    {
        canScore = false;
        yield return new WaitForSeconds(1);
        canScore = true;
    }

    IEnumerator ShootCoroutine()
    {
        canShoot= false;
        yield return new WaitForSeconds(m_ShootPeriod);
        canShoot = true;
    }

    IEnumerator JumpCoroutine()
    {
        m_Rigidbody.AddForce(new Vector3(0, 10, 0), ForceMode.Impulse);
        canJump = false;
        yield return new WaitForSeconds(2f);
        canJump = true;
    }

    private void FixedUpdate()
    {

        // comportement dynamique cinétique (non-kinematic)
        // Time.fixedDeltaTime
        if (!GameManager.Instance.IsPlaying) return;
   

        //float vInput = Input.GetAxis("Vertical");
        float hInput = Input.GetAxis("Horizontal");
        Vector3 horizontalVect = transform.right * m_TranslationSpeed * hInput* Time.fixedDeltaTime;
        Vector3 forwardVect = transform.forward * m_ForwardSpeed * Time.fixedDeltaTime;
        m_Rigidbody.MovePosition(transform.position + forwardVect + horizontalVect);
        enemyWall.transform.position = new Vector3(0, enemyWall.GetComponent<Renderer>().bounds.size.y/2, this.transform.position.z+ distanceFromPlayer);
        if (canJump && (Input.GetKey(KeyCode.UpArrow) || Input.GetButton("Jump") ))
        {
            StartCoroutine(JumpCoroutine());
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (!invincible) {
                Debug.Log(name + " Collision with " + collision.gameObject.name);
                EventManager.Instance.Raise(new PlayerHasBeenHitEvent() { ePlayerController = this });
                Destroy(collision.gameObject);
                StartCoroutine(InvicibilityCoroutine());
            }
            else
            {
                Destroy(collision.gameObject);
            }
        }
    }

    IEnumerator InvicibilityCoroutine()
    {
        invincible = true;
        //m_Color.a = 1f;
        Color c = new Color(0, 255, 255);
        m_Renderer.material.color = c;
        yield return new WaitForSeconds(invulnerabilityTime);
        m_Color.a = 1f;
        m_Renderer.material.color = m_Color;
        invincible = false;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("EndChunk"))
        {
            EventManager.Instance.Raise(new PlayerHasReachedEndChunk() {});
            Destroy(collider.gameObject);
            EventManager.Instance.Raise(new ChunkTriggerDestroyed() { });
        }
    }

    void ShootBullet()
    {
        GameObject bulletGO = Instantiate(m_BulletPrefab, m_BulletSpawnPoint.position, Quaternion.identity);
        bulletGO.GetComponent<Rigidbody>().AddForce(m_BulletSpawnPoint.forward * (m_BulletSpeed+m_ForwardSpeed), ForceMode.VelocityChange);
        bulletGO.transform.SetParent(this.transform);
        Destroy(bulletGO, m_LifeDuration);
    }

    private void Reset()
    {
        canShoot = true;
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

