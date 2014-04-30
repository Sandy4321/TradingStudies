using System;
using System.ComponentModel;
using System.Drawing;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;

namespace NinjaTrader.Indicator
{
	[Description("ADXVMA")]
	[Gui.Design.DisplayName("ADXVMA")]
// ReSharper disable once InconsistentNaming
	public class ADXVMA : Indicator
	{
		private		DataSeries	@out;
		private		int			adxPeriod	= 6;
		private		double		chandeEma;
		private		double		hhv			= double.MinValue;
		private		double		llv			= double.MaxValue;
		private		DataSeries	mdi;
		private		DataSeries	mdm;
		private		DataSeries	pdi;
		private		DataSeries	pdm;
		private		double		weightDi;
		private		double		weightDm;
		private		double		weightDx;

		protected override void Initialize()
		{
			Add(new Plot(Color.FromKnownColor(KnownColor.Lime), PlotStyle.Line, "ADXVMAPlot"));
			Overlay				= true;
			PriceTypeSupported	= false;
			pdi					= new DataSeries(this);
			pdm					= new DataSeries(this);
			mdm					= new DataSeries(this);
			mdi					= new DataSeries(this);
			@out				= new DataSeries(this);
			weightDx			= ADXPeriod;
			weightDm			= ADXPeriod;
			weightDi			= ADXPeriod;
			chandeEma			= ADXPeriod;
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar < 2)
			{
				Value.Set(0);
				pdm.Set(0);
				mdm.Set(0);
				pdi.Set(0);
				mdi.Set(0);
				@out.Set(0);
				return;
			}
			try
			{
				const int i = 0;
				pdm.Set(0);
				mdm.Set(0);
				if (Close[i] > Close[i + 1])
					pdm.Set(Close[i] - Close[i + 1]); //This array is not displayed.
				else
					mdm.Set(Close[i + 1] - Close[i]); //This array is not displayed.

				pdm.Set(((weightDm - 1) * pdm[i + 1] + pdm[i]) / weightDm); //ema.
				mdm.Set(((weightDm - 1) * mdm[i + 1] + mdm[i]) / weightDm); //ema.

				double tr = pdm[i] + mdm[i];

				if (tr > 0)
				{
					pdi.Set(pdm[i]/tr);
					mdi.Set(mdm[i]/tr);
				} //Avoid division by zero. Minimum step size is one unnormalized price pip.
				else
				{
					pdi.Set(0);
					mdi.Set(0);
				}

				pdi.Set(((weightDi - 1) * pdi[i + 1] + pdi[i]) / weightDi); //ema.
				mdi.Set(((weightDi - 1) * mdi[i + 1] + mdi[i]) / weightDi); //ema.

				double diDiff = pdi[i] - mdi[i];
				if (diDiff < 0)
					diDiff = -diDiff; //Only positive momentum signals are used.
				double diSum = pdi[i] + mdi[i];
				if (diSum > 0)
					@out.Set(diDiff/diSum); //Factional, near zero when PDM==MDM (horizonal), near 1 for laddering.
				else
					@out.Set(0);

				@out.Set(((weightDx - 1)*@out[i + 1] + @out[i])/weightDx);

				if (@out[i] > @out[i + 1])
				{
					hhv = @out[i];
					llv = @out[i + 1];
				}
				else
				{
					hhv = @out[i + 1];
					llv = @out[i];
				}

				for (int j = 1; j < Math.Min(ADXPeriod, CurrentBar); j++)
				{
					if (@out[i + j + 1] > hhv) hhv = @out[i + j + 1];
					if (@out[i + j + 1] < llv) llv = @out[i + j + 1];
				}


				double diff	= hhv - llv; //Veriable reference scale, adapts to recent activity level, unnormalized.
				double vi	= 0; //Zero case. This fixes the output at its historical level. 
				if (diff > 0)
					vi = (@out[i] - llv)/diff; //Normalized, 0-1 scale.

				double val = ((chandeEma - vi)*Value[i + 1] + vi*Close[i])/chandeEma;

				Value.Set(val); //Chande VMA formula with ema built in.
			}
			catch (Exception ex)
			{
				Print(ex.ToString());
			}
		}

		#region Properties

		[Description("ADX Period")]
		[GridCategory("Parameters")]
// ReSharper disable once InconsistentNaming
		public int ADXPeriod
		{
			get { return adxPeriod; }
			set { adxPeriod = Math.Max(1, value); }
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
        private ADXVMA[] cacheADXVMA = null;

        private static ADXVMA checkADXVMA = new ADXVMA();

        /// <summary>
        /// ADXVMA
        /// </summary>
        /// <returns></returns>
        public ADXVMA ADXVMA(int aDXPeriod)
        {
            return ADXVMA(Input, aDXPeriod);
        }

        /// <summary>
        /// ADXVMA
        /// </summary>
        /// <returns></returns>
        public ADXVMA ADXVMA(Data.IDataSeries input, int aDXPeriod)
        {
            if (cacheADXVMA != null)
                for (int idx = 0; idx < cacheADXVMA.Length; idx++)
                    if (cacheADXVMA[idx].ADXPeriod == aDXPeriod && cacheADXVMA[idx].EqualsInput(input))
                        return cacheADXVMA[idx];

            lock (checkADXVMA)
            {
                checkADXVMA.ADXPeriod = aDXPeriod;
                aDXPeriod = checkADXVMA.ADXPeriod;

                if (cacheADXVMA != null)
                    for (int idx = 0; idx < cacheADXVMA.Length; idx++)
                        if (cacheADXVMA[idx].ADXPeriod == aDXPeriod && cacheADXVMA[idx].EqualsInput(input))
                            return cacheADXVMA[idx];

                ADXVMA indicator = new ADXVMA();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.ADXPeriod = aDXPeriod;
                Indicators.Add(indicator);
                indicator.SetUp();

                ADXVMA[] tmp = new ADXVMA[cacheADXVMA == null ? 1 : cacheADXVMA.Length + 1];
                if (cacheADXVMA != null)
                    cacheADXVMA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheADXVMA = tmp;
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
        /// ADXVMA
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ADXVMA ADXVMA(int aDXPeriod)
        {
            return _indicator.ADXVMA(Input, aDXPeriod);
        }

        /// <summary>
        /// ADXVMA
        /// </summary>
        /// <returns></returns>
        public Indicator.ADXVMA ADXVMA(Data.IDataSeries input, int aDXPeriod)
        {
            return _indicator.ADXVMA(input, aDXPeriod);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// ADXVMA
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ADXVMA ADXVMA(int aDXPeriod)
        {
            return _indicator.ADXVMA(Input, aDXPeriod);
        }

        /// <summary>
        /// ADXVMA
        /// </summary>
        /// <returns></returns>
        public Indicator.ADXVMA ADXVMA(Data.IDataSeries input, int aDXPeriod)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ADXVMA(input, aDXPeriod);
        }
    }
}
#endregion
