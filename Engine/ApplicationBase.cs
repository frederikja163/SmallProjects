using System;
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
        
        public ApplicationBase(int width, int height, string title)
        {
            Assets.AddAssembly(Assembly.GetCallingAssembly());
            
            _window = new Window(width, height, title);
            _window.MakeCurrent();
            GLLoader.LoadBindings(new GLFWBindingsContext());
        }

        public void Run()
        {

            while (_window.IsRunning)
            {
                GL.Clear(ClearBufferMask.ColorBufferBit);
                
                OnRender();
                
                _window.SwapBuffers();
                
                Window.PollInput();
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