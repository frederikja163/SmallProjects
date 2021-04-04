using System;
using OpenTK.Graphics.OpenGL;

namespace Engine.Rendering
{
    public sealed class Texture : IDisposable
    {
        internal readonly uint Handle;

        public Texture(int width, int height)
        {
            Handle = GL.GenTexture();
            // TODO: Fix the opengl spec.
            GL.TexImage2D(TextureTarget.Texture2d, 0, (int)InternalFormat.Rgba32f, width, height, 0,
                PixelFormat.Bgra, PixelType.Float, IntPtr.Zero);
            
            GL.TextureParameteri(Handle, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TextureParameteri(Handle, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.GenerateTextureMipmap(Handle);
        }

        public void Bind(TextureUnit unit)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2d, Handle);
        }

        public void Dispose()
        {
            GL.DeleteTexture(Handle);
        }
    }
}