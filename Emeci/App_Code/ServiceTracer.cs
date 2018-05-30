using System;
using System.IO;
using static System.IO.Directory;
using static System.Configuration.ConfigurationManager;

namespace Emeci.App_Code
{
    public class ServiceTracer
    {
        public static void Log(string Message)
        {
            string Folder = AppSettings["RutaLog"];
            if (!Exists(Folder))
                CreateDirectory(Folder);

            string FileName =
                $"{Folder}{DateTime.Now.Day.ToString("00")}{DateTime.Now.Month.ToString("00")}{DateTime.Now.Year.ToString("0000")}.log";

            FileInfo Info = new FileInfo(FileName);
            FileStream Stream = null;
            if (!Info.Exists)
                Stream = Info.Create();
            else Stream = new FileStream(FileName, FileMode.Append);

            StreamWriter Write = new StreamWriter(Stream);
            Write.WriteLine($"{DateTime.Now.ToString("HH:mm")}: {Message}");
            Write.Close();
        }
    }
}