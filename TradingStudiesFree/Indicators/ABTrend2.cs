using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.Design;

namespace NinjaTrader.Indicator
{
	[Description("AB trend 2 indicator")]
	public class AbTrend2 : Indicator
	{
		private		const		DashStyle			dash0Style				= DashStyle.Dot;
		private		const		PlotStyle			plot0Style				= PlotStyle.Line;
		private		const		int					plot0Width				= 1;
		private		readonly	Color				neutralColor			= Color.Transparent;
		private					Color				backgroundcolorDn		= Color.LightPink;
		private					Color				backgroundcolorUp		= Color.LightCyan;
		private					bool				colorAllBackgrounds		= true;
		private					bool				colorbackground			= true;
		private					bool				currentUpTrend			= true;
		private					DashStyle			dash1Style				= DashStyle.Dot;
		private					Color				downColor				= Color.Red;
		private					bool				gap;
		private					ATR					mae;
		private					anaMovingMedian		mm;
		private					double				multiplier				= 2.0;
		private					double				newStop;
		private					double				offset;
		private					int					opacity					= 80;
		private					bool				paintBars				= true;
		private					int					periodAtr				= 10;
		private					int					periodMedian			= 3;
		private					PlotStyle			plot1Style				= PlotStyle.Line;
		private					int					plot1Width				= 2;
		private					double				priorStop;
		private					bool				priorUpTrend			= true;
		private					bool				showArrows				= true;
		private					bool				showStopLine			= true;
		private					Color				upColor					= Color.Navy;
		private					BoolSeries			upTrend;

		[GridCategory("Options")]
		[Description("Background Color Dn")]
		[Gui.Design.DisplayName("5.3 Background Color Dn")]
		public Color BackgroundcolorDn
		{
			get { return backgroundcolorDn; }
			set { backgroundcolorDn = value; }
		}

		[Browsable(false)]
		public string BackgroundcolorDnSerialize
		{
			get { return SerializableColor.ToString(backgroundcolorDn); }
			set { backgroundcolorDn = SerializableColor.FromString(value); }
		}

		[GridCategory("Options")]
		[Description("Background Color Up")]
		[Gui.Design.DisplayName("5.2 Background Color Up")]
		public Color BackgroundcolorUp
		{
			get { return backgroundcolorUp; }
			set { backgroundcolorUp = value; }
		}

		[Browsable(false)]
		public string BackgroundcolorUpSerialize
		{
			get { return SerializableColor.ToString(backgroundcolorUp); }
			set { backgroundcolorUp = SerializableColor.FromString(value); }
		}

		[Description("Color ALL Backgrounds?")]
		[GridCategory("Options")]
		[Gui.Design.DisplayName("5.1 Color ALL Backgrounds?")]
		public bool ColorAllBackgrounds
		{
			get { return colorAllBackgrounds; }
			set { colorAllBackgrounds = value; }
		}

		[Gui.Design.DisplayName("4.Color Background?")]
		[Description("Color Background?")]
		[GridCategory("Options")]
		public bool ColorBackground
		{
			get { return colorbackground; }
			set { colorbackground = value; }
		}

		[GridCategory("Plots")]
		[Gui.Design.DisplayName("Dash Style Stop Line")]
		[Description("DashStyle for Stop Line.")]
		public DashStyle Dash1Style
		{
			get { return dash1Style; }
			set { dash1Style = value; }
		}

		[Description("Select color for downtrend")]
		[Gui.Design.DisplayName("Downtrend Color")]
		[XmlIgnore]
		[GridCategory("Plot Colors")]
		public Color DownColor
		{
			get { return downColor; }
			set { downColor = value; }
		}

		[Browsable(false)]
		public string DownColorSerialize
		{
			get { return SerializableColor.ToString(downColor); }
			set { downColor = SerializableColor.FromString(value); }
		}

		[Description("Factor2 period is multiplier of ATR")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayName("2.Factor2")]
		public double Multiplier
		{
			get { return multiplier; }
			set { multiplier = Math.Max(0.0, value); }
		}

		[Description("Background Opacity")]
		[Gui.Design.DisplayName("5.4 Background Opacity")]
		[GridCategory("Options")]
		public int Opacity
		{
			get { return opacity; }
			set { opacity = value; }
		}

		[Description("Color the price bars in the direction of the trend?")]
		[Gui.Design.DisplayName("1.Paint Bars")]
		[GridCategory("Options")]
		public bool PaintBars
		{
			get { return paintBars; }
			set { paintBars = value; }
		}

		[Gui.Design.DisplayName("1.Factor1")]
		[Description("Factor1 Period is ATR period")]
		[GridCategory("Parameters")]
		public int PeriodAtr
		{
			get { return periodAtr; }
			set { periodAtr = Math.Max(1, value); }
		}

		[GridCategory("Parameters")]
		[Description("Factor3 period is average price period")]
		[Gui.Design.DisplayName("3.Factor3")]
		public int PeriodMedian
		{
			get { return periodMedian; }
			set { periodMedian = Math.Max(1, value); }
		}

		[Description("PlotStyle for Stop Line.")]
		[Gui.Design.DisplayName("Plot Style Stop Line")]
		[GridCategory("Plots")]
		public PlotStyle Plot1Style
		{
			get { return plot1Style; }
			set { plot1Style = value; }
		}

		[Gui.Design.DisplayName("Width Stop Line")]
		[GridCategory("Plots")]
		[Description("Width for Stop Line.")]
		public int Plot1Width
		{
			get { return plot1Width; }
			set { plot1Width = Math.Max(1, value); }
		}

		[GridCategory("Options")]
		[Description("Show Arrows when Trendline is violated?")]
		[Gui.Design.DisplayName("2.Show Arrows")]
		public bool ShowArrows
		{
			get { return showArrows; }
			set { showArrows = value; }
		}

		[Description("Show Stop Line")]
		[GridCategory("Options")]
		[Gui.Design.DisplayName("3.Show Trend Stop Line")]
		public bool ShowStopLine
		{
			get { return showStopLine; }
			set { showStopLine = value; }
		}

		[XmlIgnore]
		[Browsable(false)]
		public DataSeries StopDot
		{
			get { return Values[0]; }
		}

		[XmlIgnore]
		[Browsable(false)]
		public DataSeries StopLine
		{
			get { return Values[1]; }
		}

		[XmlIgnore]
		[Gui.Design.DisplayName("Uptrend Color")]
		[Description("Select color for Rising Trend")]
		[GridCategory("Plot Colors")]
		public Color UpColor
		{
			get { return upColor; }
			set { upColor = value; }
		}

		[Browsable(false)]
		public string UpColorSerialize
		{
			get { return SerializableColor.ToString(upColor); }
			set { upColor = SerializableColor.FromString(value); }
		}

		[Browsable(false)]
		[XmlIgnore]
		public BoolSeries UpTrend
		{
			get { return upTrend; }
		}

		protected override void Initialize()
		{
			Add(new Plot(Color.Gray, PlotStyle.Line, "StopDot"));
			Add(new Plot(Color.Gray, PlotStyle.Line, "StopLine"));

			Overlay				= true;
			PriceTypeSupported	= false;
			PlotsConfigurable	= false;
			upTrend				= new BoolSeries(this);
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
				newStop			= mm[0] - (Multiplier * offset);
				currentUpTrend	= true;
				if (!priorUpTrend)
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
				newStop			= mm[0] + (Multiplier * offset);
				currentUpTrend	= false;
				if (priorUpTrend)
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
				currentUpTrend = priorUpTrend;
				upTrend.Set(priorUpTrend);
				StopDot.Set(priorStop);
				StopLine.Set(priorStop);
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
			}
			if (colorbackground)
				BackColor = Color.FromArgb(opacity, currentUpTrend ? backgroundcolorUp : backgroundcolorDn);
			if (colorAllBackgrounds)
				BackColorAll = Color.FromArgb(opacity, currentUpTrend ? backgroundcolorUp : backgroundcolorDn);
			if (ShowArrows)
			{
				if (currentUpTrend && !priorUpTrend)
				{
					DrawArrowUp("arrow" + CurrentBar, true, 0, newStop - (0.5 * offset), upColor);
					PlaySound("Alert4.wav");
				}
				else if (!currentUpTrend && priorUpTrend)
				{
					DrawArrowDown("arrow" + CurrentBar, true, 0, newStop + (0.5 * offset), downColor);
					PlaySound("Alert4.wav");
				}
				else
					RemoveDrawObject("arrow" + CurrentBar);
			}
			if (ShowStopLine)
				PlotColors[1][0] = currentUpTrend && !priorUpTrend ? (gap ? neutralColor : upColor) : (currentUpTrend ? upColor : (!currentUpTrend && priorUpTrend ? (gap ? neutralColor : downColor) : downColor));
		}

		protected override void OnStartUp()
		{
			Plots[0].Pen.Width		= plot0Width;
			Plots[0].PlotStyle		= plot0Style;
			Plots[0].Pen.DashStyle	= dash0Style;
			Plots[1].Pen.Width		= plot1Width;
			Plots[1].PlotStyle		= plot1Style;
			Plots[1].Pen.DashStyle	= dash1Style;
			gap						= plot1Style == PlotStyle.Line || plot1Style == PlotStyle.Square;
			Plots[1].Pen.Color		= ShowStopLine ? Color.Gray : Color.Transparent;
			mm						= anaMovingMedian(Medians[0], periodMedian);
			mae						= ATR(Closes[0], periodAtr);
		}
	}
}
#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private AbTrend2[] cacheABTrend2 = null;

        private static AbTrend2 checkABTrend2 = new AbTrend2();

        /// <summary>
        /// AB trend 2 indicator
        /// </summary>
        /// <returns></returns>
        public AbTrend2 ABTrend2(Color backgroundcolorDn, Color backgroundcolorUp, bool colorAllBackgrounds, bool colorBackground, DashStyle dash1Style, Color downColor, double multiplier, int opacity, bool paintBars, int periodATR, int periodMedian, PlotStyle plot1Style, int plot1Width, bool showArrows, bool showStopLine, Color upColor)
        {
            return ABTrend2(Input, backgroundcolorDn, backgroundcolorUp, colorAllBackgrounds, colorBackground, dash1Style, downColor, multiplier, opacity, paintBars, periodATR, periodMedian, plot1Style, plot1Width, showArrows, showStopLine, upColor);
        }

        /// <summary>
        /// AB trend 2 indicator
        /// </summary>
        /// <returns></returns>
        public AbTrend2 ABTrend2(Data.IDataSeries input, Color backgroundcolorDn, Color backgroundcolorUp, bool colorAllBackgrounds, bool colorBackground, DashStyle dash1Style, Color downColor, double multiplier, int opacity, bool paintBars, int periodATR, int periodMedian, PlotStyle plot1Style, int plot1Width, bool showArrows, bool showStopLine, Color upColor)
        {
            if (cacheABTrend2 != null)
                for (int idx = 0; idx < cacheABTrend2.Length; idx++)
                    if (cacheABTrend2[idx].BackgroundcolorDn == backgroundcolorDn && cacheABTrend2[idx].BackgroundcolorUp == backgroundcolorUp && cacheABTrend2[idx].ColorAllBackgrounds == colorAllBackgrounds && cacheABTrend2[idx].ColorBackground == colorBackground && cacheABTrend2[idx].Dash1Style == dash1Style && cacheABTrend2[idx].DownColor == downColor && Math.Abs(cacheABTrend2[idx].Multiplier - multiplier) <= double.Epsilon && cacheABTrend2[idx].Opacity == opacity && cacheABTrend2[idx].PaintBars == paintBars && cacheABTrend2[idx].PeriodAtr == periodATR && cacheABTrend2[idx].PeriodMedian == periodMedian && cacheABTrend2[idx].Plot1Style == plot1Style && cacheABTrend2[idx].Plot1Width == plot1Width && cacheABTrend2[idx].ShowArrows == showArrows && cacheABTrend2[idx].ShowStopLine == showStopLine && cacheABTrend2[idx].UpColor == upColor && cacheABTrend2[idx].EqualsInput(input))
                        return cacheABTrend2[idx];

            lock (checkABTrend2)
            {
                checkABTrend2.BackgroundcolorDn = backgroundcolorDn;
                backgroundcolorDn = checkABTrend2.BackgroundcolorDn;
                checkABTrend2.BackgroundcolorUp = backgroundcolorUp;
                backgroundcolorUp = checkABTrend2.BackgroundcolorUp;
                checkABTrend2.ColorAllBackgrounds = colorAllBackgrounds;
                colorAllBackgrounds = checkABTrend2.ColorAllBackgrounds;
                checkABTrend2.ColorBackground = colorBackground;
                colorBackground = checkABTrend2.ColorBackground;
                checkABTrend2.Dash1Style = dash1Style;
                dash1Style = checkABTrend2.Dash1Style;
                checkABTrend2.DownColor = downColor;
                downColor = checkABTrend2.DownColor;
                checkABTrend2.Multiplier = multiplier;
                multiplier = checkABTrend2.Multiplier;
                checkABTrend2.Opacity = opacity;
                opacity = checkABTrend2.Opacity;
                checkABTrend2.PaintBars = paintBars;
                paintBars = checkABTrend2.PaintBars;
                checkABTrend2.PeriodAtr = periodATR;
                periodATR = checkABTrend2.PeriodAtr;
                checkABTrend2.PeriodMedian = periodMedian;
                periodMedian = checkABTrend2.PeriodMedian;
                checkABTrend2.Plot1Style = plot1Style;
                plot1Style = checkABTrend2.Plot1Style;
                checkABTrend2.Plot1Width = plot1Width;
                plot1Width = checkABTrend2.Plot1Width;
                checkABTrend2.ShowArrows = showArrows;
                showArrows = checkABTrend2.ShowArrows;
                checkABTrend2.ShowStopLine = showStopLine;
                showStopLine = checkABTrend2.ShowStopLine;
                checkABTrend2.UpColor = upColor;
                upColor = checkABTrend2.UpColor;

                if (cacheABTrend2 != null)
                    for (int idx = 0; idx < cacheABTrend2.Length; idx++)
                        if (cacheABTrend2[idx].BackgroundcolorDn == backgroundcolorDn && cacheABTrend2[idx].BackgroundcolorUp == backgroundcolorUp && cacheABTrend2[idx].ColorAllBackgrounds == colorAllBackgrounds && cacheABTrend2[idx].ColorBackground == colorBackground && cacheABTrend2[idx].Dash1Style == dash1Style && cacheABTrend2[idx].DownColor == downColor && Math.Abs(cacheABTrend2[idx].Multiplier - multiplier) <= double.Epsilon && cacheABTrend2[idx].Opacity == opacity && cacheABTrend2[idx].PaintBars == paintBars && cacheABTrend2[idx].PeriodAtr == periodATR && cacheABTrend2[idx].PeriodMedian == periodMedian && cacheABTrend2[idx].Plot1Style == plot1Style && cacheABTrend2[idx].Plot1Width == plot1Width && cacheABTrend2[idx].ShowArrows == showArrows && cacheABTrend2[idx].ShowStopLine == showStopLine && cacheABTrend2[idx].UpColor == upColor && cacheABTrend2[idx].EqualsInput(input))
                            return cacheABTrend2[idx];

                AbTrend2 indicator = new AbTrend2();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.BackgroundcolorDn = backgroundcolorDn;
                indicator.BackgroundcolorUp = backgroundcolorUp;
                indicator.ColorAllBackgrounds = colorAllBackgrounds;
                indicator.ColorBackground = colorBackground;
                indicator.Dash1Style = dash1Style;
                indicator.DownColor = downColor;
                indicator.Multiplier = multiplier;
                indicator.Opacity = opacity;
                indicator.PaintBars = paintBars;
                indicator.PeriodAtr = periodATR;
                indicator.PeriodMedian = periodMedian;
                indicator.Plot1Style = plot1Style;
                indicator.Plot1Width = plot1Width;
                indicator.ShowArrows = showArrows;
                indicator.ShowStopLine = showStopLine;
                indicator.UpColor = upColor;
                Indicators.Add(indicator);
                indicator.SetUp();

                AbTrend2[] tmp = new AbTrend2[cacheABTrend2 == null ? 1 : cacheABTrend2.Length + 1];
                if (cacheABTrend2 != null)
                    cacheABTrend2.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheABTrend2 = tmp;
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
        /// AB trend 2 indicator
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.AbTrend2 ABTrend2(Color backgroundcolorDn, Color backgroundcolorUp, bool colorAllBackgrounds, bool colorBackground, DashStyle dash1Style, Color downColor, double multiplier, int opacity, bool paintBars, int periodATR, int periodMedian, PlotStyle plot1Style, int plot1Width, bool showArrows, bool showStopLine, Color upColor)
        {
            return _indicator.ABTrend2(Input, backgroundcolorDn, backgroundcolorUp, colorAllBackgrounds, colorBackground, dash1Style, downColor, multiplier, opacity, paintBars, periodATR, periodMedian, plot1Style, plot1Width, showArrows, showStopLine, upColor);
        }

        /// <summary>
        /// AB trend 2 indicator
        /// </summary>
        /// <returns></returns>
        public Indicator.AbTrend2 ABTrend2(Data.IDataSeries input, Color backgroundcolorDn, Color backgroundcolorUp, bool colorAllBackgrounds, bool colorBackground, DashStyle dash1Style, Color downColor, double multiplier, int opacity, bool paintBars, int periodATR, int periodMedian, PlotStyle plot1Style, int plot1Width, bool showArrows, bool showStopLine, Color upColor)
        {
            return _indicator.ABTrend2(input, backgroundcolorDn, backgroundcolorUp, colorAllBackgrounds, colorBackground, dash1Style, downColor, multiplier, opacity, paintBars, periodATR, periodMedian, plot1Style, plot1Width, showArrows, showStopLine, upColor);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// AB trend 2 indicator
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.AbTrend2 ABTrend2(Color backgroundcolorDn, Color backgroundcolorUp, bool colorAllBackgrounds, bool colorBackground, DashStyle dash1Style, Color downColor, double multiplier, int opacity, bool paintBars, int periodATR, int periodMedian, PlotStyle plot1Style, int plot1Width, bool showArrows, bool showStopLine, Color upColor)
        {
            return _indicator.ABTrend2(Input, backgroundcolorDn, backgroundcolorUp, colorAllBackgrounds, colorBackground, dash1Style, downColor, multiplier, opacity, paintBars, periodATR, periodMedian, plot1Style, plot1Width, showArrows, showStopLine, upColor);
        }

        /// <summary>
        /// AB trend 2 indicator
        /// </summary>
        /// <returns></returns>
        public Indicator.AbTrend2 ABTrend2(Data.IDataSeries input, Color backgroundcolorDn, Color backgroundcolorUp, bool colorAllBackgrounds, bool colorBackground, DashStyle dash1Style, Color downColor, double multiplier, int opacity, bool paintBars, int periodATR, int periodMedian, PlotStyle plot1Style, int plot1Width, bool showArrows, bool showStopLine, Color upColor)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ABTrend2(input, backgroundcolorDn, backgroundcolorUp, colorAllBackgrounds, colorBackground, dash1Style, downColor, multiplier, opacity, paintBars, periodATR, periodMedian, plot1Style, plot1Width, showArrows, showStopLine, upColor);
        }
    }
}
#endregion
