using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;

#region GameManager Events
public class GameMenuEvent : SDD.Events.Event
{
}
public class GamePlayEvent : SDD.Events.Event
{
}
public class GamePauseEvent : SDD.Events.Event
{
}
public class GameResumeEvent : SDD.Events.Event
{
}
public class GameOverEvent : SDD.Events.Event
{
}
public class GameVictoryEvent : SDD.Events.Event
{
}

public class GameStatisticsChangedEvent : SDD.Events.Event
{
	public float eBestScore { get; set; }
	public float eScore { get; set; }
	public int eNLives { get; set; }
    public int eLevel { get; set; }
}
#endregion

#region MenuManager Events
public class EscapeButtonClickedEvent : SDD.Events.Event
{
}
public class PlayButtonClickedEvent : SDD.Events.Event
{
}
public class ResumeButtonClickedEvent : SDD.Events.Event
{
}

public class TimerBeforePlayEvent : SDD.Events.Event
{
}

public class MainMenuButtonClickedEvent : SDD.Events.Event
{
}

public class QuitButtonClickedEvent : SDD.Events.Event
{ }

public class NextLevelButtonClickedEvent : SDD.Events.Event
{
}

public class LevelButtonClickedEvent : SDD.Events.Event
{
   public int levelIndex;
}


#endregion

#region Game Manager Additional Event

public class ReplaySameLevelEvent : SDD.Events.Event
{

}
public class GoToNextLevelEvent : SDD.Events.Event
{
}
#endregion

#region Score Event
public class ScoreItemEvent : SDD.Events.Event
{
	public IScore eScore;
}
#endregion

#region Player Events
public class PlayerHasBeenHitEvent : SDD.Events.Event
{
    public PlayerController ePlayerController;
}

public class PlayerHasReachedEndChunk : SDD.Events.Event
{

}
#endregion

#region enemy Events
public class EnemyHasBeenHitEvent : SDD.Events.Event
{

}
#endregion

#region LevelsManager Events
public class LevelHasBeenInstantiatedEvent : SDD.Events.Event
{
    public int eLevel;
}
#endregion

#region Chunk Events
public class ChunkTriggerDestroyed : SDD.Events.Event
{

}

#endregion
