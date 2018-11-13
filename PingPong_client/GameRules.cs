using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PingPong_client {
    class GameRules {
        Socket serverSocket;
        byte[] sdata;
        byte[] rdata;
        int scoreLeft = 0;
        int scoreRight = 0;
        public GameRules(Socket serverSocket) {
            this.serverSocket = serverSocket;
        }

        public void Start() {
            Render.RenderStartPosion();
            NetworkStream stream = new NetworkStream(serverSocket);
            string answer;

            sdata = Encoding.Default.GetBytes("READY");
            stream.Write(sdata, 0, sdata.Length);

            stream.Read(rdata, 0, 5);
            answer = Encoding.Default.GetString(rdata);
            Helper.DeleteSpaces(ref answer);

            if (answer == "BAD") {
                stream.Close();
                Render.RenderError("BAD");
                return;
            } else if (answer == "START") {
                while (scoreLeft < 5 && scoreRight < 5) {

                }
            }

            if (scoreLeft == 5) {
                Render.RenderStatisticInfo("Left win");
            } else if (scoreRight == 5) {
                Render.RenderStatisticInfo("Right win");
            }
            Render.RenderStatisticInfo("Finish the game");

            stream.Close();
        }
    }
}
