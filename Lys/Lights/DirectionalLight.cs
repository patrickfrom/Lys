using OpenTK.Mathematics;

namespace Lys.Lights;

public struct DirectionalLight(Vector3 direction, Vector3 ambient, Vector3 diffuse, Vector3 specular)
{
    public Vector3 Direction = direction;
    
    public Vector3 Ambient = ambient;
    public Vector3 Diffuse = diffuse;
    public Vector3 Specular = specular;
}