using UnityEngine;

namespace Joolyter.Unity.Interfaces
{
    /// <summary>
    /// Interface to call contents from <c>*.Unity</c> namespace and assign in <c>*.KSP</c> namespace.
    /// </summary>
    public interface IPrefabLoader
    {
        /// <value>
        /// Asset bundle that contains file browser prefabs.
        /// </value>
        AssetBundle FileBrowserPrefabs { get; }

        /// <value>
        /// Game object that represents one line in Python console.
        /// </value>
        GameObject ConsoleOutputGameObject { get; }
    }
}
