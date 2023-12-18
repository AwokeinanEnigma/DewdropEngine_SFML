#region

using DewDrop.Resources;
using DewDrop.UserInput;
using DewDrop.Utilities;
using fNbt;
using ImGuiNET;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

#endregion

namespace DewDrop;

// this handles the graphics part of the engine, such as rendering and the window
public static partial class Engine {
	#region Properties

    /// <summary>
    ///     The main Render Window the game is using.
    /// </summary>
    public static RenderWindow Window { get; private set; }

    /// <summary>
    ///     The Render Texture that the game is rendering.
    /// </summary>
    public static RenderTexture RenderTexture { get; private set; }

	public static int FrameBufferScale { get; set; } = 5;

	public static long Frame { get; private set; }

	#endregion

	#region Fields

	// handles the window
	// like, closin' disposin' and drawin'
	// that's supposed to rhyme
	// see https://www.sfml-dev.org/documentation/2.5.1/classsf_1_1RenderWindow.php for more info 

	// this is easier to explain
	// basically, everything is rendered to this
	// hence the name, render texture
	// what we can do with this is pass it to the RenderWindow so the RenderWindow can render it
	// we can also apply effects to the whole screen using it
	// see https://www.sfml-dev.org/documentation/2.5.1/classsf_1_1RenderTexture.php for more info

	// hard to explain but basically when rendering to a render target, this defines settings for rendering to said render target
	// like blend mode, shader, and other stuff
	public static RenderStates frameBufferState;

	// i'm tired of explaining
	// https://www.sfml-dev.org/tutorials/2.5/graphics-vertex-array.php
	// it defines a shape, dude.
	public static VertexArray frameBufferVertexArray;

	// size of the screen
	public static Vector2 ScreenSize { get; private set; }
	public static Vector2 HalfScreenSize { get; private set; }

	#endregion

	#region Events

    /// <summary>
    ///     Invoked when the window is created
    /// </summary>
    public static event Action OnWindowCreated;

    /// <summary>
    ///     Invoked when the current window is destroyed
    /// </summary>
    public static event Action OnWindowDestroyed;

	#endregion

	// gets shit going
	static void InitializeGraphics (EngineConfigurationData config) {
		InitializeFrameBuffer();
		FrameBufferScale = config.DefaultBufferScale == 0 ? 3 : config.DefaultBufferScale;
		SetWindow(config.Fullscreen, config.VSync);
	}

	// initializes the frame buffer, makes shit render!
	static void InitializeFrameBuffer () {
		// make the frame buffer with our screen size
		RenderTexture = new RenderTexture((uint)ScreenSize.x, (uint)ScreenSize.y);

		// now make our frame buffer states using the frame buffer's texture
		frameBufferState = new RenderStates(BlendMode.Alpha, Transform.Identity, RenderTexture.Texture, null);

		// This throws an error ( for obvious reasons )
		//frameBufferState.Shader.SetUniform("texture", frameBufferState.Texture);
		// make a square
		frameBufferVertexArray = new VertexArray(PrimitiveType.Quads, 4U);

		// now, actually make the square
		// postition coord: -180, -90 || tex coord: 0,0 
		frameBufferVertexArray[0U] = new Vertex(new Vector2f(-HalfScreenSize.x, -HalfScreenSize.y), new Vector2f(0f, 0f));
		// postition coord: 180, -90 || tex coord: 320,0 
		frameBufferVertexArray[1U] = new Vertex(new Vector2f(HalfScreenSize.x, -HalfScreenSize.y), new Vector2f(ScreenSize.x, 0f));
		// postition coord: 180,90 || tex coord: 320,180
		frameBufferVertexArray[2U] = new Vertex(new Vector2f(HalfScreenSize.x, HalfScreenSize.y), new Vector2f(ScreenSize.x, ScreenSize.y));
		// postition coord: -180, 90 || tex coord: 0,180
		frameBufferVertexArray[3U] = new Vertex(new Vector2f(-HalfScreenSize.x, HalfScreenSize.y), new Vector2f(0f, ScreenSize.y));
	}

	static void SetWindow (bool goFullscreen, bool vsync) {
		// Kill our current window so we can create a new one
		if (Window != null) {
			// Dettach everything from the current window
			Window.Closed -= HandleClosingRequest;

			Input.Instance.DetachFromWindow(Window);

			// Kill it!
			Window.Close();
			Window.Dispose();

			// nullable check because we don't know if anything is subscribed to this event
			OnWindowDestroyed?.Invoke();
		}

		float cos = (float)Math.Cos(0);
		float sin = (float)Math.Sin(0);
		Styles style;
		VideoMode desktopMode;

		if (goFullscreen) {
			style = Styles.Fullscreen;
			desktopMode = VideoMode.DesktopMode;

			float fullScreenMin = Math.Min(desktopMode.Width/ScreenSize.x, desktopMode.Height/ScreenSize.y);
			float fullscreenWidth = (desktopMode.Width - ScreenSize.x*fullScreenMin)/2f;
			float fullscreenHeight = (desktopMode.Height - ScreenSize.y*fullScreenMin)/2f;

			int width = (int)(HalfScreenSize.x*fullScreenMin);
			int height = (int)(HalfScreenSize.y*fullScreenMin);

			frameBufferState.Transform = new Transform(cos*fullScreenMin, sin, fullscreenWidth + width, -sin, cos*fullScreenMin, fullscreenHeight + height, 0f, 0f, 1f);
		} else {

			int halfWidthScale = (int)(HalfScreenSize.x*FrameBufferScale);
			int halfHeightScale = (int)(HalfScreenSize.y*FrameBufferScale);
			style = Styles.Close;
			desktopMode = new VideoMode((uint)ScreenSize.x*(uint)FrameBufferScale, (uint)ScreenSize.y*(uint)FrameBufferScale);

			frameBufferState.Transform = new Transform(cos*FrameBufferScale, sin*FrameBufferScale, halfWidthScale, -sin*FrameBufferScale, cos*FrameBufferScale, halfHeightScale, 0f, 0f, 1f);
		}


		Window = new RenderWindow(desktopMode, "Dewdrop Engine", style);
		Window.Closed += HandleClosingRequest;

		Input.Instance.AttachToWindow(Window);

		if (vsync) //|| force_vsync)
		{
			//window.SetFramerateLimit(target_framerate);
			Window.SetVerticalSyncEnabled(true);
			//   
			//window.SetFramerateLimit(target_framerate);
		} else {
			Window.SetFramerateLimit(60);

		}

		// nullable check because we don't know if anything is subscribed to this event
		OnWindowCreated?.Invoke();
	}

	
	static void HandleClosingRequest (object sender, EventArgs e) {
		RenderWindow renderWindow = (RenderWindow)sender;
		renderWindow.Close();
		Outer.LogInfo("Window closed.");
		ImGui.SaveIniSettingsToDisk("imgui.ini");
		NbtFile file = new NbtFile(GlobalData.SerializeToNbt());
		file.SaveToFile(ApplicationData.ConfigPath, NbtCompression.ZLib);
	}

	public static void TakeScreenshot () {
		Image snapshot = RenderTexture.Texture.CopyToImage();

		string fileName = string.Format("screenshot{0}.png", Directory.GetFiles("./", "screenshot*.png").Length);

		snapshot.SaveToFile(fileName);
		Outer.LogInfo("Screenshot saved as \"{0}\"", fileName);
	}
}
