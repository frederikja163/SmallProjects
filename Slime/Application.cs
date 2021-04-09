using System;
using System.Runtime.InteropServices;
using Engine;
using Engine.Rendering;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Slime
{
    public sealed class Application : ApplicationBase
    {
        public const int ParticleCount = 100;
        
        [StructLayout(LayoutKind.Sequential, Size = 16)]
        private struct Particle
        {
            private readonly Vector2 _position;
            private readonly float _angle;

            public Particle(Vector2 position, float angle)
            {
                _position = position;
                _angle = angle;
                _ = 0;
            }
        }
        
        private readonly ShaderProgram _particleRendererShader;
        private readonly ShaderProgram _particleComputeShader;
        private readonly ShaderStorageBufferObject<Particle> _particleBuffer;
        private readonly VertexArray _vao;

        private readonly Texture _texture;
        private readonly Framebuffer _framebuffer;
        
        public Application(int width, int height, string title) : base(width, height, title)
        {
            _particleRendererShader = ShaderProgram.CreateVertexFragment("particle.vert", "particle.frag");
            _particleComputeShader = ShaderProgram.CreateCompute("particle.comp");

            Particle[] particles = new Particle[ParticleCount];
            for (int i = 0; i < particles.Length; i++)
            {
                // particles[i] = new Particle(RandomF.NextVector2(-1, 1), RandomF.NextFloat(0, MathF.PI * 2));
                particles[i] = new Particle(RandomF.NextVector2(-1, 1).Normalized() * RandomF.NextFloat(), RandomF.NextFloat(0, MathF.PI * 2));
            }
            _particleBuffer = new ShaderStorageBufferObject<Particle>(0, particles);
            
            _vao = new VertexArray();
            _vao.AddVertexAttribute(_particleBuffer, 0, 2, VertexAttribType.Float);

            _texture = new Texture(width, height);
            _texture.Bind(TextureUnit.Texture1);
            _particleComputeShader.SetUniform(_particleComputeShader.GetUniformLocation("uTexture1"), 1);
            _framebuffer = new Framebuffer(_texture);
        }

        protected override void OnRender()
        {
            _particleComputeShader.Bind();
            GL.DispatchCompute(ParticleCount, 1, 1);
            
            _framebuffer.Bind(FramebufferTarget.Framebuffer);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            new Rect(new Color4<Rgba>(0f, 0f, 0f, 0.01f)).Draw();
            
            _vao.Bind();
            _particleRendererShader.Bind();
            GL.DrawArrays(PrimitiveType.Points, 0, ParticleCount);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            
            new Rect(_texture).Draw();
        }
    }
}