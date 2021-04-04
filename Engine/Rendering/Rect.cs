using System;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Engine.Rendering
{
    public sealed class Rect
    {
        private struct Point
        {
            private readonly Vector2 _position;
            private readonly Vector2 _textureCoordinates;

            public Point(Vector2 position, Vector2 textureCoordinates)
            {
                _position = position;
                _textureCoordinates = textureCoordinates;
            }
        }
        private static readonly VertexArray Vao;
        private static readonly BufferObject<Point> Vbo;
        private static readonly BufferObject<uint> Ebo;
        private static readonly ShaderProgram Shader;
        
        private static readonly int TextureLocation;
        private static readonly int ColorLocation;
        
        static Rect()
        {
            Shader = ShaderProgram.CreateVertexFragment("Assets.rect.vert", "Assets.rect.frag");
            TextureLocation = Shader.GetUniformLocation("uTexture");
            ColorLocation = Shader.GetUniformLocation("uColor");
            
            Ebo = new BufferObject<uint>(0, 1, 2, 0, 2, 3);
            Vbo = new BufferObject<Point>(new[]
            {
                new Point( new Vector2(1, 1), Vector2.One),
                new Point(new Vector2(1, -1), Vector2.UnitX), 
                new Point(new Vector2(-1, -1), Vector2.Zero), 
                new Point(new Vector2(-1, 1), Vector2.UnitY), 
                
            });
            Vao = new VertexArray(Ebo);
            Vao.AddVertexAttribute(Vbo, Shader.GetAttributeLocation("vPosition"), 2, VertexAttribType.Float, 0);
            Vao.AddVertexAttribute(Vbo, Shader.GetAttributeLocation("vTextureCoordinate"), 2, VertexAttribType.Float, 2 * sizeof(float));
            
        }
        
        private readonly Texture? _texture;
        private readonly Color4<Rgba>? _color;

        public Rect(Texture texture)
        {
            _texture = texture;
            _color = null;
        }

        public Rect(Color4<Rgba> color)
        {
            _texture = null;
            _color = color;
        }

        public void Draw()
        {
            Vao.Bind();
            Shader.Bind();
            if (_texture != null)
            {
                _texture.Bind(TextureUnit.Texture0);
            }
            else
            {
                Texture.White.Bind(TextureUnit.Texture0);
            }
            Shader.SetUniform(TextureLocation, 0);
            if (_color != null)
            {
                Shader.SetUniform(ColorLocation, _color.Value);
            }
            else
            {
                Shader.SetUniform(ColorLocation, Color4.White);
            }
            
            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, IntPtr.Zero);
        }
    }
}