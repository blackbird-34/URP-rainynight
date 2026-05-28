using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public static AudioManager Instance;
	private AudioSource audioSource;

	void Awake()
	{
		// 实现单例模式，确保全局只有一个实例
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject); // 场景切换时不被销毁
			audioSource = GetComponent<AudioSource>();
		}
		else
		{
			Destroy(gameObject);
		}
	}

	// 外部通过调用此方法来播放音效
	public void PlaySound(AudioClip clip, float volume = 1.0f)
	{
		if (clip != null)
		{
			audioSource.PlayOneShot(clip, volume);
		}
	}
}