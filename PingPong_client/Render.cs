﻿using System;
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
        private static int countInfo = 0;

        private static int countChars = 0;
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
                }
            }

            Console.BackgroundColor = ConsoleColor.Blue;
            for (int i = Helper.origRow; i < ConsoleSettings.heightStatistics; i++) {
                Console.SetCursorPosition(ConsoleSettings.widthGame, i);
                for (int j = ConsoleSettings.widthGame; j < ConsoleSettings.widthStatistics; j++) {
                    if (j == ConsoleSettings.widthGame) {
                        Console.Write("|");
                        continue;
                    }
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
                        continue;
                    }
                    Console.Write(" ");
                }
            }

            Console.BackgroundColor = ConsoleColor.DarkBlue;

            // Filling the chat input area
            for (int i = ConsoleSettings.heightConsole - 1; i < ConsoleSettings.heightConsole; i++) {
                for (int j = Helper.origCol; j < ConsoleSettings.widthConsole; j++) {
                    if (i == ConsoleSettings.heightConsole - 1) {
                        Helper.WriteAt("-", j, i);
                        continue;
                    }
                    Helper.WriteAt(" ", j, i);
                }
            }
        }
        public static void RenderGame(int posLeftRacket, int posRightRacket, int[] posBall) {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.White;

            for (int i = Helper.origRow; i < ConsoleSettings.heightGame - 1; i++) {
                Console.SetCursorPosition(Helper.origCol, i);
                for (int j = Helper.origCol; j < ConsoleSettings.widthGame; j++) {
                    Console.Write(" ");
                }
            }

            Helper.WriteAt("|", 0, posLeftRacket);
            Helper.WriteAt("|", 59, posRightRacket);
            Helper.WriteAt("@", posBall[0], posBall[1]);
        }
        public static void FastRenderGame(int posLeftRacket, int posRightRacket, int[] posBall, int oldPosLeft, int oldPosRight, int[] oldPosBall) {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.White;

            for (int i = 0; i < ConsoleSettings.heightGame - 1; i++) {
                Helper.WriteAt(" ", 0, i);
            }

            for (int i = 0; i < ConsoleSettings.heightGame - 1; i++) {
                Helper.WriteAt(" ", 59, i);
            }

            Helper.WriteAt(" ", oldPosBall[0], oldPosBall[1]);

            Helper.WriteAt("|", 0, posLeftRacket);
            Helper.WriteAt("|", 59, posRightRacket);
            Helper.WriteAt("@", posBall[0], posBall[1]);
        }
        public static void RenderStatisticZone(string leftName, string rightName, int pointLeft, int pointRight) {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;

            if (leftName.Length > 18) {
                leftName = "Such a big boy";
            } else if (rightName.Length > 18) {
                rightName = "kto toot";
            }

            Helper.WriteAt(leftName, ConsoleSettings.widthGame + 2, 2);
            //Helper.WriteAt("-", 79, 2);
            for (int i = 1; i < 6; i++) {
                Helper.WriteAt("|", 79, i);
            }
            Helper.WriteAt(rightName, ConsoleSettings.widthGame + 22, 2);

            Helper.WriteAt(pointLeft.ToString(), 69, 5);
            Helper.WriteAt(pointRight.ToString(), 89, 5);

            Helper.WriteAt("---------------------------------------", 61, 6);
        }
        public static void RenderStatistic(int pointLeft, int pointRight) {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
            Helper.WriteAt("\t\t\t\t\t\t\t\t", 62, 5);
            Helper.WriteAt(pointLeft.ToString(), 69, 5);
            Helper.WriteAt(pointRight.ToString(), 89, 5);
        }
        public static void RenderError(string error) {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.White;
            Helper.WriteAt(error, ConsoleSettings.widthConsole / 2 - error.Length, ConsoleSettings.heightConsole / 2);
        }
        public static void RenderError() {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.White;
            Helper.WriteAt("Error", ConsoleSettings.widthConsole / 2, ConsoleSettings.heightConsole / 2);
        }
        public static void RenderStatisticInfo(string str) {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
            Helper.WriteAt(str, 69, 7 + countInfo);
            countInfo++;
        }
        public static void RenderStartPosion() {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.White;

            Helper.WriteAt("|", 0, 11);
            Helper.WriteAt("|", 59, 11);
            Helper.WriteAt("0", 29, 11);
        }
        public static void RenderTime(int time) {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
            Helper.WriteAt(time.ToString(), 78, 0);
        }
        public static void RenderSendMessage(ConsoleKey key) {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            string ch = key.ToString();
            Helper.WriteAt(ch, countChars, 39);
            countChars++;
        }
        public static void RenderSendMessage(string msg) {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Helper.WriteAt(msg, countChars, 39);
            countChars++;
        }
        public static void RenderBackspace() {
            if (countChars > 1) {
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                countChars--;
                Helper.WriteAt(" ", countChars, 39);
            }
        }
        public static void RenderEnter() {
            string bspaces = "\t\t\t\t\t\t\t\t\t";

            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Helper.WriteAt(bspaces, 0, 39);
            countChars = 0;
        }
        public static void RenderChat(Chat chat) {
            string[] strs = chat.toStrings();
            Console.BackgroundColor = ConsoleColor.DarkCyan;

            int i = 0;
            foreach (string str in strs) {
                Helper.WriteAt("\t\t\t\t\t\t\t\t\t", 0, i + 24);
                Helper.WriteAt(str, 0, i + 24);
                i++;
            }
        }
    }
}
