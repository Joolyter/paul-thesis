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

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

namespace SimpleFileBrowser
{
	/// <summary>
	/// Class in namespace <c>SimpleFileBrowser</c>.
	/// 
	/// Obtained from GitHub (14.06.2022): https://bit.ly/3QuF9SF
	/// Published under MIT License
	/// Copyright (c) 2016 Süleyman Yasir KULA
	/// </summary>
	public class FileBrowserDeleteConfirmationPanel : MonoBehaviour
	{
		public delegate void OnDeletionConfirmed();

#pragma warning disable 0649
		[SerializeField]
		private Text titleLabel;

		[SerializeField]
		private GameObject[] deletedItems;

		[SerializeField]
		private Image[] deletedItemIcons;

		[SerializeField]
		private Text[] deletedItemNames;

		[SerializeField]
		private GameObject deletedItemsRest;

		[SerializeField]
		private Text deletedItemsRestLabel;

		[SerializeField]
		private RectTransform yesButtonTransform;

		[SerializeField]
		private RectTransform noButtonTransform;

		[SerializeField]
		private float narrowScreenWidth = 380f;
#pragma warning restore 0649

		private OnDeletionConfirmed onDeletionConfirmed;

		internal void Show( FileBrowser fileBrowser, List<FileSystemEntry> items, List<int> selectedItemIndices, OnDeletionConfirmed onDeletionConfirmed )
		{
			this.onDeletionConfirmed = onDeletionConfirmed;

			for( int i = 0; i < deletedItems.Length; i++ )
				deletedItems[i].SetActive( i < selectedItemIndices.Count );

			for( int i = 0; i < deletedItems.Length && i < selectedItemIndices.Count; i++ )
			{
				deletedItemIcons[i].sprite = fileBrowser.GetIconForFileEntry( items[selectedItemIndices[i]] );
				deletedItemNames[i].text = items[selectedItemIndices[i]].Name;
			}

			if( selectedItemIndices.Count > deletedItems.Length )
			{
				deletedItemsRestLabel.text = string.Concat( "...and ", ( selectedItemIndices.Count - deletedItems.Length ).ToString(), " other" );
				deletedItemsRest.SetActive( true );
			}
			else
				deletedItemsRest.SetActive( false );

			gameObject.SetActive( true );
		}

		// Handles responsive user interface
		internal void OnCanvasDimensionsChanged( Vector2 size )
		{
			if( size.x >= narrowScreenWidth )
			{
				yesButtonTransform.anchorMin = new Vector2( 0.5f, 0f );
				yesButtonTransform.anchorMax = new Vector2( 0.75f, 1f );
				noButtonTransform.anchorMin = new Vector2( 0.75f, 0f );
			}
			else
			{
				yesButtonTransform.anchorMin = Vector2.zero;
				yesButtonTransform.anchorMax = new Vector2( 0.5f, 1f );
				noButtonTransform.anchorMin = new Vector2( 0.5f, 0f );
			}
		}

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WSA || UNITY_WSA_10_0
		private void LateUpdate()
		{
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
			if( Keyboard.current != null )
#endif
			{
				// Handle keyboard shortcuts
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
				if( Keyboard.current[Key.Enter].wasPressedThisFrame || Keyboard.current[Key.NumpadEnter].wasPressedThisFrame )
#else
				if( Input.GetKeyDown( KeyCode.Return ) || Input.GetKeyDown( KeyCode.KeypadEnter ) )
#endif
					YesButtonClicked();

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
				if( Keyboard.current[Key.Escape].wasPressedThisFrame )
#else
				if( Input.GetKeyDown( KeyCode.Escape ) )
#endif
					NoButtonClicked();
			}
		}
#endif

		internal void RefreshSkin( UISkin skin )
		{
			Image background = GetComponentInChildren<Image>();
			background.color = skin.DeletePanelBackgroundColor;
			background.sprite = skin.DeletePanelBackground;

			skin.ApplyTo( yesButtonTransform.GetComponent<Button>() );
			skin.ApplyTo( noButtonTransform.GetComponent<Button>() );

			skin.ApplyTo( titleLabel, skin.DeletePanelTextColor );
			skin.ApplyTo( deletedItemsRestLabel, skin.DeletePanelTextColor );

			for( int i = 0; i < deletedItemNames.Length; i++ )
				skin.ApplyTo( deletedItemNames[i], skin.DeletePanelTextColor );

			for( int i = 0; i < deletedItems.Length; i++ )
				deletedItems[i].GetComponent<LayoutElement>().preferredHeight = skin.FileHeight;
		}

		public void YesButtonClicked()
		{
			gameObject.SetActive( false );

			if( onDeletionConfirmed != null )
				onDeletionConfirmed();
		}

		public void NoButtonClicked()
		{
			gameObject.SetActive( false );
		}
	}
}