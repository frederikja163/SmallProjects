using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Graphics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using GlfwWindow = OpenTK.Windowing.GraphicsLibraryFramework.Window;

namespace Engine.Platform
{
    public sealed class Window : IDisposable
    {
        internal unsafe struct WindowHandle
        {
            internal readonly GlfwWindow* Handle;

            internal WindowHandle(GlfwWindow* handle)
            {
                Handle = handle;
            }

            public static implicit operator GlfwWindow*(WindowHandle handle) => handle.Handle;

            public static implicit operator WindowHandle(GlfwWindow* handle) => new WindowHandle(handle);
        }
        
        private static readonly Dictionary<WindowHandle, Window> _handleToWindow =
            new Dictionary<WindowHandle, Window>();

        internal WindowHandle Handle;

        public Window(int width, int height, string title)
        {
            if (!_handleToWindow.Any())
            {
                if (!GLFW.Init())
                {
                    throw new Exception("Glfw failed to initialize!");
                }
                GLLoader.LoadBindings(new GLFWBindingsContext());
            }
            
            GLFW.WindowHint(WindowHintOpenGlProfile.OpenGlProfile, OpenGlProfile.Core);
            GLFW.WindowHint(WindowHintInt.ContextVersionMajor, 4);
            GLFW.WindowHint(WindowHintInt.ContextVersionMinor, 5);
            unsafe
            {
                Handle = GLFW.CreateWindow(width, height, title, null, null);
            }
            _handleToWindow.Add(Handle, this);
        }
        
        public bool IsRunning
        {
            get
            {
                unsafe
                {
                    return !GLFW.WindowShouldClose(Handle);
                }
            }
            set
            {
                unsafe
                {
                    GLFW.SetWindowShouldClose(Handle, !value);
                }
            }
        }

        public void MakeCurrent()
        {
            unsafe
            {
                GLFW.MakeContextCurrent(Handle);
            }
        }

        public void SwapBuffers()
        {
            unsafe
            {
                GLFW.SwapBuffers(Handle);
            }
        }

        public void Dispose()
        {
            _handleToWindow.Remove(Handle);
            unsafe
            {
                GLFW.DestroyWindow(Handle);
            }

            if (!_handleToWindow.Any())
            {
                GLFW.Terminate();
            }
        }

        public static void PollInput()
        {
            GLFW.PollEvents();
        }
    }
}