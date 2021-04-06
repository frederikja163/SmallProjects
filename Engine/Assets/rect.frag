#version 450 core
in vec2 fTextureCoordinate;

out vec4 Color;

uniform sampler2D uTexture;
uniform vec4 uColor;

#define tex(x, y) texture(uTexture, fTextureCoordinate + vec2(x, y))

void main()
{    
    Color = texture(uTexture, fTextureCoordinate) * uColor;
}