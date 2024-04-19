using GLTFast.Newtonsoft.Schema;

namespace SdVTouching
{
    public class MpegMaterialHaptic
    {
        public string name;
        public Vibration[] vibration;
        public class Vibration
        {
            public TextureInfo texture;
            public string type;
        }
    }
}