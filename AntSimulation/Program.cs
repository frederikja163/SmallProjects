using System;

namespace AntSimulation
{
    class Program
    {
        static void Main(string[] args)
        {
            Application app = new Application(800, 600, "AntSimulation");
            app.Run();
            app.Dispose();
        }
    }
}