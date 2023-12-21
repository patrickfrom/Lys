using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Lys.Renderer;
using OpenTK.Graphics.OpenGL4;

namespace Lys.OLD;

public class Mesh
{
    public Vertex[] Vertices;
    public int[] Indices;
    public Texture[] Textures;

    private int _vao;
    private int _vbo;
    private int _ebo;
    
    public Mesh(List<Vertex> vertices, List<int> indices, List<Texture> textures)
    {
        Vertices = vertices.ToArray();
        Indices = indices.ToArray();
        Textures = textures.ToArray();
        
        SetupMesh();
    }

    public void Draw(Shader shader)
    {
        var diffuseNumber = 1;
        var specularNumber = 1;

        for (var i = 0; i < Textures.Length; i++)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + i);

            var number = "";
            var name = Textures[i].Type;

            if (name == "texture_diffuse")
            {
                number = diffuseNumber.ToString();
            } else if (name == "texture_specular")
            {
                number = specularNumber.ToString();
            }
            
            shader.SetInt($"material.{name}{number}", i);
            GL.BindTexture(TextureTarget.Texture2D, Textures[i].Id);
        }
        GL.ActiveTexture(TextureUnit.Texture0);
        
        GL.BindVertexArray(_vao);
        GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
        GL.BindVertexArray(0);
    }

    private void SetupMesh()
    {
        _vao = GL.GenVertexArray();
        GL.BindVertexArray(_vao);

        _vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * Unsafe.SizeOf<Vertex>(), Vertices, BufferUsageHint.StaticDraw);

        _ebo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Length * sizeof(int), Indices, BufferUsageHint.StaticDraw);
        
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Unsafe.SizeOf<Vertex>(), 0);
        
        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, Unsafe.SizeOf<Vertex>(), Marshal.OffsetOf<Vertex>("Normal"));
        
        GL.EnableVertexAttribArray(2);
        GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, Unsafe.SizeOf<Vertex>(), Marshal.OffsetOf<Vertex>("TexCoords"));
        
        GL.BindVertexArray(0);
    }
}