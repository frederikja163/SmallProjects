using System;
using System.Reflection;
using Engine;
using Engine.Rendering;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Window = Engine.Platform.Window;

namespace Sandbox
{
    internal sealed class Program
    {
        private static void Main(string[] args)
        {
            using Window window = new Window(800, 600, "Sandbox");
            window.MakeCurrent();
            GLLoader.LoadBindings(new GLFWBindingsContext());
            GL.ClearColor(1, 0, 1, 1);
            
            Assets.AddAssembly(Assembly.GetExecutingAssembly());
            
            using VertexShader vertexShader = Assets.GetAsset<VertexShader>("Assets.shader.vert");
            using FragmentShader fragmentShader = Assets.GetAsset<FragmentShader>("Assets.shader.frag");
            using ShaderProgram shader = new ShaderProgram(vertexShader, fragmentShader);

            using BufferObject<uint> ebo = new BufferObject<uint>(0, 1, 2);
            using BufferObject<Vector3> vbo = new BufferObject<Vector3>(Vector3.One, Vector3.UnitX, Vector3.Zero);
            using VertexArray vao = new VertexArray(ebo);
            vao.AddVertexAttribute(vbo, shader.GetAttributeLocation("vPosition"), 3, VertexAttribType.Float);

            while (window.IsRunning)
            {
                GL.Clear(ClearBufferMask.ColorBufferBit);
                
                vao.Bind();
                shader.Bind();
                GL.DrawElements(PrimitiveType.Triangles, 3, DrawElementsType.UnsignedInt, IntPtr.Zero);

                window.SwapBuffers();
                
                Window.PollInput();
            }
        }
    }
}