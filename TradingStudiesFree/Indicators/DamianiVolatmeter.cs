using System;
using System.ComponentModel;
using System.Drawing;
using System.Xml.Serialization;
using DamianiUtilities;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;

namespace DamianiUtilities
{
// ReSharper disable InconsistentNaming
	public enum AverageType { SMA, EMA, WMA, HMA, T3, LinReg, VOLMA, ATR, StdDev, CCI, RSI, ROC };
// ReSharper restore InconsistentNaming
}

namespace NinjaTrader.Indicator
{
	/// <summary>
	///     Converted to NT by Roonius, with contributions by Elliott Wave.
	/// </summary>
	[Description("Used to filter whipsawed markets out. When noise line (blue) is above signal line (green) even trading ranges strategies will have poor performance - by Luis Guilherme Damiani.")]
	public class DamianiVolatmeter : Indicator
	{
		private		double			atrs;
		private		AverageType		averageType		= AverageType.WMA;
		private		double			k;
		private		bool			lagsupressor	= true;
		private		double			myatr;
		private		double			sd;
		private		int				sedAtr			= 40;
		private		int				sedStd			= 100;
		private		double			thresholdLevel	= 1.4;
		private		int				visAtr			= 13;
		private		int				visStd			= 20;

		protected override void Initialize()
		{
			Add(new Plot(new Pen(Color.Navy,		2),		PlotStyle.Line, "Noise"));
			Add(new Plot(new Pen(Color.Red,			4),		PlotStyle.Line, "DoNotTrade"));
			Add(new Plot(new Pen(Color.ForestGreen,	2),		PlotStyle.Line, "Trending"));

			PriceTypeSupported = false;
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar < Math.Max(SedAtr, SedStd))
			{
				Trending.Set(0);
				return;
			}

			myatr	= ATR(Close, VisAtr)[0];
			atrs	= Lagsupressor ? myatr / ATR(Close, SedAtr)[0] + 0.5 * (Trending[1] - Trending[3]) : myatr / ATR(Close, SedAtr)[0];
			sd		= MyStdDev(Typical, VisStd, averageType) / MyStdDev(Typical, SedStd, averageType);
			k		= ThresholdLevel - sd;

			if (k > atrs)
			{
				BackColor = Color.MistyRose;
				DoNotTrade.Set(0);
			}

			Trending.Set(atrs);

			if (k >= 0)
				Noise.Set(k);
		}

		public double MyStdDev(IDataSeries price, int p, AverageType at)
		{
			double avg;
			switch (at)
			{
				case AverageType.EMA	: avg = EMA(price, p)[0];			break;
				case AverageType.SMA	: avg = SMA(price, p)[0];			break;
				case AverageType.HMA	: avg = HMA(price, p)[0];			break;
				case AverageType.LinReg	: avg = LinReg(price, p)[0];		break;
				case AverageType.VOLMA	: avg = VOLMA(price, p)[0];			break;
				case AverageType.ATR	: avg = 1000 * ATR(price, p)[0];	break;
				case AverageType.StdDev	: avg = 1000 * StdDev(price, p)[0];	break;
				case AverageType.CCI	: avg = CCI(price, p)[0];			break;
				case AverageType.RSI	: avg = RSI(price, p, 3)[0];		break;
				case AverageType.ROC	: avg = ROC(price, p)[0];			break;
				default					: avg = WMA(price, p)[0];			break;
			}
			double sum = 0;
			for (int barsBack = Math.Min(CurrentBar, p - 1); barsBack >= 0; barsBack--)
				sum += (price[barsBack] - avg) * (price[barsBack] - avg);

			return (Math.Sqrt(sum / Math.Min(CurrentBar + 1, p)));
		}

		#region Properties

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries Noise
		{
			get { return Values[0]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries DoNotTrade
		{
			get { return Values[1]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries Trending
		{
			get { return Values[2]; }
		}

		[Description("")]
		[GridCategory("Parameters")]
		public int VisAtr
		{
			get { return visAtr; }
			set { visAtr = Math.Max(1, value); }
		}

		[Description("")]
		[GridCategory("Parameters")]
		public int VisStd
		{
			get { return visStd; }
			set { visStd = Math.Max(1, value); }
		}

		[Description("")]
		[GridCategory("Parameters")]
		public int SedAtr
		{
			get { return sedAtr; }
			set { sedAtr = Math.Max(1, value); }
		}

		[Description("")]
		[GridCategory("Parameters")]
		public int SedStd
		{
			get { return sedStd; }
			set { sedStd = Math.Max(1, value); }
		}

		[Description("")]
		[GridCategory("Parameters")]
		public double ThresholdLevel
		{
			get { return thresholdLevel; }
			set { thresholdLevel = value; }
		}

		[Description("")]
		[GridCategory("Parameters")]
		public bool Lagsupressor
		{
			get { return lagsupressor; }
			set { lagsupressor = value; }
		}

		[Description("Moving Average type to use (WMA, EMA, SMA, HMA, VOLMA, StdDev, ATR, LinReg, CCI, RSI, ROC")]
		[GridCategory("Parameters")]
		public AverageType AverageType
		{
			get { return averageType; }
			set { averageType = value; }
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
        private DamianiVolatmeter[] cacheDamianiVolatmeter = null;

        private static DamianiVolatmeter checkDamianiVolatmeter = new DamianiVolatmeter();

        /// <summary>
        /// Used to filter whipsawed markets out. When noise line (blue) is above signal line (green) even trading ranges strategies will have poor performance - by Luis Guilherme Damiani.
        /// </summary>
        /// <returns></returns>
        public DamianiVolatmeter DamianiVolatmeter(AverageType averageType, bool lagsupressor, int sedAtr, int sedStd, double thresholdLevel, int visAtr, int visStd)
        {
            return DamianiVolatmeter(Input, averageType, lagsupressor, sedAtr, sedStd, thresholdLevel, visAtr, visStd);
        }

        /// <summary>
        /// Used to filter whipsawed markets out. When noise line (blue) is above signal line (green) even trading ranges strategies will have poor performance - by Luis Guilherme Damiani.
        /// </summary>
        /// <returns></returns>
        public DamianiVolatmeter DamianiVolatmeter(Data.IDataSeries input, AverageType averageType, bool lagsupressor, int sedAtr, int sedStd, double thresholdLevel, int visAtr, int visStd)
        {
            if (cacheDamianiVolatmeter != null)
                for (int idx = 0; idx < cacheDamianiVolatmeter.Length; idx++)
                    if (cacheDamianiVolatmeter[idx].AverageType == averageType && cacheDamianiVolatmeter[idx].Lagsupressor == lagsupressor && cacheDamianiVolatmeter[idx].SedAtr == sedAtr && cacheDamianiVolatmeter[idx].SedStd == sedStd && Math.Abs(cacheDamianiVolatmeter[idx].ThresholdLevel - thresholdLevel) <= double.Epsilon && cacheDamianiVolatmeter[idx].VisAtr == visAtr && cacheDamianiVolatmeter[idx].VisStd == visStd && cacheDamianiVolatmeter[idx].EqualsInput(input))
                        return cacheDamianiVolatmeter[idx];

            lock (checkDamianiVolatmeter)
            {
                checkDamianiVolatmeter.AverageType = averageType;
                averageType = checkDamianiVolatmeter.AverageType;
                checkDamianiVolatmeter.Lagsupressor = lagsupressor;
                lagsupressor = checkDamianiVolatmeter.Lagsupressor;
                checkDamianiVolatmeter.SedAtr = sedAtr;
                sedAtr = checkDamianiVolatmeter.SedAtr;
                checkDamianiVolatmeter.SedStd = sedStd;
                sedStd = checkDamianiVolatmeter.SedStd;
                checkDamianiVolatmeter.ThresholdLevel = thresholdLevel;
                thresholdLevel = checkDamianiVolatmeter.ThresholdLevel;
                checkDamianiVolatmeter.VisAtr = visAtr;
                visAtr = checkDamianiVolatmeter.VisAtr;
                checkDamianiVolatmeter.VisStd = visStd;
                visStd = checkDamianiVolatmeter.VisStd;

                if (cacheDamianiVolatmeter != null)
                    for (int idx = 0; idx < cacheDamianiVolatmeter.Length; idx++)
                        if (cacheDamianiVolatmeter[idx].AverageType == averageType && cacheDamianiVolatmeter[idx].Lagsupressor == lagsupressor && cacheDamianiVolatmeter[idx].SedAtr == sedAtr && cacheDamianiVolatmeter[idx].SedStd == sedStd && Math.Abs(cacheDamianiVolatmeter[idx].ThresholdLevel - thresholdLevel) <= double.Epsilon && cacheDamianiVolatmeter[idx].VisAtr == visAtr && cacheDamianiVolatmeter[idx].VisStd == visStd && cacheDamianiVolatmeter[idx].EqualsInput(input))
                            return cacheDamianiVolatmeter[idx];

                DamianiVolatmeter indicator = new DamianiVolatmeter();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.AverageType = averageType;
                indicator.Lagsupressor = lagsupressor;
                indicator.SedAtr = sedAtr;
                indicator.SedStd = sedStd;
                indicator.ThresholdLevel = thresholdLevel;
                indicator.VisAtr = visAtr;
                indicator.VisStd = visStd;
                Indicators.Add(indicator);
                indicator.SetUp();

                DamianiVolatmeter[] tmp = new DamianiVolatmeter[cacheDamianiVolatmeter == null ? 1 : cacheDamianiVolatmeter.Length + 1];
                if (cacheDamianiVolatmeter != null)
                    cacheDamianiVolatmeter.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheDamianiVolatmeter = tmp;
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
        /// Used to filter whipsawed markets out. When noise line (blue) is above signal line (green) even trading ranges strategies will have poor performance - by Luis Guilherme Damiani.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.DamianiVolatmeter DamianiVolatmeter(AverageType averageType, bool lagsupressor, int sedAtr, int sedStd, double thresholdLevel, int visAtr, int visStd)
        {
            return _indicator.DamianiVolatmeter(Input, averageType, lagsupressor, sedAtr, sedStd, thresholdLevel, visAtr, visStd);
        }

        /// <summary>
        /// Used to filter whipsawed markets out. When noise line (blue) is above signal line (green) even trading ranges strategies will have poor performance - by Luis Guilherme Damiani.
        /// </summary>
        /// <returns></returns>
        public Indicator.DamianiVolatmeter DamianiVolatmeter(Data.IDataSeries input, AverageType averageType, bool lagsupressor, int sedAtr, int sedStd, double thresholdLevel, int visAtr, int visStd)
        {
            return _indicator.DamianiVolatmeter(input, averageType, lagsupressor, sedAtr, sedStd, thresholdLevel, visAtr, visStd);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Used to filter whipsawed markets out. When noise line (blue) is above signal line (green) even trading ranges strategies will have poor performance - by Luis Guilherme Damiani.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.DamianiVolatmeter DamianiVolatmeter(AverageType averageType, bool lagsupressor, int sedAtr, int sedStd, double thresholdLevel, int visAtr, int visStd)
        {
            return _indicator.DamianiVolatmeter(Input, averageType, lagsupressor, sedAtr, sedStd, thresholdLevel, visAtr, visStd);
        }

        /// <summary>
        /// Used to filter whipsawed markets out. When noise line (blue) is above signal line (green) even trading ranges strategies will have poor performance - by Luis Guilherme Damiani.
        /// </summary>
        /// <returns></returns>
        public Indicator.DamianiVolatmeter DamianiVolatmeter(Data.IDataSeries input, AverageType averageType, bool lagsupressor, int sedAtr, int sedStd, double thresholdLevel, int visAtr, int visStd)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.DamianiVolatmeter(input, averageType, lagsupressor, sedAtr, sedStd, thresholdLevel, visAtr, visStd);
        }
    }
}
#endregion
