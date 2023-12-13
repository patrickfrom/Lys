#version 440

struct Material {
    sampler2D diffuse;
    sampler2D specular;
    
    float shininess;
};

struct Light {
    vec3 position;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

struct DirectionalLight {
    vec3 direction;
    
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

struct PointLight {
    vec3 position;
    
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
    
    float constant;
    float linear;
    float quadratic;
};

out vec4 FragColor;

in vec3 normal;
in vec3 fragPos;
in vec2 texCoords;

uniform Material material;
uniform Light light;
uniform DirectionalLight directionalLight;
uniform PointLight pointLight;
uniform vec3 viewPos;

vec3 CalculateLight() {
    vec3 ambient = light.ambient * texture(material.diffuse, texCoords).rgb;

    vec3 norm = normalize(normal);
    vec3 lightDir = normalize(light.position - fragPos);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = light.diffuse * diff * texture(material.diffuse, texCoords).rgb;

    vec3 viewDir = normalize(viewPos - fragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    vec3 specular = light.specular * spec * texture(material.specular, texCoords).rgb;

    return ambient + diffuse + specular;
}

vec3 CalculateDirectionalLight() {
    vec3 ambient = directionalLight.ambient * texture(material.diffuse, texCoords).rgb;

    vec3 norm = normalize(normal);
    vec3 lightDir = normalize(-directionalLight.direction);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = directionalLight.diffuse * diff * texture(material.diffuse, texCoords).rgb;

    vec3 viewDir = normalize(viewPos - fragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    vec3 specular = directionalLight.specular * spec * texture(material.specular, texCoords).rgb;

    return ambient + diffuse + specular;
}

vec3 CalculatePointLight() {
    vec3 ambient = pointLight.ambient * texture(material.diffuse, texCoords).rgb;

    vec3 norm = normalize(normal);
    vec3 lightDir = normalize(pointLight.position - fragPos);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = pointLight.diffuse * diff * texture(material.diffuse, texCoords).rgb;

    vec3 viewDir = normalize(viewPos - fragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    vec3 specular = pointLight.specular * spec * texture(material.specular, texCoords).rgb;
    
    float distance = length(pointLight.position - fragPos);
    float attenuation = 1.0 / (pointLight.constant + pointLight.linear * distance + pointLight.quadratic * (distance * distance));
    
    ambient *= attenuation;
    diffuse *= attenuation;
    specular *= attenuation;

    return ambient + diffuse + specular;
}

void main() {

    FragColor = vec4(CalculateDirectionalLight() + CalculateLight() + CalculatePointLight(), 1.0);
}
