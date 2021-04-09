using System;
using Engine.Rendering;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Slime
{
    public sealed class TrailMap
    {
        private readonly Framebuffer _framebuffer;
        private readonly Texture _texture;
        private readonly VertexArray _vao;
        private readonly BufferObject<float> _vbo;
        private readonly BufferObject<uint> _ebo;
        private readonly ShaderProgram _trailProgram;
        
        public TrailMap(int width, int height)
        {
            _texture = new Texture(width, height);
            _framebuffer = new Framebuffer(_texture);

            _vbo = new BufferObject<float>(
                1, 1, 1, 1,
                1, -1, 1, 0,
                -1, -1, 0, 0,
                -1, 1, 0, 1
                );
            _ebo = new BufferObject<uint>(0, 1, 2, 0, 2, 3);
            _vao = new VertexArray(_ebo);
            _vao.AddVertexAttribute(_vbo, 0, 2, VertexAttribType.Float);
            _vao.AddVertexAttribute(_vbo, 1, 2, VertexAttribType.Float, 2 * sizeof(float));
            
            _trailProgram = ShaderProgram.CreateVertexFragment("trail.vert", "trail.frag");
        }

        public void Bind()
        {
            _framebuffer.Bind(FramebufferTarget.Framebuffer);
        }

        public void Unbind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }
        
        public void Draw()
        {
            _vao.Bind();
            _trailProgram.Bind();
            _texture.Bind(TextureUnit.Texture0);
            _trailProgram.SetUniform(_trailProgram.GetUniformLocation("uTexture0"), 0);
            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, IntPtr.Zero);
            
            // new Rect(_texture).Draw();
        }

        public void Diffuse()
        {
            _framebuffer.Bind(FramebufferTarget.Framebuffer);
            
            Draw();
            
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }
    }
}