using System;
using System.Drawing;
using System.ComponentModel;
using System.Xml.Serialization;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;

namespace NinjaTrader.Indicator
{
	/// <summary>
	/// Variable moving average
	/// Changed by Peter 090612  	Changed oder of first 4 variable so they match with 'OnBarUpdate' and 'Properties'
	/// 							Added TEMA and put 'PriceTypeSupported' to the value 'true' (if you change this too in DMA you can select Close, Open, Typical, etc. )
	/// </summary>
	[Description("Variable moving average")]
	[Gui.Design.DisplayName("MA Variable")]
	public class MAV : Indicator
	{
		private int _mAPeriod = 1; // Default setting for MAPeriod
		private MAType _mAType = MAType.HMA;
		private int _typeInt;

		public enum MAType			// A list of the valid types of moving averages that may be selected.
		{
			SMA = 1,					// Simple Moving Average.
			EMA = 2,					// Exponential Moving Average.
			WMA = 3,					// Weighted Moving Average.
			HMA = 4,					// Hull Moving Average.
			MAX = 5,					// Weighted Moving Average.
			MIN = 6,					// Weighted Moving Average.
			VMA = 7,
			Wilder = 8,
			EhlersFilter = 9,
			ADXVMA = 10,
			ZeroLagEMA = 11,
			TMA = 12,
			VWMA = 13,
			SMMA = 14,
			DynamicSR = 15,
			TEMA = 16,
			CMA = 17,
			RSX = 18,
			VWAP = 19,

			None = 20
		}


		protected override void Initialize()
		{
			Add(new Plot(Color.FromKnownColor(KnownColor.Green), PlotStyle.Line, "MA"));
			Overlay = true;
		}

		protected override void OnBarUpdate()
		{
			switch (_mAType)
			{
				case MAType.SMA:
					MA.Set(SMA(Input, MAPeriod)[0]);
					break;
				case MAType.EMA:
					MA.Set(EMA(Input, MAPeriod)[0]);
					break;
				case MAType.WMA:
					MA.Set(WMA(Input, MAPeriod)[0]);
					break;
				case MAType.HMA:
					MA.Set(HMA(Input, MAPeriod)[0]);
					break;
				case MAType.MAX:
					MA.Set(MAX(Input, MAPeriod)[0]);
					break;
				case MAType.MIN:
					MA.Set(MIN(Input, MAPeriod)[0]);
					break;
				case MAType.VMA:
					MA.Set(VMA(Input, MAPeriod / 2, MAPeriod)[0]);
					break;
				case MAType.Wilder:
					MA.Set(Wilder(Input, MAPeriod)[0]);
					break;
				case MAType.EhlersFilter:
					MA.Set(EhlersFilter(Input, MAPeriod)[0]);
					break;
				case MAType.ADXVMA:
					MA.Set(ADXVMA(Input, MAPeriod)[0]);
					break;
				case MAType.ZeroLagEMA:
					MA.Set(ZeroLagEMA(Input, MAPeriod)[0]);
					break;
				case MAType.TMA:
					MA.Set(TMA(Input, MAPeriod)[0]);
					break;
				case MAType.VWMA:
					MA.Set(VWMA(Input, MAPeriod)[0]);
					break;
				case MAType.SMMA:
					MA.Set(SMMA(Input, MAPeriod)[0]);
					break;
				case MAType.DynamicSR:
					MA.Set((DynamicSR(Input, MAPeriod).Support[0] + DynamicSR(Input, MAPeriod).Resistance[0]) * 0.5);
					break;
				case MAType.TEMA:
					MA.Set(TEMA(Input, MAPeriod)[0]);
					break;
				case MAType.CMA:
					MA.Set(CMA(Input, 0, MAPeriod)[0]);
					break;
				case MAType.RSX:
					MA.Set(RSX(Input, MAPeriod)[0]);
					break;
				default:
					MA.Set(Input[0]);
					break;
			}
		}
		#region Properties
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
		[XmlIgnore]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
		public DataSeries MA
		{
			get { return Values[0]; }
		}

		[Description("")]
		[GridCategory("Parameters")]
		public int MAPeriod
		{
			get { return _mAPeriod; }
			set { _mAPeriod = Math.Max(1, value); }

		}

		[Description("")]
		[GridCategory("Parameters")]
		public int TypeInt
		{
			get { return _typeInt; }
			set
			{
				//_typeInt = Math.Max( 1, Math.Min( 14, value ) );
				_typeInt = value;
				switch (_typeInt)
				{
					case (int)MAType.SMA:
						_mAType = MAType.SMA;
						break;
					case (int)MAType.EMA:
						_mAType = MAType.EMA;
						break;
					case (int)MAType.WMA:
						_mAType = MAType.WMA;
						break;
					case (int)MAType.HMA:
						_mAType = MAType.WMA;
						break;
					case (int)MAType.MAX:
						_mAType = MAType.MAX;
						break;
					case (int)MAType.MIN:
						_mAType = MAType.MIN;
						break;
					case (int)MAType.VMA:
						_mAType = MAType.VMA;
						break;
					case (int)MAType.Wilder:
						_mAType = MAType.Wilder;
						break;
					case (int)MAType.EhlersFilter:
						_mAType = MAType.EhlersFilter;
						break;
					case (int)MAType.ADXVMA:
						_mAType = MAType.ADXVMA;
						break;
					case (int)MAType.ZeroLagEMA:
						_mAType = MAType.ZeroLagEMA;
						break;
					case (int)MAType.TMA:
						_mAType = MAType.TMA;
						break;
					case (int)MAType.VWMA:
						_mAType = MAType.VWMA;
						break;
					case (int)MAType.SMMA:
						_mAType = MAType.SMMA;
						break;
					case (int)MAType.DynamicSR:
						_mAType = MAType.DynamicSR;
						break;
					case (int)MAType.TEMA:
						_mAType = MAType.TEMA;
						break;
					case (int)MAType.CMA:
						_mAType = MAType.CMA;
						break;
					case (int)MAType.RSX:
						_mAType = MAType.RSX;
						break;
					case (int)MAType.VWAP:
						_mAType = MAType.VWAP;
						break;
					default:
						_mAType = MAType.None;
						break;
				}
			}
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
		private MAV[] cacheMAV = null;

		private static MAV checkMAV = new MAV();

		/// <summary>
		/// Variable moving average
		/// </summary>
		/// <returns></returns>
		public MAV MAV(int mAPeriod, int typeInt)
		{
			return MAV(Input, mAPeriod, typeInt);
		}

		/// <summary>
		/// Variable moving average
		/// </summary>
		/// <returns></returns>
		public MAV MAV(Data.IDataSeries input, int mAPeriod, int typeInt)
		{
			if (cacheMAV != null)
				for (int idx = 0; idx < cacheMAV.Length; idx++)
					if (cacheMAV[idx].MAPeriod == mAPeriod && cacheMAV[idx].TypeInt == typeInt && cacheMAV[idx].EqualsInput(input))
						return cacheMAV[idx];

			lock (checkMAV)
			{
				checkMAV.MAPeriod = mAPeriod;
				mAPeriod = checkMAV.MAPeriod;
				checkMAV.TypeInt = typeInt;
				typeInt = checkMAV.TypeInt;

				if (cacheMAV != null)
					for (int idx = 0; idx < cacheMAV.Length; idx++)
						if (cacheMAV[idx].MAPeriod == mAPeriod && cacheMAV[idx].TypeInt == typeInt && cacheMAV[idx].EqualsInput(input))
							return cacheMAV[idx];

				MAV indicator = new MAV();
				indicator.BarsRequired = BarsRequired;
				indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
				indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
				indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
				indicator.Input = input;
				indicator.MAPeriod = mAPeriod;
				indicator.TypeInt = typeInt;
				Indicators.Add(indicator);
				indicator.SetUp();

				MAV[] tmp = new MAV[cacheMAV == null ? 1 : cacheMAV.Length + 1];
				if (cacheMAV != null)
					cacheMAV.CopyTo(tmp, 0);
				tmp[tmp.Length - 1] = indicator;
				cacheMAV = tmp;
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
		/// Variable moving average
		/// </summary>
		/// <returns></returns>
		[Gui.Design.WizardCondition("Indicator")]
		public Indicator.MAV MAV(int mAPeriod, int typeInt)
		{
			return _indicator.MAV(Input, mAPeriod, typeInt);
		}

		/// <summary>
		/// Variable moving average
		/// </summary>
		/// <returns></returns>
		public Indicator.MAV MAV(Data.IDataSeries input, int mAPeriod, int typeInt)
		{
			return _indicator.MAV(input, mAPeriod, typeInt);
		}
	}
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
	public partial class Strategy : StrategyBase
	{
		/// <summary>
		/// Variable moving average
		/// </summary>
		/// <returns></returns>
		[Gui.Design.WizardCondition("Indicator")]
		public Indicator.MAV MAV(int mAPeriod, int typeInt)
		{
			return _indicator.MAV(Input, mAPeriod, typeInt);
		}

		/// <summary>
		/// Variable moving average
		/// </summary>
		/// <returns></returns>
		public Indicator.MAV MAV(Data.IDataSeries input, int mAPeriod, int typeInt)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return _indicator.MAV(input, mAPeriod, typeInt);
		}
	}
}
#endregion
