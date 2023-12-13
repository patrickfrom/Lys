using System.Drawing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Lys.Scenes;

public struct SpotLight(Vector3 position, Vector3 color, float constant = 1.0f)
{
    public Vector3 Position = position;
    public Vector3 Color = color;
    public float Constant = constant;
}

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
    private Shader _skyboxShader;

    private int _vao;
    private int _vbo;
    private int _ebo;

    private int _lightVao;
    private Vector3 _lightPos = new(2, -3, 2);
    private Texture2D _container;
    private Texture2D _containerSpecular;
    private Vector3 _lightColor = new(1.0f, 1.0f, 1.0f);

    private SpotLight[] _pointLights =
    {
        new(new Vector3(55,1,0), new Vector3(1,0,0)),
        new(new Vector3(0,-1,0), new Vector3(0,1,0), 2.5f),
        new(new Vector3(0,0,3), new Vector3(0,1,1)),
    };

    private Skybox _redSpaceSkybox;
    private int _skyboxVao;
    private int _skyboxVbo;

    private Model _dragon;

    public override void OnLoad()
    {
        base.OnLoad();
        _dragon = new Model("Assets/Models/fg_spkMgDragon.obj");
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

        _skyboxVao = GL.GenVertexArray();
        GL.BindVertexArray(_skyboxVao);

        _skyboxVbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _skyboxVbo);
        GL.BufferData(BufferTarget.ArrayBuffer, Skybox.SkyboxVertices.Length * sizeof(float), Skybox.SkyboxVertices,
            BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);

        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

        _defaultShader = new Shader("Assets/Shaders/LightCasterScene/default.vert",
            "Assets/Shaders/LightCasterScene/default.frag");
        _lightCubeShader = new Shader("Assets/Shaders/LightMapScene/lightCube.vert",
            "Assets/Shaders/LightMapScene/lightCube.frag");
        _skyboxShader = new Shader("Assets/Shaders/skybox.vert", "Assets/Shaders/skybox.frag");
        
        var redSkyboxPaths = new[]
        {
            "Assets/Skybox/RedSpace/bkg3_right1.png",
            "Assets/Skybox/RedSpace/bkg3_left2.png",
            "Assets/Skybox/RedSpace/bkg3_top3.png",
            "Assets/Skybox/RedSpace/bkg3_bottom4.png",
            "Assets/Skybox/RedSpace/bkg3_front5.png",
            "Assets/Skybox/RedSpace/bkg3_back6.png",
        };

        _redSpaceSkybox = new Skybox(redSkyboxPaths);
        _skyboxShader.Use();
        _skyboxShader.SetInt("skybox", 0);

        _container = new Texture2D("Assets/Textures/container2.png");
        _containerSpecular = new Texture2D("Assets/Textures/container2_specular.png");

        _defaultShader.SetInt("material.texture_diffuse1", 0);
        _defaultShader.SetInt("material.texture_specular1", 1);

        _camera = new Camera(Vector3.UnitZ * 3, window.ClientSize.X / (float)window.ClientSize.Y, window.KeyboardState,
            window.MouseState);

        window.CursorState = CursorState.Grabbed;

        GL.Enable(EnableCap.DepthTest);
        GL.CullFace(CullFaceMode.Back);
    }



    public override void OnRender(FrameEventArgs e)
    {
        base.OnRender(e);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        GL.Enable(EnableCap.CullFace);
        _defaultShader.SetMatrix4("view", _camera.GetViewMatrix());
        _defaultShader.SetMatrix4("projection", _camera.GetProjectionMatrix());

        GL.BindVertexArray(_vao);
        var model = Matrix4.Identity;
        model *= Matrix4.CreateFromAxisAngle(new Vector3(1, 1, 1), 35f);
        model *= Matrix4.CreateTranslation(1, 0, 2);
        _defaultShader.SetMatrix4("model", model);
        _defaultShader.SetVector3("viewPos", _camera.Position);
        _defaultShader.SetMatrix3("normalInverse", new Matrix3(model.Inverted()));

        _defaultShader.SetFloat("material.shininess", 8.0f);

        _container.Use();
        _containerSpecular.Use(1);
        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

        GL.BindVertexArray(_vao);
        model = Matrix4.Identity;
        _defaultShader.SetMatrix4("model", model);
        _defaultShader.SetVector3("viewPos", _camera.Position);
        _defaultShader.SetMatrix3("normalInverse", new Matrix3(model.Inverted()));

        _defaultShader.SetFloat("material.shininess", 8.0f);

        _container.Use();
        _containerSpecular.Use(1);
        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        GL.BindTexture(TextureTarget.Texture2D, 0);
        
        _lightCubeShader.SetMatrix4("view", _camera.GetViewMatrix());
        _lightCubeShader.SetMatrix4("projection", _camera.GetProjectionMatrix());

        GL.BindVertexArray(_lightVao);
        model = Matrix4.Identity;
        model *= Matrix4.CreateScale(0.5f);
        model *= Matrix4.CreateTranslation(_lightPos);

        _lightCubeShader.SetMatrix4("model", model);
        _lightCubeShader.SetVector3("color", _lightColor);
        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

        _defaultShader.Use();
        model = Matrix4.Identity;
        model *= Matrix4.CreateScale(50f);
        model *= Matrix4.CreateTranslation(new Vector3(3,3,3));
        _defaultShader.SetMatrix4("model", model);
        _dragon.Draw(_defaultShader);
        
        var diffuseColor = _lightColor * new Vector3(0.5f);
        var ambientColor = diffuseColor * new Vector3(0.5f);    

        _defaultShader.SetVector3("directionalLight.direction", new Vector3(-5, -10, 0));
        _defaultShader.SetVector3("directionalLight.ambient", ambientColor);
        _defaultShader.SetVector3("directionalLight.diffuse", diffuseColor);
        _defaultShader.SetVector3("directionalLight.specular", new Vector3(1.0f, 1.0f, 1.0f));

        for (var i = 0; i < 3; i++)
        {
            var pointLight = _pointLights[i];
            _defaultShader.SetVector3($"pointLight[{i}].position", pointLight.Position);
            _defaultShader.SetVector3($"pointLight[{i}].diffuse", pointLight.Color);
            _defaultShader.SetFloat($"pointLight[{i}].constant", pointLight.Constant);
            _defaultShader.SetFloat($"pointLight[{i}].linear", 0.09f);
            _defaultShader.SetFloat($"pointLight[{i}].quadratic", 0.002f);

            GL.BindVertexArray(_vao);
            model = Matrix4.Identity;
            model *= Matrix4.CreateScale(0.25f);
            model *= Matrix4.CreateTranslation(pointLight.Position);

            _lightCubeShader.SetMatrix4("model", model);
            _lightCubeShader.SetVector3("color", pointLight.Color);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        }
        
        // spot light
        _defaultShader.SetVector3("spotLight.position", new Vector3(0, 1, 0));
        _defaultShader.SetVector3("spotLight.direction", new Vector3(0,-1,0));
        _defaultShader.SetVector3("spotLight.diffuse", new Vector3(1.0f, 0.0f, 0));
        _defaultShader.SetFloat("spotLight.constant", 0.2f);
        _defaultShader.SetFloat("spotLight.cutOff", float.DegreesToRadians(55));
        _defaultShader.SetFloat("spotLight.outerCutOff", float.DegreesToRadians(35));
        _defaultShader.SetFloat("spotLight.linear", 0.09f);
        _defaultShader.SetFloat("spotLight.quadratic", 0.002f);

        GL.BindVertexArray(_vao);
        model = Matrix4.Identity;
        model *= Matrix4.CreateScale(0.25f);
        model *= Matrix4.CreateTranslation(new Vector3(0, 1, 0));

        _lightCubeShader.SetMatrix4("model", model);
        _lightCubeShader.SetVector3("color", new Vector3(1.0f, 0, 0));
        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        
        GL.Disable(EnableCap.CullFace);

        GL.DepthFunc(DepthFunction.Lequal);
        _skyboxShader.Use();
        _skyboxShader.SetMatrix4("view", new Matrix4(new Matrix3(_camera.GetViewMatrix())));
        _skyboxShader.SetMatrix4("projection", _camera.GetProjectionMatrix());
        GL.BindVertexArray(_skyboxVao);
        _redSpaceSkybox.Use();
        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        GL.DepthFunc(DepthFunction.Less);
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