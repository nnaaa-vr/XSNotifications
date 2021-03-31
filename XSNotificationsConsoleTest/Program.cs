using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using XSNotifications;
using XSNotifications.Enum;

namespace XSNotificationsTestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            using (XSNotifier notifier = new XSNotifier())
            {
                string content = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.";

                for (int i = 0; i < 3; i++)
                    notifier.SendNotification(new XSNotification()
                    {
                        SourceApp = "んなあぁ's Bee Emporium",
                        Content = content,
                        Title = "んなあぁ",
                        Opacity = 0.7f
                    });
            }
        }
    }
}
