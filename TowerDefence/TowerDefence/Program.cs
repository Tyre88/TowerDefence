using System;

namespace TowerDefence
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (TowerDefenceGame game = new TowerDefenceGame())
            {
                game.Run();
            }
        }
    }
#endif
}

