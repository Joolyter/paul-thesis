//using System.Diagnostics;
//using System.IO;

//namespace Joolyter.Unity
//{
//    /// <summary>
//    /// Converter for Python and Jupyter Notebook files.
//    /// 
//    /// Uses p2j package for python: https://bit.ly/3L6kSlj
//    /// </summary>
//    public static class PyJupConverter
//    {
//        private static string _dir = null;
//        private static string _fileName = null;
//        private static string _newExt = null;
//        private static string _newPath = null;

//        /// <summary>
//        /// Converts file from given path to different file format.
//        /// 
//        /// Options:
//        /// [In]: *.py -> [Out]: *.ipynb
//        /// [In]: *.ipynb -> [Out]: *.py
//        /// </summary>
//        /// <param name="path">Path to initial file</param>
//        /// <returns>Path to new file</returns>
//        public static string Converter(string path)
//        {
//            // Assign local variables
//            // Differ between initial file types and adjust arguments
//            _dir = $@"{Path.GetDirectoryName(path)}\";
//            _fileName = Path.GetFileNameWithoutExtension(path);
//            string ext = Path.GetExtension(path);

//            string stdArguments = $@"-o {path} -t ";

//            if (ext == ".py")
//            {
//                _newExt = @".ipynb";
//            }
//            else if (ext == ".ipynb")
//            {
//                _newExt = @".py";
//            }
//            else
//            {
//                UnityEngine.Debug.LogError("[PyJupConverter.Converter]: Given file type must be \'*.py\' or \'*.ipynb\'.");
//                return null;
//            }

//            _newPath = $@"{_dir}{_fileName}{_newExt}";

//            string arguments = $@"{stdArguments}{_newPath}";
//            _newPath = Execute(path, ext, arguments);

//            return _newPath;
//        }

//        /// <summary>
//        /// Starts p2j and executes conversion.
//        /// </summary>
//        /// <param name="path">Path to initial file</param>
//        /// <param name="ext">Extension of inital file</param>
//        /// <param name="arguments">Arguments p2j is called with</param>
//        /// <returns>Path to new file</returns>
//        private static string Execute(string path, string ext, string arguments)
//        {
//            // Start process p2j
//            // Decide new file type
//            // TODO: Overload method
//            Process exec = new Process()
//            {
//                StartInfo = new ProcessStartInfo
//                {
//                    FileName = "p2j",
//                    Arguments = arguments,
//                    UseShellExecute = false,
//                    CreateNoWindow = true
//                }
//            };

//            exec.Start();
//            exec.WaitForExit();

//            string newPath = $@"{_dir}{_fileName}{_newExt}";

//            if (File.Exists(newPath))
//                return newPath;
//            else
//            {
//                UnityEngine.Debug.LogError($"[PyJupConverter.Execute]: New path \'{newPath}\' does not exist");
//                return path;
//            }
//        }
//    }
//}

using System.Diagnostics;
using System.IO;

namespace Joolyter.Unity.Functions
{
    /// <summary>
    /// Converter for Python and Jupyter Notebook files.
    /// 
    /// Uses p2j package: https://bit.ly/3L6kSlj
    /// 
    /// TODO: Streamline class by using static fields. Comment above started that already.
    /// </summary>
    public static class PyJupConverter
    {
        /// <summary>
        /// Converts file from given path to different file format.
        /// 
        /// Options:
        /// [In]: *.py -> [Out]: *.ipynb
        /// [In]: *.ipynb -> [Out]: *.py
        /// </summary>
        /// <param name="path">Path to initial file</param>
        /// <returns>Path to new file</returns>
        public static string Converter(string path)
        {
            // Assign local variables
            // Differentiate between initial file types and adjust arguments
            string ext = Path.GetExtension(path);
            string stdArguments = $@"-o {path}";
            string newPath;

            if (ext == ".py")
                newPath = Execute(path, ext, $@"{stdArguments}");
            else if (ext == ".ipynb")
                newPath = Execute(path, ext, $@"-r {stdArguments}");
            else
                return null;

            return newPath;
        }

        /// <summary>
        /// Starts p2j and executes conversion.
        /// </summary>
        /// <param name="path">Path to initial file</param>
        /// <param name="ext">Extension of inital file</param>
        /// <param name="arguments">Arguments p2j is called with</param>
        /// <returns>Path to new file</returns>
        private static string Execute(string path, string ext, string arguments)
        {
            // Decide new file type
            // Assemble new path and delete existing file
            // (p2j does not overwrite but append to existing files, eventhough "-o" is given)
            // Start process p2j
            // TODO: Overload method

            string newExt = null;

            if (ext == ".ipynb")
                newExt = ".py";
            else if (ext == ".py")
                newExt = ".ipynb";

            string newPath = $@"{Path.GetDirectoryName(path)}\{Path.GetFileNameWithoutExtension(path)}{newExt}";

            if (File.Exists(newPath))
                File.Delete(newPath);

            Process exec = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "p2j",
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            exec.Start();
            exec.WaitForExit();

            if (File.Exists(newPath))
                return newPath;
            else
            {
                UnityEngine.Debug.LogError("[PyConverter.Execute]: Something went wrong! File was not created.");
                return null;
            }
        }
    }
}
