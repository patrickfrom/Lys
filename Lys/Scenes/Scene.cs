using System.Drawing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Lys.Scenes;

public abstract class Scene(NativeWindow window, string title = "Default Scene")
{
    public string Title = title;

    public virtual void OnLoad()
    {
    }

    public virtual void OnRender(FrameEventArgs e)
    {
    }

    public virtual void OnUpdate(FrameEventArgs e)
    {
    }

    public virtual void OnUnload()
    {
    }
}