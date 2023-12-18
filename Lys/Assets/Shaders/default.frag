#version 440

out vec4 FragColor;

in vec3 Normal;
in vec2 TexCoords;
in vec3 FragPos;

uniform vec3 viewPos;
uniform sampler2D diffuse;

void main() {
    FragColor = texture(diffuse, TexCoords);
}