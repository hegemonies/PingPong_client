using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingPong_client {
    class Helper {
        public static int origRow = Console.CursorTop;
        public static int origCol = Console.CursorLeft;
        public static string DeleteSpaces(string str) {
            int countSpaces = 0;
            for (int i = str.Length - 1; !char.IsWhiteSpace(str[i]) &&
                                        !char.IsLetterOrDigit(str[i]) &&
                                        !char.IsSymbol(str[i]); i--) {
                countSpaces++;
            }
            str = str.Substring(0, str.Length - countSpaces);

            return str;
        }
        public static void WriteAt(string s, int x, int y) {
            try {
                Console.SetCursorPosition(origCol + x, origRow + y);
                Console.Write(s);
            } catch (ArgumentOutOfRangeException e) {
                Console.Clear();
                Console.WriteLine(e.Message);
            }
        }
    }
}
