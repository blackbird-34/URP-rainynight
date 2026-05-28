## 《雨后街道》——URP 实时渲染 + 第一人称收集游戏

## 项目简介

《雨后街道》是一款基于 Unity URP 的第一人称收集游戏。项目从零搭建，专注于**写实雨夜图形渲染**、**动态天气效果**以及**完整游戏系统架构**。玩家在湿润的街道上移动，收集散布的光点，通过存档系统保存进度，最终达成胜利。


## 核心功能

- **第一人称控制器**：WASD 移动 + 鼠标视角。
- **收集玩法**：触碰光点 → 播放音效 → 计数+1 → 光点消失 → UI 实时更新。
- **存档系统**：JSON 持久化，保存收集进度、已收集光点索引、玩家位置、胜利状态。
- **状态机管理**：Menu / Playing / Completed 三态，控制 UI 显隐、玩家输入、光标锁定。
- **对象池技术**：收集物复用，避免 GC 压力。
- **事件驱动架构**：全局事件中心解耦 UI / 音频 / 存档模块。
- **URP 渲染**：动态天空盒、湿路面反射、Bloom 后处理、雨丝粒子与落地涟漪。
- **UI 系统**：开始界面（新游戏/继续游戏）、实时计数、结束画面、ESC 快捷退出。


## 玩法演示
 [演示视频](https://www.bilibili.com/video/BV1xx... )（待补充）


## 技术架构

### 核心模块

| 模块 | 实现 |
|------|------|
| **渲染** | URP、Shader Graph（天空盒）、Particle System（雨/涟漪）、Post-Processing（Bloom/Color Grading） |
| **游戏逻辑** | C# + 事件中心 + 状态机 + 对象池 |
| **数据持久化** | JsonUtility + 本地文件（Application.persistentDataPath） |
| **音频** | 单例 AudioManager + PlayOneShot |
| **编辑器工具** | ProBuilder、Git 版本控制 |



## 开发环境与工具

| 工具 | 用途 |
|------|------|
| Unity 2022.3 LTS | 游戏引擎 |
| Visual Studio 2022 | 代码编辑器 |
| Git / GitHub | 版本控制 |
| ProBuilder | 场景白模搭建 |
| Shader Graph | 自定义天空盒 |
| Blender | 简单模型调整 |


## 项目结构

```
Assets/
├── _Project/
│   ├── Art/               # 材质、纹理、模型
│   ├── Audio/             # 背景音乐、音效
│   ├── Prefabs/           # 收集物、特效预制体
│   ├── Scenes/            # StartScene, MainScene
│   ├── Scripts/
│   │   ├── Core/          # 事件中心、状态机、对象池
│   │   ├── Managers/      # UIManager, AudioManager, SaveManager
│   │   ├── Player/        # FirstPersonController
│   │   ├── Collectibles/  # Collectible 脚本
│   │   └── Environment/   # 雨滴涟漪生成
│   └── Settings/          # URP 配置文件
├── Packages/              # 项目依赖
└── ProjectSettings/       # 工程设置
```


## 更新

- **v1.0**（2025.04）：初始版本，完成场景搭建、雨滴灯光渲染。
- **v2.0**（2025.05）：初始版本，完成核心玩法、存档、规范代码。


<img width="2558" height="1599" alt="屏幕截图 2026-05-28 195614" src="https://github.com/user-attachments/assets/133ccf46-e17d-417a-b3e5-364acf18d3de" />
<img width="2559" height="1599" alt="屏幕截图 2026-05-28 195724" src="https://github.com/user-attachments/assets/3ef7d8f2-79a0-447b-a622-f4bf7a40ecdc" />
<img width="2559" height="1599" alt="屏幕截图 2026-05-28 195918" src="https://github.com/user-attachments/assets/3e3eee75-3f99-42d0-9558-f47b0ce69f18" />
<img width="1512" height="805" alt="屏幕截图 2026-05-07 050511" src="https://github.com/user-attachments/assets/eea26421-c017-45d4-9378-cf8e2b4a4733" />




