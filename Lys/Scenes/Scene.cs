using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Lys.Scenes;

public abstract class Scene(NativeWindow window)
{
    protected virtual string Title => "Default Scene";

    public WindowState WindowState
    {
        get => window.WindowState;
        set => window.WindowState = value;
    }
    
    public KeyboardState KeyboardState { get; set; } = window.KeyboardState;
    public MouseState MouseState { get; set; } = window.MouseState;
    public CursorState CursorState {
        get => window.CursorState;
        set => window.CursorState = value;
    }
    
    public virtual void OnLoad()
    {
        window.Title = Title;
    }

    public virtual void OnRender(FrameEventArgs e)
    {
    }

    public virtual void OnUpdate(FrameEventArgs e)
    {
        if (KeyboardState.IsKeyPressed(Keys.F11))
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