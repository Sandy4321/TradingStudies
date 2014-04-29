using System;
using System.ComponentModel;
using System.Drawing;
using System.Xml.Serialization;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;

namespace NinjaTrader.Indicator
{
	[Description("")]
	public class HilbertTransform : Indicator
	{
		private DataSeries	detrender;
		private DataSeries	i1;
		private DataSeries	i2;
		private DataSeries	im;
		private DataSeries	jI;
		private DataSeries	jQ;
		private DataSeries	period;
		private DataSeries	q1;
		private DataSeries	q2;
		private DataSeries	re;
		private DataSeries	smooth;
		private DataSeries	smoothPeriod;
		private int			wMaPeriods = 10;

		protected override void Initialize()
		{
			Add(new Plot(Color.Orange, PlotStyle.Line, "InPhase"));
			Add(new Plot(Color.Green,  PlotStyle.Line, "Quadrature"));
			Add(new Line(Color.Gray, 0, "ZeroLine"));
			Overlay				= false;
			smooth				= new DataSeries(this);
			detrender			= new DataSeries(this);
			i1					= new DataSeries(this);
			q1					= new DataSeries(this);
			jI					= new DataSeries(this);
			jQ					= new DataSeries(this);
			i2					= new DataSeries(this);
			q2					= new DataSeries(this);
			re					= new DataSeries(this);
			im					= new DataSeries(this);
			period				= new DataSeries(this);
			smoothPeriod		= new DataSeries(this);
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar < 50) return;

			smooth   .Set((4 * Median[0] + 3 * Median[1] + 2 * Median[2] + Median[3]) / 10);
			detrender.Set((0.0962 * smooth[0] + 0.5769 * smooth[2] - 0.5769 * smooth[4] - 0.0962 * smooth[6]) * (0.075 * period[1] + .54));

			//InPhase and Quadrature components			
			q1.Set((0.0962 * detrender[0] + 0.5769 * detrender[2] - 0.5769 * detrender[4] - 0.0962 * detrender[6]) * (0.075 * period[1] + 0.54));
			i1.Set(detrender[3]);

			//Advance the phase of I1 and Q1 by 90 degrees
			jI.Set((0.0962 * i1[0] + 0.5769 * i1[2] - 0.5769 * i1[4] - 0.0962 * i1[6]) * (0.075 * period[1] + .54));
			jQ.Set((0.0962 * q1[0] + 0.5769 * q1[2] - 0.5769 * q1[4] - 0.0962 * q1[6]) * (0.075 * period[1] + .54));

			//Phasor Addition
			i2.Set(i1[0] - jQ[0]);
			q2.Set(q1[0] + jI[0]);

			//Smooth the I and Q components before applying the discriminator
			i2.Set(0.2 * i2[0] + 0.8 * i2[1]);
			q2.Set(0.2 * q2[0] + 0.8 * q2[1]);

			//Homodyne Discriminator
			re.Set(i2[0] * i2[1] + q2[0] * q2[1]);
			im.Set(i2[0] * q2[1] - q2[0] * i2[1]);
			re.Set(0.2 * re[0] + 0.8 * re[1]);
			im.Set(0.2 * im[0] + 0.8 * im[1]);

			double rad2Deg = 180.0 / (4.0 * Math.Atan(1));

			if (Math.Abs(im[0]) > double.Epsilon && Math.Abs(re[0]) > double.Epsilon)
				period.Set(360 / (Math.Atan(im[0] / re[0]) * rad2Deg));
			if (period[0] > (1.5 * period[1]))
				period.Set(1.5 * period[1]);
			if (period[0] < (0.67 * period[1]))
				period.Set(0.67 * period[1]);
			if (period[0] < 6)
				period.Set(6);
			if (period[0] > 50)
				period.Set(50);

			period		.Set(0.2 * period[0] + 0.8 * period[1]);
			smoothPeriod.Set(0.33 * period[0] + 0.67 * smoothPeriod[1]);
			InPhase		.Set(i1[0]);
			Quadrature	.Set(q1[0]);
		}

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries InPhase
		{
			get { return Values[0]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries Quadrature
		{
			get { return Values[1]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries CycleSmoothPeriod
		{
			get { return smoothPeriod; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries CyclePeriod
		{
			get { return period; }
		}

		[Description("")]
		[GridCategory("Parameters")]
		public int WmaPeriods
		{
			get { return wMaPeriods; }
			set { wMaPeriods = Math.Max(3, value); }
		}
	}
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private HilbertTransform[] cacheHilbertTransform = null;

        private static HilbertTransform checkHilbertTransform = new HilbertTransform();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public HilbertTransform HilbertTransform(int wMAPeriods)
        {
            return HilbertTransform(Input, wMAPeriods);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public HilbertTransform HilbertTransform(Data.IDataSeries input, int wMAPeriods)
        {
            if (cacheHilbertTransform != null)
                for (int idx = 0; idx < cacheHilbertTransform.Length; idx++)
                    if (cacheHilbertTransform[idx].WmaPeriods == wMAPeriods && cacheHilbertTransform[idx].EqualsInput(input))
                        return cacheHilbertTransform[idx];

            lock (checkHilbertTransform)
            {
                checkHilbertTransform.WmaPeriods = wMAPeriods;
                wMAPeriods = checkHilbertTransform.WmaPeriods;

                if (cacheHilbertTransform != null)
                    for (int idx = 0; idx < cacheHilbertTransform.Length; idx++)
                        if (cacheHilbertTransform[idx].WmaPeriods == wMAPeriods && cacheHilbertTransform[idx].EqualsInput(input))
                            return cacheHilbertTransform[idx];

                HilbertTransform indicator = new HilbertTransform();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.WmaPeriods = wMAPeriods;
                Indicators.Add(indicator);
                indicator.SetUp();

                HilbertTransform[] tmp = new HilbertTransform[cacheHilbertTransform == null ? 1 : cacheHilbertTransform.Length + 1];
                if (cacheHilbertTransform != null)
                    cacheHilbertTransform.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheHilbertTransform = tmp;
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
        public Indicator.HilbertTransform HilbertTransform(int wMAPeriods)
        {
            return _indicator.HilbertTransform(Input, wMAPeriods);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Indicator.HilbertTransform HilbertTransform(Data.IDataSeries input, int wMAPeriods)
        {
            return _indicator.HilbertTransform(input, wMAPeriods);
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
        public Indicator.HilbertTransform HilbertTransform(int wMAPeriods)
        {
            return _indicator.HilbertTransform(Input, wMAPeriods);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Indicator.HilbertTransform HilbertTransform(Data.IDataSeries input, int wMAPeriods)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.HilbertTransform(input, wMAPeriods);
        }
    }
}
#endregion
