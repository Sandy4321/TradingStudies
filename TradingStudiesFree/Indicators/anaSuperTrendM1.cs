#region Using declarations

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.Design;
using NinjaTrader.Indicator;

#endregion

namespace NinjaTrader.Indicator
{
	[Description("Modified SuperTrend Indicator based on a Moving Median")]
// ReSharper disable once InconsistentNaming
	public class anaSuperTrendM1 : Indicator
	{
		private readonly	Color				neutralColor	= Color.Transparent;
		private				ATR					mae;
		private				anaMovingMedian		mm;
		private				bool				candles;
		private				bool				currentUpTrend	= true;
		private				DashStyle			dash0Style		= DashStyle.Dot;
		private				DashStyle			dash1Style		= DashStyle.Solid;
		private				Color				downColor		= Color.Red;
		private				bool				gap;
		private				double				multiplier		= 2;
		private				double				newStop;
		private				double				offset;
		private				int					periodAtr		= 3;
		private				int					periodMedian	= 3;
		private				PlotStyle			plot0Style		= PlotStyle.Dot;
		private				int					plot0Width		= 1;
		private				PlotStyle			plot1Style		= PlotStyle.Line;
		private				int					plot1Width		= 1;
		private				double				priorStop;
		private				bool				priorUpTrend	= true;
		private				bool				showArrows		= true;
		private				bool				showStopDots	= true;
		private				bool				showStopLine	= true;
		private				Color				upColor			= Color.RoyalBlue;
		private				BoolSeries			upTrend;

		protected override void Initialize()
		{
			Add(new Plot(Color.Gray, PlotStyle.Dot, "StopDot"));
			Add(new Plot(Color.Gray, PlotStyle.Line, "StopLine"));
			CalculateOnBarClose		= false;
			Overlay					= true;
			PriceTypeSupported		= false;
			PlotsConfigurable		= false;
			upTrend					= new BoolSeries(this);
		}

		protected override void OnStartUp()
		{
			Plots[0].Pen.Width		= plot0Width;
			Plots[0].PlotStyle		= plot0Style;
			Plots[0].Pen.DashStyle	= dash0Style;
			Plots[1].Pen.Width		= plot1Width;
			Plots[1].PlotStyle		= plot1Style;
			Plots[1].Pen.DashStyle	= dash1Style;
			gap						= (plot1Style == PlotStyle.Line) || (plot1Style == PlotStyle.Square);
			Plots[0].Pen.Color		= ShowStopDots ? Color.Gray : Color.Transparent;
			Plots[1].Pen.Color		= ShowStopLine ? Color.Gray : Color.Transparent;
			mm						= anaMovingMedian(Medians[0], periodMedian);
			mae						= ATR(Closes[0], periodAtr);
			candles					= ChartControl != null && ChartControl.ChartStyleType == ChartStyleType.CandleStick;
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar == 0)
			{
				upTrend.Set(true);
				StopDot.Set(Close[0]);
				StopLine.Set(Close[0]);
				PlotColors[0][0] = neutralColor;
				PlotColors[1][0] = neutralColor;
				return;
			}
			if (FirstTickOfBar)
			{
				priorUpTrend	= upTrend[1];
				priorStop		= StopLine[1];
				offset			= mae[1];
			}
			if (Close[0] > priorStop)
			{
				upTrend.Set(true);
				newStop			= mm[0] - Multiplier*offset;
				currentUpTrend	= true;
				if (!priorUpTrend) // trend change up
				{
					StopDot.Set(newStop);
					StopLine.Set(newStop);
				}
				else
				{
					StopDot.Set(Math.Max(newStop, priorStop));
					StopLine.Set(Math.Max(newStop, priorStop));
				}
			}
			else if (Close[0] < priorStop)
			{
				upTrend.Set(false);
				newStop			= mm[0] + Multiplier*offset;
				currentUpTrend	= false;
				if (priorUpTrend) // trend change down
				{
					StopDot.Set(newStop);
					StopLine.Set(newStop);
				}
				else
				{
					StopDot.Set(Math.Min(newStop, priorStop));
					StopLine.Set(Math.Min(newStop, priorStop));
				}
			}
			else
			{
				upTrend.Set(priorUpTrend);
				StopDot.Set(priorStop);
				StopLine.Set(priorStop);
				currentUpTrend = priorUpTrend;
			}

			if (PaintBars)
			{
				if (currentUpTrend)
				{
					CandleOutlineColor	= upColor;
					BarColor			= upColor;
				}
				else
				{
					CandleOutlineColor	= downColor;
					BarColor			= downColor;
				}
				if (Open[0] < Close[0] && candles)
					BarColor = Color.Transparent;
			}
			if (ShowArrows)
				if (currentUpTrend && !priorUpTrend)
					DrawArrowUp("arrow" + CurrentBar, true, 0, newStop - 0.5*offset, upColor);
				else if (!currentUpTrend && priorUpTrend)
					DrawArrowDown("arrow" + CurrentBar, true, 0, newStop + 0.5*offset, downColor);
				else
					RemoveDrawObject("arrow" + CurrentBar);
			if (ShowStopDots)
				PlotColors[0][0] = currentUpTrend ? upColor : downColor;
			if (ShowStopLine)
				PlotColors[1][0] = currentUpTrend && !priorUpTrend
										? (gap ? neutralColor : upColor)
										: (currentUpTrend
											? upColor
											: (!currentUpTrend && priorUpTrend ? (gap ? neutralColor : downColor) : downColor));
		}

		#region Properties

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries StopDot
		{
			get { return Values[0]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries StopLine
		{
			get { return Values[1]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public BoolSeries UpTrend
		{
			get { return upTrend; }
		}

		[Description("Median Period")]
		[Category("Parameters")]
		[Gui.Design.DisplayName("Median Period")]
		public int PeriodMedian
		{
			get { return periodMedian; }
			set { periodMedian = Math.Max(1, value); }
		}

		[Description("ATR Period")]
		[Category("Parameters")]
		[Gui.Design.DisplayName("ATR Period")]
		public int PeriodAtr
		{
			get { return periodAtr; }
			set { periodAtr = Math.Max(1, value); }
		}

		[Description("ATR Multiplier")]
		[Category("Parameters")]
		[Gui.Design.DisplayName("ATR Multiplier")]
		public double Multiplier
		{
			get { return multiplier; }
			set { multiplier = Math.Max(0.0, value); }
		}

		[Description("Color the bars in the direction of the trend?")]
		[Category("Options")]
		[Gui.Design.DisplayName("Paint Bars")]
		public bool PaintBars { get; set; }

		[Description("Show Arrows when Trendline is violated?")]
		[Category("Options")]
		[Gui.Design.DisplayName("Show Arrows")]
		public bool ShowArrows
		{
			get { return showArrows; }
			set { showArrows = value; }
		}

		[Description("Show Stop Dots")]
		[Category("Options")]
		[Gui.Design.DisplayName("Show Stop Dots")]
		public bool ShowStopDots
		{
			get { return showStopDots; }
			set { showStopDots = value; }
		}

		[Description("Show Stop Line")]
		[Category("Options")]
		[Gui.Design.DisplayName("Show Stop Line")]
		public bool ShowStopLine
		{
			get { return showStopLine; }
			set { showStopLine = value; }
		}

		/// <summary>
		/// </summary>
		[XmlIgnore]
		[Description("Select color for Rising Trend")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Uptrend")]
		public Color UpColor
		{
			get { return upColor; }
			set { upColor = value; }
		}

		// Serialize Color object
		[Browsable(false)]
		public string UpColorSerialize
		{
			get { return SerializableColor.ToString(upColor); }
			set { upColor = SerializableColor.FromString(value); }
		}

		/// <summary>
		/// </summary>
		[XmlIgnore]
		[Description("Select color for downtrend")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Downtrend")]
		public Color DownColor
		{
			get { return downColor; }
			set { downColor = value; }
		}

		// Serialize Color object
		[Browsable(false)]
		public string DownColorSerialize
		{
			get { return SerializableColor.ToString(downColor); }
			set { downColor = SerializableColor.FromString(value); }
		}

		/// <summary>
		/// </summary>
		[Description("Width for Stop Dots.")]
		[Category("Plots")]
		[Gui.Design.DisplayName("Width Stop Dots")]
		public int Plot0Width
		{
			get { return plot0Width; }
			set { plot0Width = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Description("PlotStyle for Stop Dots.")]
		[Category("Plots")]
		[Gui.Design.DisplayName("Plot Style Stop Dots")]
		public PlotStyle Plot0Style
		{
			get { return plot0Style; }
			set { plot0Style = value; }
		}

		/// <summary>
		/// </summary>
		[Description("DashStyle for Stop Dots.")]
		[Category("Plots")]
		[Gui.Design.DisplayName("Dash Style Stop Dots")]
		public DashStyle Dash0Style
		{
			get { return dash0Style; }
			set { dash0Style = value; }
		}

		/// <summary>
		/// </summary>
		[Description("Width for Stop Line.")]
		[Category("Plots")]
		[Gui.Design.DisplayName("Width Stop Line")]
		public int Plot1Width
		{
			get { return plot1Width; }
			set { plot1Width = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Description("PlotStyle for Stop Line.")]
		[Category("Plots")]
		[Gui.Design.DisplayName("Plot Style Stop Line")]
		public PlotStyle Plot1Style
		{
			get { return plot1Style; }
			set { plot1Style = value; }
		}

		/// <summary>
		/// </summary>
		[Description("DashStyle for Stop Line.")]
		[Category("Plots")]
		[Gui.Design.DisplayName("Dash Style Stop Line")]
		public DashStyle Dash1Style
		{
			get { return dash1Style; }
			set { dash1Style = value; }
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
		private static readonly anaSuperTrendM1 checkanaSuperTrendM1 = new anaSuperTrendM1();
		private anaSuperTrendM1[] cacheanaSuperTrendM1;

		/// <summary>
		/// Modified SuperTrend Indicator based on a Moving Median
		/// </summary>
		/// <returns></returns>
		public anaSuperTrendM1 anaSuperTrendM1(double multiplier, int periodATR, int periodMedian)
		{
			return anaSuperTrendM1(Input, multiplier, periodATR, periodMedian);
		}

		/// <summary>
		/// Modified SuperTrend Indicator based on a Moving Median
		/// </summary>
		/// <returns></returns>
		public anaSuperTrendM1 anaSuperTrendM1(IDataSeries input, double multiplier, int periodATR, int periodMedian)
		{
			if (cacheanaSuperTrendM1 != null)
				for (int idx = 0; idx < cacheanaSuperTrendM1.Length; idx++)
					if (Math.Abs(cacheanaSuperTrendM1[idx].Multiplier - multiplier) <= double.Epsilon &&
					    cacheanaSuperTrendM1[idx].PeriodAtr == periodATR && cacheanaSuperTrendM1[idx].PeriodMedian == periodMedian &&
					    cacheanaSuperTrendM1[idx].EqualsInput(input))
						return cacheanaSuperTrendM1[idx];

			lock (checkanaSuperTrendM1)
			{
				checkanaSuperTrendM1.Multiplier = multiplier;
				multiplier = checkanaSuperTrendM1.Multiplier;
				checkanaSuperTrendM1.PeriodAtr = periodATR;
				periodATR = checkanaSuperTrendM1.PeriodAtr;
				checkanaSuperTrendM1.PeriodMedian = periodMedian;
				periodMedian = checkanaSuperTrendM1.PeriodMedian;

				if (cacheanaSuperTrendM1 != null)
					for (int idx = 0; idx < cacheanaSuperTrendM1.Length; idx++)
						if (Math.Abs(cacheanaSuperTrendM1[idx].Multiplier - multiplier) <= double.Epsilon &&
						    cacheanaSuperTrendM1[idx].PeriodAtr == periodATR && cacheanaSuperTrendM1[idx].PeriodMedian == periodMedian &&
						    cacheanaSuperTrendM1[idx].EqualsInput(input))
							return cacheanaSuperTrendM1[idx];

				var indicator = new anaSuperTrendM1();
				indicator.BarsRequired = BarsRequired;
				indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
				indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
				indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
				indicator.Input = input;
				indicator.Multiplier = multiplier;
				indicator.PeriodAtr = periodATR;
				indicator.PeriodMedian = periodMedian;
				Indicators.Add(indicator);
				indicator.SetUp();

				var tmp = new anaSuperTrendM1[cacheanaSuperTrendM1 == null ? 1 : cacheanaSuperTrendM1.Length + 1];
				if (cacheanaSuperTrendM1 != null)
					cacheanaSuperTrendM1.CopyTo(tmp, 0);
				tmp[tmp.Length - 1] = indicator;
				cacheanaSuperTrendM1 = tmp;
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
		/// Modified SuperTrend Indicator based on a Moving Median
		/// </summary>
		/// <returns></returns>
		[WizardCondition("Indicator")]
		public anaSuperTrendM1 anaSuperTrendM1(double multiplier, int periodATR, int periodMedian)
		{
			return _indicator.anaSuperTrendM1(Input, multiplier, periodATR, periodMedian);
		}

		/// <summary>
		/// Modified SuperTrend Indicator based on a Moving Median
		/// </summary>
		/// <returns></returns>
		public anaSuperTrendM1 anaSuperTrendM1(IDataSeries input, double multiplier, int periodATR, int periodMedian)
		{
			return _indicator.anaSuperTrendM1(input, multiplier, periodATR, periodMedian);
		}
	}
}

// This namespace holds all strategies and is required. Do not change it.

namespace NinjaTrader.Strategy
{
	public partial class Strategy : StrategyBase
	{
		/// <summary>
		/// Modified SuperTrend Indicator based on a Moving Median
		/// </summary>
		/// <returns></returns>
		[WizardCondition("Indicator")]
		public anaSuperTrendM1 anaSuperTrendM1(double multiplier, int periodATR, int periodMedian)
		{
			return _indicator.anaSuperTrendM1(Input, multiplier, periodATR, periodMedian);
		}

		/// <summary>
		/// Modified SuperTrend Indicator based on a Moving Median
		/// </summary>
		/// <returns></returns>
		public anaSuperTrendM1 anaSuperTrendM1(IDataSeries input, double multiplier, int periodATR, int periodMedian)
		{
			if (InInitialize && input == null)
				throw new ArgumentException(
					"You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return _indicator.anaSuperTrendM1(input, multiplier, periodATR, periodMedian);
		}
	}
}

#endregion