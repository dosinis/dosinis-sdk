using System;
using System.Collections.Generic;

namespace DosinisSDK
{
    [Serializable]
	public class Observable<T>
	{
		private T value;

		public T Value
		{
			get => value;

			set
			{
				if (EqualityComparer<T>.Default.Equals(this.value, value) == false)
				{
					this.value = value;
					OnValueChanged?.Invoke(value);
				}
			}
		}

		public event Action<T> OnValueChanged;

		// Construct

		public Observable(T value)
		{
			this.value = value;
		}
	}
}
