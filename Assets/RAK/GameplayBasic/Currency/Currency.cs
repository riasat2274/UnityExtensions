using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
namespace RAK
{
    public class Currency
    {
        public static CurrencyManager<CurrencyType> _coinMan;
        public static CurrencyManager<CurrencyType> coinMan
        {
            get
            {
                if (_coinMan == null)
                {
                    _coinMan = new CurrencyManager<CurrencyType>();
                    _coinMan.EnsureInit(CurrencyType.BASE, 0);
                    _coinMan.EnsureInit(CurrencyType.CORE, BuildSpecManager.initialCoinBonus);
                    _coinMan.EnsureInit(CurrencyType.TOKEN, 10);
                    //_coinMan.EnsureInit(CurrencyType.ENERGY, EnergyTracker.ENERGY_CAP);
                }
                return _coinMan;
            }
        }


        public static double Balance(CurrencyType type = CurrencyType.BASE)
        {
            return coinMan.GetBalance(type);
        }
        public static void Transaction(CurrencyType type, double value)
        {
            if (BuildSpecManager.disableCoinReduction && value < 0) return;
            coinMan.ChangeBy(type, value);
        }

        public static bool Spend(CurrencyType type, double value)
        {
            if (BuildSpecManager.disableCoinReduction) return true;
            if (value > coinMan.GetBalance(type))
            {
                return false;
            }
            else
            {
                coinMan.ChangeBy(type, -value);
                return true;
            }
        }
        public static void Earn(CurrencyType type, double value)
        {
            coinMan.ChangeBy(type, value);
        }

        public static bool Spend(double value)
        {
            return Spend((CurrencyType)0, value);
        }
        public static void Earn(double value)
        {
            Earn((CurrencyType)0, value);
        }

        public static event Action<CurrencyType> onResourceRequested;
        public static void RequestMore(CurrencyType type)
        {
            onResourceRequested?.Invoke(type);
        }

    }
    public enum CurrencyType
    {
        FREE = -1,
        BASE = 0,
        CORE = 1,
        TOKEN = 2,
        STAR = 3,
        ENERGY = 4,
        KEY= 5,

        
    }
}