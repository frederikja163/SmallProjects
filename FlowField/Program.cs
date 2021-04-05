namespace FlowField
{
    internal sealed class Program
    {
        private static void Main(string[] args)
        {
            using (Application app = new Application(1080, 720, "Flow field"))
            {
                app.Run();
            }
        }
    }
}