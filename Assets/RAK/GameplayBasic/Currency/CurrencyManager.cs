using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RAK;
namespace RAK
{
    public class CurrencyManager<T_enum>
    {
        private Dictionary<T_enum, CurrencyNatural<T_enum>> naturals = new Dictionary<T_enum, CurrencyNatural<T_enum>>();

        private Dictionary<T_enum, Action<double>> balanceChangeEvents = new Dictionary<T_enum, Action<double>>();



        public void EnsureInit(T_enum id, double ini = 0)
        {
            if (!naturals.ContainsKey(id))
            {
                System.Action<double> onChange = null;
                balanceChangeEvents.Add(id, onChange);
                CurrencyNatural<T_enum> newC = new CurrencyNatural<T_enum>(id, ini, (double change) =>
                {
                    if (!balanceChangeEvents.ContainsKey(id)) return;
                    balanceChangeEvents[id]?.Invoke(change);
                });
                naturals.Add(id, newC);
            }
        }
        public void AddListner_BalanceChanged(T_enum id, System.Action<double> onChange)
        {
            EnsureInit(id);
            balanceChangeEvents[id] += onChange;
        }
        public void RemoveListner_BalanceChanged(T_enum id, System.Action<double> onChange)
        {
            EnsureInit(id);
            balanceChangeEvents[id] -= onChange;
        }

        public bool ChangeBy(T_enum id, double change)
        {
            EnsureInit(id);
            CurrencyNatural<T_enum> currency = naturals[id];
            if (currency.IsChangeValid(change))
            {
                currency.ChangeValueBy(change);
                return true;
            }
            else
                return false;
        }

        public double GetBalance(T_enum id)
        {
            EnsureInit(id);
            return naturals[id].balance;
        }

        public class CurrencyNatural<T>
        {
            T id;
            string idText;
            string hardDataID;
            HardData<double> _balance;


            event System.Action<double> onBalanceChanged;
            double initialValue;
            public double balance
            {
                private set
                {
                    if (_balance == null)
                    {
                        _balance = new HardData<double>(hardDataID, initialValue);
                    }
                    double change = value - _balance.value;
                    _balance.value = value;

                    onBalanceChanged?.Invoke(change);
                }

                get
                {
                    if (_balance == null)
                    {
                        _balance = new HardData<double>(hardDataID, initialValue);
                    }
                    return _balance;
                }
            }
            public CurrencyNatural(T ID, double initValue, System.Action<double> onChange)
            {

                id = ID;
                idText = ID.ToString();
                hardDataID = string.Format("BALANCE_{0}_{1}", typeof(T).ToString(), idText);
                initialValue = initValue;

                onBalanceChanged += onChange;
            }

            public bool IsChangeValid(double value)
            {
                if (value < 0)
                {
                    if ((-value) > balance) return false;
                }

                return true;
            }
            public void ChangeValueBy(double value)
            {
                if (value < 0)
                {
                    if ((-value) > balance) return;
                }

                balance += value;
            }
        }

        //public class CurrencyExtended<T> //: CurrencyNatural <T>
        //{
        //    T id;
        //    string idText;
        //    string hardDataID;
        //    HardData<int> _balance;


        //    event System.Action onBalanceChanged;
        //    int initialValue;
        //    public int balance
        //    {
        //        private set
        //        {
        //            if (_balance == null)
        //            {
        //                _balance = new HardData<int>(hardDataID, initialValue);
        //            }
        //            _balance.value = value;

        //            onBalanceChanged?.Invoke();
        //        }

        //        get
        //        {
        //            if (_balance == null)
        //            {
        //                _balance = new HardData<int>(hardDataID, initialValue);
        //            }
        //            return _balance;
        //        }
        //    }
        //    public CurrencyExtended(T ID, int initValue, System.Action onChange)
        //    {

        //        id = ID;
        //        idText = ID.ToString();
        //        hardDataID = string.Format("BALANCE_{0}_{1}", typeof(T).ToString(), idText);
        //        initialValue = initValue;

        //        onBalanceChanged += onChange;
        //    }

        //    public bool IsChangeValid(int value)
        //    {
        //        if (value < 0)
        //        {
        //            if ((-value) > balance) return false;
        //        }

        //        return true;
        //    }



        //    public void ChangeValueBy(int value)
        //    {
        //        if (value < 0)
        //        {
        //            if ((-value) > balance) return;
        //        }

        //        balance += value;
        //    }

        //    //public enum Power
        //    //{

        //    //}
        //}
    }
}