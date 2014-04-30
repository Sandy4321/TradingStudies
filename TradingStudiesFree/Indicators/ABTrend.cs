using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.Design;
using NinjaTrader.Indicator;

namespace ABAutoUtility
{
	public enum FilterOrder
	{
		FirstOrder,
		SecondOrder,
		ThirdOrder
	}

	public class HslColor
	{
		// Fields
		private int alpha;
		private double hue;
		private double luminosity;
		private double saturation;

		// Methods
		public HslColor()
		{
			hue = 1.0;
			saturation = 1.0;
			luminosity = 1.0;
			alpha = 255;
		}

		public HslColor(Color color)
		{
			hue = 1.0;
			saturation = 1.0;
			luminosity = 1.0;
			alpha = 255;
			SetRgb(color.R, color.G, color.B);
		}

		public HslColor(double hue, double saturation, double luminosity)
		{
			this.hue = 1.0;
			this.saturation = 1.0;
			this.luminosity = 1.0;
			alpha = 255;
			Hue = hue;
			Saturation = saturation;
			Luminosity = luminosity;
		}

		public HslColor(int red, int green, int blue)
		{
			hue = 1.0;
			saturation = 1.0;
			luminosity = 1.0;
			alpha = 255;
			SetRgb(red, green, blue);
		}

		public HslColor(double hue, double saturation, double luminosity, int alpha)
		{
			this.hue = 1.0;
			this.saturation = 1.0;
			this.luminosity = 1.0;
			this.alpha = 255;
			Hue = hue;
			Saturation = saturation;
			Luminosity = luminosity;
			this.alpha = alpha;
		}

		public int Alpha
		{
			get { return alpha; }
			set { alpha = Math.Min(value, 255); }
		}

		public double Hue
		{
			get { return (hue * 1.0); }
			set { hue = CheckRange(value / 1.0); }
		}

		public double Luminosity
		{
			get { return (luminosity * 1.0); }
			set { luminosity = CheckRange(value / 1.0); }
		}

		public double Saturation
		{
			get { return (saturation * 1.0); }
			set { saturation = CheckRange(value / 1.0); }
		}

		private static double CheckRange(double value)
		{
			if (value < 0.0)
			{
				value = 0.0;
				return value;
			}
			if (value > 1.0)
				value = 1.0;
			return value;
		}

		private static double GetColorComponent(double temp1, double temp2, double temp3)
		{
			temp3 = MoveIntoRange(temp3);
			if (temp3 < 0.16666666666666666)
			{
				return (temp1 + (((temp2 - temp1) * 6.0) * temp3));
			}
			if (temp3 < 0.5)
			{
				return temp2;
			}
			if (temp3 < 0.66666666666666663)
			{
				return (temp1 + (((temp2 - temp1) * (0.66666666666666663 - temp3)) * 6.0));
			}
			return temp1;
		}

		private static double GetTemp2(HslColor hslColor)
		{
			if (hslColor.luminosity < 0.5)
			{
				return (hslColor.luminosity * (1.0 + hslColor.saturation));
			}
			return ((hslColor.luminosity + hslColor.saturation) - (hslColor.luminosity * hslColor.saturation));
		}

		private static double MoveIntoRange(double temp3)
		{
			if (temp3 < 0.0)
			{
				temp3++;
				return temp3;
			}
			if (temp3 > 1.0)
			{
				temp3--;
			}
			return temp3;
		}

		public static implicit operator Color(HslColor hslColor)
		{
			double num = 0.0;
			double num2 = 0.0;
			double num3 = 0.0;
			if (Math.Abs(hslColor.luminosity) > double.Epsilon)
			{
				if (Math.Abs(hslColor.saturation) < double.Epsilon)
				{
					num = num2 = num3 = hslColor.luminosity;
				}
				else
				{
					double num4 = GetTemp2(hslColor);
					double num5 = (2.0 * hslColor.luminosity) - num4;
					num = GetColorComponent(num5, num4, hslColor.hue + 0.33333333333333331);
					num2 = GetColorComponent(num5, num4, hslColor.hue);
					num3 = GetColorComponent(num5, num4, hslColor.hue - 0.33333333333333331);
				}
			}
			return Color.FromArgb(hslColor.alpha, (int)(255.0 * num), (int)(255.0 * num2), (int)(255.0 * num3));
		}

		public static implicit operator HslColor(Color color)
		{
			return new HslColor
				{
					hue = (color.GetHue()) / 360.0,
					luminosity = color.GetBrightness(),
					saturation = color.GetSaturation()
				};
		}

		public void SetRgb(int red, int green, int blue)
		{
			HslColor color = Color.FromArgb(red, green, blue);
			hue = color.hue;
			saturation = color.saturation;
			luminosity = color.luminosity;
		}

		public string ToRgbString()
		{
			Color color = this;
			return string.Format("R: {0:#0.##} G: {1:#0.##} B: {2:#0.##}", color.R, color.G, color.B);
		}

		public override string ToString()
		{
			return string.Format("H: {0:#0.##} S: {1:#0.##} L: {2:#0.##}", Hue, Saturation, Luminosity);
		}
	}

	public enum InputSignal
	{
		Price,
		Time,
		Volume
	}

	public enum MAType
	{
		// ReSharper disable InconsistentNaming
		ADXVMA,
		SMMA,
		T3Tillson,
		T3FulksMatulich,
		SMA,
		EMA,
		HMA,
		JMA,
		JMAFast,
		JMASlow,
		TMA,
		DEMA,
		TEMA,
		REMA,
		WMA,
		SineWMA,
		VMA,
		VMAFast,
		VMASlow,
		VWMA,
		LSMA,
		LSMAe,
		ILRS,
		IE2,
		ZeroLagEMA,
		ZeroLagHATEMA,
		ZeroLagTEMA,
		NonLagMA,
		StepMA,
		ModifiedOptimumEllipticFilter,
		iTrend,
		AdvancedAMA,
		FRAMA,
		MAMA,
		Laguerre,
		Kalman,
		UnscentedKalman,
		UnscentedTurbo,
		UnscentedFast,
		UnscentedSlow
		// ReSharper restore InconsistentNaming
	}

	public enum SymbolType
	{
		Dot,
		Square,
		TriangleUp,
		TriangleDown,
		Diamond,
		ArrowUp,
		ArrowDown,
		None
	}

	public static class Util
	{
		private static readonly Color downColor = Color.OrangeRed;
		private static readonly StringFormat stringFormat = new StringFormat();
		private static readonly SolidBrush textBrush = new SolidBrush(Color.Black);
		private static readonly Font textFont = new Font("Arial", 8f);
		private static readonly Color upColor = Color.YellowGreen;

		static Util()
		{
			stringFormat.Alignment = StringAlignment.Far;
		}

		public static void DrawSlopeString(Indicator ind, Graphics graphics, Rectangle bounds, string plotName, int plotNum, int slopeVal, int boxNum)
		{
			string s = plotName + ": ";
			StringBuilder builder = new StringBuilder();
			builder.Append(slopeVal.ToString(CultureInfo.InvariantCulture));
			builder.Append(Convert.ToChar(176));
			Color color = (slopeVal > 0) ? upColor : downColor;
			textBrush.Color = color;
			Brush brush = textBrush;
			if (plotNum != -1)
			{
				if (ind.Plots[plotNum] == null)
					s = "Err: Plot Does not exsit!";
				else
					brush = (ind.Plots[plotNum].Pen.Color == Color.Transparent) ? textBrush : ind.Plots[plotNum].Pen.Brush;
			}
			graphics.DrawString(s, textFont, brush, ((bounds.X + bounds.Width) - 28), ((bounds.Y + bounds.Height) - (boxNum * 20)), stringFormat);
			graphics.DrawString(builder.ToString(), textFont, textBrush, ((bounds.X + bounds.Width) - 3), ((bounds.Y + bounds.Height) - (boxNum * 20)), stringFormat);
		}

		public static void DrawSymbol(Indicator ind, SymbolType symbol, string tag, int barsAgo, double y, Color color)
		{
			const bool autoScale = false;
			switch (symbol)
			{
				case SymbolType.Dot:
					ind.DrawDot(tag, autoScale, barsAgo, y, color);
					return;

				case SymbolType.Square:
					ind.DrawSquare(tag, autoScale, barsAgo, y, color);
					return;

				case SymbolType.TriangleUp:
					ind.DrawTriangleUp(tag, autoScale, barsAgo, y, color);
					return;

				case SymbolType.TriangleDown:
					ind.DrawTriangleDown(tag, autoScale, barsAgo, y, color);
					return;

				case SymbolType.Diamond:
					ind.DrawDiamond(tag, autoScale, barsAgo, y, color);
					return;

				case SymbolType.ArrowUp:
					ind.DrawArrowUp(tag, autoScale, barsAgo, y, color);
					return;

				case SymbolType.ArrowDown:
					ind.DrawArrowDown(tag, autoScale, barsAgo, y, color);
					break;

				case SymbolType.None:
					break;

				default:
					return;
			}
		}

		public static void DrawValueString(Indicator ind, Graphics graphics, Rectangle bounds, string plotName, int plotNum, DataSeries series, int boxNum)
		{
			int num2 = ind.CalculateOnBarClose ? (ind.ChartControl.BarsPainted - 2) : (ind.ChartControl.BarsPainted - 1);
			if (ind.Bars != null)
			{
				int index = ((ind.ChartControl.LastBarPainted - ind.ChartControl.BarsPainted) - 1) + num2;
				if (ind.ChartControl.ShowBarsRequired || ((index - ind.Displacement) >= ind.BarsRequired))
				{
					double num3 = series.Get(index);
					string s = plotName + ": ";
					StringBuilder builder = new StringBuilder();
					builder.Append(Math.Round(num3, 3).ToString(CultureInfo.InvariantCulture));
					Color color = (num3 > 0.0) ? upColor : downColor;
					textBrush.Color = color;
					Brush brush = textBrush;
					if (plotNum != -1)
					{
						if (ind.Plots[plotNum] == null)
							s = "Err: Plot Does not exsit!";
						else
							brush = (ind.Plots[plotNum].Pen.Color == Color.Transparent) ? textBrush : ind.Plots[plotNum].Pen.Brush;
					}
					graphics.DrawString(s, textFont, brush, (bounds.X + bounds.Width) - 37, (bounds.Y + bounds.Height) - (boxNum * 20), stringFormat);
					graphics.DrawString(builder.ToString(), textFont, textBrush, (bounds.X + bounds.Width) - 3, (bounds.Y + bounds.Height) - (boxNum * 20), stringFormat);
				}
			}
		}

		public static void DrawValueString(Indicator ind, Graphics graphics, Rectangle bounds, string plotName, int plotNum, double val, int boxNum)
		{
			string s = plotName + ": ";
			StringBuilder builder = new StringBuilder();
			builder.Append(Math.Round(val, 3).ToString(CultureInfo.InvariantCulture));
			Color color = (val > 0.0) ? upColor : downColor;
			textBrush.Color = color;
			Brush brush = textBrush;
			if (plotNum != -1)
			{
				if (ind.Plots[plotNum] == null)
					s = "Err: Plot Does not exsit!";
				else
					brush = (ind.Plots[plotNum].Pen.Color == Color.Transparent) ? textBrush : ind.Plots[plotNum].Pen.Brush;
			}
			graphics.DrawString(s, textFont, brush, ((bounds.X + bounds.Width) - 37), ((bounds.Y + bounds.Height) - (boxNum * 20)), stringFormat);
			graphics.DrawString(builder.ToString(), textFont, textBrush, ((bounds.X + bounds.Width) - 3), ((bounds.Y + bounds.Height) - (boxNum * 20)), stringFormat);
		}

		public static int GetSlope(Indicator ind, DataSeries series, int index)
		{
			return !series.ContainsValue(3) ? 0 : (int)((57.295779513082323 * Math.Atan(((series.Get(index) - ((series.Get(index + 1) + series.Get(index + 2)) / 2.0)) / 1.5) / ind.TickSize)) * -1.0);
		}

		public static int GetSlope(NinjaTrader.Strategy.Strategy strat, DataSeries series, int index)
		{
			return !series.ContainsValue(3) ? 0 : (int)((57.295779513082323 * Math.Atan(((series.Get(index) - ((series.Get(index + 1) + series.Get(index + 2)) / 2.0)) / 1.5) / strat.TickSize)) * -1.0);
		}

		public static List<int> ParseInputList(string input)
		{
			List<int> list = new List<int>();
			if (!string.IsNullOrEmpty(input))
				list.AddRange(input.Split(new[] { ',' }).Select(int.Parse));
			return list;
		}
	}
}

namespace NinjaTrader.Indicator
{
	[Description("PTS trend indicator.")]
	public class AbTrend : Indicator
	{
		private readonly Pen penDown = new Pen(Color.Transparent, 2f);
		private readonly Pen penTurn = new Pen(Color.Transparent, 1f);
		private readonly Pen penUp = new Pen(Color.Transparent, 2f);
		private Color backgroundcolorDn = Color.Orange;
		private Color backgroundcolorUp = Color.LightGreen;
		private Color barColorDown = Color.Maroon;
		private Color barColorUp = Color.Navy;
		private double beta;
		private bool colorAlLbackgrounds = true;
		private bool colorBars = true;
		private bool colorbackground = true;
		private int distance = 2;
		private bool drawArrow = true;
		private double expFactor;
		private Dictionary<int, double[]> fbank;
		private DataSeries lSigma;
		private double lengthParam;
		private int opacity = 50;
		private int period = 10;
		private double permissivity;
		private double phase;
		private readonly StringFormat stringFormat = new StringFormat();
		private DataSeries uSigma;
		private DataSeries vx;
		private DataSeries vxAvg;
		private DataSeries vxTrend;
		private double windowLength;
		private DataSeries zlr;

		[Category("Visual")]
		[Description("Background Color Dn")]
		[Gui.Design.DisplayName("05d.Background Color Dn")]
		public Color BackgroundcolorDn
		{
			get { return backgroundcolorDn; }
			set { backgroundcolorDn = value; }
		}

		[Browsable(false)]
		public string BackgroundcolorDnSerialize
		{
			get { return SerializableColor.ToString(backgroundcolorDn); }
			set { backgroundcolorDn = SerializableColor.FromString(value); }
		}

		[Description("Background Color Up")]
		[Category("Visual")]
		[Gui.Design.DisplayName("05c.Background Color Up")]
		public Color BackgroundcolorUp
		{
			get { return backgroundcolorUp; }
			set { backgroundcolorUp = value; }
		}

		[Browsable(false)]
		public string BackgroundcolorUpSerialize
		{
			get { return SerializableColor.ToString(backgroundcolorUp); }
			set { backgroundcolorUp = SerializableColor.FromString(value); }
		}

		[Description("Color of down bars")]
		[Category("Visual")]
		[Gui.Design.DisplayName("03. Down color")]
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

		[Category("Visual")]
		[Gui.Design.DisplayName("02. Up color")]
		[Description("Color of up bars")]
		public Color BarColorUp
		{
			get { return barColorUp; }
			set { barColorUp = value; }
		}

		[XmlIgnore]
		[Browsable(false)]
		public int Bias
		{
			get { return GetBias(0); }
		}

		[Category("Visual")]
		[Description("Color ALL Backgrounds?")]
		[Gui.Design.DisplayName("05b.Color ALL Backgrounds?")]
		public bool ColorAllBackgrounds
		{
			get { return colorAlLbackgrounds; }
			set { colorAlLbackgrounds = value; }
		}

		[Gui.Design.DisplayName("05a.Color Background?")]
		[Category("Visual")]
		[Description("Color Background?")]
		public bool ColorBackground
		{
			get { return colorbackground; }
			set { colorbackground = value; }
		}

		[Category("Visual")]
		[Description("Color Bars?")]
		[Gui.Design.DisplayName("01. Color Bars?")]
		public bool ColorBars
		{
			get { return colorBars; }
			set { colorBars = value; }
		}

		[Gui.Design.DisplayName("04a.Ticks from arrow?")]
		[Description("Number of ticks between the price bar and the arrow.")]
		[Category("Visual")]
		public int Distance
		{
			get { return distance; }
			set { distance = Math.Max(-1, value); }
		}

		[XmlIgnore]
		[Browsable(false)]
		public DataSeries DotL
		{
			get { return Values[3]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries DotU
		{
			get { return Values[2]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries DotWarn
		{
			get { return Values[4]; }
		}

		[Category("Visual")]
		[Gui.Design.DisplayName("04.Draw Arrow On Price Panel?")]
		[Description("Draw Arrow on Price Panel?")]
		public bool DrawArrow
		{
			get { return drawArrow; }
			set { drawArrow = value; }
		}

		[Gui.Design.DisplayName("06.Background Opacity")]
		[Category("Visual")]
		[Description("Background Opacity")]
		public int Opacity
		{
			get { return opacity; }
			set { opacity = value; }
		}

		[Description("")]
		[Gui.Design.DisplayName("Factor1")]
		[Category("Parameters")]
		public int Period
		{
			get { return period; }
			set { period = value; }
		}

		[Category("Parameters")]
		[Gui.Design.DisplayName("Factor2")]
		[Description("[-150,150]")]
		public double Phase
		{
			get { return phase; }
			set { phase = value; }
		}

		[XmlIgnore]
		[Browsable(false)]
		public DataSeries Prediction
		{
			get { return Values[1]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries RawTrend
		{
			get { return Values[0]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public int ThresholdCross
		{
			get
			{
				if (CrossAbove(RawTrend, 0.0, 1) || ((RawTrend[0] > 0.0) && CrossAbove(RawTrend, Prediction, 1)))
					return 1;
				if (CrossBelow(RawTrend, 0.0, 1))
					return -1;
				return 0;
			}
		}

		[XmlIgnore]
		[Browsable(false)]
		public DataSeries Zlr
		{
			get { return zlr; }
		}

		public void DrawColorLine(Graphics graphics, Rectangle bounds, double min, double max, DataSeries series, Pen upPen, Pen downPen)
		{
			if (Bars == null) return;
			int barPaintWidth = ChartControl.ChartStyle.GetBarPaintWidth(ChartControl.BarWidth);
			SmoothingMode smoothingMode = graphics.SmoothingMode;
			int num2 = -1;
			int num3 = -1;
			Pen pen = null;
			GraphicsPath path = null;
			for (int i = 0;
				 i <= (Math.Min(ChartControl.BarsPainted, series.Count - ChartControl.FirstBarPainted) - 1);
				 i++)
			{
				int index = ((ChartControl.LastBarPainted - ChartControl.BarsPainted) + 1) + i;
				if (ChartControl.ShowBarsRequired || ((index - Displacement) >= BarsRequired))
				{
					double d = series.Get(index);
					if (!double.IsNaN(d))
					{
						int num7 = (((ChartControl.CanvasRight - ChartControl.BarMarginRight) - (barPaintWidth / 2)) -
									((ChartControl.BarsPainted - 1) * ChartControl.BarSpace)) + (i * ChartControl.BarSpace);
						int num8 = (bounds.Y + bounds.Height) - ((int)(((d - min) / ChartControl.MaxMinusMin(max, min)) * bounds.Height));
						if (num2 >= 0)
						{
							Pen pen2 = (d > 0.0) ? upPen : downPen;
							if (pen2 != pen)
							{
								if (path != null)
									if (pen != null) graphics.DrawPath(pen, path);
								path = new GraphicsPath();
								pen = pen2;
							}
							if (path != null) path.AddLine(num2, num3, num7, num8);
						}
						num2 = num7;
						num3 = num8;
					}
					if (pen != null)
					{
						graphics.SmoothingMode = SmoothingMode.AntiAlias;
						graphics.DrawPath(pen, path);
						graphics.SmoothingMode = smoothingMode;
					}
				}
			}
		}

		private void DrawDots()
		{
			if ((RawTrend[0] < 0.0) && (Prediction[0] < 0.0))
			{
				if (((Slope(RawTrend, 2, 0) < 0.0) && (Slope(Prediction, 2, 0) < 0.0)) &&
					(RawTrend[0] > Prediction[0]))
					DotWarn.Set(0.0);
				if (RawTrend[0] < Prediction[0])
					DotL.Set(0.0);
				else
					DotU.Set(0.0);
			}
			else if ((RawTrend[0] > 0.0) && (Prediction[0] > 0.0))
			{
				if (((Slope(RawTrend, 2, 0) > 0.0) && (Slope(Prediction, 2, 0) > 0.0)) &&
					(RawTrend[0] < Prediction[0]))
					DotWarn.Set(0.0);
				if (RawTrend[0] > Prediction[0])
					DotU.Set(0.0);
				else
					DotL.Set(0.0);
			}
		}

		private int GetBias(int index)
		{
			int num = 0 + (RawTrend[index] > 0.0 ? 1 : -1);
			return num + (Prediction[index] > 0.0 ? 1 : -1);
		}

		protected override void Initialize()
		{
			Add(new Plot(Color.Transparent, "rawTrend"));
			Add(new Plot(Color.Transparent, "predictTrend"));
			Plots[1].PlotStyle = PlotStyle.Bar;
			Plots[1].Pen.DashStyle = DashStyle.Dot;
			Add(new Plot(Color.Transparent, "DotU"));
			Plots[2].Pen.DashStyle = DashStyle.Dot;
			Plots[2].PlotStyle = PlotStyle.Dot;
			Add(new Plot(Color.Transparent, "DotL"));
			Plots[3].Pen.DashStyle = DashStyle.Dot;
			Plots[3].PlotStyle = PlotStyle.Dot;
			Add(new Plot(Color.Transparent, "DotWarn"));
			Plots[4].Pen.DashStyle = DashStyle.Dot;
			Plots[4].PlotStyle = PlotStyle.Dot;
			Plots[4].Pen.Width = 2f;
			Add(new Plot(Color.Transparent, "rawTrendSlope"));
			Add(new Plot(Color.Cyan, "predictSlope"));
			fbank = new Dictionary<int, double[]>();
			vxAvg = new DataSeries(this);
			vxTrend = new DataSeries(this);
			vx = new DataSeries(this);
			lSigma = new DataSeries(this);
			uSigma = new DataSeries(this);
			zlr = new DataSeries(this);
			penUp.DashStyle = DashStyle.Dot;
			penDown.DashStyle = DashStyle.Dot;
			penTurn.DashStyle = DashStyle.Dot;
			stringFormat.Alignment = StringAlignment.Far;
			Overlay = false;
			CalculateOnBarClose = true;
			PriceTypeSupported = true;
			DrawOnPricePanel = true;
			DisplayInDataBox = true;
			PlotsConfigurable = false;
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar < 2)
			{
				uSigma[0] = Input[0];
				lSigma[0] = Input[0];
			}
			else
			{
				SetWeights();
				double num = Input[0];
				double num2 = num - uSigma[1];
				double num3 = num - lSigma[1];
				double num4 = Math.Abs(num2);
				double num5 = Math.Abs(num3);
				if (num4 > num5)
					vx[0] = num4;
				if (num4 < num5)
					vx[0] = num5;
				if (Math.Abs(num4 - num5) < double.Epsilon)
					vx[0] = 0.0;
				vxAvg[0] = vxAvg[1] + (0.1 * (vx[0] - vx[Math.Min(CurrentBar, 10)]));
				vxTrend[0] = SMA(vxAvg, Math.Min(CurrentBar, 65))[0];
				if (CurrentBar <= 64)
					vxTrend[0] = vxTrend[1] + ((2.0 * (vxAvg[0] - vxTrend[1])) / 65.0);
				double num6 = 0.0;
				if (vxTrend[0] > 0.0)
					num6 = vx[0] / vxTrend[0];
				num6 = Math.Min(Math.Pow(lengthParam, 1.0 / expFactor), num6);
				if (num6 < 1.0)
					num6 = 1.0;
				double d = Math.Pow(num6, expFactor);
				double num8 = Math.Pow(beta, Math.Sqrt(d));
				double x = windowLength / (windowLength + 2.0);
				double num10 = Math.Pow(x, d);
				uSigma[0] = (num2 > 0.0) ? num : (num - (num8 * num2));
				lSigma[0] = (num3 < 0.0) ? num : (num - (num8 * num3));
				int currentBar = CurrentBar;
				double[] numArray = new double[5];
				fbank[currentBar] = numArray;
				if (currentBar == 2)
				{
					numArray[0] = num;
					numArray[2] = num;
					numArray[4] = num;
				}
				else
				{
					numArray[0] = ((1.0 - num10) * num) + (num10 * fbank[currentBar - 1][0]);
					numArray[1] = ((num - numArray[0]) * (1.0 - x)) + (x * fbank[currentBar - 1][1]);
					numArray[2] = numArray[0] + (permissivity * numArray[1]);
					numArray[3] = ((numArray[2] - fbank[currentBar - 1][4]) * Math.Pow(1.0 - num10, 2.0)) + (Math.Pow(num10, 2.0) * fbank[currentBar - 1][3]);
					numArray[4] = fbank[currentBar - 1][4] + numArray[3];
				}
				RawTrend.Set(numArray[1]);
				Prediction.Set(numArray[3]);
				DrawDots();
				if (colorBars)
				{
					if (RawTrend[0] > 0.0)
					{
						BarColor = barColorUp;
						CandleOutlineColor = Color.Silver;
					}
					if (RawTrend[0] < 0.0)
					{
						BarColor = barColorDown;
						CandleOutlineColor = Color.Silver;
					}
				}
				if (drawArrow)
				{
					if (CrossAbove(RawTrend, 0.0, 1))
					{
						DrawArrowUp(CurrentBar.ToString(CultureInfo.InvariantCulture), true, 0, Low[0] - (distance * TickSize), Color.Navy);
						Alert("Long", Priority.High, "Long Entry", "Alert4.wav", 10, Color.Black, Color.Yellow);
					}
					if (CrossBelow(RawTrend, 0.0, 1))
					{
						DrawArrowDown(CurrentBar.ToString(CultureInfo.InvariantCulture), true, 0, High[0] + (distance * TickSize), Color.Red);
						Alert("Short", Priority.High, "Short Entry", "Alert4.wav", 10, Color.Black, Color.Yellow);
					}
				}
				if (colorbackground)
				{
					if (!CrossAbove(RawTrend, 0.0, 1) && (RawTrend[0] <= 0.0))
					{
						if (CrossBelow(RawTrend, 0.0, 1) || (RawTrend[0] < 0.0))
						{
							BackColor = Color.FromArgb(opacity, backgroundcolorDn);
						}
					}
					else
					{
						BackColor = Color.FromArgb(opacity, backgroundcolorUp);
					}
				}
				if (colorAlLbackgrounds)
				{
					if (CrossAbove(RawTrend, 0.0, 1) || (RawTrend[0] > 0.0))
					{
						BackColorAll = Color.FromArgb(opacity, backgroundcolorUp);
					}
					else if (CrossBelow(RawTrend, 0.0, 1) || (RawTrend[0] < 0.0))
					{
						BackColorAll = Color.FromArgb(opacity, backgroundcolorDn);
					}
				}
			}
		}

		public override void Plot(Graphics graphics, Rectangle bounds, double min, double max)
		{
			base.Plot(graphics, bounds, min, max);
			DrawColorLine(graphics, bounds, min, max, Prediction, penUp, penDown);
		}

		private void SetWeights()
		{
			permissivity = Math.Max(0.1, Math.Min(((0.01 * Phase) + 1.5), 3.0));
			windowLength = 0.5 * (Period - 1);
			lengthParam = Math.Max(0.0, ((Math.Log(Math.Sqrt(windowLength)) / Math.Log(2.0)) + 2.0));
			expFactor = Math.Max(0.5, (lengthParam - 2.0));
			windowLength *= 0.9;
			double num = Math.Sqrt(windowLength) * lengthParam;
			beta = num / (num + 1.0);
		}
	}
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
	public partial class Indicator : IndicatorBase
	{
		private AbTrend[] cacheABTrend = null;

		private static AbTrend checkABTrend = new AbTrend();

		/// <summary>
		/// PTS trend indicator.
		/// </summary>
		/// <returns></returns>
		public AbTrend ABTrend(int period, double phase)
		{
			return ABTrend(Input, period, phase);
		}

		/// <summary>
		/// PTS trend indicator.
		/// </summary>
		/// <returns></returns>
		public AbTrend ABTrend(Data.IDataSeries input, int period, double phase)
		{
			if (cacheABTrend != null)
				for (int idx = 0; idx < cacheABTrend.Length; idx++)
					if (cacheABTrend[idx].Period == period && Math.Abs(cacheABTrend[idx].Phase - phase) <= double.Epsilon && cacheABTrend[idx].EqualsInput(input))
						return cacheABTrend[idx];

			lock (checkABTrend)
			{
				checkABTrend.Period = period;
				period = checkABTrend.Period;
				checkABTrend.Phase = phase;
				phase = checkABTrend.Phase;

				if (cacheABTrend != null)
					for (int idx = 0; idx < cacheABTrend.Length; idx++)
						if (cacheABTrend[idx].Period == period && Math.Abs(cacheABTrend[idx].Phase - phase) <= double.Epsilon && cacheABTrend[idx].EqualsInput(input))
							return cacheABTrend[idx];

				AbTrend indicator = new AbTrend();
				indicator.BarsRequired = BarsRequired;
				indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
				indicator.Input = input;
				indicator.Period = period;
				indicator.Phase = phase;
				Indicators.Add(indicator);
				indicator.SetUp();

				AbTrend[] tmp = new AbTrend[cacheABTrend == null ? 1 : cacheABTrend.Length + 1];
				if (cacheABTrend != null)
					cacheABTrend.CopyTo(tmp, 0);
				tmp[tmp.Length - 1] = indicator;
				cacheABTrend = tmp;
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
		/// PTS trend indicator.
		/// </summary>
		/// <returns></returns>
		[Gui.Design.WizardCondition("Indicator")]
		public Indicator.AbTrend ABTrend(int period, double phase)
		{
			return _indicator.ABTrend(Input, period, phase);
		}

		/// <summary>
		/// PTS trend indicator.
		/// </summary>
		/// <returns></returns>
		public Indicator.AbTrend ABTrend(Data.IDataSeries input, int period, double phase)
		{
			return _indicator.ABTrend(input, period, phase);
		}
	}
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
	public partial class Strategy : StrategyBase
	{
		/// <summary>
		/// PTS trend indicator.
		/// </summary>
		/// <returns></returns>
		[Gui.Design.WizardCondition("Indicator")]
		public Indicator.AbTrend ABTrend(int period, double phase)
		{
			return _indicator.ABTrend(Input, period, phase);
		}

		/// <summary>
		/// PTS trend indicator.
		/// </summary>
		/// <returns></returns>
		public Indicator.AbTrend ABTrend(Data.IDataSeries input, int period, double phase)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return _indicator.ABTrend(input, period, phase);
		}
	}
}
#endregion
