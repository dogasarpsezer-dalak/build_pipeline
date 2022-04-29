using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using  System.Net.;
using System.Timers;

namespace DalakGITConsole
{
    public class AutoBuildProgram
    {
        public static Timer commitGetTimer = new Timer(3000);
        public static string oldCommitHash = "";
        public static string pathOfRepo = "";
        public static void Main(string[] args)
        {
            Console.WriteLine("WELCOME TO AUTOMATION BUILD APP THAT GAVE THE INTERN ONLY A LOT OF PAIN :) PRESS ANY KEY TO ABORT");
            Console.WriteLine("Enter the repo path");
            pathOfRepo = Console.ReadLine();
            commitGetTimer.Enabled = true;
            commitGetTimer.Elapsed += TriggerGIT;
            commitGetTimer.AutoReset = true;
            commitGetTimer.Start();
            Console.ReadKey();
        }

        public static void TriggerGITHUBWindows(Object sender, ElapsedEventArgs e)
        {
            
        }
        
        public static void TriggerGIT(Object sender,ElapsedEventArgs e)
        {
            string path =  pathOfRepo;
            string pathRepo = path.Replace(@"\","/");
            string gitLogCommand = string.Format(" --git-dir={0}/.git --work-tree={0} log -1 --pretty=format:\"%H\"",pathRepo);
            string gitPullCommand = string.Format(" --git-dir={0}/.git --work-tree={0} pull ",pathRepo);;
            
            ProcessStartInfo processStartInfo = new ProcessStartInfo()
            {
                //OS or EXE should start the process
                UseShellExecute = false,
                
                //The Output will be Standard Output Stream
                RedirectStandardOutput = true,
                
                //Application to start
                FileName = "git.exe",
                
                Arguments = gitPullCommand
            };
            
            Process gitProcess = new Process();
            gitProcess.StartInfo = processStartInfo;
            gitProcess.Start();

            processStartInfo.Arguments = gitLogCommand;
            gitProcess.Start();
            string output = gitProcess.StandardOutput.ReadToEnd();
            gitProcess.WaitForExit();
            
            if (!string.Equals(output, oldCommitHash))
            {
                oldCommitHash = output;
                Console.WriteLine(output);
            }
        }
    }
}