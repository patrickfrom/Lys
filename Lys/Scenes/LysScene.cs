using System.Drawing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Lys.Scenes;

public class LysScene(NativeWindow window) : Scene(window)
{
    protected override string Title => "Lys";

    public override void OnLoad()
    {
        base.OnLoad();
        GL.ClearColor(Color.Navy);
    }

    public override void OnRender(FrameEventArgs e)
    {
        base.OnRender(e);
        GL.Clear(ClearBufferMask.ColorBufferBit);
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