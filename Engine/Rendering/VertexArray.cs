using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace Engine.Rendering
{
    public sealed class VertexArray : IDisposable
    {
        internal readonly uint Handle;
        private uint _totalVertexBuffers = 0;
        private readonly Dictionary<uint, uint> _vertexBufferBindings = new Dictionary<uint, uint>();

        public VertexArray(BufferObject<uint> ebo)
        {
            Handle = GL.CreateVertexArray();
            GL.VertexArrayElementBuffer(Handle, ebo.Handle);
        }

        public VertexArray()
        {
            Handle = GL.CreateVertexArray();
        }

        public void AddVertexAttribute<T>(BufferObject<T> vertexBuffer, int attribLocation, int count,
            VertexAttribType attribType, int relativeOffset = 0)
            where T : unmanaged
        {
            if (!_vertexBufferBindings.TryGetValue(vertexBuffer.Handle, out uint bufferBinding))
            {
                bufferBinding = ++_totalVertexBuffers;
                _vertexBufferBindings.Add(vertexBuffer.Handle, bufferBinding);
                unsafe
                {
                    GL.VertexArrayVertexBuffer(Handle, bufferBinding, vertexBuffer.Handle, IntPtr.Zero, sizeof(T));
                }
            }
            GL.VertexArrayAttribBinding(Handle, (uint)attribLocation, bufferBinding);
            GL.VertexArrayAttribFormat(Handle, (uint)attribLocation, count, attribType, false, (uint)relativeOffset);
            GL.EnableVertexArrayAttrib(Handle, (uint)attribLocation);
        }

        public void Bind()
        {
            GL.BindVertexArray(Handle);
        }

        public void Dispose()
        {
            GL.DeleteVertexArray(Handle);
        }
    }
}