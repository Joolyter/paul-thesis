using System;
using System.Collections.Generic;
using Joolyter.Unity;
using Joolyter.Unity.Interfaces;
using KSP.UI;
using KSP.UI.Screens;
using SimpleFileBrowser;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Joolyter.KSP
{
    /// <summary>
    /// Holds KSP side of application. Inherits from class <c>MonoBehavior</c>.
    /// Implements interfaces <c>IJoolyter</c> and <c>ISimpleFileBrowser</c>.
    /// </summary>
    /// <seealso cref="MonoBehaviour"/>
    /// <seealso cref="IJoolyter"/>
    /// <seealso cref="ISimpleFileBrowser"/>
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    class Joolyter : MonoBehaviour, IJoolyter, ISimpleFileBrowser
    {
        #region Class's fields and properties
        /// <summary>
        /// Represents KSP toolbar icon.
        /// </summary>
        private ApplicationLauncherButton _launcherButton;

        /// <summary>
        /// Represents <c>JoolyterMain</c> component of Unity canvas game object.
        /// </summary>
        private JoolyterMain _mainWindow = null;

        /// <summary>
        /// Represents state of control lock on KSP triggered by <c>FileBrowser</c>.
        /// </summary>
        private bool _sfbCtrlLockActive = false;

        /// <value>
        /// Instance of class <c>Joolyter</c> - implementing singleton pattern.
        /// Not thread safe by definition but as <c>Instance</c> gets assigned in
        /// <c>Awake()</c> which is called ONCE when script is loaded implemantion
        /// should be sufficient. Else: see https://bit.ly/3KV8zbD and apply
        /// </value>
        public static Joolyter Instance { get; private set; } = null;
        #endregion

        #region Interface implemantation
        #region IJoolyter - maybe redundant TODO: check and delete
        public bool IsActive { get; private set; }
        public Vector2 Position { get; set; } = new Vector2();

        public void ClampToScreen(RectTransform rect)
        {
            UIMasterController.ClampToScreen(rect, -rect.sizeDelta/2);
        }

        public void SyncFontSize()
        {
            // Get all InputFields in tuple, get all text components in children, assign value of
            // JoolyterMain's FontSize property to text component's font size.
            // Needs TMP and thus, done here
            TMPInputFieldHolder[] inputs = _mainWindow.GetComponentsInChildren<TMPInputFieldHolder>();
            foreach (TMPInputFieldHolder input in inputs)
            {
                TMP_Text[] texts = input.GetComponentsInChildren<TMP_Text>();

                foreach (TMP_Text text in texts)
                    text.fontSize = _mainWindow.FontSize;
            }
        }

        public void SelectConsoleInput()
        {
            GameObject.Find("ConsoleInputField").GetComponent<TMPInputFieldHolder>().Select();
        }
        #endregion

        #region ISimpleFileBrowser
        /// <summary>
        /// Interface copy of <c>FileBrowser</c>'s singleton.
        /// 
        /// TODO: Use static property <c>FileBrowser.IsOpen</c> instead of violating singleton definition.
        /// </summary>
        public FileBrowser SFBInstance { get; set; }
        #endregion
        #endregion

        #region MonoBehaviour methods
        private void Awake()
        {
            // Assign interface to use methods in Unity-focused namespace
            FileBrowser.FileBrowserInterface = this;

            // Add ToolbarIcon
            GameEvents.onGUIApplicationLauncherReady.Add(OnGUIAppLauncherReady);
            GameEvents.onGUIApplicationLauncherUnreadifying.Add(OnGUIAppLauncherUnreadifying);

            // Further implemantation of singleton pattern
            if (Instance == null)
            {
                Instance = this;
            }
            else
                Destroy(this);
        }

        private void Start()
        {
            // Redundant
            // TODO: delete
            Position = JoolyterSettings.Instance.EditorWindowPosition;
        }

        private void Update()
        {
            // Lock KSP's control inputs if FileBrowsser is opened
            if (SFBInstance != null)
            {
                // TODO: Use static FileBrowser.IsOpen. Makes interface and external singleton obsolete.
                if (SFBInstance.isActiveAndEnabled)
                {
                    InputLockManager.SetControlLock(SFBInstance.name);
                    _sfbCtrlLockActive = true;
                }
                else
                    InputLockManager.RemoveControlLock(SFBInstance.name);
            }

            // Demonstrates Bug [inactive]
            //BugDemoEscapeChar();
        }

        private void OnDestroy()
        {
            // removes the method that adds the button, otherwise it would get added multiple times.
            // needed since the script gets re-initialized by KSP at every scene change.
            GameEvents.onGUIApplicationLauncherReady.Remove(OnGUIAppLauncherReady);
            GameEvents.onGUIApplicationLauncherUnreadifying.Remove(OnGUIAppLauncherUnreadifying);
            ApplicationLauncher.Instance.RemoveModApplication(_launcherButton);

            _mainWindow.Destroy();

            IsActive = false;
            Destroy(_mainWindow.transform);

            if (JoolyterSettings.Instance.Save())
                Debug.Log($"[Joolyter]: Settings saved.");
        }
        #endregion

        /// <summary>
        /// Post processes imported prefab.
        /// 
        /// Fixes issues with processing some properties in <c>JoolyterLoader</c> before instantiation and adds listeners.
        /// </summary>
        /// <param name="prefab">Prefab that needs post processing</param>
        private static void PostProcessPrefab(JoolyterMain prefab)
        {
            // Hook up listeners(Add methods that get called at certain events)
            TMPInputFieldHolder[] inputs = prefab.GetComponentsInChildren<TMPInputFieldHolder>();

            foreach (TMPInputFieldHolder input in inputs)
            {
                // Lock game controls when typing in input fields
                input.onSelect.AddListener(delegate { InputLockManager.SetControlLock(input.gameObject.name); });
                input.onDeselect.AddListener(delegate { InputLockManager.RemoveControlLock(input.gameObject.name); });

                // TODO: Move to JoolyterMain by using InputHandler/-Holder events or
                //       add CopyListeners() method to ProcessPrefab() in JoolyterLoader
                // For CodeInputField activate line numbering and indication of change in title
                if (input.gameObject.name == "CodeInputField")
                    input.onValueChanged.AddListener(delegate
                    {
                        prefab.LineNumbering();
                        prefab.AddEditedIndicator();
                    });
                // For ConsoleInputField activate method that submits input if [Return]-key is pressed
                else if (input.gameObject.name == "ConsoleInputField")
                    input.onEndEdit.AddListener(delegate
                    {
                        if (Input.GetKeyDown(KeyCode.Return))
                            prefab.SubmitInputLine();
                    });
            }

            // Set text alignment
            GameObject obj = prefab.gameObject;

            List<TMP_Text> texts = new List<TMP_Text>();

            texts.AddRange(obj.GetChild("FileMenu").GetComponentsInChildren<TMP_Text>());
            texts.AddRange(obj.GetChild("ViewMenu").GetComponentsInChildren<TMP_Text>());
            texts.AddRange(obj.GetChild("ExecutionMenu").GetComponentsInChildren<TMP_Text>());

            foreach (TMP_Text text in texts)
            {
                text.alignment = TextAlignmentOptions.Midline;
                text.parseCtrlCharacters = false;
            }

            obj.GetChild("Title").GetComponent<TMP_Text>().alignment = TextAlignmentOptions.MidlineLeft;
            obj.GetChild("ConsoleInputField").GetComponentInChildren<TMP_Text>().alignment = TextAlignmentOptions.MidlineLeft;
            obj.GetChild("LineNumberOutputField").GetComponentInChildren<TMP_Text>().alignment = TextAlignmentOptions.TopRight;

            // Set InputField settings
            TMPInputFieldHolder lineNumbers = obj.GetChild("LineNumberOutputField").GetComponent<TMPInputFieldHolder>();
            TMPInputFieldHolder codeInput = obj.GetChild("CodeInputField").GetComponent<TMPInputFieldHolder>();
            TMPInputFieldHolder consoleInput = obj.GetChild("ConsoleInputField").GetComponent<TMPInputFieldHolder>();

            // Deactivate selection of all text inside InputField it is clicked
            // Deactivate reset of InputField if is deactivated
            lineNumbers.onFocusSelectAll = false;
            codeInput.onFocusSelectAll = false;
            consoleInput.onFocusSelectAll = false;
            lineNumbers.resetOnDeActivation = false;
            codeInput.resetOnDeActivation = false;
            consoleInput.resetOnDeActivation = false;
            lineNumbers.readOnly = true;

            // Hook up scrollbars to CodeInputField and LineNumberingOutput thow scrollbars are chosen
            // because syncing them turned out to be much more reliable than hooking one scrollbar up to both fields.
            codeInput.verticalScrollbar = obj.GetChild("CodeInput Scrollbar Vertical").GetComponent<Scrollbar>();
            lineNumbers.verticalScrollbar = obj.GetChild("NumberOutput Scrollbar Vertical").GetComponent<Scrollbar>();
        }

        /// <summary>
        /// Changes visibility to given parameter.
        /// </summary>
        /// <param name="isOn">Value visibilty is set to</param>
        private void SetActive(bool isOn)
        {
            // If _mainWindow is instantiated and hooked up,
            // set active (visible) and change indicator property
            if (_mainWindow != null)
            {
                _mainWindow.gameObject.SetActive(isOn);
                IsActive = isOn;
            }
        }

        /// <summary>
        /// Switches main window on and off
        /// </summary>
        private void WindowSwitch()
        {
            // If window is not visible: Open
            // otherwise: close
            if (!IsActive)
                OpenWindow();
            else
                CloseWindow();
        }

        /// <summary>
        /// Opens main window.
        /// 
        /// If opened for first time, window gets instatiated and set in Unity hierarchy.
        /// </summary>
        private void OpenWindow()
        {
            // Check if prefab was loaded (Reason is to not spam console and log with errors)
            if (JoolyterLoader.JoolyterMainWindow == null)
            {
                Debug.LogError("[Joolyter]: JoolyterCanvas prefan was not loaded.");
                return;
            }

            // If method is called for first time and thus, _mainWindow == null:
            // Instatiate (Unity method to add prefab to scene)
            // Set in scene hierarchy
            // Post process prefab
            if (_mainWindow == null)
            {
                _mainWindow = Instantiate(JoolyterLoader.JoolyterMainWindow).GetComponent<JoolyterMain>();

                _mainWindow.transform.SetParent(MainCanvasUtil.MainCanvas.transform);

                PostProcessPrefab(_mainWindow);

                //_mainWindow.SetInitialState(Instance);

                // Workaround for wierd sizing issue at first opening
                // "turning it on and off again" seems to help
                // TODO: fix in Unity
                SetActive(true);
                SetActive(false);
            }

            // Make prefabe visible
            SetActive(true);
        }

        /// <summary>
        /// Closes the main window.
        /// </summary>
        private void CloseWindow()
        {
            // Hide main window
            SetActive(false);
        }

        /// <summary>
        /// Adds toolbar button in KSP.
        /// </summary>
        /// <exception cref="Exception">
        /// Thwon when adding icon failed.
        /// </exception>
        private void OnGUIAppLauncherReady()
        {
            try
            {
                // KSP API documentation on method used: https://bit.ly/3cP8RnT
                _launcherButton = ApplicationLauncher.Instance.AddModApplication(
                    WindowSwitch,
                    WindowSwitch,
                    null,
                    null,
                    null,
                    null,
                    (ApplicationLauncher.AppScenes.SPACECENTER | ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.MAPVIEW | ApplicationLauncher.AppScenes.TRACKSTATION),
                    JoolyterLoader.ToolbarIcon);
            }
            catch (Exception ex)
            {
                Debug.LogError("[Joolyter]: Failed to add Applauncher button");
                Debug.LogException(ex);
            }
        }

        /// <summary>
        /// Fixes toolbar icon duplication issue.
        /// 
        /// Copied from Gyse from KSP forum: https://bit.ly/3qgwFUv
        /// </summary>
        /// <param name="scene">Actual game scene</param>
        private void OnGUIAppLauncherUnreadifying(GameScenes scene)
        {
            // remove button
            if ((ApplicationLauncher.Instance != null) && (_launcherButton != null))
            {
                ApplicationLauncher.Instance.RemoveModApplication(_launcherButton);
            }
        }

        #region Test methods
        /// <summary>
        /// Method <c>BugDemoEscapeChar</c> demonstrates escape character bug
        /// </summary>
        private void BugDemoEscapeChar()
        {
            if (Input.GetKeyDown("space"))
            {
                GameObject obj = MainCanvasUtil.MainCanvas.gameObject;

                TextMeshProUGUI[] tmps = obj.GetComponentsInChildren<TextMeshProUGUI>();

                foreach (TextMeshProUGUI tmp in tmps)
                {
                    //tmp.parseCtrlCharacters = !tmp.parseCtrlCharacters;
                    Debug.Log(tmp.name);
                    Debug.Log(tmp.parseCtrlCharacters);

                    if (tmp.parseCtrlCharacters)
                        tmp.parseCtrlCharacters = false;

                    if (!tmp.text.Contains(@"abc \n"))
                    {
                        tmp.text = tmp.text + @" abc \n sometext";
                    }
                    else 
                    { 
                        tmp.text = tmp.text + @" abc \\n sometext";
                    }
                }
            }
        }
        #endregion
    }
}