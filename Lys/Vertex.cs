using OpenTK.Mathematics;

namespace Lys;

public struct Vertex(Vector3 position, Vector3 normals, Vector2 texCoords)
{
    public Vector3 Position = position;
    public Vector3 Normals = normals;
    public Vector2 TexCoords = texCoords;
}