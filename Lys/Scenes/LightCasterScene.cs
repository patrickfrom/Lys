using System.Drawing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Lys.Scenes;

public class LightCasterScene(NativeWindow window, string title = "Default Scene") : Scene(window, title)
{
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

    private Camera _camera;

    private Shader _defaultShader;
    private Shader _lightCubeShader;

    private int _vao;
    private int _vbo;
    private int _ebo;

    private int _lightVao;
    private Vector3 _lightPos = new(2, -3, 2);
    private Texture2D _container;
    private Texture2D _containerSpecular;
    private Vector3 _lightColor = new(1.0f, 1.0f, 1.0f);

    public override void OnLoad()
    {
        base.OnLoad();
        
        GL.ClearColor(Color.Navy);

        _vao = GL.GenVertexArray();
        GL.BindVertexArray(_vao);

        _vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

        _ebo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(int), _indices, BufferUsageHint.StaticDraw);

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
        
        _defaultShader = new Shader("Assets/Shaders/LightCasterScene/default.vert", "Assets/Shaders/LightCasterScene/default.frag");
        _lightCubeShader = new Shader("Assets/Shaders/LightMapScene/lightCube.vert", "Assets/Shaders/LightMapScene/lightCube.frag");
        
        _container = new Texture2D("Assets/Textures/rusted-panels_albedo.png");
        _containerSpecular = new Texture2D("Assets/Textures/rusted-panels_metallic.png");
        
        _defaultShader.SetInt("material.diffuse", 0);
        _defaultShader.SetInt("material.specular", 1);
        
        _camera = new Camera(Vector3.UnitZ * 3, window.ClientSize.X / (float)window.ClientSize.Y, window.KeyboardState,
            window.MouseState);

        window.CursorState = CursorState.Grabbed;
        
        GL.Enable(EnableCap.DepthTest);
        GL.CullFace(CullFaceMode.Back);
    }


    private Vector3[] _pointLights =
    {
        new(0,1,0),
        new(0, -1, 0),
        new(0, 0, 1),
    };
    
    public override void OnRender(FrameEventArgs e)
    {
        base.OnRender(e);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        
        GL.Enable(EnableCap.CullFace);
        _defaultShader.SetMatrix4("view", _camera.GetViewMatrix());
        _defaultShader.SetMatrix4("projection", _camera.GetProjectionMatrix());
        
        GL.BindVertexArray(_vao);
        var model = Matrix4.Identity;
        _defaultShader.SetMatrix4("model", model);
        _defaultShader.SetVector3("viewPos", _camera.Position);
        _defaultShader.SetMatrix3("normalInverse", new Matrix3(model.Inverted()));

        _defaultShader.SetFloat("material.shininess", 8.0f);
        
        _container.Use();
        _containerSpecular.Use(1);
        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        
        _lightCubeShader.SetMatrix4("view", _camera.GetViewMatrix());
        _lightCubeShader.SetMatrix4("projection", _camera.GetProjectionMatrix());
        
        GL.BindVertexArray(_vao);
        model = Matrix4.CreateTranslation(_lightPos);
        model *= Matrix4.CreateScale(0.5f);
        _lightCubeShader.SetMatrix4("model", model);
        _lightCubeShader.SetVector3("color", _lightColor);
        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        
        var diffuseColor = _lightColor * new Vector3(0.5f);
        var ambientColor = diffuseColor * new Vector3(0.5f);

        /*_defaultShader.SetVector3("light.position", _lightPos);
        _defaultShader.SetVector3("light.ambient", ambientColor);
        _defaultShader.SetVector3("light.diffuse", diffuseColor);
        _defaultShader.SetVector3("light.specular", new Vector3(1.0f, 1.0f, 1.0f));*/
        
        _defaultShader.SetVector3("directionalLight.direction", new Vector3(-5, -10, 0));
        _defaultShader.SetVector3("directionalLight.ambient", ambientColor);
        _defaultShader.SetVector3("directionalLight.diffuse", diffuseColor);
        _defaultShader.SetVector3("directionalLight.specular", new Vector3(1.0f, 1.0f, 0.0f));

        for (var i = 0; i < 3; i++)
        {
            _defaultShader.SetVector3($"pointLight[{i}].position", _pointLights[i]);
            _defaultShader.SetVector3($"pointLight[{i}].diffuse", diffuseColor);
            _defaultShader.SetFloat($"pointLight[{i}].constant",  0.2f);
            _defaultShader.SetFloat($"pointLight[{i}].linear",    0.09f);
            _defaultShader.SetFloat($"pointLight[{i}].quadratic", 1.002f);
        }

    }

    public override void OnUpdate(FrameEventArgs e)
    {
        if (window.KeyboardState.IsKeyPressed(Keys.F))
        {
            window.CursorState = window.CursorState == CursorState.Grabbed ? CursorState.Normal : CursorState.Grabbed;
        }

        if (window.CursorState == CursorState.Grabbed)
        {
            _camera.Update(e.Time);
        }
        
        base.OnUpdate(e);
    }

    public override void OnUnload()
    {
        base.OnUnload();
    }
}