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
        private static VertexArray _vao;
        private static BufferObject<Point> _vbo;
        private static BufferObject<uint> _ebo;
        private static ShaderProgram _shader;

        static Rect()
        {
            _ebo = new BufferObject<uint>(0, 1, 2);
            _vbo = new BufferObject<Point>(new[]
            {
                new Point( new Vector2(1, 1), Vector2.One),
                new Point(new Vector2(1, -1), Vector2.UnitX), 
                new Point(new Vector2(-1, -1), Vector2.Zero), 
                new Point(new Vector2(-1, 1), Vector2.UnitY), 
                
            });
            _vao = new VertexArray(_ebo);
        }
    }
}