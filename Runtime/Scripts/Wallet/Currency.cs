using UnityEngine;
using System;

namespace DosinisSDK.Wallet
{
    [CreateAssetMenu(fileName = "Currency", menuName = "DosinisSDK/Wallet/Currency")]
    public class Currency : ScriptableObject
    {
        // Serialized
        
        public Sprite icon;
        public string currencyName;
        public int initAmount;

        // NonSerialized
        
        [NonSerialized] private CurrencyRef currencyRef;

        // Events

        public event Action<int, int> OnAmountChanged; // final, added

        // Currency

        public void Add(int num)
        {
            currencyRef.amount += num;
            OnAmountChanged?.Invoke(currencyRef.amount, num);
        }

        internal void Set(CurrencyRef currencyRef)
        {
            this.currencyRef = currencyRef;
        }

        public bool Spend(int num)
        {
            if (CanAfford(num) == false)
            {
                return false;
            }

            currencyRef.amount -= num;

            if (currencyRef.amount < 0)
                currencyRef.amount = 0;

            OnAmountChanged?.Invoke(currencyRef.amount, -num);

            return true;
        }

        public bool CanAfford(int num)
        {
            return currencyRef.amount - num >= 0;
        }

        public override string ToString()
        {
            return currencyRef.amount.ToString();
        }

        public static implicit operator int(Currency c) => c.currencyRef.amount;
    }
}
