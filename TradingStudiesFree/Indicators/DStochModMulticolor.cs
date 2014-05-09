using System;
using System.ComponentModel;
using System.Drawing;
using System.Xml.Serialization;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;

namespace NinjaTrader.Indicator
{
	[Description("")]
	public class DStochModMulticolor : Indicator
	{
		private int period = 55;
		private int emaperiod = 15;
		private DataSeries p1 = null;
		private DataSeries p2 = null;
		private DataSeries p3 = null;
		private bool showpaintbars = false;

		protected override void Initialize()
		{
			Add(new Plot(new Pen(Color.Blue, 3), PlotStyle.Line, "Rising"));
			Add(new Plot(new Pen(Color.Yellow, 3), PlotStyle.Line, "Falling"));
			Add(new Plot(new Pen(Color.White, 3), PlotStyle.Line, "Neutral"));
			Add(new Line(Color.White, 90, "Upper"));
			Add(new Line(Color.White, 50, "Mid"));
			Add(new Line(Color.White, 10, "Lower"));
			Overlay = false;
			PriceTypeSupported = false;
			p1 = new DataSeries(this, MaximumBarsLookBack.Infinite);
			p2 = new DataSeries(this, MaximumBarsLookBack.Infinite);
			p3 = new DataSeries(this, MaximumBarsLookBack.Infinite);
		}

		protected override void OnBarUpdate()
		{
			double r = MAX(High, Period)[0] - MIN(Low, Period)[0];
			p1.Set(((Close[0] - MIN(Low, Period)[0]) / (Math.Abs(r) < double.Epsilon ? 1 : r)) * 100);
			p2.Set(EMA(p1, EmaPeriod)[0]);

			double s = MAX(p2, Period)[0] - MIN(p2, Period)[0];
			p3.Set(((p2[0] - MIN(p2, Period)[0]) / (Math.Abs(s) < double.Epsilon ? 1 : s)) * 100);
			RisingPlot.Set(EMA(p3, EmaPeriod)[0]);

			// Checks to make sure we have at least 1 bar before continuing
			if (CurrentBar < 1)
				return;

			// Rising() returns true when the current value is greater than the value of the previous bar.
			if (Rising(RisingPlot))
			{
				PlotColors[0][0] = Plots[0].Pen.Color;
				if (showpaintbars)
					BarColor = Color.FromKnownColor(KnownColor.Blue);
			}

			// Falling() returns true when the current value is less than the value of the previous bar.
			else
				if (Falling(RisingPlot))
				{
					PlotColors[0][0] = Plots[1].Pen.Color;
					if (showpaintbars)
						BarColor = Color.FromKnownColor(KnownColor.Yellow);
				}

				else
				{
					PlotColors[0][0] = Plots[2].Pen.Color;
					if (showpaintbars)
						BarColor = Color.FromKnownColor(KnownColor.White);
				}
		}



		#region Properties
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries RisingPlot
		{
			get { return Values[0]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries FallingPlot
		{
			get { return Values[1]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries NeutralPlot
		{
			get { return Values[2]; }
		}

		[Description("Period")]
		[Category("Parameters")]
		public int Period
		{
			get { return period; }
			set { period = Math.Max(1, value); }
		}
		[Description("EMAPeriod")]
		[Category("Parameters")]
		public int EmaPeriod
		{
			get { return emaperiod; }
			set { emaperiod = Math.Max(1, value); }
		}
		[Description("Show Paintbar on Candles?")]
		[Category("Parameters")]
		public bool ShowPaintbars
		{
			get { return showpaintbars; }
			set { showpaintbars = value; }
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
		private DStochModMulticolor[] cacheDStochModMulticolor = null;

		private static DStochModMulticolor checkDStochModMulticolor = new DStochModMulticolor();

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public DStochModMulticolor DStochModMulticolor(int eMAPeriod, int period, bool showPaintbars)
		{
			return DStochModMulticolor(Input, eMAPeriod, period, showPaintbars);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public DStochModMulticolor DStochModMulticolor(Data.IDataSeries input, int eMAPeriod, int period, bool showPaintbars)
		{
			if (cacheDStochModMulticolor != null)
				for (int idx = 0; idx < cacheDStochModMulticolor.Length; idx++)
					if (cacheDStochModMulticolor[idx].EmaPeriod == eMAPeriod && cacheDStochModMulticolor[idx].Period == period && cacheDStochModMulticolor[idx].ShowPaintbars == showPaintbars && cacheDStochModMulticolor[idx].EqualsInput(input))
						return cacheDStochModMulticolor[idx];

			lock (checkDStochModMulticolor)
			{
				checkDStochModMulticolor.EmaPeriod = eMAPeriod;
				eMAPeriod = checkDStochModMulticolor.EmaPeriod;
				checkDStochModMulticolor.Period = period;
				period = checkDStochModMulticolor.Period;
				checkDStochModMulticolor.ShowPaintbars = showPaintbars;
				showPaintbars = checkDStochModMulticolor.ShowPaintbars;

				if (cacheDStochModMulticolor != null)
					for (int idx = 0; idx < cacheDStochModMulticolor.Length; idx++)
						if (cacheDStochModMulticolor[idx].EmaPeriod == eMAPeriod && cacheDStochModMulticolor[idx].Period == period && cacheDStochModMulticolor[idx].ShowPaintbars == showPaintbars && cacheDStochModMulticolor[idx].EqualsInput(input))
							return cacheDStochModMulticolor[idx];

				DStochModMulticolor indicator = new DStochModMulticolor();
				indicator.BarsRequired = BarsRequired;
				indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
				indicator.Input = input;
				indicator.EmaPeriod = eMAPeriod;
				indicator.Period = period;
				indicator.ShowPaintbars = showPaintbars;
				Indicators.Add(indicator);
				indicator.SetUp();

				DStochModMulticolor[] tmp = new DStochModMulticolor[cacheDStochModMulticolor == null ? 1 : cacheDStochModMulticolor.Length + 1];
				if (cacheDStochModMulticolor != null)
					cacheDStochModMulticolor.CopyTo(tmp, 0);
				tmp[tmp.Length - 1] = indicator;
				cacheDStochModMulticolor = tmp;
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
		public Indicator.DStochModMulticolor DStochModMulticolor(int eMAPeriod, int period, bool showPaintbars)
		{
			return _indicator.DStochModMulticolor(Input, eMAPeriod, period, showPaintbars);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public Indicator.DStochModMulticolor DStochModMulticolor(Data.IDataSeries input, int eMAPeriod, int period, bool showPaintbars)
		{
			return _indicator.DStochModMulticolor(input, eMAPeriod, period, showPaintbars);
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
		public Indicator.DStochModMulticolor DStochModMulticolor(int eMAPeriod, int period, bool showPaintbars)
		{
			return _indicator.DStochModMulticolor(Input, eMAPeriod, period, showPaintbars);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public Indicator.DStochModMulticolor DStochModMulticolor(Data.IDataSeries input, int eMAPeriod, int period, bool showPaintbars)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return _indicator.DStochModMulticolor(input, eMAPeriod, period, showPaintbars);
		}
	}
}
#endregion
