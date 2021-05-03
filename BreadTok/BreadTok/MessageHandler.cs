using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BreadTok
{
    class MessageHandler
    {
        public static void requireField()
        {
            MessageBox.Show("Please Fill The Required Fields");
        }

        public static bool confirmYesNo(string msg)
        {
            MessageBoxResult res = MessageBox.Show(msg, "Confirm Message", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
