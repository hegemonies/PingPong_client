﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingPong_client {
    class ClientChatWraper {
        public string Who { get; private set; }
        public string Message { get; private set; }
        public ClientChatWraper(string Who, string Message) {
            this.Who = Who;
            this.Message = Message;
        }
        public string toString() {
            return Who + ": " + Message;
        }
    }
}
