#version 450 core
layout(location = 0) in vec2 vPosition;

out vec2 fPosition;

void main()
{
    gl_Position = vec4(vPosition, 0, 1);
    fPosition = vPosition;
}