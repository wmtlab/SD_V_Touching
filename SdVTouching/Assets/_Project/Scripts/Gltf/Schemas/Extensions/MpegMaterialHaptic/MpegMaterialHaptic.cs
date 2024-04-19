using GLTFast.Newtonsoft.Schema;

namespace SdVTouching.Gltf
{
    public class MpegMaterialHaptic
    {
        public string Name { get; set; }
        public Vibration[] Vibrations { get; set; }
        public class Vibration
        {
            public TextureInfo Texture { get; set; }
            public string Type { get; set; }
        }
    }
}