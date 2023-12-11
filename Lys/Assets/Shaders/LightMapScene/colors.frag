#version 440 core
struct Material {
    sampler2D diffuse;
    sampler2D specular;
    sampler2D emission;

    float shininess;
    float emissionBrightness;
};

struct Light {
    vec3 position;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

out vec4 FragColor;

in vec3 normal;
in vec3 fragPos;
in vec2 texCoords;

uniform Material material;
uniform Light light;
uniform vec3 viewPos;

void main() {
    vec3 ambient = light.ambient * texture(material.diffuse, texCoords).rgb;

    vec3 norm = normalize(normal);
    vec3 lightDir = normalize(light.position - fragPos);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = light.diffuse * diff * texture(material.diffuse, texCoords).rgb;

    vec3 viewDir = normalize(viewPos - fragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    vec3 specular = light.specular * spec * texture(material.specular, texCoords).rgb;

    vec3 emission = texture(material.emission, texCoords).rgb * material.emissionBrightness * floor(vec3(1.0) - texture(material.specular,texCoords).rgb);
    
    vec3 result = ambient + diffuse + specular + emission;
    FragColor = vec4(result, 1.0);
}