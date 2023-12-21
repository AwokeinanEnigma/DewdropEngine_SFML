using DewDrop;
using DewDrop.Utilities;
using DewDrop.Scenes;
using DewDrop.UserInput;
using DewDrop.Wren;
using Prototype;
using Prototype.Scenes;
using System.Reflection;

namespace RotatingHelloWorldSfmlDotNetCoreCSharp; 

class Program
{
	[STAThread]
	static void Main(string[] args)
	{
		Engine.Initialize(new EngineConfigurationData()
		{
			Application = new EngineConfigurationData.ApplicationData() {
				Name = "Prototype",
				Version = "0.0.1",	
				Developer = "Enigma",
				ConfigPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/DewDrop/Prototype/config.nbt"
			},
			// recommended resolutions:
			// 320x180
			// 480x270
			// unrecommended resolutions:
			// 640x360
			// 960x540
					
			EnableImGui = true,
			ScreenSize = new Vector2(320, 180),
			Fullscreen = false,
			VSync = true,
			DebugMode = true,
			DefaultBufferScale = 4,
			StartScene = new SpriteBatchScene(),
			WrenTypes = WrenManager.FindWrenTypes(typeof(Program).Assembly).ToList()
		});
	}
}