using UnityEngine;
using TMPro;
using System.Collections.Generic;
using GameCore;

public class UIManager : MonoBehaviour
{
	public static UIManager Instance { get; private set; }

	[Header("UI 面板")]
	[SerializeField] private GameObject startPanel;
	[SerializeField] private GameObject endPanel;
	[SerializeField] private TextMeshProUGUI countText;

	[Header("收集设置")]
	[SerializeField] private int totalCount = 4;   // 实际由对象池的 spawnPoints 数量决定，这里仅作备用

	[Header("玩家")]
	[SerializeField] private Transform playerTransform;
	[SerializeField] private MonoBehaviour playerController;   // FirstPersonController

	[Header("收集物对象池")]
	[SerializeField] private CollectiblePool collectiblePool;   // 必须拖入

	private HashSet<int> collectedIndexSet = new HashSet<int>();
	private int currentCount = 0;
	private bool isGameCompleted = false;
	private Vector3 playerStartPosition;

	// ---------------------------------------------------------------------
	// Unity 生命周期
	// ---------------------------------------------------------------------
	private void Awake()
	{
		if (Instance == null) Instance = this;
		else Destroy(gameObject);

		playerStartPosition = playerTransform.position;
	}

	private void OnEnable()
	{
		GameEvents.OnCollectibleCollected += HandleCollectibleCollected;
		GameEvents.OnStateChanged += HandleStateChanged;
	}

	private void OnDisable()
	{
		GameEvents.OnCollectibleCollected -= HandleCollectibleCollected;
		GameEvents.OnStateChanged -= HandleStateChanged;
	}

	private void Start()
	{
		// 确保对象池已生成所有收集物（在 Awake 中已生成）
		if (collectiblePool == null)
		{
			Debug.LogError("UIManager: 未指定 CollectiblePool！");
			return;
		}

		// 从对象池获取总收集数量（根据生成点数量）
		totalCount = collectiblePool.GetTotalCount();
		UpdateCountUI();

		// 初始状态由 GameStateManager 决定，UIManager 只响应状态变化
		// 手动初始化一次界面（防止状态事件未触发）
		HandleStateChanged(GameState.Menu);
	}

	// ---------------------------------------------------------------------
	// 事件响应
	// ---------------------------------------------------------------------
	private void HandleCollectibleCollected(int index)
	{
		if (isGameCompleted) return;
		if (collectedIndexSet.Contains(index)) return;

		collectedIndexSet.Add(index);
		currentCount++;
		UpdateCountUI();
		SaveCurrentProgress();

		// 对象池中的收集物已经在 OnTriggerEnter 里自动归还了，这里不需要额外操作

		if (currentCount >= totalCount)
		{
			GameComplete();
		}
	}

	private void HandleStateChanged(GameState newState)
	{
		switch (newState)
		{
			case GameState.Menu:
				startPanel.SetActive(true);
				endPanel.SetActive(false);
				if (playerController != null)
					playerController.enabled = false;
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
				break;

			case GameState.Playing:
				startPanel.SetActive(false);
				endPanel.SetActive(false);
				if (playerController != null)
					playerController.enabled = true;
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
				break;

			case GameState.Completed:
				endPanel.SetActive(true);
				startPanel.SetActive(false);
				if (playerController != null)
					playerController.enabled = false;
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
				break;
		}
	}

	// ---------------------------------------------------------------------
	// 游戏流程控制（供按钮调用）
	// ---------------------------------------------------------------------
	public void StartNewGame()
	{
		SaveManager.Instance?.DeleteSave();
		ResetGameState();            // 重置内存状态并清空对象池
		GameStateManager.Instance?.ChangeState(GameState.Playing);
	}

	public void ContinueGame()
	{
		SaveData data = SaveManager.Instance?.LoadGame();
		if (data != null && !data.isGameCompleted)
		{
			// 恢复计数
			currentCount = data.collectedCount;
			totalCount = data.totalCount;
			UpdateCountUI();

			// 恢复玩家位置
			playerTransform.position = new Vector3(data.playerPosX, data.playerPosY, data.playerPosZ);

			// 恢复已收集索引集合
			collectedIndexSet.Clear();
			foreach (int idx in data.collectedIndexes)
				collectedIndexSet.Add(idx);

			// 重置对象池（所有收集物重新出现）
			collectiblePool.ResetAllCollectibles();

			// 根据 collectedIndexSet 将已收集的物体归还到池中（隐藏）
			foreach (int idx in collectedIndexSet)
				collectiblePool.ReturnByIndex(idx);

			// 切换到游戏中状态
			GameStateManager.Instance?.ChangeState(GameState.Playing);
		}
		else
		{
			StartNewGame();
		}
	}

	public void RestartGame()
	{
		StartNewGame();   // 内部已包含重置和状态切换
	}

	// ---------------------------------------------------------------------
	// 内部逻辑
	// ---------------------------------------------------------------------
	private void ResetGameState()
	{
		// 重置内存数据
		collectedIndexSet.Clear();
		currentCount = 0;
		isGameCompleted = false;
		UpdateCountUI();

		// 重置对象池（所有收集物重新生成并激活）
		collectiblePool.ResetAllCollectibles();

		// 重置玩家位置
		playerTransform.position = playerStartPosition;

		// 如果有额外需要重置的模块，可在此添加
	}

	private void GameComplete()
	{
		if (isGameCompleted) return;
		isGameCompleted = true;
		SaveCurrentProgress();

		// 切换到完成状态（UI 由状态机自动处理）
		GameStateManager.Instance?.ChangeState(GameState.Completed);
	}

	private void UpdateCountUI()
	{
		if (countText != null)
			countText.text = $"{currentCount} / {totalCount}";
	}

	private void SaveCurrentProgress()
	{
		if (SaveManager.Instance == null) return;

		SaveData data = new SaveData();
		data.collectedCount = currentCount;
		data.totalCount = totalCount;
		data.isGameCompleted = isGameCompleted;

		data.playerPosX = playerTransform.position.x;
		data.playerPosY = playerTransform.position.y;
		data.playerPosZ = playerTransform.position.z;

		data.collectedIndexes.Clear();
		foreach (int idx in collectedIndexSet)
			data.collectedIndexes.Add(idx);

		SaveManager.Instance.SaveGame(data);
	}
}