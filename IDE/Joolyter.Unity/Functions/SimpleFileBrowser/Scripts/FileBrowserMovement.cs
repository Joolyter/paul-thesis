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
	/// Class in namespace <c>SimpleFileBrowser</c> that holds movement methods.
	/// 
	/// Obtained from GitHub (14.06.2022): https://bit.ly/3QuF9SF
	/// Published under MIT License
	/// Copyright (c) 2016 Süleyman Yasir KULA
	/// </summary>
	public class FileBrowserMovement : MonoBehaviour
	{
		#region Variables
#pragma warning disable 0649
		private FileBrowser fileBrowser;
		private RectTransform canvasTR;
		private Camera canvasCam;

		[SerializeField]
		private RectTransform window;

		[SerializeField]
		private RecycledListView listView;
#pragma warning restore 0649

		private Vector2 initialTouchPos = Vector2.zero;
		private Vector2 initialAnchoredPos, initialSizeDelta;
		#endregion

		#region Initialization Functions
		public void Initialize( FileBrowser fileBrowser )
		{
			this.fileBrowser = fileBrowser;
			canvasTR = fileBrowser.GetComponent<RectTransform>();
		}
		#endregion

		#region Pointer Events
		public void OnDragStarted( BaseEventData data )
		{
			PointerEventData pointer = (PointerEventData) data;

			canvasCam = pointer.pressEventCamera;
			RectTransformUtility.ScreenPointToLocalPointInRectangle( window, pointer.pressPosition, canvasCam, out initialTouchPos );
		}

		public void OnDrag( BaseEventData data )
		{
			PointerEventData pointer = (PointerEventData) data;

			Vector2 touchPos;
			RectTransformUtility.ScreenPointToLocalPointInRectangle( window, pointer.position, canvasCam, out touchPos );
			window.anchoredPosition += touchPos - initialTouchPos;
		}

		public void OnEndDrag( BaseEventData data )
		{
			fileBrowser.EnsureWindowIsWithinBounds();
		}

		public void OnResizeStarted( BaseEventData data )
		{
			PointerEventData pointer = (PointerEventData) data;

			canvasCam = pointer.pressEventCamera;
			initialAnchoredPos = window.anchoredPosition;
			initialSizeDelta = window.sizeDelta;
			RectTransformUtility.ScreenPointToLocalPointInRectangle( canvasTR, pointer.pressPosition, canvasCam, out initialTouchPos );
		}

		public void OnResize( BaseEventData data )
		{
			PointerEventData pointer = (PointerEventData) data;

			Vector2 touchPos;
			RectTransformUtility.ScreenPointToLocalPointInRectangle( canvasTR, pointer.position, canvasCam, out touchPos );

			Vector2 delta = touchPos - initialTouchPos;
			Vector2 newSize = initialSizeDelta + new Vector2( delta.x, -delta.y );
			Vector2 canvasSize = canvasTR.sizeDelta;

			if( newSize.x < fileBrowser.minWidth ) newSize.x = fileBrowser.minWidth;
			if( newSize.y < fileBrowser.minHeight ) newSize.y = fileBrowser.minHeight;

			if( newSize.x > canvasSize.x ) newSize.x = canvasSize.x;
			if( newSize.y > canvasSize.y ) newSize.y = canvasSize.y;

			newSize.x = (int) newSize.x;
			newSize.y = (int) newSize.y;

			delta = newSize - initialSizeDelta;

			window.anchoredPosition = initialAnchoredPos + new Vector2( delta.x * 0.5f, delta.y * -0.5f );

			if( window.sizeDelta != newSize )
			{
				window.sizeDelta = newSize;
				fileBrowser.OnWindowDimensionsChanged( newSize );
			}

			listView.OnViewportDimensionsChanged();
		}

		public void OnEndResize( BaseEventData data )
		{
			fileBrowser.EnsureWindowIsWithinBounds();
		}
		#endregion
	}
}