using OpenTK.Graphics.OpenGL4;

namespace Lys.Renderer;

public class VertexBuffer
{
    private readonly int _rendererId;
    
    public VertexBuffer(float[] vertices, int size)
    {
        _rendererId = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _rendererId);
        GL.BufferData(BufferTarget.ArrayBuffer, size, vertices, BufferUsageHint.StaticDraw);
    }
    
    public void Bind()
    {
        GL.BindBuffer(BufferTarget.ArrayBuffer, _rendererId);
    }
    
    public void Unbind()
    {
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
    }
}