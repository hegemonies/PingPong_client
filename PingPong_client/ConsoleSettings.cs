using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingPong_client {
    class ConsoleSettings {
        public static int widthConsole { get; private set; }
        public static int heightConsole { get; private set; }

        public static int widthGame { get; private set; }
        public static int heightGame { get; private set; }

        public static int widthChat { get; private set; }
        public static int heightChat { get; private set; }

        public static int widthStatistics { get; private set; }
        public static int heightStatistics { get; private set; }
        public static void Initial() {
            Console.Title = "Network Ping Pong with chatting";

            widthConsole = 100;
            heightConsole = 40;

            widthGame = (int)(widthConsole * 0.6);
            heightGame = (int)(heightConsole * 0.6);

            widthChat = widthConsole;
            heightChat = heightConsole - heightGame - 1;

            widthStatistics = widthConsole;
            heightStatistics = heightGame;

            Console.SetBufferSize(100, 40);
            Console.SetWindowSize(widthConsole, heightConsole);

            Render.RenderWelcomeZone();
        }
    }
}
