using UnityEngine;

using GameCore;
[DisallowMultipleComponent] // 禁止重复挂载
public class TourCamera : MonoBehaviour
{
	[Header("移动配置")]
	[Tooltip("基础移动速度（单位：米/秒）")]
	public float moveSpeed = 5.0f; // 建议默认值5，更贴合实际漫游体验
	[Tooltip("视角旋转速度（单位：度/秒）")]
	public float rotateSpeed = 90.0f;
	[Tooltip("Shift加速倍率")]
	public float shiftRate = 2.0f;

	[Header("旋转限制（可选扩展）")]
	[Tooltip("垂直旋转最小角度（防止低头过度）")]
	[Range(-89, 0)] public float minVerticalAngle = -60f;
	[Tooltip("垂直旋转最大角度（防止抬头过度）")]
	[Range(0, 89)] public float maxVerticalAngle = 60f;

	private Transform cameraTrans; // 相机Transform缓存
	private float currentVerticalAngle; // 记录垂直旋转角度（用于限制）
	private Vector3 moveDirection; // 最终移动方向向量

	/// <summary>
	/// 初始化（缓存组件，避免重复GetComponent）
	/// </summary>
	private void Start()
	{
		// 优先使用挂载对象的Transform，也可指定外部相机
		cameraTrans = gameObject.transform;
		// 初始化垂直旋转角度（基于相机初始欧拉角）
		currentVerticalAngle = cameraTrans.eulerAngles.x;
	}

	/// <summary>
	/// 每帧更新（处理输入和相机变换）
	/// </summary>
	private void Update()
	{
		// 仅在编辑器/运行时响应（可选：可移除限制，适配打包后）
		if (Application.isEditor || Application.isPlaying)
		{
			HandleInput(); // 处理键鼠输入
			UpdateCameraMovement(); // 更新相机位置
			UpdateCameraRotation(); // 更新相机旋转
		}
	}

	/// <summary>
	/// 处理所有输入事件（输入逻辑单独封装，便于维护）
	/// </summary>
	private void HandleInput()
	{
		// 1. 重置移动方向
		moveDirection = Vector3.zero;

		// 2. Shift加速逻辑（修复原脚本Bug：重复按下Shift导致速度无限倍）
		float currentSpeedMultiplier = Input.GetKey(KeyCode.LeftShift) ? shiftRate : 1f;
		float finalMoveSpeed = moveSpeed * currentSpeedMultiplier;

		// 3. WASD前后左右移动（基于相机自身朝向）
		if (Input.GetKey(KeyCode.S)) moveDirection += cameraTrans.forward;
		if (Input.GetKey(KeyCode.W)) moveDirection -= cameraTrans.forward;
		if (Input.GetKey(KeyCode.D)) moveDirection -= cameraTrans.right;
		if (Input.GetKey(KeyCode.A)) moveDirection += cameraTrans.right;

		// 4. QE垂直升降（世界坐标系Y轴，避免旋转后升降方向偏移）
		if (Input.GetKey(KeyCode.Q)) moveDirection += Vector3.down;
		if (Input.GetKey(KeyCode.E)) moveDirection += Vector3.up;

		// 5. 归一化方向向量（避免斜向移动速度翻倍）
		if (moveDirection.magnitude > 1f)
		{
			moveDirection.Normalize();
		}

		// 6. 应用速度倍率
		moveDirection *= finalMoveSpeed;
	}

	/// <summary>
	/// 更新相机位置（移动逻辑）
	/// </summary>
	private void UpdateCameraMovement()
	{
		// 基于世界坐标系移动（保证升降方向始终为世界Y轴）
		cameraTrans.Translate(moveDirection * Time.deltaTime, Space.World);
	}

	/// <summary>
	/// 更新相机旋转（右键视角控制）
	/// </summary>
	private void UpdateCameraRotation()
	{
		if (Input.GetMouseButton(1)) // 按住鼠标右键
		{
			// 1. 水平旋转（绕世界Y轴，左右拖动鼠标）
			float horizontalRotate = Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime;
			cameraTrans.RotateAround(cameraTrans.position, Vector3.up, horizontalRotate);

			// 2. 垂直旋转（绕相机自身X轴，上下拖动鼠标，带角度限制）
			float verticalRotate = -Input.GetAxis("Mouse Y") * rotateSpeed * Time.deltaTime;
			currentVerticalAngle += verticalRotate;
			// 限制垂直旋转角度，防止相机翻转
			currentVerticalAngle = Mathf.Clamp(currentVerticalAngle, minVerticalAngle, maxVerticalAngle);

			// 3. 应用垂直旋转（重置X轴欧拉角，避免万向节死锁）
			Vector3 currentEuler = cameraTrans.eulerAngles;
			cameraTrans.eulerAngles = new Vector3(currentVerticalAngle, currentEuler.y, currentEuler.z);
		}


	}

	private void OnEnable()
	{
		GameEvents.OnStateChanged += HandleStateChanged;
	}

	private void OnDisable()
	{
		GameEvents.OnStateChanged -= HandleStateChanged;
	}

	private void HandleStateChanged(GameState newState)
	{
		enabled = (newState == GameState.Playing);
	}
}


