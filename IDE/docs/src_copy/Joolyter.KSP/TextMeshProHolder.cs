#region license
/*The MIT License (MIT)
CWTextMeshProHolder - An extension of TextMeshProUGUI for updating certain elements of the text

Copyright (c) 2016 DMagic

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
#endregion

using Joolyter.Unity;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace Joolyter.KSP
{
	/// <summary>
	/// Extension class of <c>TextMeshProUGUI</c>, that adds methods
	/// to manipulate properties at events raised through <c>TextHandler</c>.
	/// 
	/// Obtained from GitHub
	/// Copyright (c) 2016 DMagic
	/// Source: https://bit.ly/3L22Lgs
	/// </summary>
	public class TextMeshProHolder : TextMeshProUGUI
	{
		private TextHandler _handler;

		new private void Awake()
		{
			// Add listeners to built-in TMP events to raise TextHandler's events
			base.Awake();

			_handler = GetComponent<TextHandler>();

			if (_handler == null)
				return;

			_handler.OnColorUpdate.AddListener(new UnityAction<Color>(UpdateColor));
			_handler.OnTextUpdate.AddListener(new UnityAction<string>(UpdateText));
			_handler.OnFontChange.AddListener(new UnityAction<int>(UpdateFontSize));
		}

		/// <summary>
		/// Adds listeners to built-in TMP events to raise TextHandler's events
		/// </summary>
		/// <param name="h"><c>TextHandler to process</c></param>
		public void Setup(TextHandler h)
		{
			// Add listeners to built-in TMP events to raise TextHandler's events
			base.Awake();
			_handler = h;

			_handler.OnColorUpdate.AddListener(new UnityAction<Color>(UpdateColor));
			_handler.OnTextUpdate.AddListener(new UnityAction<string>(UpdateText));
			_handler.OnFontChange.AddListener(new UnityAction<int>(UpdateFontSize));
		}

		/// <summary>
		/// Updates color
		/// </summary>
		/// <param name="c"><c>Color</c> to update to</param>
		private void UpdateColor(Color c)
		{
			color = c;
		}

		/// <summary>
		/// Updates text
		/// </summary>
		/// <param name="t"><c>string</c> to update to</param>
		private void UpdateText(string t)
		{
			text = t;

            _handler.PreferredSize = new Vector2(preferredWidth, preferredHeight);
        }

		/// <summary>
		/// Updates font size
		/// </summary>
		/// <param name="size"><c>int</c> to update to</param>
		private void UpdateFontSize(int size)
		{
			fontSize = size;
		}
	}
}