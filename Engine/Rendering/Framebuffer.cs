using System;
using OpenTK.Graphics.OpenGL;

namespace Engine.Rendering
{
    public sealed class Framebuffer : IDisposable
    {
        internal readonly uint Handle;

        public Framebuffer(Texture texture)
        {
            Handle = GL.GenFramebuffer();
            GL.NamedFramebufferTexture(Handle, FramebufferAttachment.ColorAttachment0, texture.Handle, 0);
        }

        public void Bind(FramebufferTarget target)
        {
            GL.BindFramebuffer(target, Handle);
            GL.DrawBuffer(DrawBufferMode.ColorAttachment0);
        }
        
        public void Dispose()
        {
            GL.DeleteFramebuffer(Handle);
        }
    }
}