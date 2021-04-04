#version 450 core
in vec2 vPosition;
in vec2 vTextureCoordinate;

out vec2 fTextureCoordinate;

void main()
{
    gl_Position = vec4(vPosition, 0, 1);
    fTextureCoordinate = vTextureCoordinate;
}