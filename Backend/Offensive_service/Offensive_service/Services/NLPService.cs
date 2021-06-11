using System;
using System.IO;
using System.Diagnostics;
using System.Resources;

namespace Offensive_service.Services
{
    class NLPService
    {
        private readonly string _SCRIPT;
        private readonly string _FILES_DIR;

        public NLPService()
        {
            //_SCRIPT = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "\\py_src\\main.py";
            //_SCRIPT = Path.Combine(Environment.CurrentDirectory, "py_src", "main.py");
            _SCRIPT = Path.Combine(@".", "py_src", "main.py");
            //_FILES_DIR = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "\\Data\\";
            _FILES_DIR = Path.Combine(@".", "Data") + Path.DirectorySeparatorChar;
        }

        public float offensive_analysis(string fileName, string language = "en")
        { 
            return float.Parse(run_cmd(_SCRIPT, _FILES_DIR + fileName, language));
        }

        private string run_cmd(string script, string fileName, string language)
        {
            // 1) Create process info
            ProcessStartInfo psi = new ProcessStartInfo();
            //psi.FileName = @"C:\Program Files (x86)\Microsoft Visual Studio\Shared\Python37_64\python.exe";
            psi.FileName = @"C:\Users\nacho\.conda\envs\nlp\python.exe";

            // 2) Provide scrip and arguments
            psi.Arguments = string.Format("{0} {1} {2}", script, fileName, language);

            // 3) Process configuration
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;

            // 4) Execute process and get output
            string results;
            using (Process process = Process.Start(psi))
            {
                results = process.StandardOutput.ReadToEnd();
            }

            return results;
        }
    }
}
