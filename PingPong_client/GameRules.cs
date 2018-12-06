using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PingPong_client {
    class GameRules {
        Socket serverSocket;
        byte[] sendBuffer = new byte[2];
        byte[] recvBuffer = new byte[22];
        int interval = 500;
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

        public GameRules(Socket serverSocket, StartPosition startPosition) {
            this.serverSocket = serverSocket;
            this.startPosition = startPosition;
        }

        public void Start() {
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
                new Thread(StartSend).Start();
                new Thread(StartReceive).Start();
                PlayHandler();
            }
        }

        private void PlayHandler() {
            Mutex mutex = new Mutex();
            while (runGame) {
                //if (scoreLeft < 5 && scoreRight < 5) {
                //    break;
                //}

                ConsoleKey key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.DownArrow) {
                    if (startPosition == StartPosition.Left) {
                        if (posLeftRacket < 22) {
                            //oldPosLeftRacket = posLeftRacket;
                            mutex.WaitOne();
                            posLeftRacket++;
                            mutex.ReleaseMutex();
                        }
                    } else {
                        if (posRightRacket < 23) {
                            //oldPosRightRacket = posRightRacket;
                            mutex.WaitOne();
                            posRightRacket++;
                            mutex.ReleaseMutex();
                        }
                    }
                } else if (key == ConsoleKey.UpArrow) {
                    if (startPosition == StartPosition.Left) {
                        if (posLeftRacket > 1) {
                            //oldPosLeftRacket = posLeftRacket;
                            mutex.WaitOne();
                            posLeftRacket--;
                            mutex.ReleaseMutex();
                        }
                    } else {
                        if (posRightRacket > 1) {
                            //oldPosRightRacket = posRightRacket;
                            mutex.WaitOne();
                            posRightRacket--;
                            mutex.ReleaseMutex();
                        }
                    }
                }
                //Thread.Sleep(interval);

                //Render.RenderGame(posLeftRacket, posRightRacket, posBall);                
            }

            if (scoreLeft == 5) {
                Render.RenderStatisticInfo("Left win");
            } else if (scoreRight == 5) {
                Render.RenderStatisticInfo("Right win");
            }
            Render.RenderStatisticInfo("Finish the game");
        }

        public void StartSend() {
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
        public void StartReceive() {
            //int spec_interval = interval + 200;
            int spec_interval = 0;
            long elapsedTimeToStringParse = 0;

            while (true) {
                try {
                    //Thread.Sleep(spec_interval - (int)elapsedTimeToStringParse);
                    Thread.Sleep(spec_interval);
                    serverSocket.Receive(recvBuffer);

                    Stopwatch sw = new Stopwatch();
                    sw.Start();

                    string raw = Encoding.Default.GetString(recvBuffer);
                    //Helper.WriteAt(raw, 0, 35);
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
                        oldPosLeftRacket = posLeftRacket;
                        oldPosRightRacket = posRightRacket;
                        
                        if (startPosition == StartPosition.Left) {
                            //posRightRacket = int.Parse(raws[2]);
                            int tmp = int.Parse(raws[2]);
                            if (tmp < 24 || tmp >= 0) {
                                posRightRacket = tmp;
                            }
                        } else {
                            //posLeftRacket = int.Parse(raws[1]);
                            int tmp = int.Parse(raws[1]);
                            if (tmp < 24 || tmp >= 0) {
                                posLeftRacket = int.Parse(raws[1]);
                            }
                        }

                        string[] strPosBall = raws[3].Split(',');
                        oldPosBall[0] = posBall[0];
                        oldPosBall[1] = posBall[1];
                        posBall[0] = int.Parse(strPosBall[0]);
                        posBall[1] = int.Parse(strPosBall[1]);
                        string[] strScores = raws[4].Split(',');
                        scoreLeft = int.Parse(strScores[0]);
                        scoreRight = int.Parse(strScores[1]);

                        //Console.SetCursorPosition(0, 25);
                        //Console.WriteLine("Pos left: " + posLeftRacket + ".");
                        //Console.WriteLine("Pos right: " + posRightRacket + ".");
                        //Console.WriteLine("Pos ball: " + posBall[0] + " " + posBall[1] + ".");
                    }

                    sw.Stop();
                    elapsedTimeToStringParse = sw.ElapsedMilliseconds / 100;

                    //Render.RenderGame(posLeftRacket, posRightRacket, posBall);
                    Render.FastRenderGame(posLeftRacket, posRightRacket, posBall, oldPosLeftRacket, oldPosRightRacket, oldPosBall);
                    Render.RenderStatistic(scoreLeft, scoreRight);
                    Render.RenderTime(time);
                    time++;
                } catch {

                }
            }
        }
    }
}
