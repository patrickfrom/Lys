#version 440

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoord;

out vec2 texCoord;

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;

void main() {
    gl_Position = vec4(aPosition, 1.0) * model * view * projection; 
    
    texCoord = aTexCoord;
}