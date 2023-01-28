#region license
/*The MIT License (MIT)
CWTextMeshProInputHolder - An extension of TMP_InputField for updating certain elements of the input field
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
using UnityEngine.Events;
using TMPro;

namespace Joolyter.KSP
{
	/// <summary>
	/// Extension class of <c>TMP_InputField</c>, that adds methods
	/// to manipulate properties at events raised through <c>InputHandler</c>.
	/// 
	/// Obtained from GitHub
	/// Copyright (c) 2016 DMagic
	/// Source: https://bit.ly/3Bxviak
	/// </summary>
	public class TMPInputFieldHolder : TMP_InputField
	{
		private InputHandler _handler;

		new private void Awake()
		{
			// Add listeners to built-in TMP events to raise InputHandler's events
			base.Awake();

			_handler = GetComponent<InputHandler>();

			onValueChanged.AddListener(new UnityAction<string>(valueChanged));

			_handler.OnTextUpdate.AddListener(new UnityAction<string>(UpdateText));
        }

		private void Update()
		{
			// Sync IsFocused properties 
			if (_handler != null)
				_handler.IsFocused = isFocused;
		}

		/// <summary>
		/// Changes content and raises event.
		/// </summary>
		private void valueChanged(string s)
		{
			if (_handler == null)
				return;

			_handler.Text = s;

			_handler.OnValueChange.Invoke(s);
		}

		/// <summary>
		/// Updates content of TMP_InputField
		/// </summary>
		/// <param name="t"></param>
		private void UpdateText(string t)
		{
			text = t;
		}
	}
}