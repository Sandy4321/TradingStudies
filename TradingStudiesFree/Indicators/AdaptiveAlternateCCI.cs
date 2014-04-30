// TradingStudies.com
// info@tradingStudies.com

using System;
using System.ComponentModel;
using System.Drawing;
using System.Xml.Serialization;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;

namespace NinjaTrader.Indicator
{
	[Description("Adaptive Alternate CCI")]
	[Gui.Design.DisplayName("Adaptive Alternate CCI")]
	public class AdaptiveAlternativeCci : Indicator
	{
		private		DataSeries		adaptCci;
		private		double			cycPart				= 1;
		private		DataSeries		detrender;
		private		DataSeries		i1;
		private		DataSeries		i2;
		private		DataSeries		im;
		private		int				numBars				= 1;
		private		DataSeries		period;
		private		int				periodMultiplier	= 1;
		private		DataSeries		q1;
		private		DataSeries		q2;
		private		DataSeries		re;
		private		int				smaSmooth			= 1;
		private		DataSeries		smooth;
		private		DataSeries		smoothPeriod;
		private		DataSeries		vMedianPrice;

		protected override void Initialize()
		{
			Add(new Plot(Color.Black, "CCIPlot"));

			detrender			= new DataSeries(this);
			period				= new DataSeries(this);
			smooth				= new DataSeries(this);
			i1					= new DataSeries(this);
			i2					= new DataSeries(this);
			im					= new DataSeries(this);
			q1					= new DataSeries(this);
			q2					= new DataSeries(this);
			re					= new DataSeries(this);
			smoothPeriod		= new DataSeries(this);
			vMedianPrice		= new DataSeries(this);
			Overlay				= false;
			PriceType			= PriceType.Median; // (H+L)/2
			PriceTypeSupported	= true;
			adaptCci			= new DataSeries(this);
			DisplayInDataBox	= false;
			CalculateOnBarClose	= false;
			PaintPriceMarkers	= false;
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar < Math.Max(200, numBars))
				Value.Set(0);
			else
			{
				smooth.Set((4 * Input[0] + 3 * Input[1] + 2 * Input[2] + Input[3]) * 0.1);
				detrender.Set((0.0962 * smooth[0] + 0.5769 * smooth[2] - 0.5769 * smooth[4] - 0.0962 * smooth[6]) * (0.075 * period[1] + 0.54));

				// Compute InPhase and Quadrature components
				q1.Set((0.0962 * detrender[0] + 0.5769 * detrender[2] - 0.5769 * detrender[4] - 0.0962 * detrender[6]) * (0.075 * period[1] + 0.54));

				i1.Set(detrender[3]);

				// Advance the phase of _i1 and _q1 by 90}
				double jI = (0.0962 * i1[0] + 0.5769 * i1[2] - 0.5769 * i1[4] - 0.0962 * i1[6]) * (0.075 * period[1] + 0.54);
				double jQ = (0.0962 * q1[0] + 0.5769 * q1[2] - 0.5769 * q1[4] - 0.0962 * q1[6]) * (0.075 * period[1] + 0.54);

				// Phasor addition for 3 bar averaging}
				i2.Set(i1[0] - jQ);
				q2.Set(q1[0] + jI);

				// Smooth the I and Q components before applying the discriminator
				i2.Set(0.2 * i2[0] + 0.8 * i2[1]);
				q2.Set(0.2 * q2[0] + 0.8 * q2[1]);

				// Homodyne Discriminator
				re.Set(i2[0] * i2[1] + q2[0] * q2[1]);
				im.Set(i2[0] * q2[1] - q2[0] * i2[1]);

				re.Set(0.2 * re[0] + 0.8 * re[1]);
				im.Set(0.2 * im[0] + 0.8 * im[1]);

				if (Math.Abs(im[0]) > double.Epsilon && Math.Abs(re[0]) > double.Epsilon)
					period.Set(360 / ((180 / Math.PI) * Math.Atan(im[0] / re[0])));
				if (period[0] > 1.5 * period[1])
					period.Set(1.5 * period[1]);
				if (period[0] < 0.67 * period[1])
					period.Set(0.67 * period[1]);
				if (period[0] < 6)
					period.Set(6);
				if (period[0] > 200)
					period.Set(200);

				period.Set(0.2 * period[0] + 0.8 * period[1]);

				smoothPeriod.Set(0.33 * period[0] + 0.67 * smoothPeriod[1]);

				int length = periodMultiplier * (int)(cycPart * period[0]);
				double high = High[0];
				for (int i = 0; i < numBars; i++)
					if (High[i] > high)
						high = High[i];
				double low = Low[0];
				for (int i = 0; i < numBars; i++)
					if (Low[i] < low)
						low = Low[i];
				vMedianPrice.Set((high + low + Close[0]) / 3);
				double avg = 0;
				for (int count = 0; count < length; count++)
					avg = avg + vMedianPrice[count];

				avg = avg / length;
				double md = 0;
				for (int count = 0; count < length; count++)
					md = md + Math.Abs(vMedianPrice[count] - avg);

				md = md / length;

				if (Math.Abs(md) > double.Epsilon)
				{
					adaptCci.Set((vMedianPrice[0] - avg) / (0.015 * md));
					CciPlot.Set(SMA(adaptCci, smaSmooth)[0]);
				}
			}
		}

		#region Properties

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries CciPlot
		{
			get { return Values[0]; }
		}

		[Description("Use to adjust the lag")]
		[GridCategory("Parameters")]
		public double CycPart
		{
			get { return cycPart; }
			set { cycPart = value; }
		}

		[Description("Numbers of bars used in CCI Typical Price Formula")]
		[GridCategory("Parameters")]
		public int NumBars
		{
			get { return numBars; }
			set { numBars = value; }
		}

		[Description("Use to adjust SMA smoothing")]
		[GridCategory("Parameters")]
		public int Smooth
		{
			get { return smaSmooth; }
			set { smaSmooth = value; }
		}

		[Description("Use to multiply period")]
		[GridCategory("Parameters")]
		public int PeriodMultiplier
		{
			get { return periodMultiplier; }
			set { periodMultiplier = value; }
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
        private AdaptiveAlternativeCci[] cacheAdaptiveAlternativeCci = null;

        private static AdaptiveAlternativeCci checkAdaptiveAlternativeCci = new AdaptiveAlternativeCci();

        /// <summary>
        /// Adaptive Alternate CCI
        /// </summary>
        /// <returns></returns>
        public AdaptiveAlternativeCci AdaptiveAlternativeCci(double cycPart, int numBars, int periodMultiplier, int smooth)
        {
            return AdaptiveAlternativeCci(Input, cycPart, numBars, periodMultiplier, smooth);
        }

        /// <summary>
        /// Adaptive Alternate CCI
        /// </summary>
        /// <returns></returns>
        public AdaptiveAlternativeCci AdaptiveAlternativeCci(Data.IDataSeries input, double cycPart, int numBars, int periodMultiplier, int smooth)
        {
            if (cacheAdaptiveAlternativeCci != null)
                for (int idx = 0; idx < cacheAdaptiveAlternativeCci.Length; idx++)
                    if (Math.Abs(cacheAdaptiveAlternativeCci[idx].CycPart - cycPart) <= double.Epsilon && cacheAdaptiveAlternativeCci[idx].NumBars == numBars && cacheAdaptiveAlternativeCci[idx].PeriodMultiplier == periodMultiplier && cacheAdaptiveAlternativeCci[idx].Smooth == smooth && cacheAdaptiveAlternativeCci[idx].EqualsInput(input))
                        return cacheAdaptiveAlternativeCci[idx];

            lock (checkAdaptiveAlternativeCci)
            {
                checkAdaptiveAlternativeCci.CycPart = cycPart;
                cycPart = checkAdaptiveAlternativeCci.CycPart;
                checkAdaptiveAlternativeCci.NumBars = numBars;
                numBars = checkAdaptiveAlternativeCci.NumBars;
                checkAdaptiveAlternativeCci.PeriodMultiplier = periodMultiplier;
                periodMultiplier = checkAdaptiveAlternativeCci.PeriodMultiplier;
                checkAdaptiveAlternativeCci.Smooth = smooth;
                smooth = checkAdaptiveAlternativeCci.Smooth;

                if (cacheAdaptiveAlternativeCci != null)
                    for (int idx = 0; idx < cacheAdaptiveAlternativeCci.Length; idx++)
                        if (Math.Abs(cacheAdaptiveAlternativeCci[idx].CycPart - cycPart) <= double.Epsilon && cacheAdaptiveAlternativeCci[idx].NumBars == numBars && cacheAdaptiveAlternativeCci[idx].PeriodMultiplier == periodMultiplier && cacheAdaptiveAlternativeCci[idx].Smooth == smooth && cacheAdaptiveAlternativeCci[idx].EqualsInput(input))
                            return cacheAdaptiveAlternativeCci[idx];

                AdaptiveAlternativeCci indicator = new AdaptiveAlternativeCci();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.CycPart = cycPart;
                indicator.NumBars = numBars;
                indicator.PeriodMultiplier = periodMultiplier;
                indicator.Smooth = smooth;
                Indicators.Add(indicator);
                indicator.SetUp();

                AdaptiveAlternativeCci[] tmp = new AdaptiveAlternativeCci[cacheAdaptiveAlternativeCci == null ? 1 : cacheAdaptiveAlternativeCci.Length + 1];
                if (cacheAdaptiveAlternativeCci != null)
                    cacheAdaptiveAlternativeCci.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheAdaptiveAlternativeCci = tmp;
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
        /// Adaptive Alternate CCI
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.AdaptiveAlternativeCci AdaptiveAlternativeCci(double cycPart, int numBars, int periodMultiplier, int smooth)
        {
            return _indicator.AdaptiveAlternativeCci(Input, cycPart, numBars, periodMultiplier, smooth);
        }

        /// <summary>
        /// Adaptive Alternate CCI
        /// </summary>
        /// <returns></returns>
        public Indicator.AdaptiveAlternativeCci AdaptiveAlternativeCci(Data.IDataSeries input, double cycPart, int numBars, int periodMultiplier, int smooth)
        {
            return _indicator.AdaptiveAlternativeCci(input, cycPart, numBars, periodMultiplier, smooth);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Adaptive Alternate CCI
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.AdaptiveAlternativeCci AdaptiveAlternativeCci(double cycPart, int numBars, int periodMultiplier, int smooth)
        {
            return _indicator.AdaptiveAlternativeCci(Input, cycPart, numBars, periodMultiplier, smooth);
        }

        /// <summary>
        /// Adaptive Alternate CCI
        /// </summary>
        /// <returns></returns>
        public Indicator.AdaptiveAlternativeCci AdaptiveAlternativeCci(Data.IDataSeries input, double cycPart, int numBars, int periodMultiplier, int smooth)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.AdaptiveAlternativeCci(input, cycPart, numBars, periodMultiplier, smooth);
        }
    }
}
#endregion
