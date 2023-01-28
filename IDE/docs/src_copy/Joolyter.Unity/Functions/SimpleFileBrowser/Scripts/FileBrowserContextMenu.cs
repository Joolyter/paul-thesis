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
using UnityEngine.UI;

namespace SimpleFileBrowser
{
	/// <summary>
	/// Class in namespace <c>SimpleFileBrowser</c>.
	/// 
	/// Obtained from GitHub (14.06.2022): https://bit.ly/3QuF9SF
	/// Published under MIT License
	/// Copyright (c) 2016 Süleyman Yasir KULA
	/// </summary>
	public class FileBrowserContextMenu : MonoBehaviour
	{
#pragma warning disable 0649
		[SerializeField]
		private FileBrowser fileBrowser;

		[SerializeField]
		private RectTransform rectTransform;

		[SerializeField]
		private Button selectAllButton;
		[SerializeField]
		private Button deselectAllButton;
		[SerializeField]
		private Button createFolderButton;
		[SerializeField]
		private Button deleteButton;
		[SerializeField]
		private Button renameButton;

		[SerializeField]
		private GameObject selectAllButtonSeparator;

		[SerializeField]
		private Text[] allButtonTexts;
		[SerializeField]
		private Image[] allButtonSeparators;

		[SerializeField]
		private float minDistanceToEdges = 10f;
#pragma warning restore 0649

		internal void Show( bool selectAllButtonVisible, bool deselectAllButtonVisible, bool deleteButtonVisible, bool renameButtonVisible, Vector2 position, bool isMoreOptionsMenu )
		{
			selectAllButton.gameObject.SetActive( selectAllButtonVisible );
			deselectAllButton.gameObject.SetActive( deselectAllButtonVisible );
			deleteButton.gameObject.SetActive( deleteButtonVisible );
			renameButton.gameObject.SetActive( renameButtonVisible );
			selectAllButtonSeparator.SetActive( !deselectAllButtonVisible );

			rectTransform.anchoredPosition = position;
			gameObject.SetActive( true );

			if( isMoreOptionsMenu )
				rectTransform.pivot = Vector2.one;
			else
			{
				// Find the optimal pivot value
				LayoutRebuilder.ForceRebuildLayoutImmediate( rectTransform );

				Vector2 size = rectTransform.sizeDelta;
				Vector2 canvasSize = fileBrowser.rectTransform.sizeDelta;

				// Take canvas' Pivot into consideration
				Vector2 positionOffset = canvasSize;
				positionOffset.Scale( fileBrowser.rectTransform.pivot );
				position += positionOffset;

				// Try bottom-right corner first
				Vector2 cornerPos = position + new Vector2( size.x + minDistanceToEdges, -size.y - minDistanceToEdges );
				if( cornerPos.x <= canvasSize.x && cornerPos.y >= 0f )
					rectTransform.pivot = new Vector2( 0f, 1f );
				else
				{
					// Try bottom-left corner
					cornerPos = position - new Vector2( size.x + minDistanceToEdges, size.y + minDistanceToEdges );
					if( cornerPos.x >= 0f && cornerPos.y >= 0f )
						rectTransform.pivot = Vector2.one;
					else
					{
						// Try top-right corner
						cornerPos = position + new Vector2( size.x + minDistanceToEdges, size.y + minDistanceToEdges );
						if( cornerPos.x <= canvasSize.x && cornerPos.y <= canvasSize.y )
							rectTransform.pivot = Vector2.zero;
						else
						{
							// Use top-left corner
							rectTransform.pivot = new Vector2( 1f, 0f );
						}
					}
				}
			}
		}

		public void Hide()
		{
			gameObject.SetActive( false );
		}

		internal void RefreshSkin( UISkin skin )
		{
			rectTransform.GetComponent<Image>().color = skin.ContextMenuBackgroundColor;

			deselectAllButton.image.color = skin.ContextMenuBackgroundColor;
			selectAllButton.image.color = skin.ContextMenuBackgroundColor;
			createFolderButton.image.color = skin.ContextMenuBackgroundColor;
			deleteButton.image.color = skin.ContextMenuBackgroundColor;
			renameButton.image.color = skin.ContextMenuBackgroundColor;

			for( int i = 0; i < allButtonTexts.Length; i++ )
				skin.ApplyTo( allButtonTexts[i], skin.ContextMenuTextColor );

			for( int i = 0; i < allButtonSeparators.Length; i++ )
				allButtonSeparators[i].color = skin.ContextMenuSeparatorColor;
		}

		public void OnSelectAllButtonClicked()
		{
			Hide();
			fileBrowser.SelectAllFiles();
		}

		public void OnDeselectAllButtonClicked()
		{
			Hide();
			fileBrowser.DeselectAllFiles();
		}

		public void OnCreateFolderButtonClicked()
		{
			Hide();
			fileBrowser.CreateNewFolder();
		}

		public void OnDeleteButtonClicked()
		{
			Hide();
			fileBrowser.DeleteSelectedFiles();
		}

		public void OnRenameButtonClicked()
		{
			Hide();
			fileBrowser.RenameSelectedFile();
		}
	}
}