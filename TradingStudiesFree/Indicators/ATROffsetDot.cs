using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.Design;

namespace NinjaTrader.Indicator
{
	[Description("Value Charts by David Stendhal; Converted to NinjaTrader by TradingStudies.com")]
	public class AtrOffsetDot : Indicator
	{
		private Color			downColor	= Color.Red;
		private int				length		= 40; 
		private double			mltpl		= 0.5;
		private float			radius		= 0.5f;
		private Color			upColor		= Color.Green;
		private SolidBrush 		brushUp;
		private SolidBrush 		brushDown;


		protected override void Initialize()
		{
			Add(new Plot(Color.Black, PlotStyle.Dot, "HighOffset"));
			Add(new Plot(Color.Black, PlotStyle.Dot, "LowOffset"));
			Overlay					= true;
			PlotsConfigurable 		= false;
		}

		protected override void OnBarUpdate()
		{
			double x = ATR(length)[0] * mltpl;
			Values[0].Set(High[0]	+ x);
			Values[1].Set(Low[0]	- x);
		}

		protected override void OnStartUp()
		{
			brushUp		= new SolidBrush(upColor);
			brushDown	= new SolidBrush(downColor);
		}

		public override void Plot(Graphics graphics, Rectangle bounds, double min, double max)
		{

			for (int idx = LastBarIndexPainted; idx >= FirstBarIndexPainted; idx--)
			{
				if (idx - Displacement < 0 || idx - Displacement >= Bars.Count || (!ChartControl.ShowBarsRequired && idx - Displacement < BarsRequired))
					continue;

				double valH = Values[0].Get(idx);
				double valL = Values[1].Get(idx);
				int x		= ChartControl.GetXByBarIdx(BarsArray[0], idx);

				int y1 = ChartControl.GetYByValue(this, valH);
				int y2 = ChartControl.GetYByValue(this, valL);

				SmoothingMode oldSmoothingMode	= graphics.SmoothingMode;
				graphics.SmoothingMode			= SmoothingMode.AntiAlias;

				graphics.FillEllipse(brushUp,	x - radius, y1 - radius, radius * 2, radius * 2);
				graphics.FillEllipse(brushDown,	x - radius, y2 - radius, radius * 2, radius * 2);

				graphics.SmoothingMode	= oldSmoothingMode;
			}
		}

		#region Properties

		[Description("Period")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayName("ATR Period")]
		public int Length
		{
			get { return length; }
			set { length = Math.Min(Math.Max(2, value), 1000); }
		}

		[Description("Multiplier")]
		[GridCategory("Parameters")]
		public double Multiplier
		{
			get { return mltpl; }
			set { mltpl = Math.Max(value, 0); }
		}

		[Description("High Color")]
		[GridCategory("Appearance")]
		[Gui.Design.DisplayName("High Color")]
		public Color UpColor
		{
			get { return upColor; }
			set { upColor = value; }
		}

		[Description("Low Color")]
		[GridCategory("Appearance")]
		[Gui.Design.DisplayName("Low Color")]
		public Color DownColor
		{
			get { return downColor; }
			set { downColor = value; }
		}

		[Description("Radius")]
		[GridCategory("Appearance")]
		[Gui.Design.DisplayName("Radius")]
		public float Radius
		{
			get { return radius; }
			set { radius = Math.Max(0.5f, value); }
		}

		[Browsable(false)]
		public string UpColorSerialize
		{
			get { return SerializableColor.ToString(upColor); }
			set { upColor = SerializableColor.FromString(value); }
		}

		[Browsable(false)]
		public string DownColorSerialize
		{
			get { return SerializableColor.ToString(downColor); }
			set { downColor = SerializableColor.FromString(value); }
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
        private AtrOffsetDot[] cacheATROffsetDot = null;

        private static AtrOffsetDot checkATROffsetDot = new AtrOffsetDot();

        /// <summary>
        /// Value Charts by David Stendhal; Converted to NinjaTrader by TradingStudies.com
        /// </summary>
        /// <returns></returns>
        public AtrOffsetDot ATROffsetDot(Color downColor, int length, double multiplier, float radius, Color upColor)
        {
            return ATROffsetDot(Input, downColor, length, multiplier, radius, upColor);
        }

        /// <summary>
        /// Value Charts by David Stendhal; Converted to NinjaTrader by TradingStudies.com
        /// </summary>
        /// <returns></returns>
        public AtrOffsetDot ATROffsetDot(Data.IDataSeries input, Color downColor, int length, double multiplier, float radius, Color upColor)
        {
            if (cacheATROffsetDot != null)
                for (int idx = 0; idx < cacheATROffsetDot.Length; idx++)
                    if (cacheATROffsetDot[idx].DownColor == downColor && cacheATROffsetDot[idx].Length == length && Math.Abs(cacheATROffsetDot[idx].Multiplier - multiplier) <= double.Epsilon && cacheATROffsetDot[idx].Radius == radius && cacheATROffsetDot[idx].UpColor == upColor && cacheATROffsetDot[idx].EqualsInput(input))
                        return cacheATROffsetDot[idx];

            lock (checkATROffsetDot)
            {
                checkATROffsetDot.DownColor = downColor;
                downColor = checkATROffsetDot.DownColor;
                checkATROffsetDot.Length = length;
                length = checkATROffsetDot.Length;
                checkATROffsetDot.Multiplier = multiplier;
                multiplier = checkATROffsetDot.Multiplier;
                checkATROffsetDot.Radius = radius;
                radius = checkATROffsetDot.Radius;
                checkATROffsetDot.UpColor = upColor;
                upColor = checkATROffsetDot.UpColor;

                if (cacheATROffsetDot != null)
                    for (int idx = 0; idx < cacheATROffsetDot.Length; idx++)
                        if (cacheATROffsetDot[idx].DownColor == downColor && cacheATROffsetDot[idx].Length == length && Math.Abs(cacheATROffsetDot[idx].Multiplier - multiplier) <= double.Epsilon && cacheATROffsetDot[idx].Radius == radius && cacheATROffsetDot[idx].UpColor == upColor && cacheATROffsetDot[idx].EqualsInput(input))
                            return cacheATROffsetDot[idx];

                AtrOffsetDot indicator = new AtrOffsetDot();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.DownColor = downColor;
                indicator.Length = length;
                indicator.Multiplier = multiplier;
                indicator.Radius = radius;
                indicator.UpColor = upColor;
                Indicators.Add(indicator);
                indicator.SetUp();

                AtrOffsetDot[] tmp = new AtrOffsetDot[cacheATROffsetDot == null ? 1 : cacheATROffsetDot.Length + 1];
                if (cacheATROffsetDot != null)
                    cacheATROffsetDot.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheATROffsetDot = tmp;
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
        /// Value Charts by David Stendhal; Converted to NinjaTrader by TradingStudies.com
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.AtrOffsetDot ATROffsetDot(Color downColor, int length, double multiplier, float radius, Color upColor)
        {
            return _indicator.ATROffsetDot(Input, downColor, length, multiplier, radius, upColor);
        }

        /// <summary>
        /// Value Charts by David Stendhal; Converted to NinjaTrader by TradingStudies.com
        /// </summary>
        /// <returns></returns>
        public Indicator.AtrOffsetDot ATROffsetDot(Data.IDataSeries input, Color downColor, int length, double multiplier, float radius, Color upColor)
        {
            return _indicator.ATROffsetDot(input, downColor, length, multiplier, radius, upColor);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Value Charts by David Stendhal; Converted to NinjaTrader by TradingStudies.com
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.AtrOffsetDot ATROffsetDot(Color downColor, int length, double multiplier, float radius, Color upColor)
        {
            return _indicator.ATROffsetDot(Input, downColor, length, multiplier, radius, upColor);
        }

        /// <summary>
        /// Value Charts by David Stendhal; Converted to NinjaTrader by TradingStudies.com
        /// </summary>
        /// <returns></returns>
        public Indicator.AtrOffsetDot ATROffsetDot(Data.IDataSeries input, Color downColor, int length, double multiplier, float radius, Color upColor)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ATROffsetDot(input, downColor, length, multiplier, radius, upColor);
        }
    }
}
#endregion
