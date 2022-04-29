using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Timers;

namespace DalakGITConsole
{
    public class AutoBuildProgram
    {
        public static Timer commitGetTimer = new Timer(3000);
        public static string oldCommitHash = "";
        public static string pathOfRepo = "";
        public static Queue<string[]> repoBuildQueue = new Queue<string[]>();  
        public static void Main(string[] args)
        {
            Console.WriteLine("WELCOME TO AUTOMATION BUILD APP THAT GAVE THE INTERN ONLY A LOT OF PAIN :) PRESS ANY KEY TO ABORT");
            
            InitBuildRepo();
            
            /*pathOfRepo = Console.ReadLine();
            commitGetTimer.Enabled = true;
            commitGetTimer.Elapsed += TriggerGIT;
            commitGetTimer.AutoReset = true;
            commitGetTimer.Start();
            Console.ReadKey();*/
        }

        public static void InitBuildRepo()
        {
            var path = Directory.GetCurrentDirectory();
            Regex buildDirectoryRegex = new Regex(".+pipeline");
            Regex repoURLRegex = new Regex(".+git");
            Regex repoBuildStatusRegex = new Regex("[|].+[E,e][|]");
            pathOfRepo = buildDirectoryRegex.Match(path).Value;

            StreamReader streamReader = new StreamReader(pathOfRepo + "\\build_queue.txt");
            while (!streamReader.EndOfStream)
            {
                var repoString = streamReader.ReadLine();
                string[] repoStringPieces = new string[2];
                repoStringPieces[0] = repoURLRegex.Match(repoString).Value;
                repoStringPieces[1] = repoBuildStatusRegex.Match(repoString).Value;
                Console.WriteLine(repoStringPieces[1]);
                repoBuildQueue.Enqueue(repoStringPieces);
            }
        }
        public static void TriggerGIT(Object sender,ElapsedEventArgs e)
        {
            string path = pathOfRepo.Replace(@"\","/");
            string gitLogCommand = string.Format(" --git-dir={0}/.git --work-tree={0} log -1 --pretty=format:\"%H\"",path);
            string gitPullCommand = string.Format(" --git-dir={0}/.git --work-tree={0} pull ",path);
            
            ProcessStartInfo processStartInfo = new ProcessStartInfo()
            {
                //OS or EXE should start the process
                UseShellExecute = false,
                
                //The Output will be Standard Output Stream
                RedirectStandardOutput = true,
                
                //Application to start
                FileName = "git.exe",
                
                Arguments = gitLogCommand
            };
            
            Process gitProcess = new Process();
            gitProcess.StartInfo = processStartInfo;
            processStartInfo.Arguments = gitLogCommand;
            gitProcess.Start();
            string output = gitProcess.StandardOutput.ReadToEnd();
            gitProcess.WaitForExit();
            
            if (!string.Equals(output, oldCommitHash))
            {
                oldCommitHash = output;
                processStartInfo.Arguments = gitPullCommand;
                gitProcess.Start();
                gitProcess.WaitForExit();
                //Trigger Queueya al
            }
        }
    }
}