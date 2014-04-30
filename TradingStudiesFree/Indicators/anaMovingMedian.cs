using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.Design;
using NinjaTrader.Indicator;

namespace NinjaTrader.Indicator
{
	/// <summary>
	/// The anaMovingMedian (Simple Moving Average) is an indicator that shows the average value of a security's price over a period of time.
	/// </summary>
	[Description("The anaMovingMedian (Simple Moving Average) is an indicator that shows the average value of a security's price over a period of time.")]
// ReSharper disable InconsistentNaming
	public class anaMovingMedian : Indicator
// ReSharper restore InconsistentNaming
	{
		private readonly	ArrayList	mArray			= new ArrayList();
		private				bool		even			= true;
		private				int			medianIndex		= 7;
		private				int			period			= 14;
		private				int			priorIndex		= 6;

		protected override void Initialize()
		{
			Add(new Plot(Color.DeepSkyBlue, "anaMovingMedian"));
			Overlay = true;
		}

		protected override void OnStartUp()
		{
			for (int i = 0; i < Period; i++)
				mArray.Add(0.0);
			if (Period%2 == 0)
			{
				even			= true;
				medianIndex		= Period/2;
				priorIndex		= medianIndex - 1;
			}
			else
			{
				even			= false;
				medianIndex		= (Period - 1)/2;
			}
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar < Period)
			{
				int sPeriod = CurrentBar + 1;
				for (int i = 0; i < sPeriod; i++)
					mArray[i] = Input[i];
				mArray.Sort();
				Value.Set(sPeriod % 2 == 0 ? 0.5 * ((double)mArray[Period - 1 - sPeriod / 2] + (double)mArray[Period - sPeriod / 2]) : (double)mArray[Period - (1 + sPeriod) / 2]);
			}
			else
			{
				for (int i = 0; i < Period; i++)
					mArray[i] = Input[i];
				mArray.Sort();
				Value.Set(even ? 0.5 * ((double)mArray[medianIndex] + (double)mArray[priorIndex]) : (double)mArray[medianIndex]);
			}
		}

		#region Properties

		[Description("Numbers of bars used for calculations")]
		[GridCategory("Parameters")]
		public int Period
		{
			get { return period; }
			set { period = Math.Max(1, value); }
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
		private static readonly anaMovingMedian checkanaMovingMedian = new anaMovingMedian();
		private anaMovingMedian[] cacheanaMovingMedian;

		/// <summary>
		/// The anaMovingMedian (Simple Moving Average) is an indicator that shows the average value of a security's price over a period of time.
		/// </summary>
		/// <returns></returns>
		public anaMovingMedian anaMovingMedian(int period)
		{
			return anaMovingMedian(Input, period);
		}

		/// <summary>
		/// The anaMovingMedian (Simple Moving Average) is an indicator that shows the average value of a security's price over a period of time.
		/// </summary>
		/// <returns></returns>
		public anaMovingMedian anaMovingMedian(IDataSeries input, int period)
		{
			if (cacheanaMovingMedian != null)
				for (int idx = 0; idx < cacheanaMovingMedian.Length; idx++)
					if (cacheanaMovingMedian[idx].Period == period && cacheanaMovingMedian[idx].EqualsInput(input))
						return cacheanaMovingMedian[idx];

			lock (checkanaMovingMedian)
			{
				checkanaMovingMedian.Period = period;
				period = checkanaMovingMedian.Period;

				if (cacheanaMovingMedian != null)
					for (int idx = 0; idx < cacheanaMovingMedian.Length; idx++)
						if (cacheanaMovingMedian[idx].Period == period && cacheanaMovingMedian[idx].EqualsInput(input))
							return cacheanaMovingMedian[idx];

				var indicator = new anaMovingMedian();
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

				var tmp = new anaMovingMedian[cacheanaMovingMedian == null ? 1 : cacheanaMovingMedian.Length + 1];
				if (cacheanaMovingMedian != null)
					cacheanaMovingMedian.CopyTo(tmp, 0);
				tmp[tmp.Length - 1] = indicator;
				cacheanaMovingMedian = tmp;
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
		/// The anaMovingMedian (Simple Moving Average) is an indicator that shows the average value of a security's price over a period of time.
		/// </summary>
		/// <returns></returns>
		[WizardCondition("Indicator")]
		public anaMovingMedian anaMovingMedian(int period)
		{
			return _indicator.anaMovingMedian(Input, period);
		}

		/// <summary>
		/// The anaMovingMedian (Simple Moving Average) is an indicator that shows the average value of a security's price over a period of time.
		/// </summary>
		/// <returns></returns>
		public anaMovingMedian anaMovingMedian(IDataSeries input, int period)
		{
			return _indicator.anaMovingMedian(input, period);
		}
	}
}

// This namespace holds all strategies and is required. Do not change it.

namespace NinjaTrader.Strategy
{
	public partial class Strategy : StrategyBase
	{
		/// <summary>
		/// The anaMovingMedian (Simple Moving Average) is an indicator that shows the average value of a security's price over a period of time.
		/// </summary>
		/// <returns></returns>
		[WizardCondition("Indicator")]
		public anaMovingMedian anaMovingMedian(int period)
		{
			return _indicator.anaMovingMedian(Input, period);
		}

		/// <summary>
		/// The anaMovingMedian (Simple Moving Average) is an indicator that shows the average value of a security's price over a period of time.
		/// </summary>
		/// <returns></returns>
		public anaMovingMedian anaMovingMedian(IDataSeries input, int period)
		{
			if (InInitialize && input == null)
				throw new ArgumentException(
					"You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return _indicator.anaMovingMedian(input, period);
		}
	}
}

#endregion