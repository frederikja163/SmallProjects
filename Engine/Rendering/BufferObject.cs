using System;
using OpenTK.Graphics.OpenGL;

namespace Engine.Rendering
{
    public sealed class BufferObject<T> : IDisposable
     where T : unmanaged
    {
        internal readonly uint Handle;

        public BufferObject(params T[] data)
        {
            Handle = GL.CreateBuffer();
            GL.NamedBufferStorage(Handle, data, BufferStorageMask.DynamicStorageBit);
        }

        public BufferObject(int size)
        {
            Handle = GL.CreateBuffer();
            unsafe
            {
                GL.NamedBufferStorage(Handle, size * sizeof(T), IntPtr.Zero, BufferStorageMask.DynamicStorageBit);
            }
        }

        public void SubData(T[] data, int offset = 0)
        {
            unsafe
            {
                GL.NamedBufferSubData(Handle, (IntPtr)(offset * sizeof(T)), data.Length * sizeof(T), data[offset]);
            }
        }
        
        public void Dispose()
        {
            GL.DeleteBuffer(Handle);
        }
    }
}