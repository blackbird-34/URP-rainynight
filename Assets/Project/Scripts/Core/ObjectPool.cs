using UnityEngine;
using System.Collections.Generic;

namespace GameCore
{
	public class ObjectPool : MonoBehaviour
	{
		[SerializeField] private GameObject prefab;
		[SerializeField] private int initialSize = 20;

		private Queue<GameObject> pool = new Queue<GameObject>();

		private void Awake()
		{
			for (int i = 0; i < initialSize; i++)
			{
				CreateNew();
			}
		}

		private void CreateNew()
		{
			GameObject obj = Instantiate(prefab);
			obj.SetActive(false);
			pool.Enqueue(obj);
		}

		public GameObject Get()
		{
			if (pool.Count == 0)
			{
				CreateNew();
			}
			GameObject obj = pool.Dequeue();
			obj.SetActive(true);
			return obj;
		}

		public void Return(GameObject obj)
		{
			obj.SetActive(false);
			pool.Enqueue(obj);
		}
	}
}