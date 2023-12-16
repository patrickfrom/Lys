using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Lys.Scenes;

public abstract class Scene(NativeWindow window)
{
    protected virtual string Title => "Default Scene";

    public KeyboardState KeyboardState { get; set; } = window.KeyboardState;
    public WindowState WindowState { get; set; } = window.WindowState;
    public MouseState MouseState { get; set; } = window.MouseState;
    
    public virtual void OnLoad()
    {
        window.Title = Title;
    }

    public virtual void OnRender(FrameEventArgs e)
    {
    }

    public virtual void OnUpdate(FrameEventArgs e)
    {
        if (window.KeyboardState.IsKeyPressed(Keys.F11))
        {
            WindowState = WindowState != WindowState.Fullscreen
                ? WindowState.Fullscreen
                : WindowState.Normal;
        }

        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            window.Close();
        }
    }

    public virtual void OnUnload()
    {
    }
}