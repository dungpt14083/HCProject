[System.Serializable]
public enum GameEventType
{
	NONE,

	// Navigation
	GO_HOME,
	GO_LEVELS,

	// level state
	RESTART_GAME,
	PAUSE_GAME,
	RESUME_GAME,
	FINISH_LEVEL,
	SKIP_LEVEL,
	NEXT_LEVEL,

	// timer 
	END_TIMER,
	START_TIMER,
	PAUSE_TIMER,
}