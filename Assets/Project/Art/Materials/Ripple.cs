using UnityEngine;
using System.Collections.Generic;

public class RainCollisionSpawner : MonoBehaviour
{
	[Header("涟漪设置")]
	public GameObject ripplePrefab;      // 涟漪预制体
	public bool randomScale = false;      // 是否随机大小
	public Vector2 scaleRange = new Vector2(0.8f, 1.2f);

	[Header("性能限制")]
	public int maxRipplesPerFrame = 10;   // 每帧最多生成涟漪数（防止爆卡）
	public float minCollisionDistance = 0.3f; // 同一位置最小距离，避免叠加太密集

	private ParticleSystem particleSys;
	private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
	private Dictionary<Vector3, float> lastRippleTime = new Dictionary<Vector3, float>();

	void Start()
	{
		particleSys = GetComponent<ParticleSystem>();
	}

	void OnParticleCollision(GameObject other)
	{
		// 获取碰撞事件列表
		int numCollisionEvents = particleSys.GetCollisionEvents(other, collisionEvents);

		int spawned = 0;
		for (int i = 0; i < numCollisionEvents; i++)
		{
			if (spawned >= maxRipplesPerFrame) break;

			Vector3 hitPoint = collisionEvents[i].intersection;

			// 可选：避免同一位置频繁生成涟漪（防止密集爆炸）
			if (lastRippleTime.ContainsKey(hitPoint))
			{
				if (Time.time - lastRippleTime[hitPoint] < 0.2f)
					continue;
				else
					lastRippleTime[hitPoint] = Time.time;
			}
			else
			{
				lastRippleTime[hitPoint] = Time.time;
			}

			// 生成涟漪
			GameObject ripple = Instantiate(ripplePrefab, hitPoint, Quaternion.identity);

			// 可选：随机缩放
			if (randomScale)
			{
				float scale = Random.Range(scaleRange.x, scaleRange.y);
				ripple.transform.localScale = Vector3.one * scale;
			}

			spawned++;
		}

		// 清理字典中的旧记录，避免无限增长（每60帧清理一次）
		if (Time.frameCount % 60 == 0 && lastRippleTime.Count > 200)
		{
			CleanupOldEntries();
		}
	}

	void CleanupOldEntries()
	{
		List<Vector3> toRemove = new List<Vector3>();
		float now = Time.time;
		foreach (var kv in lastRippleTime)
		{
			if (now - kv.Value > 2.0f) // 2秒没更新的位置移除
				toRemove.Add(kv.Key);
		}
		foreach (var key in toRemove)
			lastRippleTime.Remove(key);
	}
}