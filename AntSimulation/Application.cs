using System;
using System.Runtime.InteropServices;
using Engine;
using Engine.Rendering;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace AntSimulation
{
    public class Application : ApplicationBase
    {
        private const int ParticleCount = 10;
        private const float ParticleSpeed = 0.01f;
        private const float TrailDiffuse = 0.01f;
        
        [StructLayout(LayoutKind.Sequential, Size = 16)]
        private struct Particle
        {
            private readonly Vector2 Position;
            private readonly Vector2 Direction;

            public Particle(Vector2 position, Vector2 direction)
            {
                Position = position;
                Direction = direction;
            }
        }

        private readonly Texture _trailTexture;
        private readonly Framebuffer _trailFramebuffer;
        private readonly ShaderStorageBufferObject<Particle> _particleBuffer;
        private readonly ShaderProgram _particleComputeShader;
        private readonly ShaderProgram _particleRenderingProgram;
        private readonly VertexArray _vao;
        
        public Application(int width, int height, string title) : base(width, height, title)
        {
            Particle[] particles = new Particle[ParticleCount];
            for (int i = 0; i < ParticleCount; i++)
            {
                particles[i] = new Particle(RandomF.NextVector2(-1, 1), RandomF.NextVector2(-1, 1).Normalized());
            }
            _particleBuffer = new ShaderStorageBufferObject<Particle>(1, particles);

            _vao = new VertexArray();
            _vao.AddVertexAttribute(_particleBuffer, 0, 2, VertexAttribType.Float, 0);

            _particleRenderingProgram = ShaderProgram.CreateVertexFragment("particle.vert", "particle.frag");
            _particleComputeShader = ShaderProgram.CreateCompute("particle.comp");
            _particleComputeShader.SetUniform(_particleComputeShader.GetUniformLocation("uSpeed"), ParticleSpeed);
            _particleComputeShader.SetUniform(_particleComputeShader.GetUniformLocation("uTrailMap"), 1);

            Vector2i size = Window.Size;
            _trailTexture = new Texture(size.X, size.Y);
            _trailFramebuffer = new Framebuffer(_trailTexture);
        }

        protected override void OnRender()
        {
            _particleComputeShader.Bind();
            _trailTexture.Bind(TextureUnit.Texture1);
            GL.DispatchCompute(1, 1, 1);
            
            _particleRenderingProgram.Bind();
            _vao.Bind();
            GL.DrawArrays(PrimitiveType.Points, 0, ParticleCount);
            
            _trailFramebuffer.Bind(FramebufferTarget.Framebuffer);
            GL.DrawArrays(PrimitiveType.Points, 0, ParticleCount);
            // new Rect(new Color4<Rgba>(0, 0, 0, TrailDiffuse)).Draw();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            
            new Rect(_trailTexture).Draw();
        }
    }
}