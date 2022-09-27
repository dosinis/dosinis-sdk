using System;
using System.Collections.Generic;
using UnityEngine;

namespace DosinisSDK.Utils
{
	[Serializable]
    public class Observable<T>
	{
		[SerializeField] private T value;

		public T Value
		{
			get => value;

			set
			{
				if (EqualityComparer<T>.Default.Equals(this.value, value) == false)
				{
					this.value = value;
					OnValueChanged?.Invoke(this.value);
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
