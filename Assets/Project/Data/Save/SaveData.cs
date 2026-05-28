using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
	public int collectedCount;
	public int totalCount;
	public bool isGameCompleted;
	public float playerPosX, playerPosY, playerPosZ;
	public List<int> collectedIndexes = new List<int>();
}