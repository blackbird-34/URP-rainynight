using UnityEngine;
using GameCore;

public class Collectible : MonoBehaviour
{
	public AudioClip collectSound;
	private int myIndex = -1;
	private bool isCollected = false;
	private Collider myCollider;
	private CollectiblePool myPool;   // 记录所属池子

	void Awake()
	{
		myCollider = GetComponent<Collider>();
	}

	public void Initialize(int index)
	{
		myIndex = index;
	}

	public void SetPool(CollectiblePool pool)
	{
		myPool = pool;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (isCollected) return;
		if (!other.CompareTag("Player")) return;

		isCollected = true;

		// 播放音效（如有需要）
		if (collectSound != null)
			AudioManager.Instance?.PlaySound(collectSound);

		// 触发事件（传递索引）
		GameEvents.OnCollectibleCollected?.Invoke(myIndex);

		// 归还到池子
		if (myPool != null)
			myPool.Return(gameObject);
		else
			gameObject.SetActive(false); // 降级方案
	}

	public void ResetCollectible()
	{
		isCollected = false;
		if (myCollider != null)
			myCollider.enabled = true;
		// 不主动 SetActive(true)，由对象池的 Get 方法激活
	}
}