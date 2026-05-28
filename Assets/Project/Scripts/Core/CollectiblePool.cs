using UnityEngine;
using System.Collections.Generic;
using GameCore;

public class CollectiblePool : MonoBehaviour
{
	[Header("预制体")]
	public GameObject collectiblePrefab;

	[Header("生成点列表（场景中各个收集物的初始位置）")]
	public List<Transform> spawnPoints;

	[Header("池设置")]
	public int initialSize = 20;        // 若spawnPoints数量多，池会自动扩容

	private Queue<GameObject> pool = new Queue<GameObject>();
	private Dictionary<GameObject, int> activeMap = new Dictionary<GameObject, int>(); // 记录激活物体及其索引

	private void Awake()
	{
		if (collectiblePrefab == null)
		{
			Debug.LogError("CollectiblePool: 未指定 collectiblePrefab！");
			return;
		}

		// 预生成收集物，数量不少于 spawnPoints 数量
		int totalNeeded = spawnPoints.Count;
		for (int i = 0; i < totalNeeded; i++)
		{
			CreateNew();
		}
	}

	private void CreateNew()
	{
		GameObject obj = Instantiate(collectiblePrefab);
		obj.SetActive(false);
		pool.Enqueue(obj);
	}

	/// <summary>
	/// 从池中获取一个收集物，设置到指定生成点并激活
	/// </summary>
	public GameObject Get(int index)
	{
		if (index < 0 || index >= spawnPoints.Count)
		{
			Debug.LogError($"索引 {index} 超出 spawnPoints 范围 (0-{spawnPoints.Count - 1})");
			return null;
		}

		// 池中无可用对象时扩容
		if (pool.Count == 0)
			CreateNew();

		GameObject obj = pool.Dequeue();
		obj.transform.position = spawnPoints[index].position;
		obj.transform.rotation = spawnPoints[index].rotation;
		obj.SetActive(true);

		// 重置收集物内部状态（必须保证 Collectible 脚本存在）
		Collectible col = obj.GetComponent<Collectible>();
		if (col != null)
		{
			col.ResetCollectible();
			col.SetPool(this);       // 让收集物知道自己的池子（以便收集时归还）
			col.Initialize(index);   // 传递索引
		}

		// 记录激活物体
		activeMap[obj] = index;
		return obj;
	}

	/// <summary>
	/// 将收集物归还到池中
	/// </summary>
	public void Return(GameObject obj)
	{
		if (obj == null) return;
		if (!activeMap.ContainsKey(obj))
		{
			// 可能已经归还过了，忽略
			return;
		}

		activeMap.Remove(obj);
		obj.SetActive(false);
		pool.Enqueue(obj);
	}

	/// <summary>
	/// 根据索引归还收集物（用于存档恢复时同步已收集状态）
	/// </summary>
	public void ReturnByIndex(int index)
	{
		GameObject target = null;
		foreach (var kv in activeMap)
		{
			if (kv.Value == index)
			{
				target = kv.Key;
				break;
			}
		}
		if (target != null)
			Return(target);
	}

	/// <summary>
	/// 重置所有收集物：回收所有已激活的，然后重新按生成点顺序全部生成
	/// </summary>
	public void ResetAllCollectibles()
	{
		// 回收所有激活的物体
		List<GameObject> activeList = new List<GameObject>(activeMap.Keys);
		foreach (var obj in activeList)
			Return(obj);

		activeMap.Clear();

		// 重新生成所有收集物
		for (int i = 0; i < spawnPoints.Count; i++)
			Get(i);
	}

	/// <summary>
	/// 获取当前还在激活状态（未收集）的物体数量
	/// </summary>
	public int GetActiveCount()
	{
		return activeMap.Count;
	}

	/// <summary>
	/// 获取总生成点数量（即场景中应有的收集物总数）
	/// </summary>
	public int GetTotalCount()
	{
		return spawnPoints.Count;
	}
}