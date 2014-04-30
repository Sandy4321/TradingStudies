using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Xml.Serialization;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;

namespace NinjaTrader.Indicator
{
// ReSharper disable InconsistentNaming
	/// <summary>
	/// The anaMACDBBLines (Moving Average Convergence/Divergence) is a trend following momentum indicator that shows the relationship between two moving averages of prices.
	/// </summary>
	[Description("The anaMACDBBLines (Moving Average Convergence/Divergence) is a trend following momentum indicator that shows the relationship between two moving averages of prices.")]
	public class anaMACDBBLines : Indicator
	{
		private int			fast			= 12;
		private int			slow			= 26;
		private int			bandPeriod 		= 10;
		private double		stdDevNumber	= 1.0;
		private int			dotsize			= 2;
		private int 		plot2Width 		= 1;
		private DashStyle	dash2Style		= DashStyle.Dot;
		private int 		plot3Width 		= 1;
		private DashStyle 	dash3Style		= DashStyle.Solid;
		private int 		plot5Width 		= 2;
		private DashStyle 	dash5Style		= DashStyle.Solid;
		private int			crossSize		= 8;
		private int 		plot7Width 		= 2;
		private Color		dotsUpOutside	= Color.Lime;	
		private Color		dotsUpInside	= Color.PaleGreen;	
		private Color	 	dotsDownOutside	= Color.Red;
		private Color		dotsDownInside	= Color.Plum;	
		private Color		dotsRim			= Color.Black;
		private Color		bbAverage		= Color.LightSkyBlue;
		private Color		bbUpper			= Color.LightSkyBlue;
		private Color		bbLower			= Color.LightSkyBlue;
		private Color		zeroPositive	= Color.Lime;
		private Color		zeroNegative	= Color.Red;
		private Color 		zeroCross		= Color.Yellow;
		private Color		connector		= Color.White;
			
		private MACD		BMACD;
		private StdDev		SDBB;

		protected override void Initialize()
		{
			Add(new Plot(Color.Gray, PlotStyle.Dot,		"BBMACD"));
			Add(new Plot(Color.Gray, PlotStyle.Dot,		"BBMACDFrame"));
			Add(new Plot(Color.Gray, PlotStyle.Line,	"Average"));
			Add(new Plot(Color.Gray, PlotStyle.Line,	"Upper"));
			Add(new Plot(Color.Gray, PlotStyle.Line,	"Lower"));
			Add(new Plot(Color.Gray, PlotStyle.Line,	"ZeroLine"));
			Add(new Plot(Color.Gray, PlotStyle.Dot,		"MACDCross"));
			Add(new Plot(Color.Gray, PlotStyle.Line,	"BBMACDLine"));
			
			PlotsConfigurable = false;
			PaintPriceMarkers = false;
		}

		protected override void OnStartUp()
		{
			BMACD=MACD(Input,fast,slow,bandPeriod);
			SDBB=StdDev(BMACD,bandPeriod);

			Plots[0].Pen.Width		= DotSize;
			Plots[0].Pen.DashStyle	= DashStyle.Dot;
			Plots[1].Pen.Width		= DotSize + 1;
			Plots[1].Pen.DashStyle	= DashStyle.Dot;
			Plots[2].Pen.Width		= plot2Width;
			Plots[2].Pen.DashStyle	= dash2Style;
			Plots[3].Pen.Width		= plot3Width;
			Plots[3].Pen.DashStyle	= dash3Style;
			Plots[4].Pen.Width		= plot3Width;
			Plots[4].Pen.DashStyle	= dash3Style;
			Plots[5].Pen.Width		= plot5Width;
			Plots[5].Pen.DashStyle	= dash5Style;
			Plots[6].Pen.Width		= CrossSize;
			Plots[7].Pen.Width		= plot7Width;

			Plots[1].Pen.Color = DotsRim;
			Plots[2].Pen.Color = BBAverage;
			Plots[3].Pen.Color = BBUpper;
			Plots[4].Pen.Color = BBLower;
			Plots[6].Pen.Color = ZeroCross;
			Plots[7].Pen.Color = Connector;
		}

		protected override void OnBarUpdate()
		{
			double macdValue = BMACD[0];
			BBMACD.Set(macdValue);
			BBMACDLine.Set(macdValue);
			BBMACDFrame.Set(macdValue);
			double avg = BMACD.Avg[0];
			Average.Set(avg);
			ZeroLine.Set(0);
			double stdDevValue = SDBB[0];
			Upper.Set(avg + StdDevNumber * stdDevValue);
			Lower.Set(avg - StdDevNumber * stdDevValue);
			
			if (Rising(BBMACD))
			{
				if(macdValue > avg + StdDevNumber * stdDevValue)
					PlotColors[0][0] = DotsUpOutside;
				else
					PlotColors[0][0] = DotsUpInside;
			}
			else
			{
				if(macdValue < avg - StdDevNumber * stdDevValue)
					PlotColors[0][0] = DotsDownOutside;
				else
					PlotColors[0][0] = DotsDownInside;
			}
			if (BBMACD[0] > 0)
			{
				PlotColors[5][0] = ZeroPositive;
				if (CurrentBar != 0 && BBMACD[1] <= 0)
					MACDCross.Set(0);
				else
					MACDCross.Reset();
			}
			else
			{
				PlotColors[5][0] = ZeroNegative;
				if (CurrentBar != 0 && BBMACD[1] > 0)
					MACDCross.Set(0);
				else
					MACDCross.Reset();
			}
		}

		#region Properties
		[Description("Band Period for Bollinger Band")]
		[GridCategory("Parameters")]
		public int BandPeriod
		{
			get { return bandPeriod; }
			set { bandPeriod = Math.Max(1, value); }
		}

		[Description("Period for fast EMA")]
		[GridCategory("Parameters")]
		public int Fast
		{
			get { return fast; }
			set { fast = Math.Max(1, value); }
		}

		[Description("Period for slow EMA")]
		[GridCategory("Parameters")]
		public int Slow
		{
			get { return slow; }
			set { slow = Math.Max(1, value); }
		}

		[Description("Number of standard deviations")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("# of Std. Dev.")]
		public double StdDevNumber
		{
			get { return stdDevNumber; }
			set { stdDevNumber = Math.Max(0, value); }
		}

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries BBMACD
		{
			get { return Values[0]; }
		}
		
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries BBMACDFrame
		{
			get { return Values[1]; }
		}
		
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries Average
		{
			get { return Values[2]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries Upper
		{
			get { return Values[3]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries Lower
		{
			get { return Values[4]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries ZeroLine
		{
			get { return Values[5]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries MACDCross
		{
			get { return Values[6]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries BBMACDLine
		{
			get { return Values[7]; }
		}

		[Description("Dotsize")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Dot Size MACD")]
		public int DotSize
		{
			get { return dotsize; }
			set { dotsize = Math.Max(1, value); }
		}

		[Description("Width for Average of Bollinger Bands.")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Width Average")]
		public int Plot2Width
		{
			get { return plot2Width; }
			set { plot2Width = Math.Max(1, value); }
		}
		
		[Description("DashStyle for Average of Bollinger Bands.")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Dash Style Average")]
		public DashStyle Dash2Style
		{
			get { return dash2Style; }
			set { dash2Style = value; }
		} 
		
		[Description("Width for Bollinger Bands.")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Width Bollinger Bands")]
		public int Plot3Width
		{
			get { return plot3Width; }
			set { plot3Width = Math.Max(1, value); }
		}
		
		[Description("DashStyle for Bollinger Bands.")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Dash Style Bollinger Bands")]
		public DashStyle Dash3Style
		{
			get { return dash3Style; }
			set { dash3Style = value; }
		} 
		
		[Description("Width for Zero Line.")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Width Zero Line")]
		public int Plot5Width
		{
			get { return plot5Width; }
			set { plot5Width = Math.Max(1, value); }
		}
		
		[Description("DashStyle for Zero Line.")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Dash Style Zero Line")]
		public DashStyle Dash5Style
		{
			get { return dash5Style; }
			set { dash5Style = value; }
		} 		
		
		[Description("Dotsize")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Dot Size ZeroCross")]
		public int CrossSize
		{
			get { return crossSize; }
			set { crossSize = Math.Max(1, value); }
		}

		[Description("Width for MACD Connectors.")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Width Connectors")]
		public int Plot7Width
		{
			get { return plot7Width; }
			set { plot7Width = Math.Max(1, value); }
		}
		
		[XmlIgnore]
		[Description("Select Color")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Dots Up Outside")]
		public Color DotsUpOutside
		{
			get { return dotsUpOutside; }
			set { dotsUpOutside = value; }
		}
		
		[Browsable(false)]
		public string DotsUpOutsideSerialize
		{
			get { return Gui.Design.SerializableColor.ToString(dotsUpOutside); }
			set { dotsUpOutside = Gui.Design.SerializableColor.FromString(value); }
		}

		[XmlIgnore]
		[Description("Select Color")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Dots Up Inside")]
		public Color DotsUpInside
		{
			get { return dotsUpInside; }
			set { dotsUpInside = value; }
		}
		
		[Browsable(false)]
		public string DotsUpInsideSerialize
		{
			get { return Gui.Design.SerializableColor.ToString(dotsUpInside); }
			set { dotsUpInside = Gui.Design.SerializableColor.FromString(value); }
		}

		[XmlIgnore]
		[Description("Select Color")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Dots Down Outside")]
		public Color DotsDownOutside
		{
			get { return dotsDownOutside; }
			set { dotsDownOutside = value; }
		}
		
		[Browsable(false)]
		public string DotsDownOutsideSerialize
		{
			get { return Gui.Design.SerializableColor.ToString(dotsDownOutside); }
			set { dotsDownOutside = Gui.Design.SerializableColor.FromString(value); }
		}

		[XmlIgnore]
		[Description("Select Color")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Dots Down Inside")]
		public Color DotsDownInside
		{
			get { return dotsDownInside; }
			set { dotsDownInside = value; }
		}
		
		[Browsable(false)]
		public string DotsDownInsideSerialize
		{
			get { return Gui.Design.SerializableColor.ToString(dotsDownInside); }
			set { dotsDownInside = Gui.Design.SerializableColor.FromString(value); }
		}

		[XmlIgnore]
		[Description("Select Color")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Dots Rim")]
		public Color DotsRim
		{
			get { return dotsRim; }
			set { dotsRim = value; }
		}
		
		[Browsable(false)]
		public string DotsRimSerialize
		{
			get { return Gui.Design.SerializableColor.ToString(dotsRim); }
			set { dotsRim = Gui.Design.SerializableColor.FromString(value); }
		}

		[XmlIgnore]
		[Description("Select Color")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Bollinger Average")]
		public Color BBAverage
		{
			get { return bbAverage; }
			set { bbAverage = value; }
		}
		
		[Browsable(false)]
		public string BBAverageSerialize
		{
			get { return Gui.Design.SerializableColor.ToString(bbAverage); }
			set { bbAverage = Gui.Design.SerializableColor.FromString(value); }
		}

		[XmlIgnore]
		[Description("Select Color")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Bollinger Upper Band")]
		public Color BBUpper
		{
			get { return bbUpper; }
			set { bbUpper = value; }
		}
		
		[Browsable(false)]
		public string BBUpperSerialize
		{
			get { return Gui.Design.SerializableColor.ToString(bbUpper); }
			set { bbUpper = Gui.Design.SerializableColor.FromString(value); }
		}

		[XmlIgnore]
		[Description("Select Color")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Bollinger Lower Band")]
		public Color BBLower
		{
			get { return bbLower; }
			set { bbLower = value; }
		}
		
		[Browsable(false)]
		public string BBLowerSerialize
		{
			get { return Gui.Design.SerializableColor.ToString(bbLower); }
			set { bbLower = Gui.Design.SerializableColor.FromString(value); }
		}

		[XmlIgnore]
		[Description("Select Color")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Zeroline Positive")]
		public Color ZeroPositive
		{
			get { return zeroPositive; }
			set { zeroPositive = value; }
		}
		
		[Browsable(false)]
		public string ZeroPositiveSerialize
		{
			get { return Gui.Design.SerializableColor.ToString(zeroPositive); }
			set { zeroPositive = Gui.Design.SerializableColor.FromString(value); }
		}

		[XmlIgnore]
		[Description("Select Color")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Zeroline Negative")]
		public Color ZeroNegative
		{
			get { return zeroNegative; }
			set { zeroNegative = value; }
		}
		
		[Browsable(false)]
		public string ZeroNegativeSerialize
		{
			get { return Gui.Design.SerializableColor.ToString(zeroNegative); }
			set { zeroNegative = Gui.Design.SerializableColor.FromString(value); }
		}

		[XmlIgnore]
		[Description("Select Color")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Zero Cross")]
		public Color ZeroCross
		{
			get { return zeroCross; }
			set { zeroCross = value; }
		}
		
		[Browsable(false)]
		public string ZeroCrossSerialize
		{
			get { return Gui.Design.SerializableColor.ToString(zeroCross); }
			set { zeroCross = Gui.Design.SerializableColor.FromString(value); }
		}

		[XmlIgnore]
		[Description("Select Color")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Connector")]
		public Color Connector
		{
			get { return connector; }
			set { connector = value; }
		}
		
		[Browsable(false)]
		public string ConnectorSerialize
		{
			get { return Gui.Design.SerializableColor.ToString(connector); }
			set { connector = Gui.Design.SerializableColor.FromString(value); }
		}
		#endregion
	}
// ReSharper restore InconsistentNaming
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private anaMACDBBLines[] cacheanaMACDBBLines = null;

        private static anaMACDBBLines checkanaMACDBBLines = new anaMACDBBLines();

        /// <summary>
        /// The anaMACDBBLines (Moving Average Convergence/Divergence) is a trend following momentum indicator that shows the relationship between two moving averages of prices.
        /// </summary>
        /// <returns></returns>
        public anaMACDBBLines anaMACDBBLines(int bandPeriod, int fast, int slow, double stdDevNumber)
        {
            return anaMACDBBLines(Input, bandPeriod, fast, slow, stdDevNumber);
        }

        /// <summary>
        /// The anaMACDBBLines (Moving Average Convergence/Divergence) is a trend following momentum indicator that shows the relationship between two moving averages of prices.
        /// </summary>
        /// <returns></returns>
        public anaMACDBBLines anaMACDBBLines(Data.IDataSeries input, int bandPeriod, int fast, int slow, double stdDevNumber)
        {
            if (cacheanaMACDBBLines != null)
                for (int idx = 0; idx < cacheanaMACDBBLines.Length; idx++)
                    if (cacheanaMACDBBLines[idx].BandPeriod == bandPeriod && cacheanaMACDBBLines[idx].Fast == fast && cacheanaMACDBBLines[idx].Slow == slow && Math.Abs(cacheanaMACDBBLines[idx].StdDevNumber - stdDevNumber) <= double.Epsilon && cacheanaMACDBBLines[idx].EqualsInput(input))
                        return cacheanaMACDBBLines[idx];

            lock (checkanaMACDBBLines)
            {
                checkanaMACDBBLines.BandPeriod = bandPeriod;
                bandPeriod = checkanaMACDBBLines.BandPeriod;
                checkanaMACDBBLines.Fast = fast;
                fast = checkanaMACDBBLines.Fast;
                checkanaMACDBBLines.Slow = slow;
                slow = checkanaMACDBBLines.Slow;
                checkanaMACDBBLines.StdDevNumber = stdDevNumber;
                stdDevNumber = checkanaMACDBBLines.StdDevNumber;

                if (cacheanaMACDBBLines != null)
                    for (int idx = 0; idx < cacheanaMACDBBLines.Length; idx++)
                        if (cacheanaMACDBBLines[idx].BandPeriod == bandPeriod && cacheanaMACDBBLines[idx].Fast == fast && cacheanaMACDBBLines[idx].Slow == slow && Math.Abs(cacheanaMACDBBLines[idx].StdDevNumber - stdDevNumber) <= double.Epsilon && cacheanaMACDBBLines[idx].EqualsInput(input))
                            return cacheanaMACDBBLines[idx];

                anaMACDBBLines indicator = new anaMACDBBLines();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.BandPeriod = bandPeriod;
                indicator.Fast = fast;
                indicator.Slow = slow;
                indicator.StdDevNumber = stdDevNumber;
                Indicators.Add(indicator);
                indicator.SetUp();

                anaMACDBBLines[] tmp = new anaMACDBBLines[cacheanaMACDBBLines == null ? 1 : cacheanaMACDBBLines.Length + 1];
                if (cacheanaMACDBBLines != null)
                    cacheanaMACDBBLines.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheanaMACDBBLines = tmp;
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
        /// The anaMACDBBLines (Moving Average Convergence/Divergence) is a trend following momentum indicator that shows the relationship between two moving averages of prices.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.anaMACDBBLines anaMACDBBLines(int bandPeriod, int fast, int slow, double stdDevNumber)
        {
            return _indicator.anaMACDBBLines(Input, bandPeriod, fast, slow, stdDevNumber);
        }

        /// <summary>
        /// The anaMACDBBLines (Moving Average Convergence/Divergence) is a trend following momentum indicator that shows the relationship between two moving averages of prices.
        /// </summary>
        /// <returns></returns>
        public Indicator.anaMACDBBLines anaMACDBBLines(Data.IDataSeries input, int bandPeriod, int fast, int slow, double stdDevNumber)
        {
            return _indicator.anaMACDBBLines(input, bandPeriod, fast, slow, stdDevNumber);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The anaMACDBBLines (Moving Average Convergence/Divergence) is a trend following momentum indicator that shows the relationship between two moving averages of prices.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.anaMACDBBLines anaMACDBBLines(int bandPeriod, int fast, int slow, double stdDevNumber)
        {
            return _indicator.anaMACDBBLines(Input, bandPeriod, fast, slow, stdDevNumber);
        }

        /// <summary>
        /// The anaMACDBBLines (Moving Average Convergence/Divergence) is a trend following momentum indicator that shows the relationship between two moving averages of prices.
        /// </summary>
        /// <returns></returns>
        public Indicator.anaMACDBBLines anaMACDBBLines(Data.IDataSeries input, int bandPeriod, int fast, int slow, double stdDevNumber)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.anaMACDBBLines(input, bandPeriod, fast, slow, stdDevNumber);
        }
    }
}
#endregion
