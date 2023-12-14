using OpenTK.Mathematics;

namespace Lys.Lights;

public struct SpotLight(Vector3 position, Vector3 direction, Vector3 color)
{
    public Vector3 Position = position;
    public Vector3 Direction = direction;
    public Vector3 Color = color;
}