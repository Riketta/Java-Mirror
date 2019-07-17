using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace java_mirror
{
    class Program
    {
        static StreamWriter writer = null;

        static void Main(string[] args)
        {
            string logfile = string.Format(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "{0}_{1}.txt"), typeof(Program).Namespace, DateTime.Now.ToString("yyyyMMdd.HHmmss.fffffff"));
            writer = new StreamWriter(logfile, true);
            Log("##### JavaMirror #####");

            Log("Args:");
            string line = "";
            foreach (var arg in args)
            {
                string argument = arg;
                if (argument.StartsWith("-") && argument.Contains("=") && argument.Contains(" "))
                {
                    Log("Escape for next argument required");
                    string[] temp = argument.Split(new char[] { '=' }, 2);
                    argument = string.Format("{0}=\"{1}\"", temp[0], temp[1]);
                }
                line += argument + " ";
                Log(string.Format("\t\"{0}\"", argument));
            }
            line = line.Trim();
            Log(string.Format("Calling target file with args \"{0}\"", line));
            Process process = StartProcess(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), @"bin\java.exe"), line);
            Log(string.Format("Process (\"{0}\") started", process.StartInfo.FileName));

            Log("Waiting until application stop with 300 seconds timeout");
            Log("Process output:");
            process.OutputDataReceived += (sender, e) => Log(e.Data);
            process.ErrorDataReceived += (sender, e) => Log(e.Data);
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit(300 * 1000);

            Log("##### Done #####");
            writer.Close();
            Console.ReadLine();
        }

        static void Log(string message)
        {
            message = string.Format("[{0}] {1}", DateTime.Now.ToString("yyyyMMdd.HHmmss.fffffff"), message);
            Console.WriteLine(message);
            lock (writer)
            {
                writer.WriteLine(message);
                writer.Flush();
            }
        }

        public static Process StartProcess(string path, string args)
        {
            Process process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = path,
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,

                }
            };
            process.Start();
            
            return process;
        }
    }
}
