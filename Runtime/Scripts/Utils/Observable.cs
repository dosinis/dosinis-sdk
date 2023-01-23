using System;
using System.Collections.Generic;
using UnityEngine;

namespace DosinisSDK.Utils
{
	[Serializable]
    public class Observable<T> : IObservable<T>
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

		public Observable(T value)
		{
			this.value = value;
		}

		public static implicit operator T(Observable<T> observable)
		{
			return observable.Value;
		}
		
		public override string ToString()
		{
			return value.ToString();
		}
	}

    public interface IObservable<out T>
    {
	    T Value { get; }
	    event Action<T> OnValueChanged;
    }
}
