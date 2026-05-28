using UnityEngine;
using GameCore;

public class GameStateManager : MonoBehaviour
{
	public static GameStateManager Instance { get; private set; }
	public GameState CurrentState { get; private set; } = GameState.Menu;

	private void Awake()
	{
		if (Instance == null) Instance = this;
		else Destroy(gameObject);
	}

	private void Start()
	{
		ChangeState(GameState.Menu);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			QuitGame();
		}
	}

	public void ChangeState(GameState newState)
	{
		if (CurrentState == newState) return;
		CurrentState = newState;
		Debug.Log($"游戏状态切换为: {newState}");
		GameEvents.OnStateChanged?.Invoke(CurrentState);
	}

	public void StartGame() => ChangeState(GameState.Playing);
	public void CompleteGame() => ChangeState(GameState.Completed);
	public void BackToMenu() => ChangeState(GameState.Menu);

	private void QuitGame()
	{
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}
}