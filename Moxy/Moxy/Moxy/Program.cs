using System;

namespace Moxy
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Moxy game = new Moxy())
            {
                game.Run();
            }
        }
    }
#endif
}

