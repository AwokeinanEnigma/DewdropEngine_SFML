using DewDrop.Utilities;

namespace DewDrop
{
    /// <summary>
    /// The engine is able to have certain aspects of it manipulated to fit your needs.
    /// This struct contains values used by the engine to determine certain aspects of itself.
    /// Such as the size of the screen, and if VSync should be enabled.
    /// </summary>
    internal struct EngineConfigurationData
    {
        public Vector2 screen_size;

        public float screen_width
        {
            get => screen_size.x;
        }

        public float screen_height
        {
            get => screen_size.y;
        }

        
    }
}
