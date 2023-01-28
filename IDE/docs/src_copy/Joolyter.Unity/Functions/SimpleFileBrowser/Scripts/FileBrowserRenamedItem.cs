#region license
/*MIT License

Copyright (c) 2016 Süleyman Yasir KULA

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
SOFTWARE.
*/
#endregion

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

namespace SimpleFileBrowser
{
	/// <summary>
	/// Class in namespace <c>SimpleFileBrowser</c> that holds movement methods.
	/// 
	/// Obtained from GitHub (14.06.2022): https://bit.ly/3QuF9SF
	/// Published under MIT License
	/// Copyright (c) 2016 Süleyman Yasir KULA
	/// </summary>
	public class FileBrowserRenamedItem : MonoBehaviour
	{
		public delegate void OnRenameCompleted( string filename );

#pragma warning disable 0649
		[SerializeField]
		private Image background;

		[SerializeField]
		private Image icon;

		[SerializeField]
		private InputField nameInputField;
		public InputField InputField { get { return nameInputField; } }
#pragma warning restore 0649

		private OnRenameCompleted onRenameCompleted;

		private RectTransform m_transform;
		public RectTransform TransformComponent
		{
			get
			{
				if( m_transform == null )
					m_transform = (RectTransform) transform;

				return m_transform;
			}
		}

		public void Show( string initialFilename, Color backgroundColor, Sprite icon, OnRenameCompleted onRenameCompleted )
		{
			background.color = backgroundColor;
			this.icon.sprite = icon;
			this.onRenameCompleted = onRenameCompleted;

			transform.SetAsLastSibling();
			gameObject.SetActive( true );

			nameInputField.text = initialFilename;
			nameInputField.ActivateInputField();
		}

		private void LateUpdate()
		{
			// Don't allow scrolling with mouse wheel while renaming a file or creating a folder
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
			if( Mouse.current != null && Mouse.current.scroll.ReadValue().y != 0f )
#else
			if( Input.mouseScrollDelta.y != 0f )
#endif
				nameInputField.DeactivateInputField();
		}

		public void OnInputFieldEndEdit( string filename )
		{
			gameObject.SetActive( false );

			// If we don't deselect the InputField manually, FileBrowser's keyboard shortcuts
			// no longer work until user clicks on a UI element and thus, deselects the InputField
			if( !EventSystem.current.alreadySelecting && EventSystem.current.currentSelectedGameObject == nameInputField.gameObject )
				EventSystem.current.SetSelectedGameObject( null );

			if( onRenameCompleted != null )
				onRenameCompleted( filename );
		}
	}
}