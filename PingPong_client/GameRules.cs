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
        int interval = 0;
        int posLeftRacket = 0;
        int posRightRacket = 0;
        int[] posBall;
        string act;
        int scoreLeft = 0;
        int scoreRight = 0;
        StartPosition startPosition;

        public GameRules(Socket serverSocket, StartPosition startPosition) {
            this.serverSocket = serverSocket;
            this.startPosition = startPosition;
        }

        public void Start() {
            new Thread(StartSend).Start();
            new Thread(StartReceive).Start();

            Render.RenderStartPosion();

            NetworkStream stream = new NetworkStream(serverSocket);

            string answer;

            sendBuffer = Encoding.Default.GetBytes("READY");
            stream.Write(sendBuffer, 0, sendBuffer.Length);
            
            stream.Read(recvBuffer, 0, 5);
            answer = Encoding.Default.GetString(recvBuffer);
            answer = Helper.DeleteSpaces(answer);

            stream.Close();
            if (answer == "BAD") {
                Render.RenderError("BAD");
            } else if (answer == "START") {
                PlayHandler();
            }
        }

        private void PlayHandler() {
            while (true) {
                if (scoreLeft < 5 && scoreRight < 5) {
                    break;
                }

                ConsoleKey key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.DownArrow) {
                    if (startPosition == StartPosition.Left) {
                        if (posLeftRacket < 23) {
                            posLeftRacket++;
                        }
                    } else {
                        if (posRightRacket < 23) {
                            posRightRacket++;
                        }
                    }
                } else if (key == ConsoleKey.UpArrow) {
                    if (startPosition == StartPosition.Left) {
                        if (posLeftRacket > 1) {
                            posLeftRacket--;
                        }
                    } else {
                        if (posRightRacket > 1) {
                            posRightRacket--;
                        }
                    }
                }
                Render.RenderGame(posLeftRacket, posRightRacket, posBall);

                if (act == "") { // TOneverDO

                }
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
            int spec_interval = interval + 100;
            long elapsedTimeToStringParse = 0;

            while (true) {
                try {
                    Thread.Sleep(spec_interval - (int)elapsedTimeToStringParse);
                    serverSocket.Receive(recvBuffer);

                    Stopwatch sw = new Stopwatch();
                    sw.Start();

                    string raw = Encoding.Default.GetString(recvBuffer);
                    string[] raws = raw.Split(';');
                    act = raws[0];
                    posLeftRacket = int.Parse(raws[1]);
                    posRightRacket = int.Parse(raws[2]);
                    string[] strPosBall = raws[3].Split(',');
                    posBall[0] = int.Parse(strPosBall[0]);
                    posBall[1] = int.Parse(strPosBall[1]);
                    string[] strScores = raws[4].Split(',');
                    scoreLeft = int.Parse(strScores[0]);
                    scoreRight = int.Parse(strScores[1]);

                    sw.Stop();
                    elapsedTimeToStringParse = sw.ElapsedMilliseconds / 100;
                } catch {

                }
            }
        }
    }
}
