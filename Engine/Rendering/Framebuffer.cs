using System;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Engine.Rendering
{
    public sealed class Framebuffer : IDisposable
    {
        internal readonly FramebufferHandle Handle;

        public Framebuffer(Texture texture)
        {
            Handle = GL.CreateFramebuffer();
            GL.NamedFramebufferTexture(Handle, FramebufferAttachment.ColorAttachment0, texture.Handle, 0);
            FramebufferStatus status = GL.CheckNamedFramebufferStatus(Handle, FramebufferTarget.Framebuffer);
            if (status != FramebufferStatus.FramebufferComplete)
            {
                throw new Exception($"Framebuffer is not complete: {status}");
            }
        }

        public void Bind(FramebufferTarget target)
        {
            GL.BindFramebuffer(target, Handle);
        }
        
        public void Dispose()
        {
            GL.DeleteFramebuffer(Handle);
        }
    }
}