using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
	public static SaveManager Instance;

	private string saveFilePath;

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
			// 存档路径：Application.persistentDataPath + "/savegame.json"
			saveFilePath = Path.Combine(Application.persistentDataPath, "savegame.json");
		}
		else
		{
			Destroy(gameObject);
		}
	}

	/// <summary>
	/// 保存游戏进度
	/// </summary>
	public void SaveGame(SaveData data)
	{
		try
		{
			string json = JsonUtility.ToJson(data, true); // true 使 JSON 可读
			File.WriteAllText(saveFilePath, json);
			Debug.Log("游戏已保存至: " + saveFilePath);
		}
		catch (System.Exception e)
		{
			Debug.LogError("保存失败: " + e.Message);
		}
	}

	/// <summary>
	/// 加载游戏进度，如果没有存档则返回 null
	/// </summary>
	public SaveData LoadGame()
	{
		if (File.Exists(saveFilePath))
		{
			try
			{
				string json = File.ReadAllText(saveFilePath);
				SaveData data = JsonUtility.FromJson<SaveData>(json);
				Debug.Log("加载存档成功");
				return data;
			}
			catch (System.Exception e)
			{
				Debug.LogError("加载失败: " + e.Message);
				return null;
			}
		}
		else
		{
			Debug.Log("没有找到存档文件");
			return null;
		}
	}

	/// <summary>
	/// 删除存档（用于新游戏）
	/// </summary>
	public void DeleteSave()
	{
		if (File.Exists(saveFilePath))
		{
			File.Delete(saveFilePath);
			Debug.Log("存档已删除");
		}
	}
}