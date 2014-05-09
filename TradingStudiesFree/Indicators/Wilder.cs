using System;
using System.ComponentModel;
using System.Drawing;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;

namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Wilder's average
    /// </summary>
    [Description("Wilder's average")]
    public class Wilder : Indicator
    {
        private int _period = 13; // Default setting for Period

        protected override void Initialize()
        {
            Add(new Plot(new Pen(Color.Purple, 2), PlotStyle.Line, "WAvg"));
            Overlay = true;
        }

        protected override void OnBarUpdate()
        {
            Value.Set((SMA(Input, _period)[1] * (_period - 1) + Input[0]) / _period);
        }

        #region Properties
        [Description("Period")]
        [GridCategory("Parameters")]
        public int Period
        {
            get { return _period; }
            set { _period = Math.Max(1, value); }
        }
        #endregion
    }
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private Wilder[] cacheWilder = null;

        private static Wilder checkWilder = new Wilder();

        /// <summary>
        /// Wilder's average
        /// </summary>
        /// <returns></returns>
        public Wilder Wilder(int period)
        {
            return Wilder(Input, period);
        }

        /// <summary>
        /// Wilder's average
        /// </summary>
        /// <returns></returns>
        public Wilder Wilder(Data.IDataSeries input, int period)
        {
            if (cacheWilder != null)
                for (int idx = 0; idx < cacheWilder.Length; idx++)
                    if (cacheWilder[idx].Period == period && cacheWilder[idx].EqualsInput(input))
                        return cacheWilder[idx];

            lock (checkWilder)
            {
                checkWilder.Period = period;
                period = checkWilder.Period;

                if (cacheWilder != null)
                    for (int idx = 0; idx < cacheWilder.Length; idx++)
                        if (cacheWilder[idx].Period == period && cacheWilder[idx].EqualsInput(input))
                            return cacheWilder[idx];

                Wilder indicator = new Wilder();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Period = period;
                Indicators.Add(indicator);
                indicator.SetUp();

                Wilder[] tmp = new Wilder[cacheWilder == null ? 1 : cacheWilder.Length + 1];
                if (cacheWilder != null)
                    cacheWilder.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheWilder = tmp;
                return indicator;
            }
        }
    }
}

// This namespace holds all market analyzer column definitions and is required. Do not change it.
namespace NinjaTrader.MarketAnalyzer
{
    public partial class Column : ColumnBase
    {
        /// <summary>
        /// Wilder's average
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Wilder Wilder(int period)
        {
            return _indicator.Wilder(Input, period);
        }

        /// <summary>
        /// Wilder's average
        /// </summary>
        /// <returns></returns>
        public Indicator.Wilder Wilder(Data.IDataSeries input, int period)
        {
            return _indicator.Wilder(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Wilder's average
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Wilder Wilder(int period)
        {
            return _indicator.Wilder(Input, period);
        }

        /// <summary>
        /// Wilder's average
        /// </summary>
        /// <returns></returns>
        public Indicator.Wilder Wilder(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Wilder(input, period);
        }
    }
}
#endregion
