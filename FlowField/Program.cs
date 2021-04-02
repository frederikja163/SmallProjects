using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Runtime.InteropServices;
using Engine;
using Engine.Rendering;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Window = Engine.Platform.Window;

namespace FlowFieldCPU
{
    internal sealed class Program
    {
        private const int ParticleCount = 100_000;
        
        private struct Particle
        {
            public Vector2 Position;
            public float Angle;
        }
        
        private static void Main(string[] args)
        {
            Assets.AddAssembly(Assembly.GetExecutingAssembly());

            using (Window window = new Window(1080, 720, "Flow field"))
            {
                window.MakeCurrent();
                GLLoader.LoadBindings(new GLFWBindingsContext());
                var random = new Random();

                Particle[] particles = new Particle[ParticleCount];
                for (int i = 0; i < particles.Length; i++)
                {
                    Vector2 position = new Vector2((float)random.NextDouble(), (float)random.NextDouble()) * 2 - Vector2.One;
                    float angle = (float)random.NextDouble() * MathF.PI * 2;
                    particles[i] = new Particle() {Position = position, Angle = angle};
                }
                ShaderProgram shader = ShaderProgram.CreateVertexFragment("shader.vert", "shader.frag");

                BufferObject<Particle> vbo = new BufferObject<Particle>(particles);
                VertexArray vao = new VertexArray();
                vao.AddVertexAttribute(vbo, shader.GetAttributeLocation("vPosition"), 2, VertexAttribType.Float);
                GL.ClearColor(0.1f, 0.1f, 0.1f, 1);
                
                Stopwatch stopwatch = new Stopwatch();
                float z = (float)random.NextDouble();
                while (window.IsRunning)
                {
                    stopwatch.Stop();
                    float dt = (float)Stopwatch.Frequency / stopwatch.ElapsedTicks;
                    window.Title = $"Flow field [FPS: {dt}]";
                    stopwatch.Reset();
                    stopwatch.Start();
                    GL.Clear(ClearBufferMask.ColorBufferBit);

                    for (var i = 0; i < particles.Length; i++)
                    {
                        float angle = particles[i].Angle;
                        Vector2 position = particles[i].Position;
                        position += new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * 0.01f;

                        if (position.X < -1)
                            position.X = 1;
                        else if (position.X > 1)
                            position.X = -1;
                        if (position.Y < -1)
                            position.Y = 1;
                        else if (position.Y > 1)
                            position.Y = -1;
                        
                        // if (position.X < -1 || position.X > 1 || position.Y < -1 || position.Y > 1)
                        // {
                        //     position = new Vector2((float)random.NextDouble(), (float)random.NextDouble()) * 2 - Vector2.One;
                        //     angle = (float)random.NextDouble() * MathF.PI * 2;
                        //     particles[i] = new Particle() {Position = position, Angle = angle, FramesAlive = 0};
                        //     continue;
                        // }

                        float perlin = (float) Perlin.Perlin3D((1 + position.X) * 8, (1 + position.Y) * 8, z);
                        particles[i].Angle = MathHelper.Lerp(angle,  perlin * 32, 0.08f);
                        particles[i].Position = position;
                    }
                    z += 0.001f;
                    vbo.SubData(particles);

                    vao.Bind();
                    shader.Bind();
                    GL.DrawArrays(PrimitiveType.Points, 0, particles.Length);
                    
                    window.SwapBuffers();
                    
                    Window.PollInput();
                }
            }
        }
    }
}