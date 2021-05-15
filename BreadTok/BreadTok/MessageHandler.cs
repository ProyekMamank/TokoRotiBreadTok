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

        public static void wrongUsernamePassword()
        {
            MessageBox.Show("The Username/Password You Entered is Invalid!");
        }

        public static void PWDoesntMatchConf()
        {
            MessageBox.Show("Password and Confirmation Password Must Match!");
        }
        
        public static void isNotNumber(string msg)
        {
            MessageBox.Show("Field " +  msg + " must be number");
        }

        public static void containSpaces(string msg)
        {
            MessageBox.Show("Field " + msg + " cannot contain spaces");
        }

        public static void usernameExists()
        {
            MessageBox.Show("Username already exists ");
        }

        public static void messageSuccess(string msg)
        {
            MessageBox.Show(msg + " Successful!");
        }

        public static void mustBeBigger(string field, int minimum)
        {
            MessageBox.Show(field + " Must be bigger than " + minimum);
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
