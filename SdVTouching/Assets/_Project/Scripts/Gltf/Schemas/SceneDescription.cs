using System.Collections.Generic;
using GLTFast.Newtonsoft.Schema;
using UnityEngine;

namespace SdVTouching.Gltf
{
    public class SceneDescription
    {
        public Root GltfRoot { get; set; }
        public Dictionary<uint, GameObject> NodeMapping { get; set; }
        public Extensions Extensions { get; set; }
    }
}