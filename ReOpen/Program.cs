using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Timers;

namespace ReOpen
{
    class Program
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        static ConsoleEventDelegate handler;   // Keeps it from getting garbage collected
                                               // Pinvoke
        private delegate bool ConsoleEventDelegate(int eventType);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        static void Main(string[] args)
        {
            //Imports the CallBack function to main thread
            handler = new ConsoleEventDelegate(ConsoleEventCallback);
            SetConsoleCtrlHandler(handler, true);
            
            //Starts timer 
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 5000;
            aTimer.Enabled = true;

            //Makes the application windows invisible in Taskbar and "Alt+Tab"
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);

            //Sets the application in assynchronous waiting mode
            Console.ReadLine();
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            //Reads the configuration file
            string[] lines = File.ReadAllLines("config.txt");
            string process = lines[0].Split('=')[1];
            string file = lines[1].Split('=')[1];

            if (Process.GetProcessesByName(process).Length > 0)
            {
                //Console.WriteLine("runninNnNNnN");
            }
            else
            {
                Process.Start(file);
            }
        }

        static bool ConsoleEventCallback(int eventType)
        {
            try
            {
                if (eventType == 2)
                {
                    //Restarts the process of THIS application if a smartass tries to close it
                    Process.Start(System.Reflection.Assembly.GetEntryAssembly().Location);
                }
            }
            catch (Exception)
            {

            }

            return false;
        }
    }


}
