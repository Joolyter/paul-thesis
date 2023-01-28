using System;
using System.IO;
using Joolyter.Unity;
using Joolyter.Unity.Interfaces;
using SimpleFileBrowser;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Joolyter.KSP
{
	/// <summary>
	/// Loads, processes and holds asset bundles to be loaded in KSP on demand of other scripts.
	/// 
	/// Closely based on DMagic's approach.
	/// Source(1): https://bit.ly/3Lju0Dv
	/// </summary>
	/* Source(2): https://bit.ly/3eGkH48
	 * 
	 * The MIT License (MIT)
	Copyright (c) 2014 DMagic

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in all
	copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
	SOFTWARE.*/
	[KSPAddon(KSPAddon.Startup.Instantly, true)]
	public class JoolyterLoader : MonoBehaviour, IPrefabLoader
	{
		/// <summary>
		/// Text Mesh Pro font asset that gets assigned to text elements.
		/// </summary>
		public static TMP_FontAsset ConsolaFont { get; private set; } = null;

		/// <summary>
		/// Main game object of application. (JoolyterCanvas with all its children)
		/// </summary>
		public static GameObject JoolyterMainWindow { get; private set; } = null;

		/// <summary>
		/// AssetBundle containing prefabs of applications main window.
		/// </summary>
		private AssetBundle _joolyterAssetBundle = null;

		/// <summary>
		/// Icon texture for toolbar
		/// </summary>
		public static Texture2D ToolbarIcon { get; private set; } = new Texture2D(2, 2);

		/// <summary>
		/// Directory from which script is executed.
		/// </summary>
		private string _baseDir = AppDomain.CurrentDomain.BaseDirectory;

		#region Interface implementation
		private AssetBundle _fileBrowserPrefabs = null;
		public AssetBundle FileBrowserPrefabs
		{
			get
			{
				if (_fileBrowserPrefabs == null)
					_fileBrowserPrefabs = AssetBundle.LoadFromFile(_baseDir + @"\GameData\Joolyter\Ressources\simplefilebrowser");
				return _fileBrowserPrefabs;
			}
		}

		private GameObject _consoleOutputGameObject;
		public GameObject ConsoleOutputGameObject
		{
			get
			{
				if (_consoleOutputGameObject == null)
				{
					_consoleOutputGameObject = _joolyterAssetBundle.LoadAsset("OutputLineText") as GameObject;
					ProcessPrefab(_consoleOutputGameObject);
				}
				return _consoleOutputGameObject;
			}
		}
		#endregion

		private void Awake()
		{
			// Replace interfaces (used as virtual classes) with this
			// Load and process prefab
			// Load toolbar icon
			JoolyterMain.PrefabLoaderInterface = this;
			FileBrowser.PrefabLoaderInterface = this;

			string path = $@"{_baseDir}\GameData\Joolyter\Ressources\";

			_joolyterAssetBundle = AssetBundle.LoadFromFile($@"{path}joolyterassetbundle");
			JoolyterMainWindow = _joolyterAssetBundle.LoadAsset("JoolyterCanvas") as GameObject;

			if (JoolyterMainWindow != null)
				ProcessPrefab(JoolyterMainWindow);

			ToolbarIcon.LoadImage(File.ReadAllBytes($@"{path}Textures\ToolbarIcon.png"));
		}

		/// <summary>
		/// Replaces all <c>UnityEngine.UI.Text</c> and <c>UnityEngine.UI.InputField</c>
		/// components of given game object with Text Mesh Pro counterparts. 
		/// </summary>
		/// <param name="obj"><c>GameObject</c> that gets processed</param>
		private void ProcessPrefab(GameObject obj)
		{
			// Get all TextHandler components
			// Replace Text components with TMP_Text
			// Get all InputHandler components
			// Replace InputField components with TMP_InputField
			if (obj == null)
				return;

			TextHandler[] textHandlers = obj.GetComponentsInChildren<TextHandler>(true);

			if (textHandlers == null)
				return;

			for (int i = 0; i < textHandlers.Length; i++)
				TMProFromText(textHandlers[i]);

			InputHandler[] inputHandlers = obj.GetComponentsInChildren<InputHandler>(true);

			if (inputHandlers == null)
				return;

			for (int i = 0; i < inputHandlers.Length; i++)
				TMPInputFromInput(inputHandlers[i]);
		}

		/// <summary>
		/// Replaces <c>UnityEngine.UI.Text</c> component of given <c>TextHandler</c>
		/// with <c>TMP_Text</c> component.
		/// </summary>
		/// <param name="handler">T<c>TextHandler</c> of game object that gets processed</param>
		private void TMProFromText(TextHandler handler)
		{
			// Get Text component
			// Store properties of Text component
			// Delete Text component
			// Add TextMeshProHolder component
			// Assign TMP component's properties with stored variables
			if (handler == null)
				return;

			Text text = handler.GetComponent<Text>();

			if (text == null)
				return;

			string t = text.text;
			Color c = text.color;
			int i = text.fontSize;
			bool r = text.raycastTarget;
			FontStyles sty = getStyle(text.fontStyle);
			TextAlignmentOptions align = getAnchor(text.alignment);
			float spacing = text.lineSpacing;
			GameObject obj = text.gameObject;

			DestroyImmediate(text);

			TextMeshProHolder tmp = obj.AddComponent<TextMeshProHolder>();

			tmp.text = t;
			tmp.color = c;
			tmp.fontSize = i;
			tmp.raycastTarget = r;
			tmp.alignment = align;
			tmp.fontStyle = sty;
			tmp.lineSpacing = spacing;
			tmp.font = Resources.Load(@"Fonts\Calibri SDF", typeof(TMP_FontAsset)) as TMP_FontAsset;
			tmp.fontSharedMaterial = Resources.Load(@"Fonts\Materials\Calibri Dropshadow", typeof(Material)) as Material;

			tmp.enableWordWrapping = true;
			tmp.isOverlay = false;
			tmp.parseCtrlCharacters = true;

			if (tmp.name == "Title")
				tmp.richText = true;
			else
			{
				tmp.richText = false;
			}

			tmp.Setup(handler);
		}

		/// <summary>
		/// Replaces <c>UnityEngine.UI.InputField</c> component of given <c>TextHandler</c>
		/// with <c>TMP_Text</c> component.
		/// </summary>
		/// <param name="handler">T<c>InputHandler</c> of game object that gets processed</param>
		private void TMPInputFromInput(InputHandler handler)
		{
			// Get InputField component
			// Store properties of InputField component
			// Delete InputField component
			// Add TMPInputFieldHolder component
			// Assign TMP component's properties with stored variables
			if (handler == null)
				return;

			InputField input = handler.GetComponent<InputField>();

			if (input == null)
				return;

			int limit = input.characterLimit;
			TMP_InputField.ContentType content = GetTMPContentType(input.contentType);
			TMP_InputField.LineType line = TMPProUtil.LineType(input.lineType);
			float caretBlinkRate = input.caretBlinkRate;
			int caretWidth = input.caretWidth;
			Color selectionColor = input.selectionColor;
			ColorBlock colors = input.colors;
			bool readOnly = !input.interactable;
			GameObject obj = input.gameObject;

			RectTransform viewport = handler.GetComponentInChildren<RectMask2D>().rectTransform;
			if (viewport == null)
				return;

			TextMeshProHolder[] children = handler.GetComponentsInChildren<TextMeshProHolder>();
			TextMeshProHolder placeholder = null;
			TextMeshProHolder textComponent = null;

			foreach (TextMeshProHolder child in children)
			{
				if (child.gameObject.name == "Placeholder")
					placeholder = child;
				else if (child.gameObject.name == "Text")
					textComponent = child;
				else
					return;
			}

			DestroyImmediate(input);

			TMPInputFieldHolder tmp = obj.AddComponent<TMPInputFieldHolder>();

			tmp.textViewport = viewport;
			tmp.placeholder = placeholder;
			tmp.textComponent = textComponent;

			tmp.characterLimit = limit;
			tmp.contentType = content;
			tmp.lineType = line;
			tmp.caretBlinkRate = caretBlinkRate;
			tmp.caretWidth = caretWidth;
			tmp.selectionColor = selectionColor;
			tmp.colors = colors;
			tmp.readOnly = readOnly;
			tmp.shouldHideMobileInput = false;
			tmp.richText = false;
			tmp.isRichTextEditingAllowed = false;
			tmp.fontAsset = UISkinManager.TMPFont;
		}

		/// <summary>
		/// Comverts font style from Unity.UI format to TMP format
		/// </summary>
		/// <param name="style"><c>FontStyle</c> that gets parsed</param>
		/// <returns></returns>
		private FontStyles getStyle(FontStyle style)
		{
			switch (style)
			{
				case FontStyle.Normal:
					return FontStyles.Normal;
				case FontStyle.Bold:
					return FontStyles.Bold;
				case FontStyle.Italic:
					return FontStyles.Italic;
				case FontStyle.BoldAndItalic:
					return FontStyles.Bold;
				default:
					return FontStyles.Normal;
			}
		}

		/// <summary>
		/// Comverts text alignment from Unity.UI format to TMP format
		/// </summary>
		/// <param name="anchor"><c>TextAnchor</c> that gets parsed</param>
		/// <returns></returns>
		private TextAlignmentOptions getAnchor(TextAnchor anchor)
		{
			switch (anchor)
			{
				case TextAnchor.UpperLeft:
					return TextAlignmentOptions.TopLeft;
				case TextAnchor.UpperCenter:
					return TextAlignmentOptions.Top;
				case TextAnchor.UpperRight:
					return TextAlignmentOptions.TopRight;
				case TextAnchor.MiddleLeft:
					return TextAlignmentOptions.MidlineLeft;
				case TextAnchor.MiddleCenter:
					return TextAlignmentOptions.Midline;
				case TextAnchor.MiddleRight:
					return TextAlignmentOptions.MidlineRight;
				case TextAnchor.LowerLeft:
					return TextAlignmentOptions.BottomLeft;
				case TextAnchor.LowerCenter:
					return TextAlignmentOptions.Bottom;
				case TextAnchor.LowerRight:
					return TextAlignmentOptions.BottomRight;
				default:
					return TextAlignmentOptions.Center;
			}
		}

		/// <summary>
		/// Comverts content type from Unity.UI format to TMP format
		/// </summary>
		/// <param name="type"><c>InputField.ContentType</c> that gets parsed</param>
		/// <returns></returns>
		private TMP_InputField.ContentType GetTMPContentType(InputField.ContentType type)
		{
			switch (type)
			{
				case InputField.ContentType.Alphanumeric:
					return TMP_InputField.ContentType.Alphanumeric;
				case InputField.ContentType.Autocorrected:
					return TMP_InputField.ContentType.Autocorrected;
				case InputField.ContentType.Custom:
					return TMP_InputField.ContentType.Custom;
				case InputField.ContentType.DecimalNumber:
					return TMP_InputField.ContentType.DecimalNumber;
				case InputField.ContentType.EmailAddress:
					return TMP_InputField.ContentType.EmailAddress;
				case InputField.ContentType.IntegerNumber:
					return TMP_InputField.ContentType.IntegerNumber;
				case InputField.ContentType.Name:
					return TMP_InputField.ContentType.Name;
				case InputField.ContentType.Password:
					return TMP_InputField.ContentType.Password;
				case InputField.ContentType.Pin:
					return TMP_InputField.ContentType.Pin;
				case InputField.ContentType.Standard:
					return TMP_InputField.ContentType.Standard;
				default:
					return TMP_InputField.ContentType.Standard;
			}
		}
	}
}