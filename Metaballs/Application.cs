using System;
using Engine;
using Engine.Rendering;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Metaballs
{
    public sealed class Application : ApplicationBase
    {
        private const int BallCount = 16;
        private readonly VertexArray _vao;
        private readonly BufferObject<uint> _ebo;
        private readonly BufferObject<Vector2> _vbo;
        private readonly ShaderProgram _shader;
        private readonly Ball[] Balls;
        private readonly int[] BallLocations;

        private struct Ball
        {
            public Vector2 Position;
            public Vector2 Direction;

            public Ball(Vector2 position, Vector2 direction)
            {
                Position = position;
                Direction = direction;
            }
        }
        
        public Application(int width, int height, string title) : base(width, height, title)
        {
            _ebo = new BufferObject<uint>(0, 1, 2, 0, 2, 3);
            _vbo = new BufferObject<Vector2>( new Vector2(1, 1),
                new Vector2(1, -1),
                new Vector2(-1, -1),
                new Vector2(-1, 1));
            _vao = new VertexArray(_ebo);
            _vao.AddVertexAttribute(_vbo, 0, 2, VertexAttribType.Float);
            _shader = ShaderProgram.CreateVertexFragment("shader.vert", "shader.frag");

            Balls = new Ball[BallCount];
            BallLocations = new int[BallCount];
            for (int i = 0; i < BallCount; i++)
            {
                BallLocations[i] = _shader.GetUniformLocation($"uBalls[{i}]");
                Balls[i] = new Ball(RandomF.NextVector2(-1, 1), RandomF.NextVector2().Normalized());
            }

        }

        protected override void OnRender()
        {
            for (int i = 0; i < BallCount; i++)
            {
                Balls[i].Direction = Vector2.Lerp(Balls[i].Direction, RandomF.NextVector2(-1, 1).Normalized(), 0.01f).Normalized();
                Balls[i].Position += Balls[i].Direction * 0.01f;
                if (Balls[i].Position.X < -1 || Balls[i].Position.X > 1)
                {
                    Balls[i].Direction.X *= -1;
                }
                if (Balls[i].Position.Y < -1 || Balls[i].Position.Y > 1)
                {
                    Balls[i].Direction.Y *= -1;
                }
                Balls[i].Position = Vector2.Clamp(Balls[i].Position, -Vector2.One, Vector2.One);
                
                _shader.SetUniform(BallLocations[i], Balls[i].Position);
            }
            
            _vao.Bind();
            _shader.Bind();
            
            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, (IntPtr)0);
        }
    }
}