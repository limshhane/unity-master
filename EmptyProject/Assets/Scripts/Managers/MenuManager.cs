
namespace LIM_TRAN_HOUACINE_NGUYEN
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
    using UnityEngine.EventSystems;
	using UnityEngine;
	using SDD.Events;
    using UnityEngine.UI;

    public class MenuManager : Manager<MenuManager>
	{

		[Header("MenuManager")]

		#region Panels
		[Header("Panels")]
		[SerializeField] GameObject m_PanelMainMenu;
		[SerializeField] GameObject m_PanelInGameMenu;
        [SerializeField] GameObject m_PanelVictory;
		[SerializeField] GameObject m_PanelGameOver;
        [SerializeField] GameObject m_PanelTimer;
        [SerializeField] GameObject m_PanelLevelMenu;

        List<GameObject> m_AllPanels;
        float timer;
		#endregion

		#region Events' subscription
		public override void SubscribeEvents()
		{
			base.SubscribeEvents();
            EventManager.Instance.AddListener<TimerBeforePlayEvent>(TimerBeforePlay);
            //Level
            EventManager.Instance.AddListener<GoToNextLevelEvent>(GoToNextLevel);
            //EventManager.Instance.AddListener<LevelButtonClickedEvent>(LevelButtonClicked);

        }

        public override void UnsubscribeEvents()
		{
			base.UnsubscribeEvents();
            EventManager.Instance.RemoveListener<TimerBeforePlayEvent>(TimerBeforePlay);

            //level
            EventManager.Instance.RemoveListener<GoToNextLevelEvent>(GoToNextLevel);
            //EventManager.Instance.RemoveListener<LevelButtonClickedEvent>(LevelButtonClicked);

        }
        #endregion

        #region Manager implementation
        protected override IEnumerator InitCoroutine()
		{
			yield break;
		}
		#endregion

		#region Monobehaviour lifecycle
		protected override void Awake()
		{
			base.Awake();
			RegisterPanels();
		}

		private void Update()
		{
			if (Input.GetButtonDown("Cancel") && GameManager.Instance.IsPlaying)
			{
				EscapeButtonHasBeenClicked();
			}
		}
		#endregion

		#region Panel Methods
		void RegisterPanels()
		{
			m_AllPanels = new List<GameObject>();
			m_AllPanels.Add(m_PanelMainMenu);
			m_AllPanels.Add(m_PanelInGameMenu);
			m_AllPanels.Add(m_PanelGameOver);
            m_AllPanels.Add(m_PanelVictory);
            m_AllPanels.Add(m_PanelTimer);
            m_AllPanels.Add(m_PanelLevelMenu);
        }

		void OpenPanel(GameObject panel)
		{
			foreach (var item in m_AllPanels)
				if (item) item.SetActive(item == panel);
		}
		#endregion

		#region UI OnClick Events
		public void EscapeButtonHasBeenClicked()
		{
			EventManager.Instance.Raise(new EscapeButtonClickedEvent());
		}

		public void PlayButtonHasBeenClicked()
		{

            EventManager.Instance.Raise(new PlayButtonClickedEvent());
		}

		public void ResumeButtonHasBeenClicked()
		{
 
            EventManager.Instance.Raise(new ResumeButtonClickedEvent());
		}

		public void MainMenuButtonHasBeenClicked()
		{
			EventManager.Instance.Raise(new MainMenuButtonClickedEvent());
		}

		public void QuitButtonHasBeenClicked()
		{
            
			EventManager.Instance.Raise(new QuitButtonClickedEvent());
		}

        #region Level
        public void LevelOneButtonHasBeenClick()
        {
            EventManager.Instance.Raise(new LevelButtonClickedEvent() { levelIndex = 0});
        }
        public void LevelTwoButtonHasBeenClick()
        {
            EventManager.Instance.Raise(new LevelButtonClickedEvent() { levelIndex = 1 });
        }
        public void LevelThreeButtonHasBeenClick()
        {
            EventManager.Instance.Raise(new LevelButtonClickedEvent() { levelIndex = 2 });
        }

        //public void LevelButtonClicked(LevelButtonClickedEvent e)
        //{
        //    Debug.Log("OUI LEVEL BUTTON");
        //    EventManager.Instance.Raise(new PlayButtonClickedEvent());
        //}
        #endregion

        public void TimerBeforePlay()
        {
            EventManager.Instance.Raise(new TimerBeforePlayEvent());
        }

        #endregion

        #region Callbacks to GameManager events
        public void LevelMenuButtonHasBeenClick()
        {
            OpenPanel(m_PanelLevelMenu);
            EventSystem.current.SetSelectedGameObject(null);
            Button button = GameObject.Find("Level1Button").GetComponent<Button>();
            EventSystem.current.SetSelectedGameObject(button.gameObject);
        }

        private void GoToNextLevel(GoToNextLevelEvent e)
        {
            //OpenPanel(null);
            EventManager.Instance.Raise(new TimerBeforePlayEvent());
        }

        protected override void GameMenu(GameMenuEvent e)
		{
			OpenPanel(m_PanelMainMenu);
            EventSystem.current.SetSelectedGameObject(null);
            Button button = GameObject.Find("PlayButton").GetComponent<Button>();
            EventSystem.current.SetSelectedGameObject(button.gameObject);
        }

		protected override void GamePlay(GamePlayEvent e)
		{
            EventManager.Instance.Raise(new TimerBeforePlayEvent());
        }

		protected override void GamePause(GamePauseEvent e)
		{
			OpenPanel(m_PanelInGameMenu);
            EventSystem.current.SetSelectedGameObject(null);
            Button button = GameObject.Find("ResumeButton").GetComponent<Button>();
            EventSystem.current.SetSelectedGameObject(button.gameObject);
        }

		protected override void GameResume(GameResumeEvent e)
		{
            EventManager.Instance.Raise(new TimerBeforePlayEvent());
        }

		protected override void GameOver(GameOverEvent e)
		{
			OpenPanel(m_PanelGameOver);
            EventSystem.current.SetSelectedGameObject(null);
            Button button = GameObject.Find("MainMenuButton").GetComponent<Button>();
            EventSystem.current.SetSelectedGameObject(button.gameObject);
        }

        protected override void GameVictory(GameVictoryEvent e)
        {
            OpenPanel(m_PanelVictory);
            EventSystem.current.SetSelectedGameObject(null);
            Button button = GameObject.Find("NextLevelButton").GetComponent<Button>();
            EventSystem.current.SetSelectedGameObject(button.gameObject);
        }

        public void NextLevelButtonHasBeenClicked()
        {
            EventManager.Instance.Raise(new NextLevelButtonClickedEvent());
        }


        protected void TimerBeforePlay(TimerBeforePlayEvent e)
        {
            OpenPanel(m_PanelTimer);
            StartCoroutine(StartCountdown());
        }

        IEnumerator StartCountdown()
        {

            //m_GameState = GameState.gameTimer;
            timer = GameManager.Instance.timerStart;
            while (timer > 0)
            {

                yield return new WaitForSecondsRealtime(1f); 
                timer--;
            }
            OpenPanel(null);


        }
        #endregion
    }

}
