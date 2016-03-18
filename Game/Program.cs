using System;

namespace Engine
{
#if WINDOWS || XBOX
    class Program
    {
        static void Main(string[] args)
        {
            using (Graven rpg = new Graven())
            {
                rpg.Run();
            }
        }
    }
#endif
} 
