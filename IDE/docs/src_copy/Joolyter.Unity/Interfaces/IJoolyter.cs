using UnityEngine;
using UnityEngine.UI;

namespace Joolyter.Unity.Interfaces
{
    /// <summary>
    /// Interface used for communication between - seems to be redundant
    /// TODO: delete
    /// </summary>
    public interface IJoolyter
    {
        /// <value>
        /// Flag for visibility of main window.
        /// TODO: Replace with property in JoolyterMain
        /// </value>
        bool IsActive { get; }

        /// <value>
        /// Position of prefab
        /// </value>
        Vector2 Position { get; set; }

        /// <summary>
        /// Fixes window position
        /// </summary>
        /// <param name="rect"></param>
        void ClampToScreen(RectTransform rect);

        /// <summary>
        /// Synchronizes font sizes between all input fields.
        /// Fix for wierd behaviour when quickly altering fint size.
        /// </summary>
        void SyncFontSize();

        /// <summary>
        /// Selects console input field
        /// </summary>
        void SelectConsoleInput();
    }
}