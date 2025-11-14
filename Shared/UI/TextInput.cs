using Sandbox.ModAPI;
using System;
using System.Collections.Generic;

namespace RichHudFramework.UI
{
	public class TextInput
	{
		public Func<char, bool> IsCharAllowedFunc;

		private readonly Action<char> OnAppendAction;
		private readonly Action OnBackspaceAction;

		public TextInput(Action<char> OnAppendAction, Action OnBackspaceAction, Func<char, bool> IsCharAllowedFunc = null)
		{
			this.OnAppendAction = OnAppendAction;
			this.OnBackspaceAction = OnBackspaceAction;
			this.IsCharAllowedFunc = IsCharAllowedFunc;
		}

		public void HandleInput()
		{
			IReadOnlyList<char> input = MyAPIGateway.Input.TextInput;

			if (SharedBinds.Back.IsPressedAndHeld || SharedBinds.Back.IsNewPressed)
				OnBackspaceAction?.Invoke();

			for (int n = 0; n < input.Count; n++)
			{
				if (input[n] != '\b' && (IsCharAllowedFunc == null || IsCharAllowedFunc(input[n])))
					OnAppendAction?.Invoke(input[n]);
			}
		}
	}
}