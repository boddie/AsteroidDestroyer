using System;

namespace AsteroidDestroyer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (AsteroidDestroyer game = new AsteroidDestroyer())
            {
                game.Run();
            }
        }
    }
}

