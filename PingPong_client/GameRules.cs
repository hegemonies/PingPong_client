using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PingPong_client {
    class GameRules {
        Socket serverSocket;
        Socket chatSocket;
        byte[] sendBuffer = new byte[2];
        byte[] recvBuffer = new byte[22];
        int interval = 400;
        int posLeftRacket = 11;
        int posRightRacket = 11;
        int oldPosLeftRacket = 11;
        int oldPosRightRacket = 11;
        int[] oldPosBall = new int[2];
        int[] posBall = new int[2];
        string act;
        int scoreLeft = 0;
        int scoreRight = 0;
        StartPosition startPosition;
        bool runGame = true;
        int time = 0;
        SoundPlayer sound = new SoundPlayer(@"scream.wav");
        Chat chat = new Chat();
        string selfNick;
        string opponentNick;
        Stopwatch sw = new Stopwatch();
        bool restartTimer = false;

        public GameRules(Socket serverSocket, Socket chatSocket, string selfNick, string opponentNick) {
            this.serverSocket = serverSocket;
            this.chatSocket = chatSocket;
            this.selfNick = selfNick;
            this.opponentNick = opponentNick;
            sound.Load();
        }

        public void Start(StartPosition startPosition) {
            this.startPosition = startPosition;
            Render.RenderStartPosion();

            NetworkStream stream = new NetworkStream(serverSocket);

            string answer;

            sendBuffer = Encoding.Default.GetBytes("READY");
            stream.Write(sendBuffer, 0, sendBuffer.Length);

            stream.Read(recvBuffer, 0, 5);
            answer = Encoding.Default.GetString(recvBuffer);
            answer = Helper.DeleteSpaces(answer);

            Thread.Sleep(2000);

            stream.Close();

            if (answer == "BAD") {
                Render.RenderError("BAD");
            } else if (answer == "START") {
                new Thread(TimerHandler).Start();
                new Thread(StartSend).Start();
                new Thread(StartReceive).Start();
                new Thread(ChatHandler).Start();
                PlayHandler();
            }
        }
        private void PlayHandler() {
            Mutex mutex = new Mutex();
            Render.RenderStatisticInfo("Start");
            string message = "";
            while (runGame) {
                ConsoleKey key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.DownArrow) {
                    if (startPosition == StartPosition.Left) {
                        if (posLeftRacket < 22) {
                            mutex.WaitOne();
                            posLeftRacket++;
                            mutex.ReleaseMutex();
                        }
                    } else {
                        if (posRightRacket < 23) {
                            mutex.WaitOne();
                            posRightRacket++;
                            mutex.ReleaseMutex();
                        }
                    }
                } else if (key == ConsoleKey.UpArrow) {
                    if (startPosition == StartPosition.Left) {
                        if (posLeftRacket > 1) {
                            mutex.WaitOne();
                            posLeftRacket--;
                            mutex.ReleaseMutex();
                        }
                    } else {
                        if (posRightRacket > 1) {
                            mutex.WaitOne();
                            posRightRacket--;
                            mutex.ReleaseMutex();
                        }
                    }
                } else if (isChar(key)) {
                    if (message.Length < 60) {
                        message += key.ToString();
                        mutex.WaitOne();
                        Render.RenderSendMessage(key);
                        mutex.ReleaseMutex();
                    }
                } else if (key == ConsoleKey.Spacebar) {
                    if (message.Length < 60) {
                        message += " ";
                        mutex.WaitOne();
                        Render.RenderSendMessage(" ");
                        mutex.ReleaseMutex();
                    }
                } else if (key == ConsoleKey.Enter) {
                    if (message.Length > 0) {
                        chat.AddMsg(selfNick, message);
                        chatSocket.Send(Encoding.Default.GetBytes(message));
                        message = "";
                        mutex.WaitOne();
                        Render.RenderEnter();
                        Render.RenderChat(chat);
                        mutex.ReleaseMutex();
                    }
                } else if (key == ConsoleKey.Backspace) {
                    if (message.Length > 1) {
                        message = message.Substring(0, message.Length - 1);
                        mutex.WaitOne();
                        Render.RenderBackspace();
                        mutex.ReleaseMutex();
                    }
                }
            }

            if (scoreLeft == 5) {
                Render.RenderStatisticInfo("Left win");
            } else if (scoreRight == 5) {
                Render.RenderStatisticInfo("Right win");
            }
            Render.RenderStatisticInfo("Finish the game");
        }
        private void StartSend() {
            while (true) {
                try {
                    Thread.Sleep(interval);
                    if (startPosition == StartPosition.Left) {
                        sendBuffer = Encoding.Default.GetBytes(posLeftRacket.ToString());
                    } else {
                        sendBuffer = Encoding.Default.GetBytes(posRightRacket.ToString());
                    }
                    serverSocket.Send(sendBuffer);
                } catch {

                }
            }
        }
        double passedTime;
        private void TimerHandler() {
            sw.Start();
            Mutex mutex = new Mutex();

            while (true) {
                passedTime = sw.ElapsedMilliseconds / 1000;
                //mutex.WaitOne();
                //Helper.WriteAt(passedTime.ToString(), 65, 15);
                //mutex.ReleaseMutex();
                if (restartTimer) {
                    sw.Start();
                    restartTimer = false;
                } else if (passedTime > 3) {
                    Render.RenderError("Second player not responding");
                    runGame = false;
                    break;
                }
                Thread.Sleep(50);
            }

            sw.Stop();
        }
        private void StartReceive() {
            int spec_interval = 0;

            Mutex mutex = new Mutex();

            while (runGame) {
                try {
                    Thread.Sleep(spec_interval);
                    serverSocket.Receive(recvBuffer);

                    mutex.WaitOne();
                    //restartTimer = true;
                    sw.Restart();
                    mutex.ReleaseMutex();

                    string raw = Encoding.Default.GetString(recvBuffer);

                    raw = Helper.DeleteSpaces(raw);
                    string[] raws = raw.Split(';');
                    act = raws[0];

                    if (act == "ACTION") {
                        string sub_action = raws[1];
                        if (sub_action == "LOSE") {
                            if (startPosition == StartPosition.Left) {
                                scoreRight++;
                            } else {
                                scoreLeft++;
                            }
                        } else if (sub_action == "WIN") {
                            if (startPosition == StartPosition.Left) {
                                scoreLeft++;
                            } else {
                                scoreRight++;
                            }
                        } else if (sub_action == "END") {
                            runGame = false;
                        }
                    } else if (act == "MOTION") {

                        if (startPosition == StartPosition.Left) {
                            int tmp = int.Parse(raws[2]);
                            if (tmp < 24 && tmp >= 0) {
                                posRightRacket = tmp;
                                oldPosRightRacket = posRightRacket;
                            }
                        } else {
                            int tmp = int.Parse(raws[1]);
                            if (tmp < 24 && tmp >= 0) {
                                posLeftRacket = tmp;
                                oldPosLeftRacket = posLeftRacket;
                            }
                        }

                        string[] strPosBall = raws[3].Split(',');
                        oldPosBall[0] = posBall[0];
                        oldPosBall[1] = posBall[1];
                        posBall[0] = int.Parse(strPosBall[0]);
                        posBall[1] = int.Parse(strPosBall[1]);

                        if (oldPosBall[0] == 58 & posBall[0] == 57) {
                            sound.Play();
                        } else if (oldPosBall[0] == 1 & posBall[0] == 2) {
                            sound.Play();
                        }

                        string[] strScores = raws[4].Split(',');
                        scoreLeft = int.Parse(strScores[0]);
                        scoreRight = int.Parse(strScores[1]);
                    }

                    //Mutex mutex = new Mutex();
                    mutex.WaitOne();

                    Render.FastRenderGame(posLeftRacket, posRightRacket, posBall, oldPosLeftRacket, oldPosRightRacket, oldPosBall);
                    Render.RenderStatistic(scoreLeft, scoreRight);
                    Render.RenderTime(time);

                    mutex.ReleaseMutex();

                    time++;
                } catch {

                }
            }
        }
        private bool isChar(ConsoleKey key) {
            if (key == ConsoleKey.A ||
                key == ConsoleKey.B ||
                key == ConsoleKey.C ||
                key == ConsoleKey.D ||
                key == ConsoleKey.E ||
                key == ConsoleKey.F ||
                key == ConsoleKey.G ||
                key == ConsoleKey.H ||
                key == ConsoleKey.I ||
                key == ConsoleKey.J ||
                key == ConsoleKey.K ||
                key == ConsoleKey.L ||
                key == ConsoleKey.M ||
                key == ConsoleKey.N ||
                key == ConsoleKey.O ||
                key == ConsoleKey.P ||
                key == ConsoleKey.Q ||
                key == ConsoleKey.R ||
                key == ConsoleKey.S ||
                key == ConsoleKey.T ||
                key == ConsoleKey.U ||
                key == ConsoleKey.V ||
                key == ConsoleKey.W ||
                key == ConsoleKey.X ||
                key == ConsoleKey.Y ||
                key == ConsoleKey.Z) {
                return true;
            }

            return false;
        }
        private void ChatHandler() {
            int sizeChatMessage = 100;
            byte[] chatBuffer = new byte[sizeChatMessage];
            while (runGame) {
                try {
                    Array.Clear(chatBuffer, 0, sizeChatMessage);
                    chatSocket.Receive(chatBuffer);
                    chat.AddMsg(opponentNick, Encoding.Default.GetString(chatBuffer));
                    Render.RenderChat(chat);
                } catch { }
            }
        }
    }
}
