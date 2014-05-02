using System;
using System.ComponentModel;
using System.Drawing;
using System.Xml.Serialization;
using CCIHistogramUtilities;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.Design;

namespace NinjaTrader.Indicator
{
	/// <summary>
	///     The Commodity Channel Index (CCI) measures the variation of a security's price from its statistical mean. High
	///     values show that prices are unusually high compared to average prices whereas low values indicate that prices are
	///     unusually low.
	/// </summary>
	[Description("The Commodity Channel Index (CCI) measures the variation of a security's price from its statistical mean. High values show that prices are unusually high compared to average prices whereas low values indicate that prices are unusually low.")]
	[Gui.Design.DisplayName("CCI_Histogram_v001 (Commodity Channel Index)")]
	// ReSharper disable once InconsistentNaming
	public class CCI_Histogram : Indicator
	{
		private	const	double					multiplier			= 2.618;
		private	const	int						smooth				= 14;
		private	const	int						length				= 14;
		private			int						objDisplacement		= 7;
		private			EnumObjectOfTypev001	objectOfType		= EnumObjectOfTypev001.Arrow;
		private			double					offset;
		private			int						period				= 20;

		public CCI_Histogram()
		{
			Overbought		= Color.DarkBlue;
			Oversold3		= Color.Magenta;
			Oversold2		= Color.Magenta;
			Overbought3		= Color.Lime;
			Overbought2		= Color.Lime;
			Oversold		= Color.DarkRed;
		}

		protected override void Initialize()
		{
			Add(new Plot(new Pen(Color.Lime, 2),		PlotStyle.Line, "CCIoverbought3")); // Upper = Overbought (>+300)
			Add(new Plot(new Pen(Color.Lime, 2),		PlotStyle.Line, "CCIoverbought2")); // Upper = Overbought (>+200)
			Add(new Plot(new Pen(Color.DarkBlue, 2),	PlotStyle.Line, "CCIoverbought"));	// Upper = Overbought (>+100)
			Add(new Plot(new Pen(Color.Yellow, 2),		PlotStyle.Line, "CCImiddle"));		// Between margins.
			Add(new Plot(new Pen(Color.DarkRed, 2),		PlotStyle.Line, "CCIoversold"));	// Lower = Oversold   (<-100)
			Add(new Plot(new Pen(Color.Magenta, 2),		PlotStyle.Line, "CCIoversold2"));	// Lower = Oversold   (<-200)
			Add(new Plot(new Pen(Color.Magenta, 2),		PlotStyle.Line, "CCIoversold3"));	// Lower = Oversold   (<-200)
			Add(new Line(new Pen(Color.Gray, 1),	300,	"Level3"));
			Add(new Line(new Pen(Color.Gray, 1),	200,	"Level2"));
			Add(new Line(new Pen(Color.Gray, 1),	100,	"Level1"));
			Add(new Line(new Pen(Color.Black, 2),	0,		"ZeroLine"));
			Add(new Line(new Pen(Color.Gray, 1),	-100,	"LevelM1"));
			Add(new Line(new Pen(Color.Gray, 1),	-200,	"LevelM2"));
			Add(new Line(new Pen(Color.Gray, 1),	-300,	"LevelM3"));

			Overlay = false;
			PriceTypeSupported = true;
		}

		protected override void OnBarUpdate()
		{
			double sma = SMA(smooth)[0];
			double mean = 0;
			for (int idx = Math.Min(CurrentBar, Period - 1); idx >= 0; idx--)
				mean += Math.Abs(Typical[idx] - SMA(Typical, Period)[0]);
			double plotval = (Typical[0] - SMA(Typical, Period)[0])/ (Math.Abs(mean) < double.Epsilon ? 1 : (0.015 * (mean/Math.Min(Period, CurrentBar + 1))));

			offset = ATR(length)[0] * multiplier;

			CcImiddle.Set(plotval);
			if (plotval >= 300)			PlotColors[3][0] = Plots[0].Pen.Color;
			else if (plotval >= 200)	PlotColors[3][0] = Plots[1].Pen.Color;
			else if (plotval >= 100)	PlotColors[3][0] = Plots[2].Pen.Color;
			else if (plotval <= -300)	PlotColors[3][0] = Plots[6].Pen.Color;
			else if (plotval <= -200)	PlotColors[3][0] = Plots[5].Pen.Color;
			else if (plotval <= -100)	PlotColors[3][0] = Plots[4].Pen.Color;

			if (plotval >= 300 && CcIoverbought3[0] >= 300)
			{
				if (!ShowObjects) return;
				double val = sma - offset;
				switch (ObjectOfType)
				{
					case EnumObjectOfTypev001.Arrow		: DrawArrowUp(string.Format("A3{0}", CurrentBar), true, 0, val, Overbought3);		break;
					case EnumObjectOfTypev001.Diamond	: DrawDiamond(string.Format("DD3{0}", CurrentBar), true, 0, val, Overbought3);		break;
					case EnumObjectOfTypev001.Dot		: DrawDot(string.Format("D3{0}", CurrentBar), true, 0, val, Overbought3);			break;
					case EnumObjectOfTypev001.Square	: DrawSquare(string.Format("S3{0}", CurrentBar), true, 0, val, Overbought3);		break;
					case EnumObjectOfTypev001.Triangle	: DrawTriangleUp(string.Format("T3{0}", CurrentBar), true, 0, val, Overbought3);	break;
				}
			}
			else if (plotval >= 200 && CcIoverbought2[0] <= 299)
			{
				if (!ShowObjects) return;
				double val = sma - offset;
				switch (ObjectOfType)
				{
					case EnumObjectOfTypev001.Arrow		: DrawArrowUp(string.Format("A2{0}", CurrentBar), true, 0, val, Overbought2);		break;
					case EnumObjectOfTypev001.Diamond	: DrawDiamond(string.Format("DD2{0}", CurrentBar), true, 0, val, Overbought2);		break;
					case EnumObjectOfTypev001.Dot		: DrawDot(string.Format("D2{0}", CurrentBar), true, 0, val, Overbought2);			break;
					case EnumObjectOfTypev001.Square	: DrawSquare(string.Format("S2{0}", CurrentBar), true, 0, val, Overbought2);		break;
					case EnumObjectOfTypev001.Triangle	: DrawTriangleUp(string.Format("T2{0}", CurrentBar), true, 0, val, Overbought2);	break;
				}
			}
			else if (plotval >= 100 && CcIoverbought[0] <= 199)
			{
				if (!ShowObjects) return;
				double val = sma - offset;
				switch (ObjectOfType)
				{
					case EnumObjectOfTypev001.Arrow		: DrawArrowUp(string.Format("A1{0}", CurrentBar), true, 0, val, Overbought);	break;
					case EnumObjectOfTypev001.Diamond	: DrawDiamond(string.Format("DD1{0}", CurrentBar), true, 0, val, Overbought);	break;
					case EnumObjectOfTypev001.Dot		: DrawDot(string.Format("D1{0}", CurrentBar), true, 0, val, Overbought);		break;
					case EnumObjectOfTypev001.Square	: DrawSquare(string.Format("S1{0}", CurrentBar), true, 0, val, Overbought);		break;
					case EnumObjectOfTypev001.Triangle	: DrawTriangleUp(string.Format("T1{0}", CurrentBar), true, 0, val, Overbought);	break;
				}
			}
			else if (plotval <= -100 && CcIoversold[0] >= -199)
			{
				if (!ShowObjects) return;
				double val = sma + offset;
				switch (ObjectOfType)
				{
					case EnumObjectOfTypev001.Arrow		: DrawArrowDown("A1" + CurrentBar, true, 0, val, Oversold);						break;
					case EnumObjectOfTypev001.Diamond	: DrawDiamond("DD1" + CurrentBar, true, 0, val, Oversold);						break;
					case EnumObjectOfTypev001.Dot		: DrawDot("D1" + CurrentBar, true, 0, val, Oversold);							break;
					case EnumObjectOfTypev001.Square	: DrawSquare(string.Format("S1{0}", CurrentBar), true, 0, val, Oversold);		break;
					case EnumObjectOfTypev001.Triangle	: DrawTriangleDown(string.Format("T1{0}", CurrentBar), true, 0, val, Oversold);	break;
				}
			}
			else if (plotval <= -200 && CcIoversold2[0] >= -299)
			{
				if (!ShowObjects) return;
				double val = sma + offset;
				switch (ObjectOfType)
				{
					case EnumObjectOfTypev001.Arrow		: DrawArrowDown(string.Format("A2{0}", CurrentBar), true, 0, val, Oversold2);		break;
					case EnumObjectOfTypev001.Diamond	: DrawDiamond(string.Format("DD2{0}", CurrentBar), true, 0, val, Oversold2);		break;
					case EnumObjectOfTypev001.Dot		: DrawDot(string.Format("D2{0}", CurrentBar), true, 0, val, Oversold2);				break;
					case EnumObjectOfTypev001.Square	: DrawSquare(string.Format("S2{0}", CurrentBar), true, 0, val, Oversold2);			break;
					case EnumObjectOfTypev001.Triangle	: DrawTriangleDown(string.Format("T2{0}", CurrentBar), true, 0, val, Oversold2);	break;
				}
			}
			else if (plotval <= -300 && CcIoversold3[0] <= -300)
			{
				if (!ShowObjects) return;
				double val = sma + offset;
				switch (ObjectOfType)
				{
					case EnumObjectOfTypev001.Arrow		: DrawArrowDown(string.Format("A3{0}", CurrentBar), true, 0, val, Oversold3);		break;
					case EnumObjectOfTypev001.Diamond	: DrawDiamond(string.Format("DD3{0}", CurrentBar), true, 0, val, Oversold3);		break;
					case EnumObjectOfTypev001.Dot		: DrawDot(string.Format("D3{0}", CurrentBar), true, 0, val, Oversold3);				break;
					case EnumObjectOfTypev001.Square	: DrawSquare(string.Format("S3{0}", CurrentBar), true, 0, val, Oversold3);			break;
					case EnumObjectOfTypev001.Triangle	: DrawTriangleDown(string.Format("T3{0}", CurrentBar), true, 0, val, Oversold3);	break;
				}
			}
		}

		[Description("DisplayObjectType: Arrow, Dot, Diamond, Square, Triangle")]
		[GridCategory("Parameters")]
		public EnumObjectOfTypev001 ObjectOfType
		{
			get { return objectOfType; }
			set { objectOfType = value; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries CcIoverbought3 { get { return Values[0]; } }

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries CcIoverbought2 { get { return Values[1]; } }

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries CcIoverbought { get { return Values[2]; } }

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries CcImiddle { get { return Values[3]; } }

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries CcIoversold { get { return Values[4]; } }

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries CcIoversold2 { get { return Values[5]; } }

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries CcIoversold3 { get { return Values[6]; } }

		[Description("Numbers of bars used for calculations")]
		[GridCategory("Parameters")]
		public int Period
		{
			get { return period; }
			set { period = Math.Max(1, value); }
		}

		[Description("Show Objects")]
		[Category("Drawing Objects")]
		public bool ShowObjects { get; set; }

		[Description("Object Displacement above/below bars")]
		[Category("Objects Displacement")]
		public int ObjectDisplacement
		{
			get { return objDisplacement; }
			set { objDisplacement = Math.Max(0, value); }
		}

		[XmlIgnore, Description("Object Overbought(100-199)")]
		[Category("Visual")]
		[Gui.Design.DisplayName("Object Overbought(100-199)")]
		public Color Overbought { get; set; }

		[Browsable(false)]
		public string OverboughtSerialize
		{
			get { return SerializableColor.ToString(Overbought); }
			set { Overbought = SerializableColor.FromString(value); }
		}

		[XmlIgnore, Description("Object Oversold(-100- -199)")]
		[Category("Visual")]
		[Gui.Design.DisplayName("Object Oversold(-100- -199)")]
		public Color Oversold { get; set; }

		[Browsable(false)]
		public string OversoldSerialize
		{
			get { return SerializableColor.ToString(Oversold); }
			set { Oversold = SerializableColor.FromString(value); }
		}

		[XmlIgnore, Description("Object Overbought2(200+)")]
		[Category("Visual")]
		[Gui.Design.DisplayName("Object Overbought2(200+)")]
		public Color Overbought2 { get; set; }

		[Browsable(false)]
		public string Overbought2Serialize
		{
			get { return SerializableColor.ToString(Overbought2); }
			set { Overbought2 = SerializableColor.FromString(value); }
		}

		[XmlIgnore, Description("Object Overbought3(300+)")]
		[Category("Visual")]
		[Gui.Design.DisplayName("Object Overbought3(300+)")]
		public Color Overbought3 { get; set; }

		[Browsable(false)]
		public string Overbought3Serialize
		{
			get { return SerializableColor.ToString(Overbought3); }
			set { Overbought3 = SerializableColor.FromString(value); }
		}

		[XmlIgnore, Description("Object Oversold2(-200- -299)")]
		[Category("Visual")]
		[Gui.Design.DisplayName("Object Oversold2(-200 - -299)")]
		public Color Oversold2 { get; set; }

		[Browsable(false)]
		public string Oversold2Serialize
		{
			get { return SerializableColor.ToString(Oversold2); }
			set { Oversold2 = SerializableColor.FromString(value); }
		}

		[XmlIgnore, Description("Object Oversold3(-300+)")]
		[Category("Visual")]
		[Gui.Design.DisplayName("Object Oversold3(-300+)")]
		public Color Oversold3 { get; set; }

		[Browsable(false)]
		public string Oversold3Serialize
		{
			get { return SerializableColor.ToString(Oversold3); }
			set { Oversold3 = SerializableColor.FromString(value); }
		}
	}
}

namespace CCIHistogramUtilities
{
	public enum EnumObjectOfTypev001
	{
		Arrow,
		Dot,
		Diamond,
		Square,
		Triangle,
	}
}
