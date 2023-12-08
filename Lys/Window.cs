using Lys.Scenes;
using Lys.Utils;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Lys;

public class Window(int width, int height, string title) : GameWindow(GameWindowSettings.Default,
    new NativeWindowSettings
    {
        ClientSize = new Vector2i(width, height),
        Title = title
    })
{

    private Scene _currentScene;
    
    protected override void OnLoad()
    {
        GlDebugger.Init();

        _currentScene = new LightMapScene(this, Title);
        _currentScene.OnLoad();
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        _currentScene.OnRender(e);        
        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        _currentScene.OnUpdate(e);
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
    }

    protected override void OnUnload()
    {
        _currentScene.OnUnload();
    }
}