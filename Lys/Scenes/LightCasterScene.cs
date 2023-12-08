using System.Drawing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Lys.Scenes;

public class LightCasterScene(NativeWindow window, string title = "Default Scene") : Scene(window, title)
{
    public override void OnLoad()
    {
        base.OnLoad();
        
        GL.ClearColor(Color.Navy);
    }

    public override void OnRender(FrameEventArgs e)
    {
        base.OnRender(e);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
    }

    public override void OnUpdate(FrameEventArgs e)
    {
        base.OnUpdate(e);
    }

    public override void OnUnload()
    {
        base.OnUnload();
    }
}