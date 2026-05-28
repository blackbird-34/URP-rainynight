using System;

namespace GameCore
{
	public static class GameEvents
	{
		// 收集物被收集，参数：收集物索引
		public static Action<int> OnCollectibleCollected;

		// 游戏完成
		public static Action OnGameCompleted;

		// 游戏重新开始（新游戏）
		public static Action OnGameRestarted;

		// 继续游戏（加载存档）
		public static Action OnGameContinued;

		// 游戏状态改变
		public static Action<GameState> OnStateChanged;
	}

	public enum GameState
	{
		Menu,       // 开始界面
		Playing,    // 游戏中
		Completed   // 游戏完成
	}
}