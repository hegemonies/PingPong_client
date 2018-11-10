using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingPong_client {
    class Render {
        private static int startLeft = Helper.origCol + 10;
        private static int startTop = Helper.origRow + 5;
        private static int offsetLeft = 3;
        private static int offsetTop = 1;
        public static void ShowList(List<SessionInfo> sessions) {
            RenderRedWelcomeZone();
            if (sessions.Count == 0) {
                return;
            }

            Console.SetCursorPosition(startLeft + offsetLeft, startTop + offsetTop);
            Console.Write("Number");
            Console.SetCursorPosition(startLeft + offsetLeft + 10, startTop + offsetTop);
            Console.Write("Player0");
            Console.SetCursorPosition(startLeft + offsetLeft + 32, startTop + offsetTop);
            Console.Write("Player1");
            Console.SetCursorPosition(startLeft + offsetLeft + 55, startTop + offsetTop);
            Console.Write("GID");
            Console.SetCursorPosition(startLeft + offsetLeft + 70, startTop + offsetTop);
            Console.Write("Free");

            foreach (SessionInfo session in sessions) {
                Console.SetCursorPosition(startLeft + offsetLeft, startTop + sessions.IndexOf(session) + offsetLeft);
                Console.Write(sessions.IndexOf(session));

                Console.SetCursorPosition(startLeft + offsetLeft + 10, startTop + sessions.IndexOf(session) + offsetLeft);
                Console.Write(session.nickNameLeft);

                Console.SetCursorPosition(startLeft + offsetLeft + 32, startTop + sessions.IndexOf(session) + offsetLeft);
                Console.Write(session.nickNameRight);

                Console.SetCursorPosition(startLeft + offsetLeft + 55, startTop + sessions.IndexOf(session) + offsetLeft);
                Console.Write(session.GID);

                Console.SetCursorPosition(startLeft + offsetLeft + 70, startTop + sessions.IndexOf(session) + offsetLeft);
                Console.Write(session.status);
            }
        }

        public static void RenderWelcomeZone() {
            RenderBlueWelcomeZone();
            RenderRedWelcomeZone();
        }
        private static void RenderBlueWelcomeZone() {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.White;

            for (int i = Helper.origRow; i < ConsoleSettings.heightConsole; i++) {
                Console.SetCursorPosition(Helper.origCol, i);
                for (int j = Helper.origCol; j < ConsoleSettings.widthConsole; j++) {
                    Console.Write(" ");
                    //Helper.WriteAt(" ", j, i);
                }
            }
            Helper.WriteAt("", 0, 0);
        }
        public static void RenderRedWelcomeZone() {
            Console.BackgroundColor = ConsoleColor.DarkRed;

            for (int i = Helper.origRow + 5; i < ConsoleSettings.heightConsole - 5; i++) {
                Console.SetCursorPosition(Helper.origCol + 10, i);
                for (int j = Helper.origCol + 10; j < ConsoleSettings.widthConsole - 10; j++) {
                    Console.Write(" ");
                    //Helper.WriteAt(" ", j, i);
                }
            }
            Helper.WriteAt("", 0, 0);
        }
        public static void RenderGame() {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.White;

            // Filling the playing area
            for (int i = Helper.origRow; i < ConsoleSettings.heightGame; i++) {
                Console.SetCursorPosition(Helper.origCol, i);
                for (int j = Helper.origCol; j < ConsoleSettings.widthGame; j++) {
                    Console.Write(" ");
                    //Helper.WriteAt(" ", j, i);
                }
            }

            Console.BackgroundColor = ConsoleColor.Blue;
            // Filling the statistic area
            for (int i = Helper.origRow; i < ConsoleSettings.heightStatistics; i++) {
                Console.SetCursorPosition(ConsoleSettings.widthGame, i);
                for (int j = ConsoleSettings.widthGame; j < ConsoleSettings.widthStatistics; j++) {
                    if (j == ConsoleSettings.widthGame) {
                        //Helper.WriteAt("|", j, i);
                        Console.Write("|");
                        continue;
                    }
                    //Helper.WriteAt(" ", j, i);
                    Console.Write(" ");
                }
            }

            Console.BackgroundColor = ConsoleColor.DarkCyan;

            // Filling the chat output area
            for (int i = ConsoleSettings.heightGame; i < ConsoleSettings.heightGame + ConsoleSettings.heightChat; i++) {
                Console.SetCursorPosition(Helper.origCol, i);
                for (int j = Helper.origCol; j < ConsoleSettings.widthChat; j++) {
                    if (i == ConsoleSettings.heightGame) {
                        Console.Write("-");
                        //Helper.WriteAt("-", j, i);
                        continue;
                    }
                    Console.Write(" ");
                    //Helper.WriteAt(" ", j, i);
                }
            }

            Console.BackgroundColor = ConsoleColor.DarkMagenta;

            // Filling the chat input area
            for (int i = ConsoleSettings.heightConsole - 1; i < ConsoleSettings.heightConsole; i++) {
                for (int j = Helper.origCol; j < ConsoleSettings.widthConsole; j++) {
                    if (i == ConsoleSettings.heightConsole - 1) {
                        //Console.Write("-");
                        Helper.WriteAt("-", j, i);
                        continue;
                    }
                    Helper.WriteAt(" ", j, i);
                }
            }
        }

        public static void RenderStatisticZone(string leftName, string rightName, int pointLeft, int pointRight) {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;

            if (leftName.Length > 18) {
                leftName = "Such a big boy";
            } else if (rightName.Length > 18) {
                rightName = "-.-";
            }

            Helper.WriteAt(leftName, ConsoleSettings.widthGame + 2, 2);
            Helper.WriteAt("-", 79, 2);
            Helper.WriteAt(rightName, ConsoleSettings.widthGame + 22, 2);

            Helper.WriteAt(pointLeft.ToString(), 69, 5);
            Helper.WriteAt(pointRight.ToString(), 89, 5);
        }
    }
}
