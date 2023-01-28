#region license
/*The MIT License (MIT)
InputHandler - Script for handling Input field object replacement with Text Mesh Pro
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

using UnityEngine;
using UnityEngine.Events;

namespace Joolyter.Unity
{
	/// <summary>
	/// Class to tag Unity's InputField elements, so TextMeshPro components can replace Unity UI's InputFields
	/// in application's *.KSP assembly.
	/// 
	/// Holds methods that allow implementation of input field manipulation in *.Unity assembly.
	/// 
	/// Obtained from GitHub (10.09.2022): https://bit.ly/3TZCTFT
	/// Published under MIT License
	/// Copyright (c) 2016 DMagic
	/// </summary>
	public class InputHandler : MonoBehaviour
	{
		/// <summary>
		/// Represents text content of TMP_InputField
		/// </summary>
		private string _text;
		/// <summary>
		/// Indicates if TMP_InputField is active and can be typed in.
		/// </summary>
		private bool _isFocused;

		/// <summary>
		/// Updates the text for the TMP_InputField.
		/// </summary>
		public class OnTextEvent : UnityEvent<string> { }
		/// <summary>
		/// Gets raised whenever anything is typed into the input field.
		/// </summary>
		public class OnValueChanged : UnityEvent<string> { }

		/// <summary>
		/// Instance of <c>OnTextEvent</c>
		/// </summary>
		private OnTextEvent _onTextUpdate = new OnTextEvent();
		/// <summary>
		/// Instance of <c>OnValueChanged</c>
		/// </summary>
		private OnValueChanged _onValueChanged = new OnValueChanged();

		/// <value>
		/// Property that gets and sets text content of TMP_InputField.
		/// </value>
		public string Text
		{
			get { return _text; }
			set { _text = value; }
		}

		/// <value>
		/// Property that gets and sets indication if TMP_InputField is active and can be typed in.
		/// </value>
		public bool IsFocused
		{
			get { return _isFocused; }
			set { _isFocused = value; }
		}

		/// <value>
		/// Property that raises Unity event to change of text content.
		/// </value>
		public UnityEvent<string> OnTextUpdate
		{
			get { return _onTextUpdate; }
		}

		/// <value>
		/// Property that raises Unity event IF content is edited.
		/// </value>
		public UnityEvent<string> OnValueChange
		{
			get { return _onValueChanged; }
		}
    }
}