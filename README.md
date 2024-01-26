# Ghibli-Style Grass Shader Package

## Description
This package provides a stylized grass shader designed to mimic the distinctive Ghibli animation style. It's perfect for game developers and animators looking to incorporate a touch of Ghibli's enchanting visuals into their Unity projects. The package includes a set of prefabs, scenes, shadergraphs, and scripts specifically tailored to create stunning, stylized grass animations.

## Prerequisites
- Unity 2022.3 or later
- Universal Render Pipeline (URP)

Ensure you have the minimum required version of Unity installed, as this package utilizes features specific to Unity 2022.3 and above, and more specifically the object bounds in the shader. You can always adapt it to lower versions, but the calculations for the bounds of the object should then be done CPU side.

## Installation

### Step 1: Clone or Download the Package
- Download the package through the package manager: Window >Package Manager > add package from git URL
- Alternatively you can clone this repository into your Unity project's packages directory.
```bash
git clone https://github.com/yourusername/ghibli-style-grass-shader.git
```

##Usage
Copy the Showcase scene locate under the Stylized-grass > Scenes in your assets folder as it is read-only when importing the package
As the package is read only, simply copy the elements you need into your assets (you may need to reassign materials)
- The customLOD script is as the name suggests, it defines a custom LOD system. There is a prefab "custom LOD group" containing 3 different LODs. Note that each LOD requires a seperate material. On each material, you need to specify the previous transition and the next transition. Those represent the percentage of visible bounds (depending on the camera's position relative to the object) at which it will switch to previous/next LOD rendere. Essentially, the fade mechanism is done on the GPU, and the CPU takes care of disabling, enabling the renderers. As such, the "next transition" on the material should match with the same parameter on the script. See below for an example:
![image](https://github.com/Chiasera/Stylized-grass/assets/70693638/6daf00b1-f33e-4111-ba72-1a74b1f94dd1)
![image](https://github.com/Chiasera/Stylized-grass/assets/70693638/bc9b7ae8-9e14-4201-8d16-8399e1349ffe)

Have fun :)

