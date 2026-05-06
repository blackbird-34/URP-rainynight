雨夜街道场景

在Unity URP管线中独立完成一个“雨后夜晚街道”的3D场景，综合运用了程序化建模、动态天空盒、多光源照明、粒子系统、后处理特效及反射探头等技术，营造出湿冷、霓虹点缀的雨天氛围。项目用于技术美术实习作品集。

场景搭建：使用ProBuilder快速搭建街道、路灯等基础模型。

动态天空：通过Shader Graph实现多云层流动、冷暖渐变的阴雨天空盒。

灯光与后处理：冷色主光+暖色路灯+霓虹自发光，配合Bloom、Color Grading强化夜景氛围。

积水反射：高光材质+法线涟漪，结合实时Reflection Probe实现路面反射。

雨丝粒子：粒子系统实现下落雨丝，碰撞产生水花，Stretched Billboard拉伸纹理增强真实感。

技术栈：Unity 2022 LTS / URP / Shader Graph / ProBuilder / Particle System / Post-processing
<img width="1512" height="805" alt="屏幕截图 2026-05-07 050511" src="https://github.com/user-attachments/assets/3ab08442-62c1-42bf-b43d-c6f58ddc15e6" />
