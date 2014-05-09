using System;
using System.ComponentModel;
using System.Drawing;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;

namespace NinjaTrader.Indicator
{
	[Description("Ehlers Filter")]
	public class EhlersFilter : Indicator
	{
		private DataSeries	coef;
		private int			count;
		private DataSeries	distance2;
		private int			length		= 20;
		private int			lookback;
		private double		num;
		private DataSeries	smooth;
		private double		sumCoef;

		[Description("Default setting for Length")]
		[GridCategory("Parameters")]
		public int Length
		{
			get { return length; }
			set { length = Math.Max(1, value); }
		}

		protected override void Initialize()
		{
			Add(new Plot(new Pen(Color.Green, 2), PlotStyle.Line, "EF"));

			Overlay					= true;
			smooth					= new DataSeries(this);
			coef					= new DataSeries(this);
			distance2				= new DataSeries(this);
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar < length)
				return;

			smooth.Set((Input[0] + 2 * Input[1] + 2 * Input[2] + Input[3]) / 6.0);
			for (count = 0; count < length; count++)
			{
				distance2.Set(0.00);
				for (lookback = 1; lookback < length; lookback++)
				{
					distance2.Set(distance2[count] + (smooth[count] - smooth[count + lookback]) * (smooth[count] - smooth[count + lookback]));
					coef.Set(count, distance2[count]);
				}
				num			= 0.00;
				sumCoef		= 0.00;
				for (count = 0; count <= length; count++)
				{
					num			= num + coef[count] * smooth[count];
					sumCoef		= sumCoef + coef[count];
				}

				Value.Set(num / (Math.Abs(sumCoef) < 0.000000001 ? 1 : sumCoef));
			}
		}
	}
}
#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private EhlersFilter[] cacheEhlersFilter = null;

        private static EhlersFilter checkEhlersFilter = new EhlersFilter();

        /// <summary>
        /// Ehlers Filter
        /// </summary>
        /// <returns></returns>
        public EhlersFilter EhlersFilter(int length)
        {
            return EhlersFilter(Input, length);
        }

        /// <summary>
        /// Ehlers Filter
        /// </summary>
        /// <returns></returns>
        public EhlersFilter EhlersFilter(Data.IDataSeries input, int length)
        {
            if (cacheEhlersFilter != null)
                for (int idx = 0; idx < cacheEhlersFilter.Length; idx++)
                    if (cacheEhlersFilter[idx].Length == length && cacheEhlersFilter[idx].EqualsInput(input))
                        return cacheEhlersFilter[idx];

            lock (checkEhlersFilter)
            {
                checkEhlersFilter.Length = length;
                length = checkEhlersFilter.Length;

                if (cacheEhlersFilter != null)
                    for (int idx = 0; idx < cacheEhlersFilter.Length; idx++)
                        if (cacheEhlersFilter[idx].Length == length && cacheEhlersFilter[idx].EqualsInput(input))
                            return cacheEhlersFilter[idx];

                EhlersFilter indicator = new EhlersFilter();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Length = length;
                Indicators.Add(indicator);
                indicator.SetUp();

                EhlersFilter[] tmp = new EhlersFilter[cacheEhlersFilter == null ? 1 : cacheEhlersFilter.Length + 1];
                if (cacheEhlersFilter != null)
                    cacheEhlersFilter.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheEhlersFilter = tmp;
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
        /// Ehlers Filter
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.EhlersFilter EhlersFilter(int length)
        {
            return _indicator.EhlersFilter(Input, length);
        }

        /// <summary>
        /// Ehlers Filter
        /// </summary>
        /// <returns></returns>
        public Indicator.EhlersFilter EhlersFilter(Data.IDataSeries input, int length)
        {
            return _indicator.EhlersFilter(input, length);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Ehlers Filter
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.EhlersFilter EhlersFilter(int length)
        {
            return _indicator.EhlersFilter(Input, length);
        }

        /// <summary>
        /// Ehlers Filter
        /// </summary>
        /// <returns></returns>
        public Indicator.EhlersFilter EhlersFilter(Data.IDataSeries input, int length)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.EhlersFilter(input, length);
        }
    }
}
#endregion
