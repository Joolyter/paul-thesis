using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Joolyter.Unity.Interfaces;
using Joolyter.Unity.Functions;
using SimpleFileBrowser;
using UnityEngine;
using UnityEngine.UI;

namespace Joolyter.Unity
{
    /// <summary>
    /// Holds Unity side of application. Inherits from MonoBehaviour.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class JoolyterMain : MonoBehaviour
    {
        #region Properties & fields
        #region GameObjects' components
        // Docstring Format:
        // "Serialized field representing [name_of_component] component of [name_of_Unity_game_object]"

        // Window
        /// <summary>
        /// Serialized field representing Transform component of JoolyterCanvas
        /// </summary>
        [SerializeField]
        private Transform _joolyterCanvas = null;
        /// <summary>
        /// Serialized field representing JoolyterMovement component of JoolyterPanel
        /// </summary>
        [SerializeField]
        private JoolyterMovement window = null;
        // TitleBar
        /// <summary>
        /// Serialized field representing TextHandler component of Title
        /// </summary>
        [SerializeField]
        private TextHandler _titleText = null;
        // InputArea
        /// <summary>
        /// Serialized field representing InputHandler component of CodeInputField
        /// </summary>
        [SerializeField]
        private InputHandler _codeInputField = null;
        /// <summary>
        /// Serialized field representing InputHandler component of LineNumberOutputField
        /// </summary>
        [SerializeField]
        private InputHandler _lineNumberingOutput = null;
        /// <summary>
        /// Serialized field representing Scrollbar component of CodeInput Scrollbar Vertical
        /// </summary>
        [SerializeField]
        private Scrollbar _codeInputScrollbar = null;
        /// <summary>
        /// Serialized field representing Scrollbar component of NumberOutput Scrollbar Vertical
        /// </summary>
        [SerializeField]
        private Scrollbar _numberOutputScrollbar = null;
        // ConsoleArea
        /// <summary>
        /// Serialized field representing GameObject Content (child of InputArea)
        /// </summary>
        [SerializeField]
        private GameObject _consolePanel = null;
        /// <summary>
        /// Serialized field representing GameObject OutputLineText (prefab)
        /// </summary>
        [SerializeField]
        private GameObject _lineObject = null;
        /// <summary>
        /// Serialized field representing InputHandler component of ConsoleInputField
        /// </summary>
        [SerializeField]
        private InputHandler _consoleInputField = null;
        #endregion

        #region Window movement fields
        /// <summary>
        /// Represents RectTransform component (later assigned to comp of JoolyterPanel)
        /// </summary>
        private RectTransform windowTR;
        /// <summary>
        /// Represents RectTransform component (later assigned to comp of JoolyterCanvas)
        /// </summary>
        private RectTransform rectTransform;
        /// <summary>
        /// Represents minimal width of window in pixels
        /// </summary>
        internal readonly int _minWidth = 440;
        /// <summary>
        /// Represents maximum width of window in pixels
        /// </summary>
        internal readonly int _minHeight = 290;
        #endregion

        #region Subwindow properties & fields & lists
        /// <summary>
        /// Represents the directory the assemmby gets executed from.
        /// </summary>
        private string _baseDir;
        // TitleBar Fields
        /// <summary>
        /// Name of IDE. Used for title bar.
        /// </summary>
        private static readonly string s_ideName = "<b>Joolyter</b>";
        /// <summary>
        /// Symbol that indicates change of file in title bar.
        /// </summary>
        private static readonly string unsavedIndicator = "*";
        /// <summary>
        /// Title shown.
        /// </summary>
        private string s_title = s_ideName;
        /// <summary>
        /// Indicates if change status is indicated.
        /// TODO: maybe redundant, check implemantation
        /// </summary>
        private bool _isIndicated = false;
        // FileMenu Properties
        /// <summary>
        /// Path to file that is worked in.
        /// </summary>
        private string _filePath = null;
        /// <summary>
        /// Path to Python file that is actually opened in this IDE.
        /// </summary>
        private string _pyPath = null;
        /// <summary>
        /// Name of file that is worked in.
        /// </summary>
        private string _fileName = null;
        /// <summary>
        /// Indicates if file is saved.
        /// </summary>
        public bool IsSaved { get; private set; } = true;
        // ViewMenu Properties
        /// <summary>
        /// Value of offset from font size of text components in children of InputFields and Console area.
        /// </summary>
        private int _sizeOffset = 0;
        /// <summary>
        /// Maximum value of offset from font size of text components in children of InputFields and Console area.
        /// </summary>
        private readonly int _maxSizeOffPos = 3;
        /// <summary>
        /// Minimal value of offset from font size of text components in children of InputFields and console area.
        /// </summary>
        private readonly int _maxSizeOffNeg = -3;
        /// <summary>
        /// Font size of text components in children of InputFields and console area.
        /// </summary>
        public int FontSize { get; private set; } = 14;
        // ExecuteMenu Properties
        /// <summary>
        /// Indicates if script is supposed to be executed directly.
        /// </summary>
        private bool _runNow = false;
        // InputArea Fields
        /// <summary>
        /// Number of lines in CodeInputField.
        /// 
        /// Warning: Use of escape characters brake this!!!
        /// </summary>
        private int _lineCount;
        // ConsoleArea Fields
        /// <summary>
        /// Maximum lines shown in console. High values impact framerate massively.
        /// 
        /// TODO: Set ConsoleOutput game objects out of visible range of scrollview inactive.
        /// </summary>
        private static readonly int _maxLines = 200;
        /// <summary>
        /// Stores all objects shown in console.
        /// </summary>
        private List<ConsoleOutput> _outputList = new List<ConsoleOutput>();
        #endregion

        #region DragHandler fields [inactive]
        //// DragHandler Fields
        //private bool dragging = false;
        //private Vector2 mouseStart;
        //private Vector3 windowStart;
        //private RectTransform rect;
        #endregion

        #region Instances
        // Interface Instances
        /// <summary>
        /// Public instance of IPrefabLoader to use its methods.
        /// </summary>
        public static IPrefabLoader PrefabLoaderInterface { get; set; }
        /// <summary>
        /// Instance of IJoolyter. Maybe redundant.
        /// 
        /// TODO: Delete
        /// </summary>
        private IJoolyter _joolyterInterface;
        // Python Interpreter Instance
        /// <summary>
        /// Instance of <c>PythonInterpreter</c>.
        /// 
        /// TODO: Consider making <c>PythonInterpreter</c> static or using singleton pattern.
        /// </summary>
        private PythonInterpreter _pythonInterpreter;
        #endregion
        #endregion

        #region MonoBehaviour methods
        private void Awake()
        {
            // Assign fields.
            _pythonInterpreter = new PythonInterpreter();

            _baseDir = AppDomain.CurrentDomain.BaseDirectory;

            rectTransform = (RectTransform) _joolyterCanvas;
            windowTR = (RectTransform) window.transform;

            window.Initialize(this);
        }

        private void Start()
        {
            EnsureWindowIsWithinBounds();

            // Hook up LineObject to prefab
            // Use of preprocessor directives does not produce reliable outcome
            // TODO: Check why and adjust as its much cleaner and more efficient
            if (!_baseDir.Contains(@"Editor\2019.4.18f1\Editor"))
                _lineObject = PrefabLoaderInterface.ConsoleOutputGameObject;

            // Setup FileBrowser
            SetupFileBrowser();

            // Start Python console
            _pythonInterpreter.StartPython();
        }

        private void Update()
        {
            // Do not update if window is not open.
            if (!isActiveAndEnabled)
                return;

            // To prevent buffer overloading print max 100 lines per frame
            int ctr = 0;
            while ((_pythonInterpreter.ConsoleWrites.Count > 0) && (ctr < 101))
            { 
                WriteLineInConsole(_pythonInterpreter.ConsoleWrites[0]);
                _pythonInterpreter.ConsoleWrites.Remove(_pythonInterpreter.ConsoleWrites[0]);
                ctr++;
            }

            // Expensive call but frame rate does not seem to be impacted
            // TODO: May be implemented as listener to OnValueChanged event of each scrok
            SyncScrollbars();
        }
        #endregion

        #region Title bar methods
        /// <summary>
        /// Adds file name to IDE-name. Is shown in title bar.
        /// </summary>
        /// <param name="name">Name of file</param>
        public void SetTitleText(string name = null)
        {
            // If name is given, add it to title
            // else set title to IDE name
            if (name != null)
            {
                s_title = $"{s_ideName} - {name}";
                _titleText.OnTextUpdate.Invoke(s_title);
            }
            else
            {
                s_title = s_ideName;
                _titleText.OnTextUpdate.Invoke(s_title);
            }
        }

        /// <summary>
        /// Adds indicator that open file is not saved.
        /// </summary>
        public void AddEditedIndicator()
        {
            // Set IsSaved false for tracking, add indicator to title
            IsSaved = false;

            // TODO: Check if title is null makes no sense???
            if ((s_title != s_ideName) || String.IsNullOrEmpty(s_title))
            {
                _titleText.OnTextUpdate.Invoke($"{s_title}{unsavedIndicator}");
            }

            _isIndicated = true;
        }
        #endregion

        #region File menu methods
        /// <summary>
        /// Method of "New"-button.
        /// 
        /// Resets editor of IDE.
        /// </summary>
        public void NewCode()
        {
            // Reset CodeInputField and class's fields.
            _codeInputField.OnTextUpdate.Invoke("");
            _filePath = null;
            _fileName = null;
            SetTitleText();
            IsSaved = false;
        }

        /// <summary>
        /// Method of "Open"-button.
        /// 
        /// Starts coroutine that opens FileBrowser window.
        /// </summary>
        public void OpenCode()
        {
            StartCoroutine(ShowLoadDialogCoroutine());
        }

        /// <summary>
        /// General method that saves contents of CodeInputField. Is directly hooked to "Save"-button.
        /// </summary>
        /// <param name="emergency">
        /// Indicates if code has to be saved now. 
        /// Is true if GameObject is destroyed and data may be lost otherwise.
        /// </param>
        public void SaveCode(bool emergency = false)
        {
            // Check for early return if CodeInputField is empty
            // If emergency is true, save code into special file to prevent unwanted overwrite
            // Else if file exists, overwrite it (normal case if "Save"-button is pressed)
            //      Differentiate if code is supposed to be saved as .ipynb- or .py-file
            // Else, run SaveAs() method
            // Adjust fields
            if (String.IsNullOrEmpty(_codeInputField.Text))
                return;

            string fileDirectory = Path.GetDirectoryName(_filePath);
            string codeToSave = _codeInputField.Text;
            if (emergency)
            {
                string now = DateTime.Now.ToString("yyyyMMddHHmm");
                if (File.Exists(_filePath))
                    File.WriteAllText($@"{fileDirectory}\{_fileName}_emergeny_save_{now}.py", codeToSave);
                else
                    File.WriteAllText($@"{_baseDir}\GameData\Joolyter\backup\emergeny_save_{now}.py", codeToSave);
            }
            else if (File.Exists(_filePath))
            {
                if (Path.GetExtension(_filePath) == ".ipynb")
                {
                    _filePath = JupyterSave(codeToSave);
                }
                else
                {
                    File.WriteAllText(_filePath, codeToSave);
                    _pyPath = _filePath;
                }
            }
            else
            {
                SaveAsCode();
                return;
            }

            _fileName = Path.GetFileName(_filePath);
            SetTitleText(_fileName);
            IsSaved = true;
            _isIndicated = false;
        }

        /// <summary>
        /// Method of "Save As"-button. Gets also called, if "Save"-button is pressed and file does not exist.
        /// 
        /// Starts coroutine that opens <c>FileBrowser</c> window.
        /// </summary>
        public void SaveAsCode()
        {
            // Check for early return if CodeInputField is empty
            // Start coroutine that opens FileBrowser window to choose how and where to save
            if (String.IsNullOrEmpty(_codeInputField.Text))
                return;

            string codeToSave;

            codeToSave = _codeInputField.Text;

            StartCoroutine(ShowSaveDialogCoroutine(codeToSave));
        }

        /// <summary>
        /// Opens <c>FileBrowser</c> window in load configuration.
        /// 
        /// 
        /// Obtained from GitHub (18.07.2022): https://bit.ly/3Dxg2Mp
        /// Changed to fit needs
        /// Published under MIT License
        /// Copyright (c) 2016 Süleyman Yasir KULA
        /// </summary>
        private IEnumerator ShowLoadDialogCoroutine()
        {
            // Open FileBrowser window and yield until it is closed.
            // If window is close with success, open file from given path
            // by differentiating if code is saved as .ipynb- or .py-file.
            // If  code is Jupyter Notebook file, convert to .py and open that
            // Link to Jupyter file is kept, so changes are saved in .ipynb-file.
            // Set up fields.
            yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files);
            
            if (FileBrowser.Success)
            {
                _filePath = FileBrowser.Result[0];

                if (Path.GetExtension(_filePath) == ".ipynb")
                    _pyPath = PyJupConverter.Converter(_filePath);
                else
                    _pyPath = _filePath;

                _codeInputField.OnTextUpdate.Invoke(File.ReadAllText(_pyPath));
                _fileName = Path.GetFileName(_filePath);
                IsSaved = true;
                _isIndicated = false;
                SetTitleText(_fileName);
            }
        }

        /// <summary>
        /// Opens <c>FileBrowser</c> window in save configuration.
        /// 
        /// 
        /// Obtained from GitHub (18.07.2022): https://bit.ly/3Dxg2Mp
        /// Changed to fit needs
        /// Published under MIT License
        /// Copyright (c) 2016 Süleyman Yasir KULA
        /// </summary>
        private IEnumerator ShowSaveDialogCoroutine(string codeToSave)
        {
            // Open FileBrowser window and yield until it is closed.
            // If window is close with success, save file to given path
            // by differentiating if code is supposed to be saved as .ipynb- or .py-file.
            // Set up fields.
            // If saving routine was started by clicking "Execute"-button, do that.
            yield return FileBrowser.WaitForSaveDialog(FileBrowser.PickMode.Files);

            if (FileBrowser.Success)
            {
                _filePath = FileBrowser.Result[0];

                if (Path.GetExtension(_filePath) == ".ipynb")
                    _filePath = JupyterSave(codeToSave);
                else
                {
                    File.WriteAllText(_filePath, codeToSave);
                    _pyPath = _filePath;
                }

                _fileName = Path.GetFileName(_filePath);
                IsSaved = true;
                _isIndicated = false;
                SetTitleText(_fileName);
                if (_runNow)
                {
                    _pythonInterpreter.StartPython(_pyPath);
                    _runNow = false;
                }
            }
        }

        /// <summary>
        /// Saves given parameter to Jupyter Notebook file.
        /// </summary>
        /// <param name="codeToSave">String that gets saved.</param>
        /// <returns>Path of Jupyter Notebook file.</returns>
        private string JupyterSave(string codeToSave)
        {
            // Create path for python file
            // Save codeToSave to Python file
            // Convert Python file to Jupyter Notebook file
            // TODO: add deletion Python file
            _pyPath = $@"{_filePath.Remove(_filePath.Length - 6)}.py";

            File.WriteAllText(_pyPath, codeToSave);

            return PyJupConverter.Converter(_pyPath);
        }
        #endregion

        #region View menu methods
        /// <summary>
        /// Method of "+"-button.
        /// 
        /// Increases font size by calling <c>SetFontSize()</c>
        /// </summary>
        public void ZoomIn()
        {
            // Check if max size is reached
            // Call SetFontSize()
            if (_sizeOffset <= _maxSizeOffPos)
            {
                _sizeOffset += 1;
                SetFontSize(FontSize + _sizeOffset);
            }
            else
                Debug.Log("[Joolyter]: Max. font size reached");
        }

        /// <summary>
        /// Method of "-"-button.
        /// 
        /// Decreases font size by calling <c>SetFontSize()</c>
        /// </summary>
        public void ZoomOut()
        {
            // Check if min size is reached
            // Call SetFontSize()
            if (_sizeOffset >= _maxSizeOffNeg)
            { 
                _sizeOffset -= 1;
                SetFontSize(FontSize + _sizeOffset);
            }
            else
                Debug.Log("[Joolyter]: Min. font size reached");
        }

        /// <summary>
        /// Sets font size to given value of parameter.
        /// </summary>
        /// <param name="fontSize">Font size that the method sets.</param>
        private void SetFontSize(int fontSize)
        {
            // Gather all TextHandler components in InputFields' children.
            // Set new font size to those
            // Apply font change to existing objects in console
            InputHandler[] inputHandlers = _joolyterCanvas.GetComponentsInChildren<InputHandler>();
            foreach (InputHandler inputHandler in inputHandlers)
            {
                TextHandler[] inputTextHandlers = inputHandler.GetComponentsInChildren<TextHandler>();
                foreach (TextHandler textHandler in inputTextHandlers)
                    textHandler.OnFontChange.Invoke(fontSize);
            }

            TextHandler[] consoleTextHandlers = _consolePanel.GetComponentsInChildren<TextHandler>();
            foreach (TextHandler textHandler in consoleTextHandlers)
                textHandler.OnFontChange.Invoke(fontSize);
        }
        #endregion

        #region Execute menu methods
        /// <summary>
        /// Method of "Execute"-button (Play-button).
        /// 
        /// Executes Python script stored in _pyPath.
        /// </summary>
        public void Execute()
        {
            // Save code
            // Execute code
            // TODO: check if code gets executed twice when SaveCode()->SaveAsCode()->_runNow==true.
            //       Implementation need optimization.
            _runNow = true;
            SaveCode();
            if (IsSaved && _runNow)
            {
                _pythonInterpreter.StartPython(_pyPath);
                _runNow = false;
            }
        }

        /// <summary>
        /// Method of "Stop Execution"-button (Stop-button).
        /// 
        /// Calls <c>PythonInterpreter.StartPythonScript</c>, which kills running process and starts new one.
        /// </summary>
        public void CancelExecution()
        {
            _pythonInterpreter.StartPython();
        }
        #endregion

        #region Input area methods
        /// <summary>
        /// Adds line numbering of CodeInputField
        /// </summary>
        public void LineNumbering()
        {
            // Add first line number
            // Count lines using escape characters
            // Add numbers according to count
            _lineNumberingOutput.OnTextUpdate.Invoke("1");
            _lineCount = _codeInputField.Text.Split('\n').Length;

            for (int i = 2; i <= _lineCount; i++)
                _lineNumberingOutput.OnTextUpdate.Invoke(_lineNumberingOutput.Text + "\n" + i.ToString());
        }

        /// <summary>
        /// Fixes issues with unsynchonous behaviour of LineNumberOutputField and CodeInputField.
        /// </summary>
        private void SyncScrollbars()
        {
            _numberOutputScrollbar.value = _codeInputScrollbar.value;
        }
        #endregion

        #region Console area methods
        /// <summary>
        /// Sends content of ConsoleInputField to <c>PythonInterpreter</c> as string.
        /// </summary>
        public void SubmitInputLine()
        {
            // Check if "Return"-key is pressed
            // Display input in console
            // Check for clear command
            // Call _pythonInterpreter.SetInput() to pass string
            // Clear InputField
            // Reselect InputField for more inputs
            WriteLineInConsole(">>" + _consoleInputField.Text);

            if (_consoleInputField.Text == "clear")
                ClearOutputs();
            else
                _pythonInterpreter.SetInput(_consoleInputField.Text);
                
            _consoleInputField.OnTextUpdate.Invoke("");

            _joolyterInterface.SelectConsoleInput();
        }

        /// <summary>
        /// Adds given string parameter to console and displays it as game object.
        /// 
        /// Based on Soupertrooper's approach: https://bit.ly/3S1y0dJ
        /// </summary>
        /// <param name="line">Text that gets added to console.</param>
        private void WriteLineInConsole(string line)
        {
            // Check for quick return
            // If maximum number objects are in console, remove excess
            // Instantiate new output (Unity for add game object to scene)
            // Store TextHandler component of game object and string with text in instance of ConsoleOutput class
            // Add that instance to list for book keeping
            // Add line to KSP.log for debugging and support
            if (line == null)
                return;

            if (_outputList.Count >= _maxLines)
            {
                Destroy(_outputList[0].LineObject.gameObject);
                _outputList.Remove(_outputList[0]);
            }

            GameObject newLine = Instantiate(_lineObject, _consolePanel.transform);
            newLine.GetComponent<TextHandler>().OnFontChange.Invoke(FontSize + _sizeOffset);

            ConsoleOutput newOutput = new ConsoleOutput()
            {
                Line = $">{line}",
                LineObject = newLine.GetComponent<TextHandler>(),
            };
            newOutput.LineObject.OnTextUpdate.Invoke(newOutput.Line);

            _outputList.Add(newOutput);

            string sender;
            if (_fileName != null)
                sender = _fileName;
            else
                sender = "Python Console";

            Debug.Log($"[Joolyter]: [{sender}]: {line}");
        }

        /// <summary>
        /// Clears list of <c>ConsoleOutput</c> instances and destroys its component's game objects.
        /// 
        /// Gets called if <c>clear</c> command in ConsoleInputField is registred.
        /// </summary>
        private void ClearOutputs()
        {
            // Clear variables set in Python process
            // Destroy console output game objects
            // Clear list of outputs
            // Register action in KSP.log
            _pythonInterpreter.SetInput("globals().clear()");

            foreach (ConsoleOutput output in _outputList)
            {
                Destroy(output.LineObject.gameObject);
            }

            _outputList.Clear();

            Debug.Log("[Joolyter]: Python console cleared.");
        }
        #endregion

        #region Movement methods
        /// <summary>
        /// Checks if main window of application is completly visible on screen.
        /// 
        /// Obtained from GitHub under MIT license
        /// Copyright(c) 2016 Süleyman Yasir KULA
        /// Source: https://bit.ly/3B5JYfF
        /// </summary>
        /* MIT License

        Copyright(c) 2016 Süleyman Yasir KULA

        Permission is hereby granted, free of charge, to any person obtaining a copy
        of this software and associated documentation files(the "Software"), to deal
        in the Software without restriction, including without limitation the rights
        to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
        copies of the Software, and to permit persons to whom the Software is
        furnished to do so, subject to the following conditions:

        The above copyright notice and this permission notice shall be included in all
        copies or substantial portions of the Software.

        THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
        IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
        FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
        AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
        LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
        OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
        SOFTWARE.*/
        internal void EnsureWindowIsWithinBounds()
        {
            // Mathematically check if window fits on canvas
            Vector2 canvasSize = rectTransform.sizeDelta;
            Vector2 windowSize = windowTR.sizeDelta;

            if (windowSize.x < _minWidth)
                windowSize.x = _minWidth;
            if (windowSize.y < _minHeight)
                windowSize.y = _minHeight;

            if (windowSize.x > canvasSize.x)
                windowSize.x = canvasSize.x;
            if (windowSize.y > canvasSize.y)
                windowSize.y = canvasSize.y;

            Vector2 windowPos = windowTR.anchoredPosition;
            Vector2 canvasHalfSize = canvasSize * 0.5f;
            Vector2 windowHalfSize = windowSize * 0.5f;
            Vector2 windowBottomLeft = windowPos - windowHalfSize + canvasHalfSize;
            Vector2 windowTopRight = windowPos + windowHalfSize + canvasHalfSize;

            if (windowBottomLeft.x < 0f)
                windowPos.x -= windowBottomLeft.x;
            else if (windowTopRight.x > canvasSize.x)
                windowPos.x -= windowTopRight.x - canvasSize.x;

            if (windowBottomLeft.y < 0f)
                windowPos.y -= windowBottomLeft.y;
            else if (windowTopRight.y > canvasSize.y)
                windowPos.y -= windowTopRight.y - canvasSize.y;

            windowTR.anchoredPosition = windowPos;
            windowTR.sizeDelta = windowSize;
        }
        #endregion

        #region Setup methods
                                    //public void SetInitialState(IJoolyter joolyterInterface)
                                    //{
                                    //    if (joolyterInterface == null)
                                    //        return;

                                    //    _joolyterInterface = joolyterInterface;
                                    //    //SetPosition(joolyterInterface.Position);
                                    //}

                                    // /<summary>
                                    // /Sets the panel position
                                    // /</summary>
                                    // /<param name="v">The x and y coordinates of the panel, measured from the top-left</param>
                                    //private void SetPosition(Vector2 v)
                                    //{
                                    //    if (rect == null)
                                    //        return;

                                    //    rect.anchoredPosition = new Vector3(v.x, v.y > 0 ? v.y * -1 : v.y, 0);
                                    //}

        /// <summary>
        /// Sets up Simple File Browser, which is obtained from GitHub.
        /// 
        /// Simple File Browser
        /// Copyright(c) 2016 Süleyman Yasir KULA
        /// Source: https://bit.ly/3cZAKJJ
        /// </summary>
        /* MIT License

        Copyright(c) 2016 Süleyman Yasir KULA

        Permission is hereby granted, free of charge, to any person obtaining a copy
        of this software and associated documentation files(the "Software"), to deal
        in the Software without restriction, including without limitation the rights
        to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
        copies of the Software, and to permit persons to whom the Software is
        furnished to do so, subject to the following conditions:

        The above copyright notice and this permission notice shall be included in all
        copies or substantial portions of the Software.

        THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
        IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
        FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
        AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
        LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
        OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
        SOFTWARE.*/
        private void SetupFileBrowser()
        {
            FileBrowser.SetFilters(
               true,
               new FileBrowser.Filter("Python", ".py"),
               new FileBrowser.Filter("Jupyter Notebook", ".ipynb"));
            FileBrowser.SetDefaultFilter(".py");
            FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");
            FileBrowser.AddQuickLink("Users", "C:\\Users", null);
            FileBrowser.AddQuickLink("Kerbal Space Program", _baseDir, null);
        }

        /// <summary>
        /// Manages closing of application.
        /// 
        /// Gets called OnDestroy() of <c>Joolyter</c>
        /// </summary>
        public void Destroy()
        {
            // If CodeInputFields contents are not saved, do so.
            // Close Python interpreter
            // Reset JoolyterLoader
            // Destroy FileBrowser
            if (!IsSaved)
            {
                SaveCode(emergency: true);
            }
            _pythonInterpreter.KillPythonExec();
            FileBrowser.DestroyInstance();
        }
        #endregion

        #region Test methods
        #endregion
    }
}