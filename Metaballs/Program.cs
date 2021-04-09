using System;

namespace Metaballs
{
    class Program
    {
        static void Main(string[] args)
        {
            Application app = new Application(800, 600, "Metaballs");
            app.Run();
            app.Dispose();
        }
    }
}