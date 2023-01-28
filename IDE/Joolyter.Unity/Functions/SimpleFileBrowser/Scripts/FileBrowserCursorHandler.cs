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

namespace SimpleFileBrowser
{
	/// <summary>
	/// Class in namespace <c>SimpleFileBrowser</c>.
	/// 
	/// Obtained from GitHub (14.06.2022): https://bit.ly/3QuF9SF
	/// Published under MIT License
	/// Copyright (c) 2016 Süleyman Yasir KULA
	/// </summary>
	public class FileBrowserCursorHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler
	{
#pragma warning disable 0649
		[SerializeField]
		private Texture2D resizeCursor;
#pragma warning restore 0649

		private bool isHovering;
		private bool isResizing;

		void IPointerEnterHandler.OnPointerEnter( PointerEventData eventData )
		{
			isHovering = true;

			if( !eventData.dragging )
				ShowResizeCursor();
		}

		void IPointerExitHandler.OnPointerExit( PointerEventData eventData )
		{
			isHovering = false;

			if( !isResizing )
				ShowDefaultCursor();
		}

		void IBeginDragHandler.OnBeginDrag( PointerEventData eventData )
		{
			isResizing = true;
			ShowResizeCursor();
		}

		void IEndDragHandler.OnEndDrag( PointerEventData eventData )
		{
			isResizing = false;

			if( !isHovering )
				ShowDefaultCursor();
		}

		private void ShowDefaultCursor()
		{
			Cursor.SetCursor( null, Vector2.zero, CursorMode.Auto );
		}

		private void ShowResizeCursor()
		{
			Cursor.SetCursor( resizeCursor, new Vector2( resizeCursor.width * 0.5f, resizeCursor.height * 0.5f ), CursorMode.Auto );
		}
	}
}