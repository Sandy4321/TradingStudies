using System;
using System.ComponentModel;
using System.Drawing;
using System.Xml.Serialization;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;

namespace NinjaTrader.Indicator
{
	/// <summary>
	///     Plots the dynamic support/resistance
	/// </summary>
	[Description("Plots the dynamic support/resistance")]
	public class DynamicSr : Indicator
	{
		private double		dynamicR;
		private double		dynamicS;
		private double		hh;
		private DataSeries	hmaSeries;
		private double		ll;
		private int			period = 21;

		protected override void Initialize()
		{
			Name			= "Dynamic S/R";
			hmaSeries		= new DataSeries(this);
			Overlay			= true;
			AutoScale		= false;
			BarsRequired	= 1;
			Add(new Plot(new Pen(Color.Blue, 3), PlotStyle.Line, "Resistance"));
			Add(new Plot(new Pen(Color.Magenta, 3), PlotStyle.Line, "Support"));
		}

		protected override void OnBarUpdate()
		{
			hmaSeries.Set(HMA(period)[0]);

			if (CurrentBar < 2) return;
			{
				hh = High[Bars.HighestBar(10)];
				if (hmaSeries[1] > hmaSeries[2] && hmaSeries[0] < hmaSeries[1])
					dynamicR = hh;
				ll = Low[Bars.LowestBar(10)];
				if (hmaSeries[1] < hmaSeries[2] && hmaSeries[0] > hmaSeries[1])
					dynamicS = ll;
			}

			Support.Set(dynamicS);
			Resistance.Set(dynamicR);
		}

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries Resistance
		{
			get { return Values[0]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries Support
		{
			get { return Values[1]; }
		}

		[Description("Numbers of bars used for calculations")]
		[GridCategory("Parameters")]
		public int Period
		{
			get { return period; }
			set { period = Math.Max(1, value); }
		}
	}
}

#region NinjaScript generated code. Neither change nor remove.

// This namespace holds all indicators and is required. Do not change it.

namespace NinjaTrader.Indicator
{
	public partial class Indicator : IndicatorBase
	{
		private static DynamicSr checkDynamicSR = new DynamicSr();
		private DynamicSr[] cacheDynamicSR = null;

		/// <summary>
		///     Plots the dynamic support/resistance
		/// </summary>
		/// <returns></returns>
		public DynamicSr DynamicSR(int period)
		{
			return DynamicSR(Input, period);
		}

		/// <summary>
		///     Plots the dynamic support/resistance
		/// </summary>
		/// <returns></returns>
		public DynamicSr DynamicSR(Data.IDataSeries input, int period)
		{
			if (cacheDynamicSR != null)
				for (int idx = 0; idx < cacheDynamicSR.Length; idx++)
					if (cacheDynamicSR[idx].Period == period && cacheDynamicSR[idx].EqualsInput(input))
						return cacheDynamicSR[idx];

			lock (checkDynamicSR)
			{
				checkDynamicSR.Period = period;
				period = checkDynamicSR.Period;

				if (cacheDynamicSR != null)
					for (int idx = 0; idx < cacheDynamicSR.Length; idx++)
						if (cacheDynamicSR[idx].Period == period && cacheDynamicSR[idx].EqualsInput(input))
							return cacheDynamicSR[idx];

				var indicator = new DynamicSr();
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

				var tmp = new DynamicSr[cacheDynamicSR == null ? 1 : cacheDynamicSR.Length + 1];
				if (cacheDynamicSR != null)
					cacheDynamicSR.CopyTo(tmp, 0);
				tmp[tmp.Length - 1] = indicator;
				cacheDynamicSR = tmp;
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
		///     Plots the dynamic support/resistance
		/// </summary>
		/// <returns></returns>
		[Gui.Design.WizardCondition("Indicator")]
		public Indicator.DynamicSr DynamicSR(int period)
		{
			return _indicator.DynamicSR(Input, period);
		}

		/// <summary>
		///     Plots the dynamic support/resistance
		/// </summary>
		/// <returns></returns>
		public Indicator.DynamicSr DynamicSR(Data.IDataSeries input, int period)
		{
			return _indicator.DynamicSR(input, period);
		}
	}
}

// This namespace holds all strategies and is required. Do not change it.

namespace NinjaTrader.Strategy
{
	public partial class Strategy : StrategyBase
	{
		/// <summary>
		///     Plots the dynamic support/resistance
		/// </summary>
		/// <returns></returns>
		[Gui.Design.WizardCondition("Indicator")]
		public Indicator.DynamicSr DynamicSR(int period)
		{
			return _indicator.DynamicSR(Input, period);
		}

		/// <summary>
		///     Plots the dynamic support/resistance
		/// </summary>
		/// <returns></returns>
		public Indicator.DynamicSr DynamicSR(Data.IDataSeries input, int period)
		{
			if (InInitialize && input == null)
				throw new ArgumentException(
					"You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return _indicator.DynamicSR(input, period);
		}
	}
}

#endregion