using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Serialization;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;

namespace NinjaTrader.Indicator
{
	[Description("")]
	public class Bf : Indicator
	{
		private	readonly	SolidBrush[]		brushes			= new SolidBrush[20];
		private				double				currentHigh;
		private				double				currentLow;
		private				string				fs;
		private				int					lastBarLo;
		private				int					lastBarHi;
		private				ToolStripMenuItem	m1;
		private				ToolStripMenuItem	m2;
		private				ToolStripSeparator	mSeparator;
		private				int					myxcoord;
		private	readonly	Pen[]				pens			= new Pen[20];
		private				double				price1			= 161.80;
		private				double				price2			= 150.00;
		private				double				price3			= 123.60;
		private				double				price4			= 100.0;
		private				double				price5			= 76.40;
		private				double				price6			= 61.80;
		private				double				price7			= 50.00;
		private				double				price8			= 38.20;
		private				double				price9			= 23.60;
		private				double				price10;
		private				double				price11			= -23.60;
		private				double				price12			= -50;
		private				double				price13;
		private				double				price14;
		private				double				price15;
		private				double				price16;
		private				double				price17;
		private				double				price18;
		private				double				price19;
		private				double				price20;
		private				double[]			prices;
		private				bool				showlabels		= true;
		private				int					starttime		= 90000;

		protected override void Initialize()
		{
			Add(new Plot(Color.Red,		"L01: 161.80%"));
			Add(new Plot(Color.Black,	"L02: 150.00%"));
			Add(new Plot(Color.Brown,	"L03: 123.60%"));
			Add(new Plot(Color.Yellow,	"L04: 100.00%"));
			Add(new Plot(Color.Blue,	"L05: 076.40%"));
			Add(new Plot(Color.Red,		"L06: 061.80%"));
			Add(new Plot(Color.Gold,	"L07: 050.00%"));
			Add(new Plot(Color.Red,		"L08: 038.20%"));
			Add(new Plot(Color.Blue,	"L09: 023.60%"));
			Add(new Plot(Color.Yellow,	"L10: 000.00%"));
			Add(new Plot(Color.Brown,	"L11: -023.60%"));
			Add(new Plot(Color.Black,	"L12: -050.00%"));

			Add(new Plot(Color.Transparent, "L13: 000.00%"));
			Add(new Plot(Color.Transparent, "L14: 000.00%"));
			Add(new Plot(Color.Transparent, "L15: 000.00%"));
			Add(new Plot(Color.Transparent, "L16: 000.00%"));
			Add(new Plot(Color.Transparent, "L17: 000.00%"));
			Add(new Plot(Color.Transparent, "L18: 000.00%"));
			Add(new Plot(Color.Transparent, "L19: 000.00%"));
			Add(new Plot(Color.Transparent, "L20: 000.00%"));

			CalculateOnBarClose		= false;
			DisplayInDataBox		= false;
			Overlay					= true;
			PaintPriceMarkers		= false;
		}

		protected override void OnStartUp()
		{
			if (ChartControl == null) 
				return;

			if (m1 == null) 
				m1 = new ToolStripMenuItem("Start from here");
			if (m2 == null) 
				m2 = new ToolStripMenuItem("Finish here");
			if (mSeparator == null) 
				mSeparator = new ToolStripSeparator();

			m1.Name				= "Start from here";
			m2.Name				= "Finish here";
			m1.Click			+= OnStartMenu;
			m2.Click			+= OnFinishMenu;
			mSeparator.Name		= "MySeparatorName";

			ChartControl.ChartPanel.MouseDown += OnMouseClick;

			if (!ChartControl.ContextMenuStrip.Items.ContainsKey("MySeparatorName"))
				ChartControl.ContextMenuStrip.Items.Insert(0, mSeparator);
			if (!ChartControl.ContextMenuStrip.Items.ContainsKey("Finish here"))
				ChartControl.ContextMenuStrip.Items.Insert(0, m2);
			if (!ChartControl.ContextMenuStrip.Items.ContainsKey("Start from here"))
				ChartControl.ContextMenuStrip.Items.Insert(0, m1);

			for (int i = 0; i < Plots.Length; i++)
			{
				pens[i] = Plots[i].Pen;
				if (showlabels) 
					brushes[i] = new SolidBrush(Plots[i].Pen.Color);
			}

			prices = new [] {	price1,		price2,		price3,		price4,		price5,		price6,		price7,		price8,		price9,		price10, 
								price11,	price12,	price13,	price14,	price15,	price16,	price17,	price18,	price19,	price20};

			Levels = new double[20];

			if (TickSize.ToString("0.#########").Length <= 2) 
				fs = "0";
			else 
				switch (TickSize.ToString("0.#########").Length)
				{
					case 3	:	fs = "0.0";			break;
					case 4	:	fs = "0.00";		break;
					case 5	:	fs = "0.000";		break;
					case 6	:	fs = "0.0000";		break;
					case 7	:	fs = "0.00000";		break;
					default	:	fs = "0.000000";	break;
				}
		}

		protected override void OnBarUpdate()
		{
			if (ChartControl != null && ChartControl.BarMarginRight < 125 && showlabels)
				ChartControl.BarMarginRight = 125;

			if (CurrentBar == 0 || Finishbar > 0) 
				return;

			if (ToTime(Time[0]) > starttime && ToTime(Time[1]) <= starttime)
			{
				Startbar	= CurrentBar;
				Finishbar	= -1;
			}

			CalculateLevels(new[] { true, false });
		}

		public override void Plot(Graphics graphics, Rectangle bounds, double min, double max)
		{
			if (Bars == null || ChartControl == null) 
				return;

			int x				= ChartControl.GetXByBarIdx(BarsArray[0], Startbar);
			int x1				= ChartControl.GetXByBarIdx(BarsArray[0], lastBarHi);
			int x2				= ChartControl.GetXByBarIdx(BarsArray[0], lastBarLo);
			int y1				= ChartControl.GetYByValue(this, currentHigh);
			int y2				= ChartControl.GetYByValue(this, currentLow);
			int textXPosition	= ChartControl.CanvasRight - ChartControl.BarMarginRight - 20;

			graphics.DrawLine(new Pen(Color.DarkGray), x1, y1, x2, y2);

			for (int i = 0; i < Plots.Length; i++)
			{
				var y = ChartControl.GetYByValue(this, Levels[i]);
				graphics.DrawLine(pens[i], x, y, ChartControl.CanvasRight - ChartControl.BarMarginRight, y);
				if (!showlabels) 
					continue;
				var fibname = string.Format("{0}%  {1}", prices[i].ToString("0.00"), Levels[i].ToString(fs));
				graphics.DrawString(fibname, ChartControl.Font, brushes[i], textXPosition, y - ChartControl.Font.GetHeight() - 1);
			}
		}

		public int GetBar(int x)
		{
			if (Bars.Count == 0) 
				return -1;

			int i2 = ChartControl.ChartStyle.GetBarPaintWidth(ChartControl.BarWidth);
			return Math.Min(Bars.Count - 1, (ChartControl.FirstBarPainted + ((x - ((((ChartControl.CanvasRight - ChartControl.BarMarginRight) - (i2 / 2)) - (ChartControl.BarSpace / 2)) - ((ChartControl.BarsPainted - 1) * ChartControl.BarSpace))) / ChartControl.BarSpace)) - Math.Max(0, ChartControl.BarsPainted - (ChartControl.LastBarPainted + 1)));
		}

		private void OnMouseClick(object sender, MouseEventArgs e)
		{
			TriggerCustomEvent(GrabCoordinates, e);
		}

		private void GrabCoordinates(object state)
		{
			myxcoord = ((MouseEventArgs)state).X;

		}

		public void OnStartMenu(object sender, EventArgs e)
		{
			TriggerCustomEvent(CalculateLevels, new [] { true, true });
			ChartControl.Invalidate();
		}

		public void OnFinishMenu(object sender, EventArgs e)
		{
			TriggerCustomEvent(CalculateLevels, new [] { false, true });
			ChartControl.Invalidate();
		}

		private void CalculateLevels(object state)
		{
			var start	= ((bool[])state)[0];
			var ev		= ((bool[])state)[1];
			if (ev)
				if (start)
					Startbar = GetBar(myxcoord);
				else
				{
					Finishbar = GetBar(myxcoord);
					if (Finishbar == CurrentBar) Finishbar = -1;
				}

			if (Finishbar > 0)
			{
				int sb			= Math.Min(Startbar, Finishbar);
				int fb			= Math.Max(Startbar, Finishbar);
				Startbar		= sb;
				Finishbar		= fb;
				currentHigh		= 0;
				currentLow		= double.MaxValue;
				for (int i = Startbar; i <= Finishbar; i++)
				{
					if (High[CurrentBar - i] > currentHigh)
					{
						currentHigh		= High[CurrentBar - i];
						lastBarHi		= i;
					}
					if (Low[CurrentBar - i] < currentLow)
					{
						currentLow		= Low[CurrentBar - i];
						lastBarLo		= i;
					}
				}
			}
			else
			{
				int hb		= HighestBar(High, CurrentBar - Startbar + 1);
				int lb		= LowestBar(Low, CurrentBar - Startbar + 1);
				lastBarHi	= CurrentBar - hb;
				lastBarLo	= CurrentBar - lb;
				currentHigh	= High[hb];
				currentLow	= Low[lb];
			}

			var difference = (lastBarHi > lastBarLo) ? (currentLow - currentHigh) : (currentHigh - currentLow);
			var startpoint = (lastBarHi > lastBarLo) ? currentHigh : currentLow;

			for (int i = 0; i < Levels.Length; i++)
				Levels[i] = startpoint + difference * prices[i] * 0.01;

			if (ev) 
				ChartControl.Invalidate(true);
		}

		protected override void OnTermination()
		{
			m1 = m2		= null;
			mSeparator	= null;

			if (ChartControl == null) 
				return;

			for (int i = 0; i < Plots.Length; i++)
			{
				pens[i]		.Dispose();
				brushes[i]	.Dispose();
			}

			ChartControl.ContextMenuStrip.Items.RemoveByKey("Start from here");
			ChartControl.ContextMenuStrip.Items.RemoveByKey("Finish here");
			ChartControl.ContextMenuStrip.Items.RemoveByKey("MySeparatorName");
			ChartControl.ChartPanel.MouseDown -= OnMouseClick;
		}

		[Description("If true will display fib level labels")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayName("Display Labels")]
		public bool ShowLabels
		{
			get { return showlabels; }
			set { showlabels = value; }
		}

		[Description("Start Time")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayName("Start Time")]
		public int StartTime
		{
			get { return starttime; }
			set { starttime = Math.Max(0, value); }
		}

		[GridCategory("Levels")]
		[Gui.Design.DisplayName("Level 01")]
		[RefreshProperties(RefreshProperties.All)]
		public double Retracement1
		{
			get { return price1; }
			set
			{
				price1 = value;
				if (Plots == null || Plots.Length < 1) return;
				Plots[0].Name = string.Format("L01: {0:000.00}%", value);
			}
		}

		[GridCategory("Levels")]
		[Gui.Design.DisplayName("Level 02")]
		[RefreshProperties(RefreshProperties.All)]
		public double Retracement2
		{
			get { return price2; }
			set
			{
				price2 = value;
				if (Plots == null || Plots.Length < 2) return;
				Plots[1].Name = string.Format("L02: {0:000.00}%", value);
			}
		}

		[GridCategory("Levels")]
		[Gui.Design.DisplayName("Level 03")]
		[RefreshProperties(RefreshProperties.All)]
		public double Retracement3
		{
			get { return price3; }
			set
			{
				price3 = value;
				if (Plots == null || Plots.Length < 3) return;
				Plots[2].Name = string.Format("L03: {0:000.00}%", value);
			}
		}

		[GridCategory("Levels")]
		[Gui.Design.DisplayName("Level 04")]
		[RefreshProperties(RefreshProperties.All)]
		public double Retracement4
		{
			get { return price4; }
			set
			{
				price4 = value;
				if (Plots == null || Plots.Length < 4) return;
				Plots[3].Name = string.Format("L04: {0:000.00}%", value);
			}
		}

		[GridCategory("Levels")]
		[Gui.Design.DisplayName("Level 05")]
		[RefreshProperties(RefreshProperties.All)]
		public double Retracement5
		{
			get { return price5; }
			set
			{
				price5 = value;
				if (Plots == null || Plots.Length < 5) return;
				Plots[4].Name = string.Format("L05: {0:000.00}%", value);
			}
		}

		[GridCategory("Levels")]
		[Gui.Design.DisplayName("Level 06")]
		[RefreshProperties(RefreshProperties.All)]
		public double Retracement6
		{
			get { return price6; }
			set
			{
				price6 = value;
				if (Plots == null || Plots.Length < 6) return;
				Plots[5].Name = string.Format("L06: {0:000.00}%", value);
			}
		}

		[GridCategory("Levels")]
		[Gui.Design.DisplayName("Level 07")]
		[RefreshProperties(RefreshProperties.All)]
		public double Retracement7
		{
			get { return price7; }
			set
			{
				price7 = value;
				if (Plots == null || Plots.Length < 7) return;
				Plots[6].Name = string.Format("L07: {0:000.00}%", value);
			}
		}

		[GridCategory("Levels")]
		[Gui.Design.DisplayName("Level 08")]
		[RefreshProperties(RefreshProperties.All)]
		public double Retracement8
		{
			get { return price8; }
			set
			{
				price8 = value;
				if (Plots == null || Plots.Length < 8) return;
				Plots[7].Name = string.Format("L08: {0:000.00}%", value);
			}
		}

		[GridCategory("Levels")]
		[Gui.Design.DisplayName("Level 09")]
		[RefreshProperties(RefreshProperties.All)]
		public double Retracement9
		{
			get { return price9; }
			set
			{
				price9 = value;
				if (Plots == null || Plots.Length < 9) return;
				Plots[8].Name = string.Format("L09: {0:000.00}%", value);
			}
		}

		[GridCategory("Levels")]
		[Gui.Design.DisplayName("Level 10")]
		[RefreshProperties(RefreshProperties.All)]
		public double Retracement10
		{
			get { return price10; }
			set
			{
				price10 = value;
				if (Plots == null || Plots.Length < 10) return;
				Plots[9].Name = string.Format("L10: {0:000.00}%", value);
			}
		}

		[GridCategory("Levels")]
		[Gui.Design.DisplayName("Level 11")]
		[RefreshProperties(RefreshProperties.All)]
		public double Retracement11
		{
			get { return price11; }
			set
			{
				price11 = value;
				if (Plots == null || Plots.Length < 11) return;
				Plots[10].Name = string.Format("L11: {0:000.00}%", value);
			}
		}

		[GridCategory("Levels")]
		[Gui.Design.DisplayName("Level 12")]
		[RefreshProperties(RefreshProperties.All)]
		public double Retracement12
		{
			get { return price12; }
			set
			{
				price12 = value;
				if (Plots == null || Plots.Length < 12) return;
				Plots[11].Name = string.Format("L12: {0:000.00}%", value);
			}
		}

		[GridCategory("Levels")]
		[Gui.Design.DisplayName("Level 13")]
		[RefreshProperties(RefreshProperties.All)]
		public double Retracement13
		{
			get { return price13; }
			set
			{
				price13 = value;
				if (Plots == null || Plots.Length < 13) return;
				Plots[12].Name = string.Format("L13: {0:000.00}%", value);
			}
		}

		[GridCategory("Levels")]
		[Gui.Design.DisplayName("Level 14")]
		[RefreshProperties(RefreshProperties.All)]
		public double Retracement14
		{
			get { return price14; }
			set
			{
				price14 = value;
				if (Plots == null || Plots.Length < 14) return;
				Plots[13].Name = string.Format("L14: {0:000.00}%", value);
			}
		}

		[GridCategory("Levels")]
		[Gui.Design.DisplayName("Level 15")]
		[RefreshProperties(RefreshProperties.All)]
		public double Retracement15
		{
			get { return price15; }
			set
			{
				price15 = value;
				if (Plots == null || Plots.Length < 15) return;
				Plots[14].Name = string.Format("L15: {0:000.00}%", value);
			}
		}

		[GridCategory("Levels")]
		[Gui.Design.DisplayName("Level 16")]
		[RefreshProperties(RefreshProperties.All)]
		public double Retracement16
		{
			get { return price16; }
			set
			{
				price16 = value;
				if (Plots == null || Plots.Length < 16) return;
				Plots[15].Name = string.Format("L16: {0:000.00}%", value);
			}
		}

		[GridCategory("Levels")]
		[Gui.Design.DisplayName("Level 17")]
		[RefreshProperties(RefreshProperties.All)]
		public double Retracement17
		{
			get { return price17; }
			set
			{
				price17 = value;
				if (Plots == null || Plots.Length < 17) return;
				Plots[16].Name = string.Format("L17: {0:000.00}%", value);
			}
		}

		[GridCategory("Levels")]
		[Gui.Design.DisplayName("Level 18")]
		[RefreshProperties(RefreshProperties.All)]
		public double Retracement18
		{
			get { return price18; }
			set
			{
				price18 = value;
				if (Plots == null || Plots.Length < 18) return;
				Plots[17].Name = string.Format("L18: {0:000.00}%", value);
			}
		}

		[GridCategory("Levels")]
		[Gui.Design.DisplayName("Level 19")]
		[RefreshProperties(RefreshProperties.All)]
		public double Retracement19
		{
			get { return price19; }
			set
			{
				price19 = value;
				if (Plots == null || Plots.Length < 19) return;
				Plots[18].Name = string.Format("L19: {0:000.00}%", value);
			}
		}

		[GridCategory("Levels")]
		[Gui.Design.DisplayName("Level 20")]
		[RefreshProperties(RefreshProperties.All)]
		public double Retracement20
		{
			get { return price20; }
			set
			{
				price20 = value;
				if (Plots == null || Plots.Length < 20) return;
				Plots[19].Name = string.Format("L20: {0:000.00}%", value);
			}
		}

		[Browsable(false), XmlIgnore]
		public double[] Levels { get; private set; }

		[Browsable(false), XmlIgnore]
		public int Startbar { get; private set; }

		[Browsable(false), XmlIgnore]
		public int Finishbar { get; private set; }
	}
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
	public partial class Indicator : IndicatorBase
	{
		private Bf[] cacheBF = null;

		private static Bf checkBF = new Bf();

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public Bf BF(bool showLabels, int startTime)
		{
			return BF(Input, showLabels, startTime);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public Bf BF(Data.IDataSeries input, bool showLabels, int startTime)
		{
			if (cacheBF != null)
				for (int idx = 0; idx < cacheBF.Length; idx++)
					if (cacheBF[idx].ShowLabels == showLabels && cacheBF[idx].StartTime == startTime && cacheBF[idx].EqualsInput(input))
						return cacheBF[idx];

			lock (checkBF)
			{
				checkBF.ShowLabels = showLabels;
				showLabels = checkBF.ShowLabels;
				checkBF.StartTime = startTime;
				startTime = checkBF.StartTime;

				if (cacheBF != null)
					for (int idx = 0; idx < cacheBF.Length; idx++)
						if (cacheBF[idx].ShowLabels == showLabels && cacheBF[idx].StartTime == startTime && cacheBF[idx].EqualsInput(input))
							return cacheBF[idx];

				Bf indicator = new Bf();
				indicator.BarsRequired = BarsRequired;
				indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
				indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
				indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
				indicator.Input = input;
				indicator.ShowLabels = showLabels;
				indicator.StartTime = startTime;
				Indicators.Add(indicator);
				indicator.SetUp();

				Bf[] tmp = new Bf[cacheBF == null ? 1 : cacheBF.Length + 1];
				if (cacheBF != null)
					cacheBF.CopyTo(tmp, 0);
				tmp[tmp.Length - 1] = indicator;
				cacheBF = tmp;
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
		public Indicator.Bf BF(bool showLabels, int startTime)
		{
			return _indicator.BF(Input, showLabels, startTime);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public Indicator.Bf BF(Data.IDataSeries input, bool showLabels, int startTime)
		{
			return _indicator.BF(input, showLabels, startTime);
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
		public Indicator.Bf BF(bool showLabels, int startTime)
		{
			return _indicator.BF(Input, showLabels, startTime);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public Indicator.Bf BF(Data.IDataSeries input, bool showLabels, int startTime)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return _indicator.BF(input, showLabels, startTime);
		}
	}
}
#endregion
