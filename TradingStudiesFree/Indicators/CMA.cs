// TradingStudies.com
// info@tradingStudies.com

using System;
using System.ComponentModel;
using System.Drawing;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;

namespace NinjaTrader.Indicator
{
	/// <summary>
	///     Cool Moving Average
	/// </summary>
	[Description("Cool Moving Average")]
// ReSharper disable once InconsistentNaming
	public class CMA : Indicator
	{
		private DataSeries a8;
		private double absValue;
		private double[] buffer;
		private DataSeries c0;
		private DataSeries c8;
		private int counterA;
		private int counterB;
		private double cycleDelta;
		private int cycleLimit;
		private double dValue;
		private double highDValue;
		private int highLimit;
		private bool initFlag;
		private double jmaValue;
		private DataSeries jmaValueBuffer;
		private double lengthDivider;
		private int limitValue;
		private double[] list;
		private double logParam;
		private int loopCriteria;
		private int loopParam;
		private double lowDValue;
		private double offset;
		private double paramA;
		private double paramB;
		private double phaseParam;
		private double[] ring1;
		private double[] ring2;
		private int s38;
		private int s40;
		private int s58;
		private int s60;
		private int s68;
		private double sValue;
		private double series;
		private int smoothing = 14;
		private double sqrtParam;
		private int startValue;

		protected override void Initialize()
		{
			Add(new Plot(new Pen(Color.Orchid, 2), PlotStyle.Line, "CMA"));
			Overlay = true;
			PriceTypeSupported = true;
			list = new double[128];
			ring1 = new double[128];
			ring2 = new double[11];
			buffer = new double[62];

			jmaValueBuffer = new DataSeries(this);
			c0 = new DataSeries(this);
			a8 = new DataSeries(this);
			c8 = new DataSeries(this);

			limitValue = 63;
			startValue = 64;
			loopParam = loopCriteria = 0;

			for (int i = 0; i <= limitValue; i++)
				list[i] = -1000000;
			for (int i = startValue; i <= 127; i++)
				list[i] = 1000000;

			initFlag = true;
			double lengthParam = (Smoothing < 1.0000000002) ? 0.0000000001 : (Smoothing - 1)/2.0;

			phaseParam = Offset < -100 ? 0.5 : (Offset > 100 ? 2.5 : Offset/100.0 + 1.5);

			logParam = Math.Log(Math.Sqrt(lengthParam))/Math.Log(2.0);
			logParam = logParam + 2.0 < 0 ? 0 : logParam + 2.0;

			sqrtParam = Math.Sqrt(lengthParam)*logParam;
			lengthParam *= 0.9;
			lengthDivider = lengthParam/(lengthParam + 2.0);
		}

		protected override void OnBarUpdate()
		{
			double jmaTempValue = (CurrentBar > 0) ? jmaValueBuffer[1] : Input[0];

			series = Input[0];
			if (loopParam < 61)
			{
				loopParam++;
				buffer[loopParam] = series;
			}
			if (loopParam > 30)
			{
				int i;
				if (initFlag)
				{
					initFlag = false;
					int diffFlag = 0;
					for (i = 1; i <= 29; i++)
						if (Math.Abs(buffer[i + 1] - buffer[i]) > 0.00000000001)
							diffFlag = 1;
					highLimit = diffFlag*30;
					paramB = (highLimit == 0) ? series : buffer[1];
					paramA = paramB;
					if (highLimit > 29)
						highLimit = 29;
				}
				else
					highLimit = 0;
				//---- big cycle
				for (i = highLimit; i >= 0; i--)
				{
					sValue = (i == 0) ? series : buffer[31 - i];
					absValue = (Math.Abs(sValue - paramA) > Math.Abs(sValue - paramB))
						? Math.Abs(sValue - paramA)
						: Math.Abs(sValue - paramB);
					dValue = absValue + 0.0000000001; //1.0e-10;
					counterA = counterA <= 1 ? 127 : counterA - 1; //starts at 127
					counterB = counterB <= 1 ? 10 : counterB - 1; //starts at 10
					if (cycleLimit < 128)
						cycleLimit++;
					cycleDelta += (dValue - ring2[counterB]);
					ring2[counterB] = dValue;
					highDValue = (cycleLimit > 10) ? cycleDelta/10.0 : cycleDelta/cycleLimit;

					if (cycleLimit > 127)
					{
						dValue = ring1[counterA];
						ring1[counterA] = highDValue;
						s68 = 64;
						s58 = s68;
						while (s68 > 1)
							if (list[s58] < dValue)
							{
								s68 /= 2;
								s58 += s68;
							}
							else if (list[s58] <= dValue)
								s68 = 1;
							else
							{
								s68 /= 2;
								s58 -= s68;
							}
					}
					else
					{
						ring1[counterA] = highDValue;
						if (limitValue + startValue > 127)
						{
							startValue--;
							s58 = startValue;
						}
						else
						{
							limitValue++;
							s58 = limitValue;
						}
						s38 = limitValue > 96 ? 96 : limitValue;
						s40 = startValue < 32 ? 32 : startValue;
					}
					//----
					s68 = 64;
					s60 = s68;
					while (s68 > 1)
					{
						if (list[s60] >= highDValue)
							if (list[s60 - 1] <= highDValue)
								s68 = 1;
							else
							{
								s68 /= 2;
								s60 -= s68;
							}
						else
						{
							s68 /= 2;
							s60 += s68;
						}
						if (s60 == 127 && highDValue > list[127])
							s60 = 128;
					}
					if (cycleLimit > 127)
					{
						if (s58 >= s60)
						{
							if (s38 + 1 > s60 && s40 - 1 < s60)
								lowDValue += highDValue;
							else if (s40 > s60 && s40 - 1 < s58)
								lowDValue += list[s40 - 1];
						}
						else if (s40 >= s60)
						{
							if (s38 + 1 < s60 && s38 + 1 > s58)
								lowDValue += list[s38 + 1];
						}
						else if (s38 + 2 > s60)
							lowDValue += highDValue;
						else if (s38 + 1 < s60 && s38 + 1 > s58)
							lowDValue += list[s38 + 1];

						if (s58 > s60)
						{
							if (s40 - 1 < s58 && s38 + 1 > s58)
								lowDValue -= list[s58];
							else if (s38 < s58 && s38 + 1 > s60)
								lowDValue -= list[s38];
						}
						else if (s38 + 1 > s58 && s40 - 1 < s58)
							lowDValue -= list[s58];
						else if (s40 > s58 && s40 < s60)
							lowDValue -= list[s40];
					}
					int j;
					if (s58 <= s60)
						if (s58 >= s60)
							list[s60] = highDValue;
						else
						{
							for (j = s58 + 1; j <= (s60 - 1); j++)
								list[j - 1] = list[j];
							list[s60 - 1] = highDValue;
						}
					else
					{
						for (j = s58 - 1; j >= s60; j--)
							list[j + 1] = list[j];
						list[s60] = highDValue;
					}

					if (cycleLimit <= 127)
					{
						lowDValue = 0;
						for (j = s40; j <= s38; j++)
							lowDValue += list[j];
					}
					//----
					if (CurrentBar == 0)
					{
						c0.Set(series);
						jmaTempValue = series;
						int leftInt = (Math.Ceiling(sqrtParam) >= 1) ? (int) Math.Ceiling(sqrtParam) : 1;
						int rightPart = (Math.Floor(sqrtParam) >= 1) ? (int) Math.Floor(sqrtParam) : 1;
						dValue = (leftInt == rightPart)
							? 1.0
							: (sqrtParam - rightPart)/(leftInt - rightPart);
					}

					loopCriteria = (loopCriteria + 1) > 31 ? 31 : loopCriteria + 1;
					double sqrtDivider = sqrtParam/(sqrtParam + 1.0);

					if (loopCriteria <= 30)
					{
						paramA = (sValue - paramA > 0) ? sValue : sValue - (sValue - paramA)*sqrtDivider;
						paramB = (sValue - paramB < 0) ? sValue : sValue - (sValue - paramB)*sqrtDivider;
						jmaTempValue = series;

						if (loopCriteria == 30)
						{
							c0[0] = series;
							int intPart = (Math.Ceiling(sqrtParam) >= 1.0) ? (int) Math.Ceiling(sqrtParam) : 1;
							int leftInt = intPart; //IntPortion(intPart); 
							intPart = (Math.Floor(sqrtParam) >= 1.0) ? (int) Math.Floor(sqrtParam) : 1;
							int rightPart = intPart; //IntPortion (intPart);
							dValue = (leftInt == rightPart)
								? 1.0
								: (sqrtParam - rightPart)/(leftInt - rightPart);
							int upShift = (rightPart <= 29) ? rightPart : 29;
							int dnShift = (leftInt <= 29) ? leftInt : 29;
							a8[0] = (series - buffer[loopParam - upShift])*(1 - dValue)/rightPart +
							        (series - buffer[loopParam - dnShift])*dValue/leftInt;
						}
					}
					else
					{
						dValue = lowDValue/(s38 - s40 + 1);
						double powerValue = (0.5 <= logParam - 2.0) ? logParam - 2.0 : 0.5;
						dValue = logParam >= Math.Pow(absValue/dValue, powerValue) ? Math.Pow(absValue/dValue, powerValue) : logParam;
						if (dValue < 1)
							dValue = 1;
						powerValue = Math.Pow(sqrtDivider, Math.Sqrt(dValue));
						paramA = (sValue - paramA > 0) ? sValue : sValue - (sValue - paramA)*powerValue;
						paramB = (sValue - paramB < 0) ? sValue : sValue - (sValue - paramB)*powerValue;
					}
				}
				// ---- end of big cycle                  			   
				if (loopCriteria > 30)
				{
					double powerValue = Math.Pow(lengthDivider, dValue);
					double squareValue = powerValue*powerValue;
					c0[0] = (1 - powerValue)*series + powerValue*c0[1];
					c8[0] = (series - c0[0])*(1.0 - lengthDivider) + lengthDivider*c8[1];
					a8[0] = (phaseParam*c8[0] + c0[0] - jmaTempValue)*
					        (powerValue*(-2.0) + squareValue + 1) + squareValue*a8[1];
					jmaTempValue += a8[0];
				}
				jmaValue = jmaTempValue;
			} //endif loopparam>30
			if (loopParam <= 30)
				jmaValue = Input[0];
			Value.Set(jmaValue);
			jmaValueBuffer[0] = jmaValue;
		}

		#region Properties

		[Description("Lookback interval")]
		[GridCategory("Parameters")]
		public int Smoothing
		{
			get { return smoothing; }
			set { smoothing = Math.Max(1, value); }
		}

		[Description("Offset")]
		[GridCategory("Parameters")]
		public double Offset
		{
			get { return offset; }
			set { offset = Math.Max(-100, value); }
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
		private static CMA checkCMA = new CMA();
		private CMA[] cacheCMA = null;

		/// <summary>
		///     Jurik Moving Average
		/// </summary>
		/// <returns></returns>
		public CMA CMA(double phase, int smoothing)
		{
			return CMA(Input, phase, smoothing);
		}

		/// <summary>
		///     Jurik Moving Average
		/// </summary>
		/// <returns></returns>
		public CMA CMA(Data.IDataSeries input, double phase, int smoothing)
		{
			if (cacheCMA != null)
				for (int idx = 0; idx < cacheCMA.Length; idx++)
					if (Math.Abs(cacheCMA[idx].Offset - phase) <= double.Epsilon && cacheCMA[idx].Smoothing == smoothing &&
					    cacheCMA[idx].EqualsInput(input))
						return cacheCMA[idx];

			lock (checkCMA)
			{
				checkCMA.Offset = phase;
				phase = checkCMA.Offset;
				checkCMA.Smoothing = smoothing;
				smoothing = checkCMA.Smoothing;

				if (cacheCMA != null)
					for (int idx = 0; idx < cacheCMA.Length; idx++)
						if (Math.Abs(cacheCMA[idx].Offset - phase) <= double.Epsilon && cacheCMA[idx].Smoothing == smoothing &&
						    cacheCMA[idx].EqualsInput(input))
							return cacheCMA[idx];

				var indicator = new CMA();
				indicator.BarsRequired = BarsRequired;
				indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
				indicator.Input = input;
				indicator.Offset = phase;
				indicator.Smoothing = smoothing;
				Indicators.Add(indicator);
				indicator.SetUp();

				var tmp = new CMA[cacheCMA == null ? 1 : cacheCMA.Length + 1];
				if (cacheCMA != null)
					cacheCMA.CopyTo(tmp, 0);
				tmp[tmp.Length - 1] = indicator;
				cacheCMA = tmp;
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
		///     Jurik Moving Average
		/// </summary>
		/// <returns></returns>
		[Gui.Design.WizardCondition("Indicator")]
		public Indicator.CMA CMA(double phase, int smoothing)
		{
			return _indicator.CMA(Input, phase, smoothing);
		}

		/// <summary>
		///     Jurik Moving Average
		/// </summary>
		/// <returns></returns>
		public Indicator.CMA CMA(Data.IDataSeries input, double phase, int smoothing)
		{
			return _indicator.CMA(input, phase, smoothing);
		}
	}
}

// This namespace holds all strategies and is required. Do not change it.

namespace NinjaTrader.Strategy
{
	public partial class Strategy : StrategyBase
	{
		/// <summary>
		///     Jurik Moving Average
		/// </summary>
		/// <returns></returns>
		[Gui.Design.WizardCondition("Indicator")]
		public Indicator.CMA CMA(double phase, int smoothing)
		{
			return _indicator.CMA(Input, phase, smoothing);
		}

		/// <summary>
		///     Jurik Moving Average
		/// </summary>
		/// <returns></returns>
		public Indicator.CMA CMA(Data.IDataSeries input, double phase, int smoothing)
		{
			if (InInitialize && input == null)
				throw new ArgumentException(
					"You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return _indicator.CMA(input, phase, smoothing);
		}
	}
}

#endregion