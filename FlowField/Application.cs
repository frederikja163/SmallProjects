using System;
using System.Diagnostics;
using Engine;
using Engine.Rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace FlowField
{
    public class Application : ApplicationBase
    {
        private const int ParticleCount = 1_000_000;
        private static readonly Color3<Rgb> BackgroundColor = Color3.Purple;
        private const float TrailLenght = 50;
        private static readonly Color3<Rgb> TrailColor = Color3.Red;
        private const float PerlinSpeed = 0.001f;
        private const float RandomWeight = 0.01f;

        private readonly Particle[] _particles = new Particle[ParticleCount];
        private readonly ShaderProgram _particleShader;
        private readonly ShaderProgram _particleComputeShader;
        private readonly BufferObject<Particle> _particleBuffer;
        private readonly VertexArray _vertexArray;
        private float _perlinZPosition;
        private readonly int _perlinZPositionLocation;
        
        private readonly Stopwatch _stopwatch;

        private readonly Texture _texture;
        private readonly Framebuffer _framebuffer;
        
        private struct Particle
        {
            public Vector2 Position;
            public float Angle;
            public float AngleOffset;
        }
        
        public Application(int width, int height, string title) : base(width, height, title)
        {
            var random = new Random();

            for (int i = 0; i < _particles.Length; i++)
            {
                Vector2 position = new Vector2((float)random.NextDouble(), (float)random.NextDouble()) * 2 - Vector2.One;
                float angle = (float)random.NextDouble() * MathF.PI * 2;
                float angleOffset = RandomWeight * ((float)random.NextDouble() - 0.5f);
                _particles[i] = new Particle() {Position = position, Angle = angle, AngleOffset = angleOffset};
            }
            _particleShader = ShaderProgram.CreateVertexFragment("particle.vert", "particle.frag");
            _particleComputeShader = ShaderProgram.CreateCompute("particle.comp");

            _particleBuffer = new ShaderStorageBufferObject<Particle>(0, _particles);
            _vertexArray = new VertexArray();
            _vertexArray.AddVertexAttribute(_particleBuffer, _particleShader.GetAttributeLocation("vPosition"), 2, VertexAttribType.Float);
                
            _texture = new Texture(1080, 720);
            _framebuffer = new Framebuffer(_texture);
                
            _stopwatch = new Stopwatch();
            _perlinZPositionLocation = _particleComputeShader.GetUniformLocation("uPositionZ");
            float z = (float)random.NextDouble();
        }
        
        protected override void OnRender()
        {
            _stopwatch.Stop();
            float dt = (float)Stopwatch.Frequency / _stopwatch.ElapsedTicks;
            Window.Title = $"Flow field [FPS: {dt}]";
            _stopwatch.Reset();
            _stopwatch.Start();
            
            // CpuTick();
            GpuTick();
            
            _framebuffer.Bind(FramebufferTarget.Framebuffer);
            
            MakeTrails();
            
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            
            new Rect(_texture).Draw();
        }

        private void MakeTrails()
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            new Rect(BackgroundColor.ToRgba(1 / TrailLenght)).Draw();

            GL.BlendFunc(BlendingFactor.SrcColor, BlendingFactor.OneMinusSrcColor);
            new Rect(new Color4<Rgba>(TrailColor.Z / 10, TrailColor.Y / 10, TrailColor.X / 10, 1)).Draw();
            GL.Disable(EnableCap.Blend);
            
            _vertexArray.Bind();
            _particleShader.Bind();
            GL.DrawArrays(PrimitiveType.Points, 0, _particles.Length);
        }

        private void GpuTick()
        {
            _particleComputeShader.Bind();
            _perlinZPosition += PerlinSpeed;
            _particleComputeShader.SetUniform(_perlinZPositionLocation, _perlinZPosition);
            GL.DispatchCompute(ParticleCount / 1000, 1, 1);
        }

        private void CpuTick()
        {
            for (var i = 0; i < _particles.Length; i++)
            {
                float angle = _particles[i].Angle;
                Vector2 position = _particles[i].Position;
                position += new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * 0.01f;
            
                if (position.X < -1)
                    position.X = 1;
                else if (position.X > 1)
                    position.X = -1;
                if (position.Y < -1)
                    position.Y = 1;
                else if (position.Y > 1)
                    position.Y = -1;
            
                float perlin = (float) Perlin.Perlin3D((1 + position.X) * 8, (1 + position.Y) * 8, _perlinZPosition);
                _particles[i].Angle = MathHelper.Lerp(angle,  perlin * 32, 0.08f);
                _particles[i].Position = position;
            }
            _particleBuffer.SubData(_particles);
        }
    }
}