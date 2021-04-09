#version 450 core
in vec2 fTexCoord;

uniform sampler2D uTexture0;

out vec4 Color;

void main()
{
    Color = texture(uTexture0, fTexCoord);
    Color = vec4(fTexCoord, 0, 1);
}