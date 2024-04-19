using System.Collections.Generic;
using UnityEngine;

namespace SdVTouching.Gltf
{
    public class Extensions
    {
        public MpegMaterialHaptic[] MpegMaterialHaptics { get; set; }
        public Dictionary<uint, Texture2D> HapticTextures { get; set; }
        public MpegSceneInteractivity[] MpegSceneInteractivities { get; set; }
        // MPEG scene interactivity
        // - triggers
        // - trigger to trigger adapter
        // - actions
        // - behaviors
    }
}