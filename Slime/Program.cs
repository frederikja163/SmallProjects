﻿using System;

namespace Slime
{
    class Program
    {
        static void Main(string[] args)
        {
            Application app = new Application(800, 600, "Slime");
            app.Run();
            app.Dispose();
        }
    }
}