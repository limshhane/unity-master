namespace LIM_TRAN_HOUACINE_NGUYEN
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using System.Linq;
	using SDD.Events;

    public class LevelManager : Manager<LevelManager>
    {
        #region All variable
        private bool PlayerInNewChunk = true;
        //Compteur de chunk traversé par le player
        private int PlayerInChunk = 0;

        #region Chunk & LevelVariable
        [Header("Management des chunks")]

        [SerializeField] public GameObject firstSpawnPos;
        [SerializeField] public Transform actualChunkPos;
        [SerializeField] private Transform nextChunkPos;
        [SerializeField] private GameObject nextChunkTriggerPrefab;
        #endregion

        #region Player Variable
        [Header("Player")]
        [SerializeField] public GameObject playerPrefab;
        #endregion

        #region Level Variable
        [Header("Level")]
        [SerializeField] GameObject[] m_LevelsPrefabs;
        private int m_CurrentLevelIndex=0;
        private GameObject m_CurrentLevelGO;
        private GroundObject m_CurrentLevel;
        public GroundObject CurrentLevel { get { return m_CurrentLevel; } }
        #endregion

        //[SerializeField] private Transform PlayerSpawnPoint;

        private GameObject newPlayer;
        private List<GameObject> chunkList = new List<GameObject>();
        private Transform PlayerPos;
        private GameObject chunkTrigger;
        #endregion

        private void ResetItems()
        {

            int levelIndex = Mathf.Max(m_CurrentLevelIndex, 0) % m_LevelsPrefabs.Length;
            m_CurrentLevelGO = m_LevelsPrefabs[levelIndex];
            while (chunkList.Count > 0)
            {
                Destroy(chunkList[chunkList.Count - 1]);
                chunkList.RemoveAt(chunkList.Count - 1);
            }

            if (chunkTrigger != null) Destroy(chunkTrigger);
            resetChunkPosition();
        }

        private void Reset()
        {
            ResetItems();
            newPlayer = Instantiate(playerPrefab);
            PlayerPos = newPlayer.transform;
            for (int i = 0; i < 10; i++)
            {
                GameObject newChunk = Instantiate(m_CurrentLevelGO, nextChunkPos.position, Quaternion.identity);
                chunkList.Add(newChunk);
                UpdateActualChunkPos(nextChunkPos);
                UpdateNextChunkPos();
            }
            chunkTrigger = createChunkTrigger();
            PlayerInChunk = 2;
            EventManager.Instance.Raise(new LevelHasBeenInstantiatedEvent() {eLevel=m_CurrentLevelIndex });
        }

        #region Manager implementation
        protected override IEnumerator InitCoroutine()
        {
            yield break;
        }
        #endregion

        #region Chunk Managment
        private GameObject createChunkTrigger()
        {
            GameObject chunk = Instantiate(nextChunkTriggerPrefab, chunkList[5].transform.position, Quaternion.identity);

            Vector3 chunkCollider = chunk.GetComponent<BoxCollider>().size;
            chunkCollider.x = m_CurrentLevelGO.GetComponent<Renderer>().bounds.size.x * 10;
            chunkCollider.y = 500;
            chunkCollider.z = m_CurrentLevelGO.GetComponent<Renderer>().bounds.size.z;
            chunk.transform.localScale = chunkCollider;
            return chunk;
        }

        private void resetChunkPosition()
        {
            this.actualChunkPos.position = firstSpawnPos.transform.position;
            this.nextChunkPos.position = firstSpawnPos.transform.position;
        }
        private void UpdateNextChunkPos()
        {
            this.nextChunkPos.position = new Vector3(0, 0, nextChunkPos.position.z + m_CurrentLevelGO.GetComponent<Renderer>().bounds.size.z);
            //return nextChunkPos;
        }

        private void UpdateActualChunkPos(Transform nextChunkPos)
        {
            this.actualChunkPos.position = new Vector3(nextChunkPos.position.x, nextChunkPos.position.y, nextChunkPos.position.z);
            //return actualChunkPos;
        }

        private void addNewChunkIn()
        {
            GameObject newChunk = Instantiate(m_CurrentLevelGO, nextChunkPos.position, Quaternion.identity);
            chunkList.Add(newChunk);
            UpdateActualChunkPos(nextChunkPos);
            UpdateNextChunkPos();
            //enemyWall.transform.position = nextChunkPos.position+ new Vector3(0,0,2*chunkPrefab.GetComponent<Renderer>().bounds.size.z);
        }

        private void DeleteLastChunk()
        {
            Destroy(chunkList[0]);
            chunkList.RemoveAt(0);
        }

        private void PlayerHasReachedEndChunk(PlayerHasReachedEndChunk e)
        {
            if (chunkList.Count >= 10)
            {
                Debug.Log("Here");
                addNewChunkIn();
                DeleteLastChunk();
                Debug.Log(PlayerInNewChunk);
            }

        }

        private void ChunkTriggerDestroyed(ChunkTriggerDestroyed e)
        {
            chunkTrigger = createChunkTrigger();
        }
        #endregion

        #region Events sub
        public override void SubscribeEvents()
        {
            base.SubscribeEvents();
            EventManager.Instance.AddListener<PlayerHasReachedEndChunk>(PlayerHasReachedEndChunk);
            EventManager.Instance.AddListener<ChunkTriggerDestroyed>(ChunkTriggerDestroyed);
            //Level
            EventManager.Instance.AddListener<GoToNextLevelEvent>(GoToNextLevel);
            EventManager.Instance.AddListener<LevelButtonClickedEvent>(LevelButtonClicked);
        }

        public override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
            EventManager.Instance.RemoveListener<PlayerHasReachedEndChunk>(PlayerHasReachedEndChunk);
            EventManager.Instance.RemoveListener<ChunkTriggerDestroyed>(ChunkTriggerDestroyed);
            EventManager.Instance.RemoveListener<GoToNextLevelEvent>(GoToNextLevel);
            EventManager.Instance.RemoveListener<LevelButtonClickedEvent>(LevelButtonClicked);
        }
        #endregion

        #region Event callback
        protected override void GamePlay(GamePlayEvent e)
        {
            Reset();
        }

        protected override void GameMenu(GameMenuEvent e)
        {
            ResetItems();
        }

        private void LevelButtonClicked(LevelButtonClickedEvent e)
        {
            //Debug.Log("OUI LEVEL BUTTON");
            m_CurrentLevelIndex = e.levelIndex;
        }
        

        public void GoToNextLevel(GoToNextLevelEvent e)
        {
            if (m_CurrentLevelIndex < m_LevelsPrefabs.Length-1) m_CurrentLevelIndex++;
            Reset();
        }
        #endregion

    }


}