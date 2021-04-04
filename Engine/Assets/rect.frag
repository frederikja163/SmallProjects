#version 450 core
in vec2 fTextureCoordinate;

out vec4 Color;

uniform sampler2D uTexture;
uniform vec4 uColor;

void main()
{
    Color = texture(uTexture, fTextureCoordinate) * uColor;
}