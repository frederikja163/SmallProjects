using System;
using System.Diagnostics.Contracts;
using System.IO;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Engine.Rendering
{
    public abstract class ShaderStage : IDisposable
    {
        internal readonly ShaderHandle Handle;
        
        protected ShaderStage(ShaderType type, StreamReader stream)
        {
            string src = stream.ReadToEnd();
            
            Handle = GL.CreateShader(type);
            GL.ShaderSource(Handle, src);
            GL.CompileShader(Handle);

            // int infoLogLength = 0;
            // GL.GetShaderiv(Handle, ShaderParameterName.InfoLogLength, infoLogLength);
            // if (infoLogLength > 0)
            // {
            //     int _ = 0;
            //     string infoLog = GL.GetShaderInfoLog(Handle, infoLogLength, ref _);
            //     throw new Exception($"{type} failed compilation: {infoLog}");
            // }
        }
        
        public void Dispose()
        {
            GL.DeleteShader(Handle);
        }
    }
    
    public sealed class VertexShader : ShaderStage
    {
        public VertexShader(StreamReader stream) : base(ShaderType.VertexShader, stream)
        {
        }
    }
    public sealed class FragmentShader : ShaderStage
    {
        public FragmentShader(StreamReader stream) : base(ShaderType.FragmentShader, stream)
        {
        }
    }

    public sealed class ComputeShader : ShaderStage
    {
        public ComputeShader(StreamReader stream) : base(ShaderType.ComputeShader, stream)
        {
        }
    }

    public sealed class ShaderProgram : IDisposable
    {
        internal readonly ProgramHandle Handle;

        [Pure]
        public static ShaderProgram CreateVertexFragment(string vertexPath, string fragmentPath)
        {
            using VertexShader vertex = Assets.GetAsset<VertexShader>(vertexPath);
            using FragmentShader fragment = Assets.GetAsset<FragmentShader>(fragmentPath);

            return new ShaderProgram(vertex, fragment);
        }

        public static ShaderProgram CreateCompute(string computePath)
        {
            using ComputeShader compute = Assets.GetAsset<ComputeShader>(computePath);

            return new ShaderProgram(compute);
        }

        public ShaderProgram(params ShaderStage[] shaderStage)
        {
            Handle = GL.CreateProgram();

            foreach (ShaderStage stage in shaderStage)
            {
                GL.AttachShader(Handle, stage.Handle);
            }
            
            GL.LinkProgram(Handle);

            int infoLogLength = 0;
            // GL.GetProgramiv(Handle, ProgramPropertyARB.InfoLogLength, infoLogLength);
            // if (infoLogLength > 0)
            // {
            //     int _ = 0;
            //     string infoLog = GL.GetProgramInfoLog(Handle, infoLogLength, ref _);
            //     throw new Exception($"Shaderprogram failed linking: {infoLog}");
            // }

            foreach (ShaderStage stage in shaderStage)
            {
                GL.DetachShader(Handle, stage.Handle);
            }
        }

        public void Bind()
        {
            GL.UseProgram(Handle);
        }
        
        public int GetAttributeLocation(string attributeName)
        {
            return GL.GetAttribLocation(Handle, attributeName);
        }
        public int GetUniformLocation(string uniformName)
        {
            return GL.GetUniformLocation(Handle, uniformName);
        }

        public void SetUniform(int location, in float value) => GL.ProgramUniform1fv(Handle, location, 1, value);
        public void SetUniform(int location, in int value) => GL.ProgramUniform1iv(Handle, location, 1, value);
        
        public void SetUniform(int location, in Vector2 value) => GL.ProgramUniform2f(Handle, location, value.X, value.Y);
        public void SetUniform(int location, in Vector2i value) => GL.ProgramUniform2i(Handle, location, value.X, value.Y);
        
        
        public void SetUniform(int location, in Vector3 value) => GL.ProgramUniform3f(Handle, location, value.X, value.Y, value.Z);
        public void SetUniform(int location, in Vector3i value) => GL.ProgramUniform3i(Handle, location, value.X, value.Y, value.Z);
        
        public void SetUniform(int location, in Vector4 value) => GL.ProgramUniform4f(Handle, location, value.X, value.Y, value.Z, value.W);
        public void SetUniform(int location, in Vector4i value) => GL.ProgramUniform4i(Handle, location, value.X, value.Y, value.Z, value.W);
        public void SetUniform(int location, in Color4<Rgba> value) => GL.ProgramUniform4f(Handle, location, value.X, value.Y, value.Z, value.W);

        public void SetUniform(int location, in Matrix4 value)
        {
            unsafe
            {
                fixed (float* ptr = &value.Row0.X)
                {
                    GL.ProgramUniformMatrix4fv(Handle, location, 1, 0, ptr);
                }
            }
        }
        
        public void Dispose()
        {
            GL.DeleteProgram(Handle);
        }
    }
}