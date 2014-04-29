// TSSuperTrend Indicator
// Version 2.5
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Xml.Serialization;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.Design;
using TSSuperTrendUtility;

namespace NinjaTrader.Indicator
{
	[Description("TSSuperTrend Indicator developed by TradingStudies.com (Version 2.5)")]
// ReSharper disable once InconsistentNaming
	public class TSSuperTrend : Indicator
	{
		private		IDataSeries				avg;
		private		Color					barColorDown		= Color.Red;
		private		Color					barColorUp			= Color.Blue;
		private		bool					colorBars;
		private		int						length				= 14;
		private		string					longAlert			= "Alert4.wav";
		private		MovingAverageType		maType				= MovingAverageType.HMA;
		private		string					mailFrom;
		private		string					mailTo;
		private		double					multiplier			= 2.618;
		private		double					offset;
		private		bool					playAlert;
		private		Color					prevColor;
		private		string					shortAlert			= "Alert4.wav";
		private		bool					showArrows;
		private		SuperTrendMode			smode				= SuperTrendMode.ATR;
		private		int						smooth				= 14;
		private		Color					tempColor;
		private		double					th;
		private		int						thisbar				= -1;
		private		double					tl					= double.MaxValue;
		private		BoolSeries				trend;

		protected override void Initialize()
		{
			Add(new Plot(Color.Green,	PlotStyle.Hash, "UpTrend"));
			Add(new Plot(Color.Red,		PlotStyle.Hash, "DownTrend"));
			Overlay				= true;
			trend				= new BoolSeries(this, MaximumBarsLookBack.Infinite);
			PriceType			= PriceType.Median;
			PriceTypeSupported	= true;
		}

		protected override void OnStartUp()
		{
			if (smooth > 1)
				switch (maType)
				{
					case MovingAverageType.SMA	: avg = SMA(Input, smooth);			break;
					case MovingAverageType.SMMA	: avg = SMMA(Input, smooth);		break;
					case MovingAverageType.TMA	: avg = TMA(Input, smooth);			break;
					case MovingAverageType.WMA	: avg = WMA(Input, smooth);			break;
					case MovingAverageType.VWMA	: avg = VWMA(Input, smooth);		break;
					case MovingAverageType.TEMA	: avg = TEMA(Input, smooth);		break;
					case MovingAverageType.HMA	: avg = HMA(Input, smooth);			break;
					case MovingAverageType.VMA	: avg = VMA(Input, smooth, smooth);	break;
					default						: avg = EMA(Input, smooth);			break;
				}
			else
				avg = Input;
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar == 0)
			{
				trend		.Set(true);
				UpTrend		.Set(Input[0]);
				DownTrend	.Set(Input[0]);
				return;
			}

			switch (smode)
			{
				case SuperTrendMode.ATR		: offset = ATR(length)[0] * Multiplier;							break;
				case SuperTrendMode.Adaptive: offset = ATR(length)[0] * HomodyneDiscriminator(Input)[0]/10;	break;
				default						: offset = Dtt(length, Multiplier);								break;
			}

			if (FirstTickOfBar) prevColor = tempColor;

			trend.Set(Close[0] > DownTrend[1] || !(Close[0] < UpTrend[1]) && trend[1]);

			if (trend[0] && !trend[1])
			{
				th = High[0];
				UpTrend.Set(Math.Max(avg[0] - offset, tl));
				if (Plots[0].PlotStyle == PlotStyle.Line) UpTrend.Set(1, DownTrend[1]);
				tempColor = barColorUp;
				if (ShowArrows) DrawArrowUp(CurrentBar.ToString(CultureInfo.InvariantCulture), true, 0, UpTrend[0] - TickSize, barColorUp);
				if (thisbar != CurrentBar)
				{
					thisbar = CurrentBar;
					if (PlayAlert) 
						PlaySound(LongAlert);
					if (SendEmail && !string.IsNullOrEmpty(mailFrom) && !string.IsNullOrEmpty(mailTo)) 
						SendMail(mailFrom, mailTo, "SuperTrend Long Signal", string.Format("{0}: {1} {2} {3} chart", Time[0], Instrument.FullName, Bars.Period.Value, Bars.Period.Id));
				}
			}
			else if (!trend[0] && trend[1])
			{
				tl = Low[0];
				DownTrend.Set(Math.Min(avg[0] + offset, th));
				if (Plots[1].PlotStyle == PlotStyle.Line) DownTrend.Set(1, UpTrend[1]);
				tempColor = barColorDown;
				if (ShowArrows) DrawArrowDown(CurrentBar.ToString(CultureInfo.InvariantCulture), true, 0, DownTrend[0] + TickSize, barColorDown);
				if (thisbar != CurrentBar)
				{
					thisbar = CurrentBar;
					if (PlayAlert) 
						PlaySound(ShortAlert);
					if (SendEmail && !string.IsNullOrEmpty(mailFrom) && !string.IsNullOrEmpty(mailTo)) 
						SendMail(mailFrom, mailTo, "SuperTrend Short Signal", string.Format("{0}: {1} {2} {3} chart", Time[0], Instrument.FullName, Bars.Period.Value, Bars.Period.Id));
				}
			}
			else
			{
				if (trend[0])
				{
					UpTrend.Set((avg[0] - offset) > UpTrend[1] ? (avg[0] - offset) : UpTrend[1]);
					th = Math.Max(th, High[0]);
				}
				else
				{
					DownTrend.Set((avg[0] + offset) < DownTrend[1] ? (avg[0] + offset) : DownTrend[1]);
					tl = Math.Min(tl, Low[0]);
				}
				RemoveDrawObject(CurrentBar.ToString(CultureInfo.InvariantCulture));
				tempColor = prevColor;
			}

			if (!colorBars) return;

			CandleOutlineColor = tempColor;
			BarColor = Open[0] < Close[0] && ChartControl.ChartStyleType == ChartStyleType.CandleStick ? Color.Transparent : tempColor;
		}

		private double Dtt(int nDay, double mult)
		{
			double hh = MAX(High, nDay)[0];
			double hc = MAX(Close, nDay)[0];
			double ll = MIN(Low, nDay)[0];
			double lc = MIN(Close, nDay)[0];
			return mult * Math.Max((hh - lc), (hc - ll));
		}

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries UpTrend
		{
			get { Update(); return Values[0]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries DownTrend
		{
			get { Update(); return Values[1]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public BoolSeries Trend
		{
			get { Update(); return trend; }
		}

		[Description("SuperTrendMode")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayName("01. SuperTrend Mode")]
		public SuperTrendMode StMode
		{
			get { return smode; }
			set { smode = value; }
		}

		[Description("ATR/DT Period")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayName("02. Period")]
		public int Length
		{
			get { return length; }
			set { length = Math.Max(1, value); }
		}

		[Description("ATR Multiplier")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayName("03. Multiplier")]
		public double Multiplier
		{
			get { return multiplier; }
			set { multiplier = Math.Max(0.0000001, value); }
		}

		[Description("Moving Average Type for smoothing")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayName("04. Moving Average Type")]
		public MovingAverageType MaType
		{
			get { return maType; }
			set { maType = value; }
		}

		[Description("Smoothing Period")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayName("05. SmoothingPeriod (MA)")]
		public int Smooth
		{
			get { return smooth; }
			set { smooth = Math.Max(1, value); }
		}

		[Description("Show Arrows when Trendline is violated?")]
		[Category("Visual")]
		[Gui.Design.DisplayName("01. Show Arrows?")]
		public bool ShowArrows
		{
			get { return showArrows; }
			set { showArrows = value; }
		}

		[Description("Color the bars in the direction of the trend?")]
		[Category("Visual")]
		[Gui.Design.DisplayName("02. Color Bars?")]
		public bool ColorBars
		{
			get { return colorBars; }
			set { colorBars = value; }
		}

		[XmlIgnore]
		[Description("Color of up bars.")]
		[Category("Visual")]
		[Gui.Design.DisplayName("03. Up color")]
		public Color BarColorUp
		{
			get { return barColorUp; }
			set { barColorUp = value; }
		}

		[Browsable(false)]
		public string BarColorUpSerialize
		{
			get { return SerializableColor.ToString(barColorUp); }
			set { barColorUp = SerializableColor.FromString(value); }
		}

		[XmlIgnore]
		[Description("Color of down bars.")]
		[Category("Visual")]
		[Gui.Design.DisplayName("04. Down color")]
		public Color BarColorDown
		{
			get { return barColorDown; }
			set { barColorDown = value; }
		}

		[Browsable(false)]
		public string BarColorDownSerialize
		{
			get { return SerializableColor.ToString(barColorDown); }
			set { barColorDown = SerializableColor.FromString(value); }
		}

		[Description("Play Alert")]
		[Category("Sounds")]
		[Gui.Design.DisplayName("01. Play Alert?")]
		public bool PlayAlert
		{
			get { return playAlert; }
			set { playAlert = value; }
		}

		[Description("File Name for long alert")]
		[Category("Sounds")]
		[Gui.Design.DisplayName("02. Long Alert")]
		public string LongAlert
		{
			get { return longAlert; }
			set { longAlert = value; }
		}

		[Description("File Name for short alert")]
		[Category("Sounds")]
		[Gui.Design.DisplayName("03. Short Alert")]
		public string ShortAlert
		{
			get { return shortAlert; }
			set { shortAlert = value; }
		}

		[Description("")]
		[Category("Email")]
		[Gui.Design.DisplayName("01. Send Email?")]
		public bool SendEmail { get; set; }

		[Description("")]
		[Category("Email")]
		[Gui.Design.DisplayName("02. Email From")]
		public string MailFrom
		{
			get { return mailFrom; }
			set { mailFrom = value; }
		}

		[Description("")]
		[Category("Email")]
		[Gui.Design.DisplayName("03. Email To")]
		public string MailTo
		{
			get { return mailTo; }
			set { mailTo = value; }
		}
	}
}
// ReSharper disable InconsistentNaming
namespace TSSuperTrendUtility
{
	public enum SuperTrendMode
	{
		ATR,
		DualThrust,
		Adaptive
	}

	public enum MovingAverageType
	{
		SMA,
		SMMA,
		TMA,
		WMA,
		VWMA,
		TEMA,
		HMA,
		EMA,
		VMA
	}
// ReSharper restore InconsistentNaming
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private TSSuperTrend[] cacheTSSuperTrend = null;

        private static TSSuperTrend checkTSSuperTrend = new TSSuperTrend();

        /// <summary>
        /// TSSuperTrend Indicator developed by TradingStudies.com (Version 2.5)
        /// </summary>
        /// <returns></returns>
        public TSSuperTrend TSSuperTrend(int length, MovingAverageType maType, double multiplier, int smooth, SuperTrendMode stMode)
        {
            return TSSuperTrend(Input, length, maType, multiplier, smooth, stMode);
        }

        /// <summary>
        /// TSSuperTrend Indicator developed by TradingStudies.com (Version 2.5)
        /// </summary>
        /// <returns></returns>
        public TSSuperTrend TSSuperTrend(Data.IDataSeries input, int length, MovingAverageType maType, double multiplier, int smooth, SuperTrendMode stMode)
        {
            if (cacheTSSuperTrend != null)
                for (int idx = 0; idx < cacheTSSuperTrend.Length; idx++)
                    if (cacheTSSuperTrend[idx].Length == length && cacheTSSuperTrend[idx].MaType == maType && Math.Abs(cacheTSSuperTrend[idx].Multiplier - multiplier) <= double.Epsilon && cacheTSSuperTrend[idx].Smooth == smooth && cacheTSSuperTrend[idx].StMode == stMode && cacheTSSuperTrend[idx].EqualsInput(input))
                        return cacheTSSuperTrend[idx];

            lock (checkTSSuperTrend)
            {
                checkTSSuperTrend.Length = length;
                length = checkTSSuperTrend.Length;
                checkTSSuperTrend.MaType = maType;
                maType = checkTSSuperTrend.MaType;
                checkTSSuperTrend.Multiplier = multiplier;
                multiplier = checkTSSuperTrend.Multiplier;
                checkTSSuperTrend.Smooth = smooth;
                smooth = checkTSSuperTrend.Smooth;
                checkTSSuperTrend.StMode = stMode;
                stMode = checkTSSuperTrend.StMode;

                if (cacheTSSuperTrend != null)
                    for (int idx = 0; idx < cacheTSSuperTrend.Length; idx++)
                        if (cacheTSSuperTrend[idx].Length == length && cacheTSSuperTrend[idx].MaType == maType && Math.Abs(cacheTSSuperTrend[idx].Multiplier - multiplier) <= double.Epsilon && cacheTSSuperTrend[idx].Smooth == smooth && cacheTSSuperTrend[idx].StMode == stMode && cacheTSSuperTrend[idx].EqualsInput(input))
                            return cacheTSSuperTrend[idx];

                TSSuperTrend indicator = new TSSuperTrend();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Length = length;
                indicator.MaType = maType;
                indicator.Multiplier = multiplier;
                indicator.Smooth = smooth;
                indicator.StMode = stMode;
                Indicators.Add(indicator);
                indicator.SetUp();

                TSSuperTrend[] tmp = new TSSuperTrend[cacheTSSuperTrend == null ? 1 : cacheTSSuperTrend.Length + 1];
                if (cacheTSSuperTrend != null)
                    cacheTSSuperTrend.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheTSSuperTrend = tmp;
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
        /// TSSuperTrend Indicator developed by TradingStudies.com (Version 2.5)
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.TSSuperTrend TSSuperTrend(int length, MovingAverageType maType, double multiplier, int smooth, SuperTrendMode stMode)
        {
            return _indicator.TSSuperTrend(Input, length, maType, multiplier, smooth, stMode);
        }

        /// <summary>
        /// TSSuperTrend Indicator developed by TradingStudies.com (Version 2.5)
        /// </summary>
        /// <returns></returns>
        public Indicator.TSSuperTrend TSSuperTrend(Data.IDataSeries input, int length, MovingAverageType maType, double multiplier, int smooth, SuperTrendMode stMode)
        {
            return _indicator.TSSuperTrend(input, length, maType, multiplier, smooth, stMode);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// TSSuperTrend Indicator developed by TradingStudies.com (Version 2.5)
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.TSSuperTrend TSSuperTrend(int length, MovingAverageType maType, double multiplier, int smooth, SuperTrendMode stMode)
        {
            return _indicator.TSSuperTrend(Input, length, maType, multiplier, smooth, stMode);
        }

        /// <summary>
        /// TSSuperTrend Indicator developed by TradingStudies.com (Version 2.5)
        /// </summary>
        /// <returns></returns>
        public Indicator.TSSuperTrend TSSuperTrend(Data.IDataSeries input, int length, MovingAverageType maType, double multiplier, int smooth, SuperTrendMode stMode)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.TSSuperTrend(input, length, maType, multiplier, smooth, stMode);
        }
    }
}
#endregion
