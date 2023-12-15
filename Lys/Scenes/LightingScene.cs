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
    
    private Texture2D _container;
    private Texture2D _containerSpecular;
    
    public override void OnLoad()
    {
        base.OnLoad();
        
        GL.ClearColor(Color.OrangeRed);

        _camera = new Camera(Vector3.UnitZ * 3, window.ClientSize.X / (float)window.ClientSize.Y, window.KeyboardState,
            window.MouseState);


        _shader = new Shader("Assets/Shaders/LightingScene/default.vert", "Assets/Shaders/LightingScene/default.frag");
       
        _container = new Texture2D("Assets/Textures/container2.png");
        _containerSpecular = new Texture2D("Assets/Textures/container2_specular.png");

        _shader.SetInt("material.diffuse", 0);
        //_shader.SetInt("material.specular", 1);
        
        window.CursorState = CursorState.Grabbed;
        
        GL.Enable(EnableCap.DepthTest);
        
        GL.Enable(EnableCap.CullFace);
        GL.CullFace(CullFaceMode.Back);
    }

    public override void OnRender(FrameEventArgs e)
    {
        base.OnRender(e);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        
        _shader.SetMatrix4("view", _camera.GetViewMatrix());
        _shader.SetMatrix4("projection", _camera.GetProjectionMatrix());
        
        _container.Use();
        _cube.Draw(_shader);
        
        _container.Use();
        _cube.Draw(_shader, new Vector3(2, 1, 2));
        
        GL.BindTexture(TextureTarget.Texture2D, 0);
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