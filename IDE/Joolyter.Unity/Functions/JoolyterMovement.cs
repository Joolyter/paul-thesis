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

namespace Joolyter.Unity.Functions
{
	/// <summary>
	/// Class that holds movement methods. 
	/// 
	/// Obtained from GitHub (14.06.2022): https://bit.ly/3RIGIxX
	/// Changed to fit needs
	/// Published under MIT License
	/// Copyright (c) 2016 Süleyman Yasir KULA
	/// </summary>
	public class JoolyterMovement : MonoBehaviour
	{
		#region Variables
		/// <summary>
		/// Instance of <c>JoolyterMain</c>. 
		/// 
		/// Is later assigned to component of JoolyterCanvas.
		/// </summary>
		private JoolyterMain _joolyterMain;
		/// <summary>
		/// Component <c>RectTransform</c> of JoolyterCanvas in Unity. 
		/// 
		/// Is later assigned to component of JoolyterCanvas.
		/// </summary>
		private RectTransform _canvasTR;
		/// <summary>
		/// Instance of <c>Camera</c>
		/// </summary>
		private Camera _canvasCam;

		/// <summary>
		/// Component <c>RectTransform</c> of JoolyterPanel in Unity.
		/// 
		/// Assigned in Unity editor.
		/// </summary>
		[SerializeField]
		private RectTransform _window;

		/// <summary>
		/// Initial Position where ResizeGizmo is clicked.
		/// </summary>
		private Vector2 _initialTouchPos = Vector2.zero;
		/// <summary>
		/// Initioal anchored position of window
		/// </summary>
		private Vector2 _initialAnchoredPos;
		/// <summary>
		/// Initial size delta of window
		/// </summary>
		private Vector2 _initialSizeDelta;
		#endregion

		#region Initialization Functions
		/// <summary>
		/// Assigns given parameter to variables.
		/// </summary>
		/// <param name="canvas"></param>
		public void Initialize(JoolyterMain canvas)
		{
			this._joolyterMain = canvas;
			_canvasTR = canvas.GetComponent<RectTransform>();
		}
		#endregion

		#region Pointer Events
		/// <summary>
		/// Method is assigned to <c>OnBeginDrag()</c> in Unity.
		/// 
		/// Gets called before a drag is started.
		/// </summary>
		public void OnDragStarted(BaseEventData data)
		{
			PointerEventData pointer = (PointerEventData)data;

			_canvasCam = pointer.pressEventCamera;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(_window, pointer.pressPosition, _canvasCam, out _initialTouchPos);
		}

		/// <summary>
		/// Method is assigned to <c>OnDrag()</c> in Unity.
		/// 
		/// Gets called while dragging.
		/// </summary>
		public void OnDrag(BaseEventData data)
		{
			PointerEventData pointer = (PointerEventData)data;

            Vector2 touchPos;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(_window, pointer.position, _canvasCam, out touchPos);
			_window.anchoredPosition += touchPos - _initialTouchPos;
		}

		/// <summary>
		/// Method is assigned to <c>OnEndDrag()</c> in Unity.
		/// 
		/// Gets called when a drag is ended.
		/// </summary>
		public void OnEndDrag()
		{
			_joolyterMain.EnsureWindowIsWithinBounds();
		}

		/// <summary>
		/// Method is assigned to <c>OnBeginDrag()</c> of ResizeGizmo in Unity. Sets starting point for resizing.
		/// 
		/// Gets called before a drag is started.
		/// </summary>
		public void OnResizeStarted(BaseEventData data)
		{
			PointerEventData pointer = (PointerEventData)data;

			_canvasCam = pointer.pressEventCamera;
			_initialAnchoredPos = _window.anchoredPosition;
			_initialSizeDelta = _window.sizeDelta;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasTR, pointer.pressPosition, _canvasCam, out _initialTouchPos);
		}

		/// <summary>
		/// Method is assigned to <c>OnBeginDrag()</c> of ResizeGizmo in Unity. Manages resizing functionality.
		/// 
		/// Gets called while dragging.
		/// </summary>
		public void OnResize(BaseEventData data)
		{
			PointerEventData pointer = (PointerEventData)data;

			Vector2 touchPos;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasTR, pointer.position, _canvasCam, out touchPos);

			Vector2 delta = touchPos - _initialTouchPos;
			Vector2 newSize = _initialSizeDelta + new Vector2(delta.x, -delta.y);
			Vector2 canvasSize = _canvasTR.sizeDelta;

			if (newSize.x < _joolyterMain._minWidth) 
				newSize.x = _joolyterMain._minWidth;

			if (newSize.y < _joolyterMain._minHeight) 
				newSize.y = _joolyterMain._minHeight;

			if (newSize.x > canvasSize.x) 
				newSize.x = canvasSize.x;

			if (newSize.y > canvasSize.y) 
				newSize.y = canvasSize.y;

			newSize.x = (int)newSize.x;
			newSize.y = (int)newSize.y;

			delta = newSize - _initialSizeDelta;

			_window.anchoredPosition = _initialAnchoredPos + new Vector2(delta.x * 0.5f, delta.y * -0.5f);

			if (_window.sizeDelta != newSize)
			{
				_window.sizeDelta = newSize;
			}
		}

		/// <summary>
		/// Method is assigned to <c>OnBeginDrag()</c> of ResizeGizmo in Unity.
		/// 
		/// Gets called when a drag is ended.
		/// </summary>
		public void OnEndResize()
		{
			_joolyterMain.EnsureWindowIsWithinBounds();
		}
		#endregion
	}
}