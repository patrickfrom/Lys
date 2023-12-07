#version 440 core

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec4 aColor;

out vec4 color;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main() {
    color = aColor;
    
    gl_Position = vec4(aPosition, 1.0) * model * view * projection;
}