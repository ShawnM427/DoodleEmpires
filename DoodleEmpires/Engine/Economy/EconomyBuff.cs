using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoodleEmpires.Engine.Economy
{
    /// <summary>
    /// Represents buffs for an economy
    /// </summary>
    public struct EconomyBuff
    {
        int _popCapRelative;
        float _stockPileRelative;
        float _resourceRateRelative;

        float _popCapMultiplier;
        float _stockPileMultiplier;
        float _resourceRateMultiplier;

        /// <summary>
        /// Gets or sets the relative population increase/decsrease caused by this buff
        /// </summary>
        public int PopulationCapRelative
        {
            get { return _popCapRelative; }
            set { _popCapRelative = value; }
        }
        /// <summary>
        /// Gets or sets the relative stockpile size increase/decsrease caused by this buff
        /// </summary>
        public float StockpileRelative
        {
            get { return _stockPileRelative; }
            set { _stockPileRelative = value; }
        }
        /// <summary>
        /// Gets or sets the relative resource per second increase/decsrease caused by this buff
        /// </summary>
        public float ResourceRateRelative
        {
            get { return _resourceRateRelative; }
            set { _resourceRateRelative = value; }
        }

        /// <summary>
        /// Gets or sets the relative population increase/decsrease caused by this buff
        /// </summary>
        public float PopulationCapMultiplier
        {
            get { return _popCapMultiplier; }
            set { _popCapMultiplier = value; }
        }
        /// <summary>
        /// Gets or sets the relative stockpile size increase/decsrease caused by this buff
        /// </summary>
        public float StockpileMultiplier
        {
            get { return _stockPileMultiplier; }
            set { _stockPileMultiplier = value; }
        }
        /// <summary>
        /// Gets or sets the relative resource per second increase/decsrease caused by this buff
        /// </summary>
        public float ResourceRateMultiplier
        {
            get { return _resourceRateMultiplier; }
            set { _resourceRateMultiplier = value; }
        }

        /// <summary>
        /// Creates a new blank economy buff
        /// </summary>
        private static EconomyBuff GetBlankBuff()
        {
            EconomyBuff buff = new EconomyBuff()
            {
                _popCapRelative = 0,
                _popCapMultiplier = 1.0f,
                _stockPileRelative = 0,
                _stockPileMultiplier = 1.0f,
                _resourceRateRelative = 0,
                _resourceRateMultiplier = 1.0f
            };
            return buff;
        }

        /// <summary>
        /// Gets an empty unit buff
        /// </summary>
        public static EconomyBuff Empty
        {
            get { return GetBlankBuff();  }
        }

        /// <summary>
        /// Handles adding two economy bufs together
        /// </summary>
        /// <param name="a">The first economy buff</param>
        /// <param name="b">The second economy buff</param>
        /// <returns>The sum of the two buffs</returns>
        public static EconomyBuff operator +(EconomyBuff a, EconomyBuff b)
        {
            return new EconomyBuff()
            {
                _popCapRelative = a._popCapRelative + b._popCapRelative,
                _stockPileRelative = a._stockPileRelative + b._stockPileRelative,
                _resourceRateRelative = a._resourceRateRelative + b._resourceRateRelative,

                _popCapMultiplier = a._popCapMultiplier + b._popCapMultiplier,
                _resourceRateMultiplier = a._resourceRateMultiplier + b._resourceRateMultiplier,
                _stockPileMultiplier = a._stockPileMultiplier + b._stockPileMultiplier
            };
        }

        /// <summary>
        /// Handles subtracting two economy bufs together
        /// </summary>
        /// <param name="a">The first economy buff</param>
        /// <param name="b">The second economy buff</param>
        /// <returns>The difference of the two buffs</returns>
        public static EconomyBuff operator -(EconomyBuff a, EconomyBuff b)
        {
            return new EconomyBuff()
            {
                _popCapRelative = a._popCapRelative - b._popCapRelative,
                _stockPileRelative = a._stockPileRelative - b._stockPileRelative,
                _resourceRateRelative = a._resourceRateRelative - b._resourceRateRelative,

                _popCapMultiplier = a._popCapMultiplier - b._popCapMultiplier,
                _resourceRateMultiplier = a._resourceRateMultiplier - b._resourceRateMultiplier,
                _stockPileMultiplier = a._stockPileMultiplier - b._stockPileMultiplier
            };
        }

        /// <summary>
        /// Handles multiplying an economy buff by a value
        /// </summary>
        /// <param name="a">The first economy buff</param>
        /// <param name="b">The value to multiply by</param>
        /// <returns>The product of the buff</returns>
        public static EconomyBuff operator *(EconomyBuff a, float b)
        {
            return new EconomyBuff()
            {
                _popCapRelative = (int)(a._popCapRelative * b),
                _stockPileRelative = a._stockPileRelative * b,
                _resourceRateRelative = a._resourceRateRelative * b,

                _popCapMultiplier = a._popCapMultiplier * b,
                _resourceRateMultiplier = a._resourceRateMultiplier * b,
                _stockPileMultiplier = a._stockPileMultiplier * b
            };
        }

        /// <summary>
        /// Handles dividing an economy buff by a value
        /// </summary>
        /// <param name="a">The first economy buff</param>
        /// <param name="b">The value to multiply by</param>
        /// <returns>The quotient of the buff</returns>
        public static EconomyBuff operator /(EconomyBuff a, float b)
        {
            return new EconomyBuff()
            {
                _popCapRelative = (int)(a._popCapRelative / b),
                _stockPileRelative = a._stockPileRelative / b,
                _resourceRateRelative = a._resourceRateRelative / b,

                _popCapMultiplier = a._popCapMultiplier / b,
                _resourceRateMultiplier = a._resourceRateMultiplier / b,
                _stockPileMultiplier = a._stockPileMultiplier / b
            };
        }
    }
}
