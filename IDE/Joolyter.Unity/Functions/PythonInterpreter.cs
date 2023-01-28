using System.Collections.Generic;
using System.Diagnostics;

namespace Joolyter.Unity.Functions
{
    /// <summary>
    /// Python interpreter methods and property
    /// </summary>
    public class PythonInterpreter
    {
        /// <summary>
        /// Python process
        /// </summary>
        private Process _pythonExec = null;
        /// <value>
        /// List of all outputs and errors produced by process
        /// </value>
        public List<string> ConsoleWrites { get; private set; } = new List<string>();

        #region Instatiation [inactive]
        //public static PythonInterpreter PyIntInstance { get; set; } = null;

        //private void Awake()
        //{
        //    if (PyIntInstance == null) //if no Instance of Setup is loaded, create 
        //    {
        //        PyIntInstance = this;
        //    }
        //    else if (PyIntInstance != null) //else destroy new instance, so actual state is preserved
        //        Destroy(this);
        //}
        #endregion

        /// <summary>
        /// Starts Python process. If parameter <c>path</c> to *.py file
        /// is given, file is executed.
        /// </summary>
        /// <param name="path">Path to Python file.</param>
        public void StartPython(string path = null)
        {
            // Check for running Python process and possibly kill it
            // Start Python process and redirect comms, possibly execute file
            // Collect Errors and Outputs in CosoleWrites
            if (_pythonExec != null && !_pythonExec.HasExited)
                _pythonExec.Kill();

            _pythonExec = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "python",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    CreateNoWindow = true
                }
            };

            if (!string.IsNullOrWhiteSpace(path))
                _pythonExec.StartInfo.Arguments = $"-u -i \"{path}\"";
            else
                _pythonExec.StartInfo.Arguments = $"-u -i";

            _pythonExec.ErrorDataReceived += (s, e) => { ConsoleWrites.Add(e.Data); };

            _pythonExec.OutputDataReceived += (s, e) => { ConsoleWrites.Add(e.Data); };

            _pythonExec.Start();
            _pythonExec.BeginErrorReadLine();
            _pythonExec.BeginOutputReadLine();
        }

        /// <summary>
        /// Kills Python process and resets field
        /// </summary>
        public void KillPythonExec()
        {
            _pythonExec.Kill();
            _pythonExec = null;
        }

        /// <summary>
        /// Submits given parameter to Python process as standard input.
        /// </summary>
        /// <param name="input">Input argument for Python</param>
        public void SetInput(string input)
        {
            _pythonExec.StandardInput.WriteLineAsync(input);
        }
    }

    /// <summary>
    /// Represents one line for Python console in Unity.
    /// 
    /// Based on Soupertrooper's approach: https://bit.ly/3S1y0dJ
    /// </summary>
    [System.Serializable]
    public class ConsoleOutput
    {
        /// <value>
        /// Text content of output/input to be shown in console
        /// </value>
        public string Line { get; set; }
        /// <value>
        /// <c>TextHandler</c> component of output prefab
        /// </value>
        public TextHandler LineObject;
    }
}