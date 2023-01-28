#region license
/*MIT License

Copyright (c) 2020 FreeThinker

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
using UnityEngine.Events;


namespace Joolyter.Unity
{
	/// <summary>
	/// Class to tag Uniy's Text elements, so TextMeshPro components can replace Unity UI's text
	/// in application's *.KSP assembly.
	/// 
	/// Holds methods that allow implementation of text manipulation in *.Unity assembly.
	/// 
	/// Obtained from GitHub (10.09.2022): https://bit.ly/3BwAjzW
	/// Published under MIT License
	/// Copyright (c) 2020 FreeThinker
	/// 
	/// Indicates to be closely based on DMagic's approach: https://bit.ly/3xfKsP7
	/// </summary>
	public class TextHandler : MonoBehaviour
	{
		/// <summary>
		/// Represents generic Unity event of type string. Is raised if text content is changed.
		/// </summary>
		public class OnTextEvent : UnityEvent<string> { }
		/// <summary>
		/// Represents generic Unity event of type Color. Is raised if text color is changed.
		/// </summary>
		public class OnColorEvent : UnityEvent<Color> { }
		/// <summary>
		/// Represents generic Unity event of type integer. Is raised if font size is changed.
		/// </summary>
		public class OnFontEvent : UnityEvent<int> { }

		/// <summary>
		/// Instance of <c>OnTextEvent</c>
		/// </summary>
		private OnTextEvent _onTextUpdate = new OnTextEvent();
		/// <summary>
		/// Instance of <c>OnTextEvent</c>
		/// </summary>
		private OnColorEvent _onColorUpdate = new OnColorEvent();
		/// <summary>
		/// Instance of <c>OnTextEvent</c>
		/// </summary>
		private OnFontEvent _onFontChange = new OnFontEvent();

		/// <summary>
		/// Private field of <c>PreferredSize</c>
		/// 
		/// May be redundant due to usage of fixed size text fields.
		/// TODO: Check and adjust
		/// </summary>
        private Vector2 _preferredSize = new Vector2();
		/// <value>
		/// Represents preferred size of text field.
		/// 
		/// May be redundant due to usage of fixed size text fields.
		/// TODO: Check and adjust
		/// </value>
		public Vector2 PreferredSize
        {
            get { return _preferredSize; }
            set { _preferredSize = value; }
        }

		/// <value>
		/// Property that raises Unity event for change of text content. 
		/// </value>
		public UnityEvent<string> OnTextUpdate
		{
			get { return _onTextUpdate; }
		}

		/// <value>
		/// Property that raises Unity event for change of font color. 
		/// </value>
		public UnityEvent<Color> OnColorUpdate
		{
			get { return _onColorUpdate; }
		}

		/// <value>
		/// Property that raises Unity event for change of font size.
		/// </value>
		public UnityEvent<int> OnFontChange
		{
			get { return _onFontChange; }
		}
	}
}
