using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingPong_client {
    class Chat {
        Dictionary<int, ClientChatWrapper> messages = new Dictionary<int, ClientChatWrapper>();
        int count = 0;
        public void AddMsg(string from, string msg) {
            messages.Add(count, new ClientChatWrapper(from, msg));
            count++;
        }
        public string[] toStrings() {
            string[] strs = new string[messages.Count];

            int j = 0;
            for (int i = (count < 12) ? 0 : (count - 12); i < messages.Count; i++, j++) {
                strs[j] = messages[i].toString();
            }

            return strs;
        }
    }
}
