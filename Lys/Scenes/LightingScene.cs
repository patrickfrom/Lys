using System.Drawing;
using Lys.Shapes;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Lys.Scenes;

public class LightingScene(NativeWindow window, string title = "Default Scene") : Scene(window, title)
{
    private readonly Cube _cube = new();
    
    private Camera _camera;

    private Shader _shader;
    
    public override void OnLoad()
    {
        base.OnLoad();
        
        GL.ClearColor(Color.OrangeRed);

        _camera = new Camera(Vector3.UnitZ * 3, window.ClientSize.X / (float)window.ClientSize.Y, window.KeyboardState,
            window.MouseState);

        window.CursorState = CursorState.Grabbed;
        
        _shader = new Shader("Assets/Shaders/LightingScene/default.vert", "Assets/Shaders/LightingScene/default.frag");
    }

    public override void OnRender(FrameEventArgs e)
    {
        base.OnRender(e);
        GL.Clear(ClearBufferMask.ColorBufferBit);
        
        _shader.SetMatrix4("view", _camera.GetViewMatrix());
        _shader.SetMatrix4("projection", _camera.GetProjectionMatrix());
        _cube.Draw(_shader);
    }

    public override void OnUpdate(FrameEventArgs e)
    {
        base.OnUpdate(e);
        _camera.Update(e.Time);
    }

    public override void OnUnload()
    {
        base.OnUnload();
        
        _cube.Dispose();
    }
}