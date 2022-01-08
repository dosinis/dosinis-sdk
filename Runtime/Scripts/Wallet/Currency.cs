using UnityEngine;
using System;

namespace DosinisSDK.Wallet
{
    [Serializable]
    public class Currency
    {
        public Sprite Icon { get; set; }

        // Serialized

        public string name;

        [SerializeField] private int amount;

        // Events

        public event Action<int, int> OnAmountChanged; // final, added

        // Currency

        public Currency(string name, int initialAmount)
        {
            this.name = name;
            this.amount = initialAmount;
        }

        public void Add(int num)
        {
            amount += num;
            OnAmountChanged?.Invoke(amount, num);
        }

        public bool Spend(int num)
        {
            if (CanAfford(num) == false)
            {
                return false;
            }

            amount -= num;

            if (amount < 0)
                amount = 0;

            OnAmountChanged?.Invoke(amount, -num);

            return true;
        }

        public bool CanAfford(int num)
        {
            return amount - num >= 0;
        }

        public override string ToString()
        {
            return amount.ToString();
        }

        public static implicit operator int(Currency c) => c.amount;
    }
}
