# SD_V_Touching
Haptic texture experience system integrated with MPEG Scene Description Standard.

## Introduction
This project is a Unity project that provides a haptic texture experience system. The system is integrated with the MPEG Scene Description Standard. The system can load the glTF files and generate the haptic signals based on the contact position and the haptic texture information. 

## Hardware & Software
1. GeomagicTouch
2. Keyboard (When GeomagicTouch is not available)
3. Unity 2020.3.48f1

## Note
Add com.unity.cloud.gltfast & Newtonsoft Json in the package manager if not exist in the project.

## Settings
1. Select `App` object in the hierarchy.
2. `UrlRoot`: The folder of glTF files. This folder should be set under `StreamingAssets`.
3. `SceneDescriptionName`: The name of the basic scene description file.
4. `ExtensionsName`: The name of the extensions file.
5. `UseMock`: If true, the system will use the mock input & output instead of GeomagicTouch. The mock input is the keyboard input, and the mock output is the none.
6. `VibrationMagnification`: The magnification of the vibration. Please modify it before playing the scene.
7. `UseCodec`: If true, the system will encode & decode the runtime haptic signals.
8. `TactileSignalCache`: Determines the buffer of runtime haptic signals waiting for encoding. If `UseCodec` is false, this value will be ignored and set to 1.
9. `WaveformUpdateDump`: Determines the update interval of the waveform.

## Referenced Packages/Plugins
1. glTFast
2. Newtonsoft Json
3. 3D Systems Haptic Direct
4. UniTask
5. XCharts
6. MPEG Haptic Codec