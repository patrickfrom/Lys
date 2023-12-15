#version 440

struct Material {
    sampler2D diffuse;
    sampler2D specular;
    
    vec3 color;
};

out vec4 FragColor;

in vec2 texCoord;

uniform Material material;

void main() {
    FragColor = texture(material.diffuse, texCoord) + vec4(material.color, 1.0);
}