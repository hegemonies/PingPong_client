using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingPong_client {
    class SessionInfo {
        public string nickNameLeft { get; private set; }
        public string nickNameRight { get; private set; }
        public int GID { get; }
        public SessionStatus status { get; private set; }

        public SessionInfo(string nickNameLeft, string nickNameRight, int GID, SessionStatus status) {
            this.nickNameLeft = nickNameLeft;
            this.nickNameRight = nickNameRight;
            this.GID = GID;
            this.status = status;
        }

        public string toString() {
            return nickNameLeft + " " + nickNameRight + " " + GID + " " + status;
        }
    }

    enum SessionStatus {
        Free,
        Busy
    }
}
