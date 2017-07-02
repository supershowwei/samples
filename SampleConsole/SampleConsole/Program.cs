using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace SampleConsole
{
    internal class Program
    {
        private static readonly AutoResetEvent Blocker = new AutoResetEvent(false);

        private static bool isClosing;

        // A delegate type to be used as the handler routine
        // for SetConsoleCtrlHandler.
        public delegate bool HandlerRoutine(CtrlTypes ctrlType);

        // An enumerated type for the control messages
        // sent to the handler routine.
        public enum CtrlTypes
        {
            CtrlCEvent = 0,

            CtrlBreakEvent,

            CtrlCloseEvent,

            CtrlLogoffEvent = 5,

            CtrlShutdownEvent
        }

        // Declare the SetConsoleCtrlHandler function
        // as external and receiving a delegate.
        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine handler, bool add);

        private static bool ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
            // Put your own handler here
            switch (ctrlType)
            {
                case CtrlTypes.CtrlCEvent:
                    isClosing = true;
                    Console.WriteLine("CTRL+C received!");
                    break;

                case CtrlTypes.CtrlBreakEvent:
                    isClosing = true;
                    Console.WriteLine("CTRL+BREAK received!");
                    break;

                case CtrlTypes.CtrlCloseEvent:
                    isClosing = true;
                    Console.WriteLine("Program being closed!");
                    break;

                case CtrlTypes.CtrlLogoffEvent:
                case CtrlTypes.CtrlShutdownEvent:
                    isClosing = true;
                    Console.WriteLine("User is logging off!");
                    break;
            }

            return true;
        }

        private static void Main(string[] args)
        {
            SetConsoleCtrlHandler(ConsoleCtrlCheck, true);

            Task.Run(
                () =>
                    {
                        while (!isClosing)
                        {
                            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff") + " Test");
                            Thread.Sleep(5000);
                        }

                        Blocker.Set();
                    });

            Console.WriteLine("Ctrl+C, Ctrl+Break or suppress the application to exit");

            Blocker.WaitOne();
        }
    }
}