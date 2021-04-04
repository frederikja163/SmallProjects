using System;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Engine.Rendering
{
    public sealed class Texture : IDisposable
    {
        public static readonly Texture White = new Texture(new Color4<Rgba>[1,1] {{Color4.White}});
        
        internal readonly uint Handle;

        public Texture(int width, int height)
        {
            Handle = GL.CreateTexture(TextureTarget.Texture2d);
            // TODO: Fix the opengl spec for the internalformat enum.
            GL.TextureStorage2D(Handle, 1, SizedInternalFormat.Rgba32f, width, height);
            
            GL.TextureParameteri(Handle, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TextureParameteri(Handle, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.GenerateTextureMipmap(Handle);
        }

        public Texture(Color4<Rgba>[,] data) : this(data.GetLength(0), data.GetLength(1))
        {
            GL.TextureSubImage2D(Handle, 0, 0, 0, data.GetLength(0), data.GetLength(1),
            PixelFormat.Rgba, PixelType.Float, data[0, 0]);
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