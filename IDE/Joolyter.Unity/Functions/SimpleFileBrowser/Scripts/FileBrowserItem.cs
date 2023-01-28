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

namespace SimpleFileBrowser
{
	/// <summary>
	/// Class in namespace <c>SimpleFileBrowser</c> that holds movement methods.
	/// 
	/// Obtained from GitHub (14.06.2022): https://bit.ly/3QuF9SF
	/// Published under MIT License
	/// Copyright (c) 2016 Süleyman Yasir KULA
	/// </summary>
	public class FileBrowserItem : ListItem, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
	{
		#region Constants
		private const float DOUBLE_CLICK_TIME = 0.5f;
		private const float TOGGLE_MULTI_SELECTION_HOLD_TIME = 0.5f;
		#endregion

		#region Variables
		protected FileBrowser fileBrowser;

#pragma warning disable 0649
		[SerializeField]
		private Image background;

		[SerializeField]
		private Image icon;
		public Image Icon { get { return icon; } }

		[SerializeField]
		private Image multiSelectionToggle;

		[SerializeField]
		private Text nameText;
#pragma warning restore 0649

#pragma warning disable 0414
		private bool isSelected, isHidden;
#pragma warning restore 0414

		private UISkin skin;

		private float pressTime = Mathf.Infinity;
		private float prevClickTime;
		#endregion

		#region Properties
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

		public string Name { get { return nameText.text; } }
		public bool IsDirectory { get; private set; }
		#endregion

		#region Initialization Functions
		public void SetFileBrowser( FileBrowser fileBrowser, UISkin skin )
		{
			this.fileBrowser = fileBrowser;
			OnSkinRefreshed( skin, false );
		}

		public void SetFile( Sprite icon, string name, bool isDirectory )
		{
			this.icon.sprite = icon;
			nameText.text = name;

			IsDirectory = isDirectory;
		}
		#endregion

		#region Messages
		private void Update()
		{
			if( fileBrowser.AllowMultiSelection && Time.realtimeSinceStartup - pressTime >= TOGGLE_MULTI_SELECTION_HOLD_TIME )
			{
				// Item is held for a while
				pressTime = Mathf.Infinity;
				fileBrowser.OnItemHeld( this );
			}
		}
		#endregion

		#region Pointer Events
		public void OnPointerClick( PointerEventData eventData )
		{
			if( eventData.button == PointerEventData.InputButton.Middle )
				return;
			else if( eventData.button == PointerEventData.InputButton.Right )
			{
				// First, select the item
				if( !isSelected )
				{
					prevClickTime = 0f;
					fileBrowser.OnItemSelected( this, false );
				}

				// Then, show the context menu
				fileBrowser.OnContextMenuTriggered( eventData.position );
				return;
			}

			if( Time.realtimeSinceStartup - prevClickTime < DOUBLE_CLICK_TIME )
			{
				prevClickTime = 0f;
				fileBrowser.OnItemSelected( this, true );
			}
			else
			{
				prevClickTime = Time.realtimeSinceStartup;
				fileBrowser.OnItemSelected( this, false );
			}
		}

		public void OnPointerDown( PointerEventData eventData )
		{
			if( eventData.button != PointerEventData.InputButton.Left )
				return;

			pressTime = Time.realtimeSinceStartup;
		}

		public void OnPointerUp( PointerEventData eventData )
		{
			if( eventData.button != PointerEventData.InputButton.Left )
				return;

			if( pressTime != Mathf.Infinity )
				pressTime = Mathf.Infinity;
			else if( fileBrowser.MultiSelectionToggleSelectionMode )
			{
				// We have activated MultiSelectionToggleSelectionMode with this press, processing the click would result in
				// deselecting this item since its selected state would be toggled
				eventData.eligibleForClick = false;
			}
		}

		public void OnPointerEnter( PointerEventData eventData )
		{
			if( !isSelected )
				background.color = skin.FileHoveredBackgroundColor;
		}

		public void OnPointerExit( PointerEventData eventData )
		{
			if( !isSelected )
				background.color = ( Position % 2 ) == 0 ? skin.FileNormalBackgroundColor : skin.FileAlternatingBackgroundColor;
		}
		#endregion

		#region Other Events
		public void SetSelected( bool isSelected )
		{
			this.isSelected = isSelected;

            background.color = isSelected ? skin.FileSelectedBackgroundColor : ((Position % 2) == 0 ? skin.FileNormalBackgroundColor : skin.FileAlternatingBackgroundColor);
            nameText.color = isSelected ? skin.FileSelectedTextColor : skin.FileNormalTextColor;

            if ( isHidden )
			{
				Color c = nameText.color;
				c.a = 0.55f;
				nameText.color = c;
			}

			if( multiSelectionToggle ) // Quick links don't have multi-selection toggle
			{
				// Don't show multi-selection toggle for folders in file selection mode
				if( fileBrowser.MultiSelectionToggleSelectionMode && ( !IsDirectory || fileBrowser.PickerMode != FileBrowser.PickMode.Files ) )
				{
					if( !multiSelectionToggle.gameObject.activeSelf )
					{
						multiSelectionToggle.gameObject.SetActive( true );

						Vector2 shiftAmount = new Vector2( multiSelectionToggle.rectTransform.sizeDelta.x, 0f );
						icon.rectTransform.anchoredPosition += shiftAmount;
						nameText.rectTransform.anchoredPosition += shiftAmount;
					}

					multiSelectionToggle.sprite = isSelected ? skin.FileMultiSelectionToggleOnIcon : skin.FileMultiSelectionToggleOffIcon;
				}
				else if( multiSelectionToggle.gameObject.activeSelf )
				{
					multiSelectionToggle.gameObject.SetActive( false );

					Vector2 shiftAmount = new Vector2( -multiSelectionToggle.rectTransform.sizeDelta.x, 0f );
					icon.rectTransform.anchoredPosition += shiftAmount;
					nameText.rectTransform.anchoredPosition += shiftAmount;

					// Clicking a file shortly after disabling MultiSelectionToggleSelectionMode does nothing, this workaround fixes that issue
					prevClickTime = 0f;
				}
			}
		}

		public void SetHidden( bool isHidden )
		{
			this.isHidden = isHidden;

			Color c = icon.color;
			c.a = isHidden ? 0.5f : 1f;
			icon.color = c;

			c = nameText.color;
			c.a = isHidden ? 0.55f : ( isSelected ? skin.FileSelectedTextColor.a : skin.FileNormalTextColor.a );
			nameText.color = c;
		}

		public void OnSkinRefreshed( UISkin skin, bool isInitialized = true )
		{
			this.skin = skin;

			TransformComponent.sizeDelta = new Vector2( TransformComponent.sizeDelta.x, skin.FileHeight );
			skin.ApplyTo( nameText, isSelected ? skin.FileSelectedTextColor : skin.FileNormalTextColor );
			icon.rectTransform.sizeDelta = new Vector2( icon.rectTransform.sizeDelta.x, -skin.FileIconsPadding );

			if( isInitialized )
				SetSelected( isSelected );
		}
		#endregion
	}
}