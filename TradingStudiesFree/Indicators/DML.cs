using System;
using System.Drawing;
using System.ComponentModel;
using System.Xml.Serialization;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
using DMLUtilities;

namespace DMLUtilities
{
	public enum SignalType
	{
// ReSharper disable InconsistentNaming
		PLOT,
		BARCOLOR,
		BACKGROUNDCOLOR
// ReSharper restore InconsistentNaming
	}
}


namespace NinjaTrader.Indicator
{
	[Description("DML")]
// ReSharper disable once InconsistentNaming
	public class DML : Indicator
	{

		private SignalType st4 = SignalType.PLOT;
		private SignalType st5 = SignalType.PLOT;
		private SignalType st2 = SignalType.PLOT;
		private SignalType st67 = SignalType.PLOT;

		private Color barColorDown = Color.Red;
		private Color barColorUp = Color.Lime;
		private SolidBrush brushDown = null;
		private SolidBrush brushUp = null;
		private SolidBrush b4 = null;
		private SolidBrush b5 = null;
		private SolidBrush b2 = null;
		private SolidBrush b67 = null;
		private Color shadowColor = Color.Black;
		private Pen shadowPen = null;
		private int shadowWidth = 1;

		private DataSeries hao, hah, hal, hac;
		private BoolSeries c4, c5, c2, c67;
		private bool playAlert;

		protected override void Initialize()
		{
			Add(new Plot(new Pen(Color.Brown, 3),		PlotStyle.Dot, "Condition4"));
			Add(new Plot(new Pen(Color.DarkGreen, 3),	PlotStyle.Dot, "Condition5"));
			Add(new Plot(new Pen(Color.DarkRed, 3),		PlotStyle.Dot, "Condition2"));
			Add(new Plot(new Pen(Color.Cyan, 3),		PlotStyle.Dot, "Condition67"));

			hao		= new DataSeries(this, MaximumBarsLookBack.Infinite);
			hah		= new DataSeries(this, MaximumBarsLookBack.Infinite);
			hal		= new DataSeries(this, MaximumBarsLookBack.Infinite);
			hac		= new DataSeries(this, MaximumBarsLookBack.Infinite);

			c4		= new BoolSeries(this, MaximumBarsLookBack.Infinite);
			c5		= new BoolSeries(this, MaximumBarsLookBack.Infinite);
			c2		= new BoolSeries(this, MaximumBarsLookBack.Infinite);
			c67		= new BoolSeries(this, MaximumBarsLookBack.Infinite);


			PaintPriceMarkers = false;
			CalculateOnBarClose = false;
			Overlay = true;
		}

		protected override void OnBarUpdate()
		{
			if (ChartControl == null || Bars == null)
				return;
			
			if (Displacement + (CalculateOnBarClose ? 1 : 0) > 0 && CurrentBar > 0 && BarColorSeries[1] != Color.Transparent)
				InitColorSeries();

			BarColorSeries.Set(Math.Max(0, CurrentBar + Math.Max(0, Displacement) + (CalculateOnBarClose ? 1 : 0)), Color.Transparent);
			CandleOutlineColorSeries.Set(Math.Max(0, CurrentBar + Math.Max(0, Displacement) + (CalculateOnBarClose ? 1 : 0)), Color.Transparent);
			Condition4.Reset();
			Condition5.Reset();
			Condition2.Reset();
			Condition67.Reset();
			if (CurrentBar == 0)
			{
				hao.Set(Open[0]);
				hah.Set(High[0]);
				hal.Set(Low[0]);
				hac.Set(Close[0]);
				return;
			}

			hac.Set((Open[0] + High[0] + Low[0] + Close[0]) * 0.25); // Calculate the close
			hao.Set((hao[1] + hac[1]) * 0.5); // Calculate the open
			hah.Set(Math.Max(High[0], hao[0])); // Calculate the high
			hal.Set(Math.Min(Low[0], hao[0])); // Calculate the low
			if (CurrentBar < 3) return;
			bool condition1 = (ZLEMA(7)[0] < hao[0] || ZLEMA(7)[0] < Median[0]) && ZLEMA(7)[0] < ZLEMA(21)[1] && hac[0] < ZLEMA(21)[2]
				&& (MACD(12, 26, 9).Diff[0] < 0 || (hac[0] < ZLEMA(21)[2] && hao[0] < ZLEMA(21)[2]) || Close[0] < SMA(21)[0] || ZLEMA(7)[0] < ZLEMA(21)[2]);

			bool condition2 = (ZLEMA(7)[0] > hao[0] || ZLEMA(7)[0] > Median[0]) && ZLEMA(7)[0] > ZLEMA(21)[1] && hac[0] > ZLEMA(21)[2]
				&& (MACD(12, 26, 9).Diff[0] > 0 || (hac[0] > ZLEMA(21)[2] && hao[0] > ZLEMA(21)[2]) || Close[0] > SMA(21)[0] || ZLEMA(7)[0] > ZLEMA(21)[2]);

			bool condition3 = (Low[0] + ((High[0] - Low[0]) * 0.1)) < SMA(21)[0] && (High[0] - ((High[0] - Low[0]) * 0.1)) < SMA(21)[0] && ZLEMA(7)[0] < ZLEMA(7)[1] && ZLEMA(11)[0] < hao[0];

			bool condition4 = condition1 && condition3;

			bool condition5 = condition1 || condition3;

			bool condition6 = ZLEMA(7)[0] < ZLEMA(7)[1] && ZLEMA(11)[0] < ZLEMA(11)[1] && ZLEMA(11)[0] < SMA(21)[0] && hac[0] < hao[0] && Close[0] < Median[0];

			bool condition7 = hac[0] < SMA(21)[0] && ZLEMA(7)[0] < ZLEMA(7)[1] && High[0] < SMA(78)[0] && SMA(21)[0] < SMA(78)[0] && SMA(78)[0] < SMA(78)[3];

			c4.Set(condition4);
			c5.Set(condition5);
			c2.Set(condition2);
			c67.Set(condition6 || condition7);
			
			if (condition6 || condition7) 
			{
				if (st67 == SignalType.PLOT) Condition67.Set(hah[0] + 3 * TickSize);
				else if (st67 == SignalType.BACKGROUNDCOLOR) BackColor = Plots[3].Pen.Color;
			}
			if (condition2)
			{
				if (st2 == SignalType.PLOT) Condition2.Set(hal[0] - TickSize);
				else if (st2 == SignalType.BACKGROUNDCOLOR) BackColor = Plots[2].Pen.Color;
			}
			if (condition5)
			{
				if (st5 == SignalType.PLOT) Condition5.Set(hah[0] + 2 * TickSize);
				else if (st5 == SignalType.BACKGROUNDCOLOR) BackColor = Plots[1].Pen.Color;
			}
			if (condition4)
			{
				if (st4 == SignalType.PLOT) Condition4.Set(hah[0] + TickSize);
				else if (st4 == SignalType.BACKGROUNDCOLOR) BackColor = Plots[0].Pen.Color;
			}

			if (FirstTickOfBar && c4[1] && !c4[2] && playAlert)
				PlaySound("Aler4.wav");
		
		}

		#region Properties
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries Condition4
		{
			get { return Values[0]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries Condition5
		{
			get { return Values[1]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries Condition2
		{
			get { return Values[2]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries Condition67
		{
			get { return Values[3]; }
		}

		[Description("Condition 4 Signal Type")]
		[Category("Visual")]
		[Gui.Design.DisplayNameAttribute("1. Condition4 Show")]
		public SignalType St4
		{
			get { return st4; }
			set { st4 = value; }
		}

		[Description("Condition 5 Signal Type")]
		[Category("Visual")]
		[Gui.Design.DisplayNameAttribute("2. Condition5 Show")]
		public SignalType St5
		{
			get { return st5; }
			set { st5 = value; }
		}

		[Description("Condition 2 Signal Type")]
		[Category("Visual")]
		[Gui.Design.DisplayNameAttribute("3. Condition2 Show")]
		public SignalType St2
		{
			get { return st2; }
			set { st2 = value; }
		}

		[Description("Condition 6 or7 Signal Type")]
		[Category("Visual")]
		[Gui.Design.DisplayNameAttribute("4. Condition67 Show")]
		public SignalType St67
		{
			get { return st67; }
			set { st67 = value; }
		}

		[Description("Play Alert?")]
		[Category("Sounds")]
		[Gui.Design.DisplayNameAttribute("1. Play Alert")]
		public bool PlayAlert
		{
			get { return playAlert; }
			set { playAlert = value; }
		}

		[XmlIgnore]
		[Description("Color of down bars.")]
		[Category("Visual")]
		[Gui.Design.DisplayNameAttribute("5. Regular Down color")]
		public Color BarColorDown
		{
			get { return barColorDown; }
			set { barColorDown = value; }
		}

		[Browsable(false)]
		public string BarColorDownSerialize
		{
			get { return Gui.Design.SerializableColor.ToString(barColorDown); }
			set { barColorDown = Gui.Design.SerializableColor.FromString(value); }
		}

		[XmlIgnore]
		[Description("Color of up bars.")]
		[Category("Visual")]
		[Gui.Design.DisplayNameAttribute("6. Regular Up color")]
		public Color BarColorUp
		{
			get { return barColorUp; }
			set { barColorUp = value; }
		}

		[Browsable(false)]
		public string BarColorUpSerialize
		{
			get { return Gui.Design.SerializableColor.ToString(barColorUp); }
			set { barColorUp = Gui.Design.SerializableColor.FromString(value); }
		}

		[XmlIgnore]
		[Description("Color of shadow line.")]
		[Category("Visual")]
		[Gui.Design.DisplayNameAttribute("7. Shadow color")]
		public Color ShadowColor
		{
			get { return shadowColor; }
			set { shadowColor = value; }
		}

		[Browsable(false)]
		public string ShadowColorSerialize
		{
			get { return Gui.Design.SerializableColor.ToString(shadowColor); }
			set { shadowColor = Gui.Design.SerializableColor.FromString(value); }
		}

		[Description("Width of shadow line.")]
		[Category("Visual")]
		[Gui.Design.DisplayNameAttribute("8. Shadow width")]
		public int ShadowWidth
		{
			get { return shadowWidth; }
			set { shadowWidth = Math.Max(value, 1); }
		}

		#endregion

		#region Miscellaneous

		private void InitColorSeries()
		{
			for (int i = 0; i <= CurrentBar + Displacement + (CalculateOnBarClose ? 1 : 0); i++)
			{
				BarColorSeries.Set(i, Color.Transparent);
				CandleOutlineColorSeries.Set(i, Color.Transparent);
			}
		}

		protected override void OnStartUp()
		{
			if (ChartControl == null || Bars == null)
				return;
			
			CalculateOnBarClose = false;

			brushUp		= new SolidBrush(barColorUp);
			brushDown	= new SolidBrush(barColorDown);
			b4	= new SolidBrush(Plots[0].Pen.Color);
			b5	= new SolidBrush(Plots[1].Pen.Color);
			b2	= new SolidBrush(Plots[2].Pen.Color);
			b67	= new SolidBrush(Plots[3].Pen.Color);
			shadowPen	= new Pen(shadowColor, shadowWidth);
		}

		protected override void OnTermination()
		{
			if (brushUp != null) brushUp.Dispose();
			if (brushDown != null) brushDown.Dispose();
			if (shadowPen != null) shadowPen.Dispose();
			if (b4 != null) b4.Dispose();
			if (b5 != null) b5.Dispose();
			if (b2 != null) b2.Dispose();
			if (b67 != null) b67.Dispose();
		}

		public override void GetMinMaxValues(ChartControl chartControl, ref double min, ref double max)
		{
			if (Bars == null || ChartControl == null)
				return;

			for (int idx = FirstBarIndexPainted; idx <= LastBarIndexPainted; idx++)
			{
				double tmpHigh = hah.Get(idx);
				double tmpLow = hal.Get(idx);

				if (tmpHigh != 0 && tmpHigh > max)
					max = tmpHigh;
				if (tmpLow != 0 && tmpLow < min)
					min = tmpLow;
			}
		}

		public override void Plot(Graphics graphics, Rectangle bounds, double min, double max)
		{
			if (Bars == null || ChartControl == null)
				return;

			int barPaintWidth = Math.Max(3, 1 + 2 * ((int)Bars.BarsData.ChartStyle.BarWidth - 1) + 2 * shadowWidth);

			for (int idx = FirstBarIndexPainted; idx <= LastBarIndexPainted; idx++)
			{
				if (idx - Displacement < 0 || idx - Displacement >= BarsArray[0].Count || (!ChartControl.ShowBarsRequired && idx - Displacement < BarsRequired))
					continue;
				double valH = hah.Get(idx);
				double valL = hal.Get(idx);
				double valC = hac.Get(idx);
				double valO = hao.Get(idx);

				bool val4 = c4.Get(idx);
				bool val5 = c5.Get(idx);
				bool val2 = c2.Get(idx);
				bool val67 = c67.Get(idx);

				int x = ChartControl.GetXByBarIdx(BarsArray[0], idx);
				int y1 = ChartControl.GetYByValue(this, valO);
				int y2 = ChartControl.GetYByValue(this, valH);
				int y3 = ChartControl.GetYByValue(this, valL);
				int y4 = ChartControl.GetYByValue(this, valC);

				graphics.DrawLine(shadowPen, x, y2, x, y3);

				if (y4 == y1)
					graphics.DrawLine(shadowPen, x - barPaintWidth / 2, y1, x + barPaintWidth / 2, y1);
				else
				{
					SolidBrush bup, bdn;

					if (val4 && st4 == SignalType.BARCOLOR) bup = bdn = b4;
					else if (val5 && st5 == SignalType.BARCOLOR) bup = bdn = b5;
					else if (val2 && st2 == SignalType.BARCOLOR) bup = bdn = b2;
					else if (val67 && st67 == SignalType.BARCOLOR) bup = bdn = b67;
					else { bup = brushUp; bdn = brushDown; }

					if (y4 > y1)
						graphics.FillRectangle(bup, x - barPaintWidth / 2, y1, barPaintWidth, y4 - y1);
					else
						graphics.FillRectangle(bdn, x - barPaintWidth / 2, y4, barPaintWidth, y1 - y4);
					graphics.DrawRectangle(shadowPen, (x - barPaintWidth / 2) + (shadowPen.Width / 2), Math.Min(y4, y1), barPaintWidth - shadowPen.Width, Math.Abs(y4 - y1));
				}
			}

			base.Plot(graphics, bounds, min, max);
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
        private DML[] cacheDML = null;

        private static DML checkDML = new DML();

        /// <summary>
        /// DML
        /// </summary>
        /// <returns></returns>
        public DML DML()
        {
            return DML(Input);
        }

        /// <summary>
        /// DML
        /// </summary>
        /// <returns></returns>
        public DML DML(Data.IDataSeries input)
        {
            if (cacheDML != null)
                for (int idx = 0; idx < cacheDML.Length; idx++)
                    if (cacheDML[idx].EqualsInput(input))
                        return cacheDML[idx];

            lock (checkDML)
            {
                if (cacheDML != null)
                    for (int idx = 0; idx < cacheDML.Length; idx++)
                        if (cacheDML[idx].EqualsInput(input))
                            return cacheDML[idx];

                DML indicator = new DML();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                DML[] tmp = new DML[cacheDML == null ? 1 : cacheDML.Length + 1];
                if (cacheDML != null)
                    cacheDML.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheDML = tmp;
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
        /// DML
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.DML DML()
        {
            return _indicator.DML(Input);
        }

        /// <summary>
        /// DML
        /// </summary>
        /// <returns></returns>
        public Indicator.DML DML(Data.IDataSeries input)
        {
            return _indicator.DML(input);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// DML
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.DML DML()
        {
            return _indicator.DML(Input);
        }

        /// <summary>
        /// DML
        /// </summary>
        /// <returns></returns>
        public Indicator.DML DML(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.DML(input);
        }
    }
}
#endregion
