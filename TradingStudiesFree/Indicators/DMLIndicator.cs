using System;
using System.ComponentModel;
using System.Drawing;
using System.Xml.Serialization;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;

namespace NinjaTrader.Indicator
{
	[Description("")]
	public class DmlIndicator : Indicator
	{
		private int myInput0 = 1;

		protected override void Initialize()
		{
			Add(new Plot(new Pen(Color.Brown, 3), PlotStyle.Dot, "Condition4"));
			Add(new Plot(new Pen(Color.DarkGreen, 3), PlotStyle.Dot, "Condition5"));
			Add(new Plot(new Pen(Color.DarkRed, 3), PlotStyle.Dot, "Condition2"));
			Add(new Plot(new Pen(Color.Cyan, 3), PlotStyle.Dot, "Condition67"));
			Overlay = true;
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar < 3) return;
			bool condition1 = (ZLEMA(7)[0] < HeikenAshi().HAOpen[0] || ZLEMA(7)[0] < Median[0])
				&& ZLEMA(7)[0] < ZLEMA(21)[1] && HeikenAshi().HAClose[0] < ZLEMA(21)[2]
				&& (MACD(12, 26, 9).Diff[0] < 0 || (HeikenAshi().HAClose[0] < ZLEMA(21)[2] && HeikenAshi().HAOpen[0] < ZLEMA(21)[2]) || Close[0] < SMA(21)[0] || ZLEMA(7)[0] < ZLEMA(21)[2]);

			bool condition2 = (ZLEMA(7)[0] > HeikenAshi().HAOpen[0] || ZLEMA(7)[0] > Median[0])
				&& ZLEMA(7)[0] > ZLEMA(21)[1] && HeikenAshi().HAClose[0] > ZLEMA(21)[2]
				&& (MACD(12, 26, 9).Diff[0] > 0 || (HeikenAshi().HAClose[0] > ZLEMA(21)[2] && HeikenAshi().HAOpen[0] > ZLEMA(21)[2]) || Close[0] > SMA(21)[0] || ZLEMA(7)[0] > ZLEMA(21)[2]);

			bool condition3 = (Low[0] + ((High[0] - Low[0]) * 0.1)) < SMA(21)[0]
				&& (High[0] - ((High[0] - Low[0]) * 0.1)) < SMA(21)[0]
				&& ZLEMA(7)[0] < ZLEMA(7)[1]
				&& ZLEMA(11)[0] < HeikenAshi().HAOpen[0];

			bool condition4 = condition1 && condition3;

			bool condition5 = condition1 || condition3;

			bool condition6 = ZLEMA(7)[0] < ZLEMA(7)[1] && ZLEMA(11)[0] < ZLEMA(11)[1] && ZLEMA(11)[0] < SMA(21)[0] && HeikenAshi().HAClose[0] < HeikenAshi().HAOpen[0] && Close[0] < Median[0];

			bool condition7 = HeikenAshi().HAClose[0] < SMA(21)[0] && ZLEMA(7)[0] < ZLEMA(7)[1] && High[0] < SMA(78)[0] && SMA(21)[0] < SMA(78)[0] && SMA(78)[0] < SMA(78)[3];

			if (condition4) Condition4.Set(High[0] + TickSize);
			if (condition5) Condition5.Set(High[0] + 2 * TickSize);
			if (condition2) Condition2.Set(Low[0] - 3 * TickSize);
			if (condition6 || condition7) Condition67.Set(High[0] + 3 * TickSize);
		}

		#region Properties
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
		[XmlIgnore]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
		public DataSeries Condition4
		{
			get { return Values[0]; }
		}

		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
		[XmlIgnore]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
		public DataSeries Condition5
		{
			get { return Values[1]; }
		}

		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
		[XmlIgnore]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
		public DataSeries Condition2
		{
			get { return Values[2]; }
		}

		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
		[XmlIgnore]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
		public DataSeries Condition67
		{
			get { return Values[3]; }
		}

		[Description("")]
		[GridCategory("Parameters")]
		public int MyInput0
		{
			get { return myInput0; }
			set { myInput0 = Math.Max(1, value); }
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
		private DmlIndicator[] cacheDMLIndicator = null;

		private static DmlIndicator checkDMLIndicator = new DmlIndicator();

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public DmlIndicator DMLIndicator(int myInput0)
		{
			return DMLIndicator(Input, myInput0);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public DmlIndicator DMLIndicator(Data.IDataSeries input, int myInput0)
		{
			if (cacheDMLIndicator != null)
				for (int idx = 0; idx < cacheDMLIndicator.Length; idx++)
					if (cacheDMLIndicator[idx].MyInput0 == myInput0 && cacheDMLIndicator[idx].EqualsInput(input))
						return cacheDMLIndicator[idx];

			lock (checkDMLIndicator)
			{
				checkDMLIndicator.MyInput0 = myInput0;
				myInput0 = checkDMLIndicator.MyInput0;

				if (cacheDMLIndicator != null)
					for (int idx = 0; idx < cacheDMLIndicator.Length; idx++)
						if (cacheDMLIndicator[idx].MyInput0 == myInput0 && cacheDMLIndicator[idx].EqualsInput(input))
							return cacheDMLIndicator[idx];

				DmlIndicator indicator = new DmlIndicator();
				indicator.BarsRequired = BarsRequired;
				indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
				indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
				indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
				indicator.Input = input;
				indicator.MyInput0 = myInput0;
				Indicators.Add(indicator);
				indicator.SetUp();

				DmlIndicator[] tmp = new DmlIndicator[cacheDMLIndicator == null ? 1 : cacheDMLIndicator.Length + 1];
				if (cacheDMLIndicator != null)
					cacheDMLIndicator.CopyTo(tmp, 0);
				tmp[tmp.Length - 1] = indicator;
				cacheDMLIndicator = tmp;
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
		/// 
		/// </summary>
		/// <returns></returns>
		[Gui.Design.WizardCondition("Indicator")]
		public Indicator.DmlIndicator DMLIndicator(int myInput0)
		{
			return _indicator.DMLIndicator(Input, myInput0);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public Indicator.DmlIndicator DMLIndicator(Data.IDataSeries input, int myInput0)
		{
			return _indicator.DMLIndicator(input, myInput0);
		}
	}
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
	public partial class Strategy : StrategyBase
	{
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		[Gui.Design.WizardCondition("Indicator")]
		public Indicator.DmlIndicator DMLIndicator(int myInput0)
		{
			return _indicator.DMLIndicator(Input, myInput0);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public Indicator.DmlIndicator DMLIndicator(Data.IDataSeries input, int myInput0)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return _indicator.DMLIndicator(input, myInput0);
		}
	}
}
#endregion
