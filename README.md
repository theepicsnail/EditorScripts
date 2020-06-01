# EditorScripts
Editor scripts for unity.

Currently this just has:
1. PackageManager.cs - installs vrc sdk2 and sdk3 and a few other useful packages I've seen (e.g. udon sharp)
2. TextureToMaterial.cs - bulk convert textures to materials (with alpha blending if the texture has transparency).

Check release tab for unitypackage
[1.0 UnityPackage](https://github.com/theepicsnail/EditorScripts/releases/download/1.0/export.unitypackage)

Git setup:
From Assets/ directory:
```bash
mkdir -p Snail/Editor/
cd Snail/Editor
git clone https://github.com/theepicsnail/EditorScripts.git .
```
