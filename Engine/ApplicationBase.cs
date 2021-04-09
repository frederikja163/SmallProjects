using System;
using System.Diagnostics;
using System.Reflection;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Window = Engine.Platform.Window;

namespace Engine
{
    public abstract class ApplicationBase : IDisposable
    {
        private readonly Window _window;
        public Window Window => _window;
        private readonly string _title;
        
        public ApplicationBase(int width, int height, string title)
        {
            Assets.AddAssembly(Assembly.GetCallingAssembly());

            _title = title;
            _window = new Window(width, height, title);
            _window.MakeCurrent();
            GLLoader.LoadBindings(new GLFWBindingsContext());
        }

        public void Run()
        {
            Stopwatch watch = new Stopwatch();
            while (_window.IsRunning)
            {
                watch.Restart();
                GL.Clear(ClearBufferMask.ColorBufferBit);
                
                OnRender();
                
                _window.SwapBuffers();
                
                Window.PollInput();
                
                watch.Stop();
                float milliseconds = watch.ElapsedTicks / (Stopwatch.Frequency / 1000f);
                _window.Title = _title + $" [{milliseconds:F5}]ms";

            }
        }

        protected abstract void OnRender();

        protected virtual void OnUnload(){}

        public void Dispose()
        {
            OnUnload();

            _window.Dispose();
        }
    }
}