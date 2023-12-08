using System.Drawing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Lys.Scenes;

public class LightMapScene(NativeWindow window, string title = "Default Scene") : Scene(window, title)
{
    private Shader _lightingShader;
    private Shader _lightCubeShader;

    private Camera _camera;

    private double _time;

    private int _vao;
    private int _lightVao;

    private float[] _vertices =
    {
        -0.5f, -0.5f, -0.5f, 0.0f, 0.0f, -1.0f, 0.0f, 0.0f,
        0.5f, -0.5f, -0.5f, 0.0f, 0.0f, -1.0f, 1.0f, 0.0f,
        0.5f, 0.5f, -0.5f, 0.0f, 0.0f, -1.0f, 1.0f, 1.0f,
        -0.5f, 0.5f, -0.5f, 0.0f, 0.0f, -1.0f, 0.0f, 1.0f,

        -0.5f, -0.5f, 0.5f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f,
        0.5f, -0.5f, 0.5f, 0.0f, 0.0f, 1.0f, 1.0f, 0.0f,
        0.5f, 0.5f, 0.5f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f,
        -0.5f, 0.5f, 0.5f, 0.0f, 0.0f, 1.0f, 0.0f, 1.0f,

        -0.5f, 0.5f, -0.5f, -1.0f, 0.0f, 0.0f, 0.0f, 0.0f,
        -0.5f, -0.5f, -0.5f, -1.0f, 0.0f, 0.0f, 1.0f, 0.0f,
        -0.5f, -0.5f, 0.5f, -1.0f, 0.0f, 0.0f, 1.0f, 1.0f,
        -0.5f, 0.5f, 0.5f, -1.0f, 0.0f, 0.0f, 0.0f, 1.0f,

        0.5f, 0.5f, 0.5f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f,
        0.5f, 0.5f, -0.5f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f,
        0.5f, -0.5f, -0.5f, 1.0f, 0.0f, 0.0f, 1.0f, 1.0f,
        0.5f, -0.5f, 0.5f, 1.0f, 0.0f, 0.0f, 0.0f, 1.0f,

        -0.5f, -0.5f, -0.5f, 0.0f, -1.0f, 0.0f, 0.0f, 0.0f,
        0.5f, -0.5f, -0.5f, 0.0f, -1.0f, 0.0f, 1.0f, 0.0f,
        0.5f, -0.5f, 0.5f, 0.0f, -1.0f, 0.0f, 1.0f, 1.0f,
        -0.5f, -0.5f, 0.5f, 0.0f, -1.0f, 0.0f, 0.0f, 1.0f,

        0.5f, 0.5f, -0.5f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f,
        0.5f, 0.5f, 0.5f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f,
        -0.5f, 0.5f, 0.5f, 0.0f, 1.0f, 0.0f, 1.0f, 1.0f,
        -0.5f, 0.5f, -0.5f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f,
    };

    private int[] _indices =
    {
        0, 3, 1,
        3, 2, 1,

        4, 5, 7,
        7, 5, 6,

        8, 9, 11,
        11, 9, 10,

        12, 15, 13,
        15, 14, 13,

        16, 17, 19,
        19, 17, 18,

        20, 23, 21,
        23, 22, 21,
    };

    private Vector3 _lightColor = new(1.0f, 1.0f, 1.0f);
    private Vector3 _lightPos = new(2, 3, 2);

    private Texture _container;
    private Texture _containerSpecular;
    private Texture _containerSpecularColor;
    private Texture _pinkColorSpecular;
    private Texture _emissionMap;
    private int _ebo;
    private int _vbo;

    public override void OnLoad()
    {
        base.OnLoad();

        GL.ClearColor(Color.Navy);

        _vao = GL.GenVertexArray();
        GL.BindVertexArray(_vao);

        _vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices,
            BufferUsageHint.StaticDraw);

        _ebo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(int), _indices,
            BufferUsageHint.StaticDraw);

        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);

        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));

        GL.EnableVertexAttribArray(2);
        GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));

        _lightVao = GL.GenVertexArray();
        GL.BindVertexArray(_lightVao);

        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);

        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);

        _lightingShader = new Shader("Assets/Shaders/LightMapScene/colors.vert", "Assets/Shaders/LightMapScene/colors.frag");
        _lightCubeShader = new Shader("Assets/Shaders/LightMapScene/lightCube.vert", "Assets/Shaders/LightMapScene/lightCube.frag");

        _container = new Texture("Assets/Textures/container2.png");
        _containerSpecular = new Texture("Assets/Textures/container2_specular.png");
        _containerSpecularColor = new Texture("Assets/Textures/lighting_maps_specular_color.png");
        _pinkColorSpecular = new Texture("Assets/Textures/pink.png");
        _emissionMap = new Texture("Assets/Textures/matrix.jpg");

        _lightingShader.Use();
        _lightingShader.SetInt("material.diffuse", 0);
        _lightingShader.SetInt("material.specular", 1);
        _lightingShader.SetInt("material.emission", 2);

        _camera = new Camera(Vector3.UnitZ * 3, window.ClientSize.X / (float)window.ClientSize.Y, window.KeyboardState,
            window.MouseState);

        window.CursorState = CursorState.Grabbed;

        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.CullFace);
        GL.CullFace(CullFaceMode.Back);
    }

    public override void OnRender(FrameEventArgs e)
    {
        if (!window.IsFocused)
            return;

        _time += 4.0 * e.Time;

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        
        DrawCube(new Vector3(0, 0, 0));
        
        _lightCubeShader.Use();
        GL.BindVertexArray(_lightVao);
        var model2 = Matrix4.CreateTranslation(_lightPos);
        model2 *= Matrix4.CreateScale(0.5f);
        _lightCubeShader.SetMatrix4("model", model2);
        _lightCubeShader.SetMatrix4("view", _camera.GetViewMatrix());
        _lightCubeShader.SetMatrix4("projection", _camera.GetProjectionMatrix());
        _lightCubeShader.SetVector3("color", _lightColor);

        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
    }

    public override void OnUpdate(FrameEventArgs e)
    {
        if (!window.IsFocused)
        {
            return;
        }

        // _lightPos.Y = (float)MathHelper.Cos(_time * 0.15f) * 5;
        // _lightPos.X = (float)MathHelper.Sin(_time * 0.15f) * 5;
        // _lightPos.Z = (float)MathHelper.Sin(_time * 0.15f) * 5;

        if (window.KeyboardState.IsKeyPressed(Keys.F11))
        {
            window.WindowState = window.WindowState != WindowState.Fullscreen
                ? WindowState.Fullscreen
                : WindowState.Normal;
        }

        if (window.KeyboardState.IsKeyDown(Keys.Escape))
        {
            window.Close();
        }

        if (window.KeyboardState.IsKeyPressed(Keys.F))
        {
            window.CursorState = window.CursorState == CursorState.Grabbed ? CursorState.Normal : CursorState.Grabbed;
        }

        if (window.CursorState == CursorState.Grabbed)
        {
            _camera.Update(e.Time);
        }
    }

    public override void OnUnload()
    {
        GL.DeleteVertexArray(_vao);
        
        GL.DeleteBuffer(_vbo);
        GL.DeleteBuffer(_ebo);

        _lightingShader.Dispose();
        _lightCubeShader.Dispose();
        
        GL.DeleteTexture(_container.Id);
        GL.DeleteTexture(_containerSpecular.Id);
        GL.DeleteTexture(_containerSpecularColor.Id);
        GL.DeleteTexture(_pinkColorSpecular.Id);
        GL.DeleteTexture(_emissionMap.Id);
    }

    private void DrawCube(Vector3 position)
    {
        _lightingShader.Use();
        GL.BindVertexArray(_vao);
        var model = Matrix4.CreateTranslation(position);
        _lightingShader.SetMatrix4("model", model);
        _lightingShader.SetMatrix4("view", _camera.GetViewMatrix());
        _lightingShader.SetMatrix4("projection", _camera.GetProjectionMatrix());

        _lightingShader.SetVector3("viewPos", _camera.Position);
        _lightingShader.SetMatrix3("normalInverse", new Matrix3(model.Inverted()));

        _lightingShader.SetFloat("material.shininess", 8.0f);
        _lightingShader.SetFloat("material.emissionBrightness", (float)MathHelper.Cos(0.7f * _time) + 2.0f);

        var diffuseColor = _lightColor * new Vector3(0.5f);
        var ambientColor = diffuseColor * new Vector3(0.2f);
        
        _lightingShader.SetVector3("light.position", _lightPos);
        _lightingShader.SetVector3("light.ambient", ambientColor);
        _lightingShader.SetVector3("light.diffuse", diffuseColor);
        _lightingShader.SetVector3("light.specular", new Vector3(1.0f, 1.0f, 1.0f));
        
        _container.Use();
        _containerSpecular.Use(1);
        _emissionMap.Use(2);
        
        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
    }
}