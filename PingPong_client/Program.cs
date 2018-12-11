using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PingPong_client {
    class Program {
        static private int gamePortServer = 7902;
        static private int chatPortServer = 7903;
        static private string ip_server = "127.0.0.1";
        static List<SessionInfo> sessions = new List<SessionInfo>();
        static void Main(string[] args) {
            Console.BackgroundColor = ConsoleColor.Black;
            ConsoleSettings.Initial();

            try {
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(ip_server), gamePortServer);
                Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                serverSocket.Connect(ipPoint);

                IPEndPoint chatIpPoint = new IPEndPoint(IPAddress.Parse(ip_server), chatPortServer);
                Socket chatServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                chatServerSocket.Connect(chatIpPoint);

                string nickName = "";
                do {
                    Render.RenderRedWelcomeZone();
                    string welcomeEnter = "Enter you nick name: ";
                    Console.SetCursorPosition(50 - welcomeEnter.Length, 20);
                    Console.Write(welcomeEnter);
                    nickName = Console.ReadLine();
                } while (nickName == "");
                
                serverSocket.Send(Encoding.Default.GetBytes(nickName));

                NetworkStream stream = new NetworkStream(serverSocket);
                byte[] sdata;

                Render.RenderRedWelcomeZone();
                Helper.WriteAt("Getting a game list...", 39, 10);

                sdata = Encoding.Default.GetBytes("GETLIST");
                stream.Write(sdata, 0, sdata.Length);

                byte[] rdata = new byte[1150];
                stream.Read(rdata, 0, 1150);
                
                string str_rdata = Encoding.Default.GetString(rdata);
                str_rdata = Helper.DeleteSpaces(str_rdata);
                string[] req;

                if (str_rdata == "BAD") {
                    Render.RenderRedWelcomeZone();
                    Console.SetCursorPosition(50, 20);
                    Console.Write("List is empty");
                    Console.SetCursorPosition(50, 21);
                    Console.Write("Press any key for exit");
                    Console.ReadKey();
                    stream.Close();
                    return;
                } else if (str_rdata == "EMPTY") {
                    Render.RenderRedWelcomeZone();
                    Helper.WriteAt("Welcome! You are the first player", 30, 5);
                    Helper.WriteAt("Creating the game...", 30, 6);
                    Array.Clear(sdata, 0, sdata.Length);
                    sdata = Encoding.Default.GetBytes("CREATEGAME");
                    stream.Write(sdata, 0, sdata.Length);
                    Thread.Sleep(2000);
                    Array.Clear(rdata, 0, rdata.Length);
                    serverSocket.Receive(rdata);
                    Render.RenderGame();
                    string nickOpponent = Encoding.Default.GetString(rdata);
                    nickOpponent = Helper.DeleteSpaces(nickOpponent);
                    Render.RenderStatisticZone(nickName, nickOpponent, 0, 0);
                    var GR = new GameRules(serverSocket, chatServerSocket, nickName, nickOpponent);
                    GR.Start(StartPosition.Left);
                } else {
                    string tmp = Encoding.Default.GetString(rdata);
                    tmp = Helper.DeleteSpaces(tmp);
                    req = tmp.Split(';');

                    if (req[0] == "BAD") {
                        Console.WriteLine("BAD");
                    } else {
                        ParseGameList(req);
                        Render.ShowList(sessions);
                    }
                }

                while (true) {
                    Console.SetCursorPosition(13, 34);
                    Console.Write("\t\t\t\t\t\t");
                    Console.SetCursorPosition(13, 34);
                    Console.Write("Choose a session (-cr to create game -r to reload list): ");
                    string answer = "";
                    do {
                        answer = Console.ReadLine();
                    } while (answer == "");

                    if (answer == "-cr") {
                        Render.RenderRedWelcomeZone();
                        Array.Clear(sdata, 0, sdata.Length);
                        sdata = Encoding.Default.GetBytes("CREATEGAME");
                        stream.Write(sdata, 0, sdata.Length);
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        Helper.WriteAt("Waiting opponent...", 30, 2);
                        Array.Clear(rdata, 0, rdata.Length);
                        serverSocket.Receive(rdata);
                        Render.RenderGame();
                        string nickOpponent = Encoding.Default.GetString(rdata);
                        nickOpponent = Helper.DeleteSpaces(nickOpponent);
                        Render.RenderStatisticZone(nickName, nickOpponent, 0, 0);
                        var GR = new GameRules(serverSocket, chatServerSocket, nickName, nickOpponent);
                        GR.Start(StartPosition.Left);
                        break;
                    } else if (answer == "-r") {
                        Array.Clear(sdata, 0, sdata.Length);
                        sdata = Encoding.Default.GetBytes("GETLIST");
                        stream.Write(sdata, 0, sdata.Length);

                        Array.Clear(rdata, 0, rdata.Length);
                        stream.Read(rdata, 0, 1150);

                        str_rdata = Encoding.Default.GetString(rdata);
                        str_rdata = Helper.DeleteSpaces(str_rdata);

                        req = Encoding.Default.GetString(rdata).Split(';');
                        ParseGameList(req);
                        Render.RenderRedWelcomeZone();
                        Render.ShowList(sessions);
                    } else {
                        int ans = int.Parse(answer);
                        if (ans > 20 || ans < 0) {
                            Helper.WriteAt("Error number (must be > 0 and < 20)", 30, 0);
                        } else {
                            int GID = sessions[ans].GID;
                            Array.Clear(sdata, 0, sdata.Length);
                            sdata = Encoding.Default.GetBytes("GOGAME;" + GID);
                            stream.Write(sdata, 0, sdata.Length);
                            
                            Render.RenderGame();
                            Array.Clear(rdata, 0, rdata.Length);
                            serverSocket.Receive(rdata);
                            string nickOpponent = Encoding.Default.GetString(rdata);
                            nickOpponent = Helper.DeleteSpaces(nickOpponent);

                            Render.RenderStatisticZone(nickOpponent, nickName, 0, 0);

                            var GR = new GameRules(serverSocket, chatServerSocket, nickName, nickOpponent);
                            GR.Start(StartPosition.Right);

                            break;
                        }
                    }
                }

                stream.Close();
            } catch (Exception exc) {
                Console.Clear();
                Console.WriteLine("\tSorry\nError: " + exc.Message);
            }

            Console.BackgroundColor = ConsoleColor.Red;
            Helper.WriteAt("Enter any key to exit", 50, 20);
            Console.ReadKey(true);
        }
        private static void ParseGameList(string[] strs) {
            foreach (string str in strs) {
                string tstr = Helper.DeleteSpaces(str);
                string[] substr = tstr.Split(',');

                if (substr.Length < 1) {
                    return;
                }

                SessionStatus status = SessionStatus.Free;
                int tmpGID = int.Parse(substr[2]);

                if (substr[3] == "Busy") {
                    status = SessionStatus.Busy;
                }

                if (!ExistSession(tmpGID)) {
                    sessions.Add(new SessionInfo(substr[0], substr[1], Int32.Parse(substr[2]), status));
                } else {
                    foreach (SessionInfo session in sessions) {
                        if (session.GID == tmpGID) {
                            session.change(substr[0], substr[1], status);
                        }
                    }

                }
            }
        }

        private static void ShowSessions() {
            foreach (SessionInfo session in sessions) {
                Console.WriteLine(session.toString());
            }
        }

        private static bool ExistSession(int GID) {
            foreach (SessionInfo session in sessions) {
                if (session.GID == GID) {
                    return true;
                }
            }

            return false;
        }
    }
}