using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Joolyter.KSP
{
    /// <summary>
    /// TODO: Delete
    /// </summary>
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    class JoolyterSettings : MonoBehaviour
    {
        [Persistent]
        public Vector2 EditorWindowPosition = new Vector2(100, -100);
        [Persistent]
        public int FontOffest { get; set; } = 0;

        private const string fileName = "PluginData/settings.cfg";
        private string fullPath;

        public static JoolyterSettings Instance { get; set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != null)
                Destroy(this);

            fullPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), fileName).Replace("\\", "/");

            if (Load())
                Debug.Log("[JoolyterSettings]: Settings file loaded");
            else if (Save())
                Debug.Log($"[JoolyterSettings]: New Settings files generated at:\n{fullPath}");
        }

        private void OnDestroy()
        {
            if (Save())
                Debug.Log($"[JoolyterSettings]: New Settings files generated at:\n{fullPath}");
        }

        public bool Load()
        {
            bool settingsLoaded;

            try
            {
                if (File.Exists(fullPath))
                {
                    ConfigNode node = ConfigNode.Load(fullPath);
                    ConfigNode unwrapped = node.GetNode(GetType().Name);
                    ConfigNode.LoadObjectFromConfig(this, unwrapped);
                    settingsLoaded = true;
                }
                else
                {
                    Debug.Log($"[JoolyterSettings]: Settings file could not be found [{fullPath}]");
                    settingsLoaded = false;
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"[JoolyterSettings]: Error while loading settings file from [{fullPath}]\n{ex}");
                settingsLoaded = false;
            }

            return settingsLoaded;
        }

        public bool Save()
        {
            bool settingsSaved;

            try
            {
                ConfigNode node = AsConfigNode();
                ConfigNode wrapper = new ConfigNode(GetType().Name);
                wrapper.AddNode(node);
                wrapper.Save(fullPath);
                settingsSaved = true;
            }
            catch (Exception ex)
            {
                Debug.Log($"[JoolyterSettings]: Error while saving settings file from [{fullPath}]\n{ex}");
                settingsSaved = false;
            }

            return settingsSaved;
        }

        private ConfigNode AsConfigNode()
        {
            try
            {
                ConfigNode node = new ConfigNode(GetType().Name);

                node = ConfigNode.CreateConfigFromObject(this, node);
                return node;
            }
            catch (Exception ex)
            {
                Debug.Log($"[PersistentThrustSettings]: Failed to generate settings file node...\n{ex}");
                return new ConfigNode(GetType().Name);
            }
        }
    }
}
