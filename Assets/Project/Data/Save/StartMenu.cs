using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
	public void NewGame()
	{
		// 删除旧存档
		if (SaveManager.Instance != null)
			SaveManager.Instance.DeleteSave();
		else
			Debug.LogWarning("SaveManager 不存在，无法删除存档");

		// 加载主场景（请替换为你的主场景名称）
		SceneManager.LoadScene("1.0");
	}

	public void ContinueGame()
	{
		// 直接加载主场景，UIManager 会自动读取存档
		SceneManager.LoadScene("1.0");
	}

	public void QuitGame()
	{
		Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
	}
}