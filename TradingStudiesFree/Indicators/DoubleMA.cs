using System;
using System.Drawing;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Indicator;

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
	/// <summary>
	/// DoubleMA, originally by Rollins on NinjaTrader Forums
	/// Modified to add additional moving average types by Big Mike (ctrlbrk.blogspot.com) 05/06/2009
	/// Modified to add Signal dataseries (-1 short, 0 neutral, 1 long) by ctrlbrk
	/// Modified to add PaintBars by sam028 2009/06/08
	/// Modified to add sound alerts by sam028 2009/06/11
	/// Modified to add different .wav file alerts by sam028 2009/06/12
	/// I also added Dmv dataseries to easily access correct closing value for each bar unlike original design 05/18/2009
	/// </summary>
	[Description("Double MA")]
	public class DoubleMA : Indicator
	{
		private int _mA2Period = 34; // Default setting for MAPeriod
		private int _mA1Period = 34; // Default setting for MAPeriod
		private MAV.MAType _mA1Type = NinjaTrader.Indicator.MAV.MAType.EMA;
		private MAV.MAType _mA2Type = NinjaTrader.Indicator.MAV.MAType.EMA;
		private bool showArrows;
		private int arrowDisplacement = 7;
		private IntSeries signal;
		private double angle = 5;
		// Paint bars
		private bool paintBar;
		private Color barColorUp = Color.Blue;
		private Color barColorDown = Color.Red;
		private Color barColorNeutral = Color.Yellow;
		// Sound	
		private bool soundon;
		private int thislong = -1;
		private int thisshort = -1;
		private string longwavfilename = "Alert4.wav";
		private string shortwavfilename = "Alert4.wav";

		protected override void Initialize()
		{
			Name = "Double MA";
			Add(new Plot(new Pen(Color.Blue, 4), PlotStyle.Line, "DoubleMA"));
			Overlay = true;
			signal = new IntSeries(this);
		}

		protected override void OnBarUpdate()
		{
			Value.Set(MAV(MAV(Input, MA1Period, (int)MA1Type), MA2Period, (int)MA2Type)[0]);

			if (CurrentBar < 2) return;

			double valueslope = (180 / Math.PI) * (Math.Atan((Value[0] - (Value[1] + Value[2]) / 2) / 1.5 / TickSize));

			if (valueslope > angle)
			{
				signal.Set(1);
				PlotColors[0][0] = BarColorUp;
			}
			else
				if (valueslope < -angle)
				{
					signal.Set(-1);
					PlotColors[0][0] = BarColorDown;
				}
				else
				{
					signal.Set(0);
					PlotColors[0][0] = BarColorNeutral;
				}

			if (paintBar)
			{
				CandleOutlineColor = ChartControl.ChartStyle.Pen.Color;
				BarColor = signal[0] == 1 ? BarColorUp : (signal[0] == -1 ? BarColorDown : BarColorNeutral);
			}

			if (showArrows)
				if (signal[0] == 1 && signal[1] != 1)
					DrawArrowUp(CurrentBar.ToString(CultureInfo.InvariantCulture), true, 0, Math.Min(Low[0], Value[0]) - TickSize * ArrowDisplacement, BarColorUp);
				else
					if (signal[0] == -1 && signal[1] != -1)
						DrawArrowDown(CurrentBar.ToString(CultureInfo.InvariantCulture), true, 0, Math.Max(High[0], Value[0]) + TickSize * ArrowDisplacement, BarColorDown);
					else
						RemoveDrawObject(CurrentBar.ToString(CultureInfo.InvariantCulture));

			if (!soundon)
				return;

			if (signal[0] == 1 && signal[1] != 1 && thislong != CurrentBar)
			{
				thislong = CurrentBar;
				PlaySound(longwavfilename);
			}
			else
				if (signal[0] == -1 && signal[1] != -1 && thisshort != CurrentBar)
				{
					thisshort = CurrentBar;
					PlaySound(shortwavfilename);
				}

		}

		#region Properties

		[Description("")]
		[GridCategory("Parameters")]
		public int MA2Period
		{
			get { return _mA2Period; }
			set { _mA2Period = Math.Max(1, value); }
		}

		[Description("")]
		[GridCategory("Parameters")]
		public int MA1Period
		{
			get { return _mA1Period; }
			set { _mA1Period = Math.Max(1, value); }

		}

		[Description("")]
		[GridCategory("Parameters")]
		public double Angle
		{
			get { return angle; }
			set { angle = Math.Max(0, value); }

		}

		[Description("Moving Average Type")]
		[GridCategory("Parameters")]
		public MAV.MAType MA1Type
		{
			get { return _mA1Type; }
			set { _mA1Type = value; }
		}

		[Description("Moving Average Type")]
		[GridCategory("Parameters")]
		public MAV.MAType MA2Type
		{
			get { return _mA2Type; }
			set { _mA2Type = value; }
		}

		[Description("Paint Bar")]
		[GridCategory("Colors")]
		[Gui.Design.DisplayNameAttribute("1. Color Bars?")]
		public bool PaintBar
		{
			get { return paintBar; }
			set { paintBar = value; }
		}

		[XmlIgnore]
		[Description("Color up")]
		[GridCategory("Colors")]
		[Gui.Design.DisplayNameAttribute("2. Color Up")]
		public Color BarColorUp
		{
			get { return barColorUp; }
			set { barColorUp = value; }
		}

		[XmlIgnore]
		[Description("Color of Down bars.")]
		[GridCategory("Colors")]
		[Gui.Design.DisplayNameAttribute("3. Color Down")]
		public Color BarColorDown
		{
			get { return barColorDown; }
			set { barColorDown = value; }
		}

		[XmlIgnore]
		[Description("Color of Neutral bars.")]
		[GridCategory("Colors")]
		[Gui.Design.DisplayNameAttribute("4. Color Neutral")]
		public Color BarColorNeutral
		{
			get { return barColorNeutral; }
			set { barColorNeutral = value; }
		}

		[Description("Show Arrows")]
		[GridCategory("Colors")]
		[Gui.Design.DisplayNameAttribute("5. Show Arrows?")]
		public bool ShowArrows
		{
			get { return showArrows; }
			set { showArrows = value; }
		}

		[Description("Draw Arrows above/below bars")]
		[GridCategory("Colors")]
		[Gui.Design.DisplayNameAttribute("6. Arrow Displacement")]
		public int ArrowDisplacement
		{
			get { return arrowDisplacement; }
			set { arrowDisplacement = Math.Max(0, value); }
		}

		[Description("Sound")]
		[GridCategory("Sound")]
		[Gui.Design.DisplayNameAttribute("1. Play Alerts?")]
		public bool SoundOn
		{
			get { return soundon; }
			set { soundon = value; }
		}

		[Description("Sound file to play for long alert.")]
		[GridCategory("Sound")]
		[Gui.Design.DisplayNameAttribute("2. Long Alert")]
		public string WavLongFileName
		{
			get { return longwavfilename; }
			set { longwavfilename = value; }
		}
		[Description("Sound file to play for short alert.")]
		[GridCategory("Sound")]
		[Gui.Design.DisplayNameAttribute("2. Short Alert")]
		public string WavShortFileName
		{
			get { return shortwavfilename; }
			set { shortwavfilename = value; }
		}
		[Browsable(false)]
		public string BarColorUpSerialize
		{
			get { return Gui.Design.SerializableColor.ToString(barColorUp); }
			set { barColorUp = Gui.Design.SerializableColor.FromString(value); }
		}
		[Browsable(false)]
		public string BarColorNeutralSerialize
		{
			get { return Gui.Design.SerializableColor.ToString(barColorNeutral); }
			set { barColorNeutral = Gui.Design.SerializableColor.FromString(value); }
		}
		[Browsable(false)]
		public string BarColorDownSerialize
		{
			get { return Gui.Design.SerializableColor.ToString(barColorDown); }
			set { barColorDown = Gui.Design.SerializableColor.FromString(value); }
		}

		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
		[XmlIgnore]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
		public IntSeries Signal
		{
			get { return signal; }
		}

		#endregion

		public override string ToString()
		{
			return string.Format(@"Double MA (Angle({0}), {1}({2}), {3}({4}))", Angle, MA1Type, MA1Period, MA2Type, MA2Period);
		}
	}
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
	public partial class Indicator : IndicatorBase
	{
		private DoubleMA[] cacheDoubleMA = null;

		private static DoubleMA checkDoubleMA = new DoubleMA();

		/// <summary>
		/// Double MA
		/// </summary>
		/// <returns></returns>
		public DoubleMA DoubleMA(double angle, int arrowDisplacement, Color barColorDown, Color barColorNeutral, Color barColorUp, int mA1Period, MAV.MAType mA1Type, int mA2Period, MAV.MAType mA2Type, bool paintBar, bool showArrows, bool soundOn, string wavLongFileName, string wavShortFileName)
		{
			return DoubleMA(Input, angle, arrowDisplacement, barColorDown, barColorNeutral, barColorUp, mA1Period, mA1Type, mA2Period, mA2Type, paintBar, showArrows, soundOn, wavLongFileName, wavShortFileName);
		}

		/// <summary>
		/// Double MA
		/// </summary>
		/// <returns></returns>
		public DoubleMA DoubleMA(Data.IDataSeries input, double angle, int arrowDisplacement, Color barColorDown, Color barColorNeutral, Color barColorUp, int mA1Period, MAV.MAType mA1Type, int mA2Period, MAV.MAType mA2Type, bool paintBar, bool showArrows, bool soundOn, string wavLongFileName, string wavShortFileName)
		{
			if (cacheDoubleMA != null)
				for (int idx = 0; idx < cacheDoubleMA.Length; idx++)
					if (Math.Abs(cacheDoubleMA[idx].Angle - angle) <= double.Epsilon && cacheDoubleMA[idx].ArrowDisplacement == arrowDisplacement && cacheDoubleMA[idx].BarColorDown == barColorDown && cacheDoubleMA[idx].BarColorNeutral == barColorNeutral && cacheDoubleMA[idx].BarColorUp == barColorUp && cacheDoubleMA[idx].MA1Period == mA1Period && cacheDoubleMA[idx].MA1Type == mA1Type && cacheDoubleMA[idx].MA2Period == mA2Period && cacheDoubleMA[idx].MA2Type == mA2Type && cacheDoubleMA[idx].PaintBar == paintBar && cacheDoubleMA[idx].ShowArrows == showArrows && cacheDoubleMA[idx].SoundOn == soundOn && cacheDoubleMA[idx].WavLongFileName == wavLongFileName && cacheDoubleMA[idx].WavShortFileName == wavShortFileName && cacheDoubleMA[idx].EqualsInput(input))
						return cacheDoubleMA[idx];

			lock (checkDoubleMA)
			{
				checkDoubleMA.Angle = angle;
				angle = checkDoubleMA.Angle;
				checkDoubleMA.ArrowDisplacement = arrowDisplacement;
				arrowDisplacement = checkDoubleMA.ArrowDisplacement;
				checkDoubleMA.BarColorDown = barColorDown;
				barColorDown = checkDoubleMA.BarColorDown;
				checkDoubleMA.BarColorNeutral = barColorNeutral;
				barColorNeutral = checkDoubleMA.BarColorNeutral;
				checkDoubleMA.BarColorUp = barColorUp;
				barColorUp = checkDoubleMA.BarColorUp;
				checkDoubleMA.MA1Period = mA1Period;
				mA1Period = checkDoubleMA.MA1Period;
				checkDoubleMA.MA1Type = mA1Type;
				mA1Type = checkDoubleMA.MA1Type;
				checkDoubleMA.MA2Period = mA2Period;
				mA2Period = checkDoubleMA.MA2Period;
				checkDoubleMA.MA2Type = mA2Type;
				mA2Type = checkDoubleMA.MA2Type;
				checkDoubleMA.PaintBar = paintBar;
				paintBar = checkDoubleMA.PaintBar;
				checkDoubleMA.ShowArrows = showArrows;
				showArrows = checkDoubleMA.ShowArrows;
				checkDoubleMA.SoundOn = soundOn;
				soundOn = checkDoubleMA.SoundOn;
				checkDoubleMA.WavLongFileName = wavLongFileName;
				wavLongFileName = checkDoubleMA.WavLongFileName;
				checkDoubleMA.WavShortFileName = wavShortFileName;
				wavShortFileName = checkDoubleMA.WavShortFileName;

				if (cacheDoubleMA != null)
					for (int idx = 0; idx < cacheDoubleMA.Length; idx++)
						if (Math.Abs(cacheDoubleMA[idx].Angle - angle) <= double.Epsilon && cacheDoubleMA[idx].ArrowDisplacement == arrowDisplacement && cacheDoubleMA[idx].BarColorDown == barColorDown && cacheDoubleMA[idx].BarColorNeutral == barColorNeutral && cacheDoubleMA[idx].BarColorUp == barColorUp && cacheDoubleMA[idx].MA1Period == mA1Period && cacheDoubleMA[idx].MA1Type == mA1Type && cacheDoubleMA[idx].MA2Period == mA2Period && cacheDoubleMA[idx].MA2Type == mA2Type && cacheDoubleMA[idx].PaintBar == paintBar && cacheDoubleMA[idx].ShowArrows == showArrows && cacheDoubleMA[idx].SoundOn == soundOn && cacheDoubleMA[idx].WavLongFileName == wavLongFileName && cacheDoubleMA[idx].WavShortFileName == wavShortFileName && cacheDoubleMA[idx].EqualsInput(input))
							return cacheDoubleMA[idx];

				DoubleMA indicator = new DoubleMA();
				indicator.BarsRequired = BarsRequired;
				indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
				indicator.Input = input;
				indicator.Angle = angle;
				indicator.ArrowDisplacement = arrowDisplacement;
				indicator.BarColorDown = barColorDown;
				indicator.BarColorNeutral = barColorNeutral;
				indicator.BarColorUp = barColorUp;
				indicator.MA1Period = mA1Period;
				indicator.MA1Type = mA1Type;
				indicator.MA2Period = mA2Period;
				indicator.MA2Type = mA2Type;
				indicator.PaintBar = paintBar;
				indicator.ShowArrows = showArrows;
				indicator.SoundOn = soundOn;
				indicator.WavLongFileName = wavLongFileName;
				indicator.WavShortFileName = wavShortFileName;
				Indicators.Add(indicator);
				indicator.SetUp();

				DoubleMA[] tmp = new DoubleMA[cacheDoubleMA == null ? 1 : cacheDoubleMA.Length + 1];
				if (cacheDoubleMA != null)
					cacheDoubleMA.CopyTo(tmp, 0);
				tmp[tmp.Length - 1] = indicator;
				cacheDoubleMA = tmp;
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
		/// Double MA
		/// </summary>
		/// <returns></returns>
		[Gui.Design.WizardCondition("Indicator")]
		public Indicator.DoubleMA DoubleMA(double angle, int arrowDisplacement, Color barColorDown, Color barColorNeutral, Color barColorUp, int mA1Period, MAV.MAType mA1Type, int mA2Period, MAV.MAType mA2Type, bool paintBar, bool showArrows, bool soundOn, string wavLongFileName, string wavShortFileName)
		{
			return _indicator.DoubleMA(Input, angle, arrowDisplacement, barColorDown, barColorNeutral, barColorUp, mA1Period, mA1Type, mA2Period, mA2Type, paintBar, showArrows, soundOn, wavLongFileName, wavShortFileName);
		}

		/// <summary>
		/// Double MA
		/// </summary>
		/// <returns></returns>
		public Indicator.DoubleMA DoubleMA(Data.IDataSeries input, double angle, int arrowDisplacement, Color barColorDown, Color barColorNeutral, Color barColorUp, int mA1Period, MAV.MAType mA1Type, int mA2Period, MAV.MAType mA2Type, bool paintBar, bool showArrows, bool soundOn, string wavLongFileName, string wavShortFileName)
		{
			return _indicator.DoubleMA(input, angle, arrowDisplacement, barColorDown, barColorNeutral, barColorUp, mA1Period, mA1Type, mA2Period, mA2Type, paintBar, showArrows, soundOn, wavLongFileName, wavShortFileName);
		}
	}
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
	public partial class Strategy : StrategyBase
	{
		/// <summary>
		/// Double MA
		/// </summary>
		/// <returns></returns>
		[Gui.Design.WizardCondition("Indicator")]
		public Indicator.DoubleMA DoubleMA(double angle, int arrowDisplacement, Color barColorDown, Color barColorNeutral, Color barColorUp, int mA1Period, MAV.MAType mA1Type, int mA2Period, MAV.MAType mA2Type, bool paintBar, bool showArrows, bool soundOn, string wavLongFileName, string wavShortFileName)
		{
			return _indicator.DoubleMA(Input, angle, arrowDisplacement, barColorDown, barColorNeutral, barColorUp, mA1Period, mA1Type, mA2Period, mA2Type, paintBar, showArrows, soundOn, wavLongFileName, wavShortFileName);
		}

		/// <summary>
		/// Double MA
		/// </summary>
		/// <returns></returns>
		public Indicator.DoubleMA DoubleMA(Data.IDataSeries input, double angle, int arrowDisplacement, Color barColorDown, Color barColorNeutral, Color barColorUp, int mA1Period, MAV.MAType mA1Type, int mA2Period, MAV.MAType mA2Type, bool paintBar, bool showArrows, bool soundOn, string wavLongFileName, string wavShortFileName)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return _indicator.DoubleMA(input, angle, arrowDisplacement, barColorDown, barColorNeutral, barColorUp, mA1Period, mA1Type, mA2Period, mA2Type, paintBar, showArrows, soundOn, wavLongFileName, wavShortFileName);
		}
	}
}
#endregion
