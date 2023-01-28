using UnityEngine;

namespace Joolyter.Unity.Interfaces
{
    /// <summary>
    /// Interface to call contents from <c>*.Unity</c> namespace and assign in <c>*.KSP</c> namespace.
    /// 
    /// TODO: Is redundant if porperty <c>FileBrowser.IsOpen</c> is used to track when simple file 
    /// browser is opened.
    /// </summary>
    public interface ISimpleFileBrowser
    {
        /// <value>
        /// Interface based copy of <c>FileBrowser.Instance</c>.
        /// Solution get singleton of <c>FileBrowser</c> which is hard to
        /// obtain because script andprefab are loaded seperate from
        /// actual application.
        /// </value>
        SimpleFileBrowser.FileBrowser SFBInstance { get; set; }
    }
}