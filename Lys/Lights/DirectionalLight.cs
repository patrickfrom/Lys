using OpenTK.Mathematics;

namespace Lys.Lights;

public struct DirectionalLight(Vector3 direction, Vector3 ambient = default, Vector3 diffuse = default, Vector3 specular = default, float brightness = 1.0f)
{
    public Vector3 Direction = direction;
    
    public float Brightness = brightness;
    
    public Vector3 Ambient = ambient != default ? ambient : new Vector3(1.0f);
    public Vector3 Diffuse = diffuse != default ? diffuse : new Vector3(1.0f);
    public Vector3 Specular = specular != default ? specular : new Vector3(1.0f);
}