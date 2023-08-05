#region

using DewDrop.UserInput;
using DewDrop.Utilities;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

#endregion

namespace DewDrop;

// this handles the graphics part of the engine, such as rendering and the window
public static partial class Engine
{
    #region Properties

    /// <summary>
    ///     The main Render Window the game is using.
    /// </summary>
    public static RenderWindow Window => window;

    /// <summary>
    ///     The Render Texture that the game is rendering.
    /// </summary>
    public static RenderTexture RenderTexture => frameBuffer;

    public static int FrameBufferScale
    {
        get => frameBufferScale;
        set => frameBufferScale = value;
    }

    public static Vector2 Screen_Size =>
        // this is in EngineGraphics.cs
        screen_size;

    public static long Frame { get; private set; }

    #endregion

    #region Fields

    // handles the window
    // like, closin' disposin' and drawin'
    // that's supposed to rhyme
    // see https://www.sfml-dev.org/documentation/2.5.1/classsf_1_1RenderWindow.php for more info 
    private static RenderWindow window;

    // this is easier to explain
    // basically, everything is rendered to this
    // hence the name, render texture
    // what we can do with this is pass it to the RenderWindow so the RenderWindow can render it
    // we can also apply effects to the whole screen using it
    // see https://www.sfml-dev.org/documentation/2.5.1/classsf_1_1RenderTexture.php for more info
    private static RenderTexture frameBuffer;

    // hard to explain but basically when rendering to a render target, this defines settings for rendering to said render target
    // like blend mode, shader, and other stuff
    public static RenderStates frameBufferState;

    // i'm tired of explaining
    // https://www.sfml-dev.org/tutorials/2.5/graphics-vertex-array.php
    // it defines a shape, dude.
    public static VertexArray frameBufferVertexArray;

    // size of the screen
    // TODO: Have games be able to set this
    private static Vector2 screen_size = new(320, 180);

    private static Vector2 half_screen_size = new(screen_size.x / 2, screen_size.y / 2);

    private static int frameBufferScale = 3;

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
    private static void InitializeGraphics()
    {
        InitializeFrameBuffer();
        SetWindow(false, true);
    }

    // initializes the frame buffer, makes shit render!
    private static void InitializeFrameBuffer()
    {
        // make the frame buffer with our screen size
        frameBuffer = new RenderTexture((uint)screen_size.x, (uint)screen_size.y);

        // now make our frame buffer states using the frame buffer's texture
        frameBufferState = new RenderStates(BlendMode.Alpha, Transform.Identity, frameBuffer.Texture, null);

        // This throws an error ( for obvious reasons )
        //frameBufferState.Shader.SetUniform("texture", frameBufferState.Texture);
        // make a square
        frameBufferVertexArray = new VertexArray(PrimitiveType.Quads, 4U);

        // now, actually make the square
        // postition coord: -180, -90 || tex coord: 0,0 
        frameBufferVertexArray[0U] = new Vertex(new Vector2f(-half_screen_size.x, -half_screen_size.y), new Vector2f(0f, 0f));
        // postition coord: 180, -90 || tex coord: 320,0 
        frameBufferVertexArray[1U] = new Vertex(new Vector2f(half_screen_size.x, -half_screen_size.y), new Vector2f(screen_size.x, 0f));
        // postition coord: 180,90 || tex coord: 320,180
        frameBufferVertexArray[2U] = new Vertex(new Vector2f(half_screen_size.x, half_screen_size.y), new Vector2f(screen_size.x, screen_size.y));
        // postition coord: -180, 90 || tex coord: 0,180
        frameBufferVertexArray[3U] = new Vertex(new Vector2f(-half_screen_size.x, half_screen_size.y), new Vector2f(0f, screen_size.y));
    }

    private static void SetWindow(bool goFullscreen, bool vsync)
    {
        // Kill our current window so we can create a new one
        if (window != null)
        {
            // Dettach everything from the current window
            window.Closed -= HandleClosingRequest;

            Input.Instance.DetachFromWindow(window);

            // Kill it!
            window.Close();
            window.Dispose();

            // nullable check because we don't know if anything is subscribed to this event
            OnWindowDestroyed?.Invoke();
        }

        float cos = (float)Math.Cos(0);
        float sin = (float)Math.Sin(0);
        Styles style;
        VideoMode desktopMode;

        if (goFullscreen)
        {
            style = Styles.Fullscreen;
            desktopMode = VideoMode.DesktopMode;

            float fullScreenMin = Math.Min(desktopMode.Width / screen_size.x, desktopMode.Height / screen_size.y);
            float fullscreenWidth = (desktopMode.Width - screen_size.x * fullScreenMin) / 2f;
            float fullscreenHeight = (desktopMode.Height - screen_size.y * fullScreenMin) / 2f;

            int width = (int)(half_screen_size.x * fullScreenMin);
            int height = (int)(half_screen_size.y * fullScreenMin);

            frameBufferState.Transform = new Transform(cos * fullScreenMin, sin, fullscreenWidth + width, -sin, cos * fullScreenMin, fullscreenHeight + height, 0f, 0f, 1f);
        }
        else
        {

            int halfWidthScale = (int)(half_screen_size.x * frameBufferScale);
            int halfHeightScale = (int)(half_screen_size.y * frameBufferScale);
            style = Styles.Close;
            desktopMode = new VideoMode((uint)screen_size.x * (uint)frameBufferScale, (uint)screen_size.y * (uint)frameBufferScale);

            frameBufferState.Transform = new Transform(cos * frameBufferScale, sin * frameBufferScale, halfWidthScale, -sin * frameBufferScale, cos * frameBufferScale, halfHeightScale, 0f, 0f, 1f);
        }


        window = new RenderWindow(desktopMode, "Dewdrop Engine", style);
        window.Closed += HandleClosingRequest;

        Input.Instance.AttachToWindow(window);

        if (vsync) //|| force_vsync)
        {
            //window.SetFramerateLimit(target_framerate);
            window.SetVerticalSyncEnabled(true);
            //   
            //window.SetFramerateLimit(target_framerate);
        }
        else
        {
            window.SetFramerateLimit(60);

        }

        // nullable check because we don't know if anything is subscribed to this event
        OnWindowCreated?.Invoke();
    }

    private static void HandleClosingRequest(object sender, EventArgs e)
    {
        RenderWindow renderWindow = (RenderWindow)sender;
        renderWindow.Close();
    }

    public static void TakeScreenshot()
    {
        Image snapshot = frameBuffer.Texture.CopyToImage();

        string fileName = string.Format("screenshot{0}.png", Directory.GetFiles("./", "screenshot*.png").Length);

        snapshot.SaveToFile(fileName);
        DDDebug.LogInfo("Screenshot saved as \"{0}\"", fileName);
    }
}