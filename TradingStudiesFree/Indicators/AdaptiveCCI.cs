// TradingStudies.com
// info@tradingStudies.com

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Xml.Serialization;
using AdaptiveCCI.Utility;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.Design;

namespace AdaptiveCCI.Utility
{
	public enum AdaptiveCciPlots
	{
		One,
		Two,
		Three,
		Four,
		None,
	}
}

namespace NinjaTrader.Indicator
{
	[Description("Adaptive CCI")]
	[Gui.Design.DisplayName("Adaptive  CCI")]
	public class AdaptiveCci : Indicator
	{
		private		DataSeries			adaptCci;
		private		bool				autoScale			= true;
		private		int					barWidth			= 2;
		private		double				cycPart				= 1;
		private		DataSeries			detrender;
		private		DataSeries			dotSeries;
		private		DataSeries			i1;
		private		DataSeries			i2;
		private		int					iNoAlts;
		private		DataSeries			im;
		private		DataSeries			lengthVars;
		private		SolidBrush			lineBrush			= new SolidBrush(Color.DimGray);
		private		int					lineWidth			= 1;
		private		AdaptiveCciPlots	noPlots				= AdaptiveCciPlots.Four;
		private		int					numBars1			= 2;
		private		int					numBars2			= 4;
		private		int					numBars3			= 6;
		private		int					numBars4			= 8;
		private		DataSeries			period;
		private		int					periodMultiplier	= 1;
		private		DataSeries			q1;
		private		DataSeries			q2;
		private		DataSeries			re;
		private		bool				showDots			= true;
		private		int					smaSmooth			= 1;
		private		DataSeries			smooth;
		private		DataSeries			vMedianPrice;

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries CciPlot
		{
			get { return Values[0]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries AltCciPlot1
		{
			get { return Values[1]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries AltCciPlot2
		{
			get { return Values[2]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries AltCciPlot3
		{
			get { return Values[3]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries AltCciPlot4
		{
			get { return Values[4]; }
		}

		[Description("Use to adjust the lag")]
		[GridCategory("Parameters")]
		public double CycPart
		{
			get { return cycPart; }
			set { cycPart = value; }
		}

		[Description("Use to adjust SMA smoothing")]
		[GridCategory("Parameters")]
		public int Smooth
		{
			get { return smaSmooth; }
			set { smaSmooth = value; }
		}

		[Description("Use to multiply period")]
		[GridCategory("Parameters")]
		public int PeriodMultiplier
		{
			get { return periodMultiplier; }
			set { periodMultiplier = value; }
		}

		[Browsable(false)]
		public string LineColorSerialize
		{
			get { return SerializableColor.ToString(BackLineColor); }
			set { BackLineColor = SerializableColor.FromString(value); }
		}

		[Description("Colour for Background Level Lines"), XmlIgnore, VisualizationOnly]
		[Category("Plots")]
		[Gui.Design.DisplayName("Grid line color")]
		public Color BackLineColor
		{
			get { return lineBrush.Color; }
			set { lineBrush = new SolidBrush(value); }
		}

		[Browsable(false)]
		public string BackLineWidthSerialize
		{
			get { return BackLineWidth.ToString(CultureInfo.InvariantCulture); }
			set { BackLineWidth = Convert.ToInt32(value); }
		}

		[Description("Use to adjust Background Level Lines' Width"), XmlIgnore, VisualizationOnly]
		[Category("Plots")]
		[Gui.Design.DisplayName("Grid line width")]
		public int BackLineWidth
		{
			get { return lineWidth; }
			set { lineWidth = Math.Max(0, value); }
		}

		[Browsable(false)]
		public string BarWidthSerialize
		{
			get { return BarWidth.ToString(CultureInfo.InvariantCulture); }
			set { BarWidth = Convert.ToInt32(value); }
		}

		[Description("Use to the width of the CCI bars"), XmlIgnore, VisualizationOnly]
		[Category("Plots")]
		[Gui.Design.DisplayName("Bar width")]
		public int BarWidth
		{
			get { return barWidth; }
			set { barWidth = Math.Max(0, value); }
		}

		[Description("Whether to plot zero line dots")]
		[Category("Plots")]
		[Gui.Design.DisplayName("Show dots")]
		public bool ShowDots
		{
			get { return showDots; }
			set { showDots = value; }
		}

		[Description("Whether to automatically scale the y axis")]
		[Category("Plots")]
		[Gui.Design.DisplayName("Auto scale")]
		public bool AdaptiveAutoScale
		{
			get { return autoScale; }
			set { autoScale = value; }
		}

		public DataSeries CciPeriod
		{
			get { return lengthVars; }
		}

		[Description("Number of Adaptive Alternative plots to plot")]
		[Category("Parameters")]
		public AdaptiveCciPlots AdaptiveCciPlotNo
		{
			get { return noPlots; }
			set { noPlots = value; }
		}

		[Description("Numbars for Adaptive Alternative - Plot One")]
		[GridCategory("Parameters")]
		public int AltNumbarsPlot1
		{
			get { return numBars1; }
			set { numBars1 = value; }
		}

		[Description("Numbars for Adaptive Alternative - Plot Two")]
		[GridCategory("Parameters")]
		public int AltNumbarsPlot2
		{
			get { return numBars2; }
			set { numBars2 = value; }
		}

		[Description("Numbars for Adaptive Alternative - Plot Three")]
		[GridCategory("Parameters")]
		public int AltNumbarsPlot3
		{
			get { return numBars3; }
			set { numBars3 = value; }
		}

		[Description("Numbars for Adaptive Alternative - Plot Four")]
		[GridCategory("Parameters")]
		public int AltNumbarsPlot4
		{
			get { return numBars4; }
			set { numBars4 = value; }
		}

		protected override void Initialize()
		{
			Add(new Plot(new Pen(Color.Black, 1),		PlotStyle.Line, "CCIPlot"));
			Add(new Plot(new Pen(Color.BlueViolet, 1),	PlotStyle.Line, "AltCCIPlot1"));
			Add(new Plot(new Pen(Color.DodgerBlue, 1),	PlotStyle.Line, "AltCCIPlot2"));
			Add(new Plot(new Pen(Color.Orange, 1),		PlotStyle.Line, "AltCCIPlot3"));
			Add(new Plot(new Pen(Color.Red, 1),			PlotStyle.Line, "AltCCIPlot4"));

			detrender			= new DataSeries(this);
			period				= new DataSeries(this);
			smooth				= new DataSeries(this);
			i1					= new DataSeries(this);
			i2					= new DataSeries(this);
			im					= new DataSeries(this);
			q1					= new DataSeries(this);
			q2					= new DataSeries(this);
			re					= new DataSeries(this);
			vMedianPrice		= new DataSeries(this);
			adaptCci			= new DataSeries(this);
			dotSeries			= new DataSeries(this);
			lengthVars			= new DataSeries(this);
			Overlay				= false;
			DisplayInDataBox	= false;
			CalculateOnBarClose	= false;
			PaintPriceMarkers	= false;
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar < 200)
				return;

			if (0 == iNoAlts)
				switch (noPlots)
				{
					case AdaptiveCciPlots.One	: iNoAlts = 1; break;
					case AdaptiveCciPlots.Two	: iNoAlts = 2; break;
					case AdaptiveCciPlots.Three	: iNoAlts = 3; break;
					case AdaptiveCciPlots.Four	: iNoAlts = 4; break;
				}

			smooth.Set((4 * Input[0] + 3 * Input[1] + 2 * Input[2] + Input[3]) * 0.1);
			detrender.Set((0.0962 * smooth[0] + 0.5769 * smooth[2] - 0.5769 * smooth[4] - 0.0962 * smooth[6]) * (0.075 * period[1] + 0.54));

			// Compute InPhase and Quadrature components
			q1.Set((0.0962 * detrender[0] + 0.5769 * detrender[2] - 0.5769 * detrender[4] - 0.0962 * detrender[6]) * (0.075 * period[1] + 0.54));

			i1.Set(detrender[3]);

			// Advance the phase of i1 and q1 by 90}
			double jI = (0.0962 * i1[0] + 0.5769 * i1[2] - 0.5769 * i1[4] - 0.0962 * i1[6]) * (0.075 * period[1] + 0.54);
			double jQ = (0.0962 * q1[0] + 0.5769 * q1[2] - 0.5769 * q1[4] - 0.0962 * q1[6]) * (0.075 * period[1] + 0.54);

			// Phasor addition for 3 bar averaging}
			i2.Set(i1[0] - jQ);
			q2.Set(q1[0] + jI);

			// Smooth the I and Q components before applying the discriminator
			i2.Set(0.2 * i2[0] + 0.8 * i2[1]);
			q2.Set(0.2 * q2[0] + 0.8 * q2[1]);

			// Homodyne Discriminator
			re.Set(i2[0] * i2[1] + q2[0] * q2[1]);
			im.Set(i2[0] * q2[1] - q2[0] * i2[1]);

			re.Set(0.2 * re[0] + 0.8 * re[1]);
			im.Set(0.2 * im[0] + 0.8 * im[1]);

			if (Math.Abs(im[0]) > double.Epsilon && Math.Abs(re[0]) > double.Epsilon)
				period.Set(360 / ((180 / Math.PI) * Math.Atan(im[0] / re[0])));
			if (period[0] > 1.5 * period[1])
				period.Set(1.5 * period[1]);
			if (period[0] < 0.67 * period[1])
				period.Set(0.67 * period[1]);
			if (period[0] < 6)
				period.Set(6);
			if (period[0] > 200)
				period.Set(200);

			period.Set(0.2 * period[0] + 0.8 * period[1]);

			int length = periodMultiplier * (int)(cycPart * period[0]);
			lengthVars.Set(length);
			//			Print(Length);
			vMedianPrice.Set((High[0] + Low[0] + Close[0]) / 3);
			double avg = 0;
			for (int count = 0; count < length; count++)
				avg = avg + vMedianPrice[count];

			avg = avg / length;
			double md = 0;
			for (int count = 0; count < length; count++)
				md = md + Math.Abs(vMedianPrice[count] - avg);

			md = md / length;

			if (Math.Abs(md) < double.Epsilon)
				return;

			adaptCci.Set((vMedianPrice[0] - avg) / (0.015 * md));
			CciPlot.Set(SMA(adaptCci, smaSmooth)[0]);

			DataSeries dsAlt1 = AdaptiveAlternativeCci(cycPart, numBars1, periodMultiplier, smaSmooth).CciPlot;
			DataSeries dsAlt2 = AdaptiveAlternativeCci(cycPart, numBars2, periodMultiplier, smaSmooth).CciPlot;
			DataSeries dsAlt3 = AdaptiveAlternativeCci(cycPart, numBars3, periodMultiplier, smaSmooth).CciPlot;
			DataSeries dsAlt4 = AdaptiveAlternativeCci(cycPart, numBars4, periodMultiplier, smaSmooth).CciPlot;

			if (iNoAlts > 0)
				AltCciPlot1.Set(dsAlt1.Get(CurrentBar));
			if (iNoAlts > 1)
				AltCciPlot2.Set(dsAlt2.Get(CurrentBar));
			if (iNoAlts > 2)
				AltCciPlot3.Set(dsAlt3.Get(CurrentBar));
			if (iNoAlts > 3)
				AltCciPlot4.Set(dsAlt4.Get(CurrentBar));

			if (iNoAlts > 3)
				if (Rising(dsAlt1) && Rising(dsAlt2) && Rising(dsAlt3) && Rising(dsAlt4))
					dotSeries.Set(1);
				else if (Falling(dsAlt1) && Falling(dsAlt2) && Falling(dsAlt3) && Falling(dsAlt4))
					dotSeries.Set(-1);
				else
					dotSeries.Set(2);
			else if (iNoAlts > 2)
				if (Rising(dsAlt1) && Rising(dsAlt2) && Rising(dsAlt3))
					dotSeries.Set(1);
				else if (Falling(dsAlt1) && Falling(dsAlt2) && Falling(dsAlt3))
					dotSeries.Set(-1);
				else
					dotSeries.Set(2);
			else if (iNoAlts > 1)
				if (Rising(dsAlt1) && Rising(dsAlt2))
					dotSeries.Set(1);
				else if (Falling(dsAlt1) && Falling(dsAlt2))
					dotSeries.Set(-1);
				else
					dotSeries.Set(2);
			else if (iNoAlts > 0)
				if (Rising(dsAlt1))
					dotSeries.Set(1);
				else if (Falling(dsAlt1))
					dotSeries.Set(-1);
				else
					dotSeries.Set(2);
			else if (Rising(CciPlot))
				dotSeries.Set(1);
			else if (Falling(CciPlot))
				dotSeries.Set(-1);
			else
				dotSeries.Set(2);
		}

		public override void GetMinMaxValues(ChartControl chartControl, ref double min, ref double max)
		{
			base.GetMinMaxValues(chartControl, ref min, ref max);
			if (!autoScale)
				if (max > (min * -1))
					min = max * -1;
				else
					max = min * -1;
			min = min - 20;
			max = max + 40;
		}

		public override void Plot(Graphics graphics, Rectangle bounds, double min, double max)
		{
			if (Bars == null)
				return;

			byte bRed	= (byte)~(ChartControl.BackColor.R);
			byte bGreen	= (byte)~(ChartControl.BackColor.G);
			byte bBlue	= (byte)~(ChartControl.BackColor.B);

			Color borderColor = Color.FromArgb(bRed, bGreen, bBlue);

			if (lineWidth > 0)
			{
				// draw back lines
				Pen linePen = new Pen(lineBrush, lineWidth);
				double lineSpace = 100;

				{
					double y = (bounds.Y + bounds.Height) - ((0 - min) / ChartControl.MaxMinusMin(max, min)) * bounds.Height;
					graphics.DrawLine(linePen, ChartControl.CanvasLeft, (int)y, ChartControl.CanvasRight, (int)y);
				}
				while (max > lineSpace || min < lineSpace * -1)
				{
					double y = (bounds.Y + bounds.Height) - ((lineSpace - min) / ChartControl.MaxMinusMin(max, min)) * bounds.Height;
					graphics.DrawLine(linePen, ChartControl.CanvasLeft, (int)y, ChartControl.CanvasRight, (int)y);
					y = (bounds.Y + bounds.Height) - (((lineSpace * -1) - min) / ChartControl.MaxMinusMin(max, min)) * bounds.Height;
					graphics.DrawLine(linePen, ChartControl.CanvasLeft, (int)y, ChartControl.CanvasRight, (int)y);

					lineSpace += 100;
				}
			}

			int bars			= ChartControl.BarsPainted;
			int barPaintWidth	= ChartControl.ChartStyle.GetBarPaintWidth(ChartControl.BarWidth);

			if (ChartControl.LastBarPainted - ChartControl.BarsPainted + 1 + bars - Bars.Count != 0)
				bars--;

			while (bars >= 0)
			{
				int index = ((ChartControl.LastBarPainted - ChartControl.BarsPainted) + 1) + bars;

				if (ChartControl.ShowBarsRequired || index - Displacement >= BarsRequired)
					try
					{
						double cciVal = CciPlot.Get(index);
						int x = (((ChartControl.CanvasRight - ChartControl.BarMarginRight) - (barPaintWidth / 2)) -
								 ((ChartControl.BarsPainted - 1) * ChartControl.BarSpace)) + (bars * ChartControl.BarSpace);
						int y1 = (bounds.Y + bounds.Height) - ((int)(((cciVal - min) / ChartControl.MaxMinusMin(max, min)) * bounds.Height));

						double alertVal = dotSeries.Get(index);

						if (!double.IsNaN(alertVal) && !double.IsNaN(cciVal))
						{
							int y = (bounds.Y + bounds.Height) - ((int)(((0 - min) / ChartControl.MaxMinusMin(max, min)) * bounds.Height));

							Color dotColor = Color.Transparent;

							if (Math.Abs(1 - alertVal) < double.Epsilon)
								dotColor = ChartControl.ChartStyle.UpColor;
							else if (Math.Abs(-1 - alertVal) < double.Epsilon)
								dotColor = ChartControl.ChartStyle.DownColor;
							else if (Math.Abs(2 - alertVal) < double.Epsilon)
								dotColor = Color.Gray;

							if (dotColor != Color.Transparent)
							{
								int barOffset = (int)((double)(barWidth - 1) / 2);

								// draw bars
								if (y1 - y > 0)
									graphics.FillRectangle(new SolidBrush(dotColor), x - barOffset, y, barWidth, y1 - y);
								else
									graphics.FillRectangle(new SolidBrush(dotColor), x - barOffset, y1, barWidth, y - y1);

								if (showDots)
								{
									// draw dots
									SmoothingMode smoothingMode = graphics.SmoothingMode;
									graphics.SmoothingMode = SmoothingMode.AntiAlias;

									Pen pen = new Pen(borderColor, 7) { DashStyle = DashStyle.Dot, DashCap = DashCap.Round };
									graphics.DrawPie(pen, x - 1, y, 2, 2, 0, 360);
									pen = new Pen(dotColor, 5) { DashStyle = DashStyle.Dot, DashCap = DashCap.Round };
									graphics.DrawRectangle(pen, x - 2, y - 1, 2, 2);
									graphics.SmoothingMode = smoothingMode;
								}
							}
						}
					}
					catch (Exception)
					{
					}
				bars--;
			}

			// Default plotting in base class. 
			base.Plot(graphics, bounds, min, max);
		}
	}
}
#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private AdaptiveCci[] cacheAdaptiveCci = null;

        private static AdaptiveCci checkAdaptiveCci = new AdaptiveCci();

        /// <summary>
        /// Adaptive CCI
        /// </summary>
        /// <returns></returns>
        public AdaptiveCci AdaptiveCci(AdaptiveCciPlots adaptiveCciPlotNo, int altNumbarsPlot1, int altNumbarsPlot2, int altNumbarsPlot3, int altNumbarsPlot4, double cycPart, int periodMultiplier, int smooth)
        {
            return AdaptiveCci(Input, adaptiveCciPlotNo, altNumbarsPlot1, altNumbarsPlot2, altNumbarsPlot3, altNumbarsPlot4, cycPart, periodMultiplier, smooth);
        }

        /// <summary>
        /// Adaptive CCI
        /// </summary>
        /// <returns></returns>
        public AdaptiveCci AdaptiveCci(Data.IDataSeries input, AdaptiveCciPlots adaptiveCciPlotNo, int altNumbarsPlot1, int altNumbarsPlot2, int altNumbarsPlot3, int altNumbarsPlot4, double cycPart, int periodMultiplier, int smooth)
        {
            if (cacheAdaptiveCci != null)
                for (int idx = 0; idx < cacheAdaptiveCci.Length; idx++)
                    if (cacheAdaptiveCci[idx].AdaptiveCciPlotNo == adaptiveCciPlotNo && cacheAdaptiveCci[idx].AltNumbarsPlot1 == altNumbarsPlot1 && cacheAdaptiveCci[idx].AltNumbarsPlot2 == altNumbarsPlot2 && cacheAdaptiveCci[idx].AltNumbarsPlot3 == altNumbarsPlot3 && cacheAdaptiveCci[idx].AltNumbarsPlot4 == altNumbarsPlot4 && Math.Abs(cacheAdaptiveCci[idx].CycPart - cycPart) <= double.Epsilon && cacheAdaptiveCci[idx].PeriodMultiplier == periodMultiplier && cacheAdaptiveCci[idx].Smooth == smooth && cacheAdaptiveCci[idx].EqualsInput(input))
                        return cacheAdaptiveCci[idx];

            lock (checkAdaptiveCci)
            {
                checkAdaptiveCci.AdaptiveCciPlotNo = adaptiveCciPlotNo;
                adaptiveCciPlotNo = checkAdaptiveCci.AdaptiveCciPlotNo;
                checkAdaptiveCci.AltNumbarsPlot1 = altNumbarsPlot1;
                altNumbarsPlot1 = checkAdaptiveCci.AltNumbarsPlot1;
                checkAdaptiveCci.AltNumbarsPlot2 = altNumbarsPlot2;
                altNumbarsPlot2 = checkAdaptiveCci.AltNumbarsPlot2;
                checkAdaptiveCci.AltNumbarsPlot3 = altNumbarsPlot3;
                altNumbarsPlot3 = checkAdaptiveCci.AltNumbarsPlot3;
                checkAdaptiveCci.AltNumbarsPlot4 = altNumbarsPlot4;
                altNumbarsPlot4 = checkAdaptiveCci.AltNumbarsPlot4;
                checkAdaptiveCci.CycPart = cycPart;
                cycPart = checkAdaptiveCci.CycPart;
                checkAdaptiveCci.PeriodMultiplier = periodMultiplier;
                periodMultiplier = checkAdaptiveCci.PeriodMultiplier;
                checkAdaptiveCci.Smooth = smooth;
                smooth = checkAdaptiveCci.Smooth;

                if (cacheAdaptiveCci != null)
                    for (int idx = 0; idx < cacheAdaptiveCci.Length; idx++)
                        if (cacheAdaptiveCci[idx].AdaptiveCciPlotNo == adaptiveCciPlotNo && cacheAdaptiveCci[idx].AltNumbarsPlot1 == altNumbarsPlot1 && cacheAdaptiveCci[idx].AltNumbarsPlot2 == altNumbarsPlot2 && cacheAdaptiveCci[idx].AltNumbarsPlot3 == altNumbarsPlot3 && cacheAdaptiveCci[idx].AltNumbarsPlot4 == altNumbarsPlot4 && Math.Abs(cacheAdaptiveCci[idx].CycPart - cycPart) <= double.Epsilon && cacheAdaptiveCci[idx].PeriodMultiplier == periodMultiplier && cacheAdaptiveCci[idx].Smooth == smooth && cacheAdaptiveCci[idx].EqualsInput(input))
                            return cacheAdaptiveCci[idx];

                AdaptiveCci indicator = new AdaptiveCci();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.AdaptiveCciPlotNo = adaptiveCciPlotNo;
                indicator.AltNumbarsPlot1 = altNumbarsPlot1;
                indicator.AltNumbarsPlot2 = altNumbarsPlot2;
                indicator.AltNumbarsPlot3 = altNumbarsPlot3;
                indicator.AltNumbarsPlot4 = altNumbarsPlot4;
                indicator.CycPart = cycPart;
                indicator.PeriodMultiplier = periodMultiplier;
                indicator.Smooth = smooth;
                Indicators.Add(indicator);
                indicator.SetUp();

                AdaptiveCci[] tmp = new AdaptiveCci[cacheAdaptiveCci == null ? 1 : cacheAdaptiveCci.Length + 1];
                if (cacheAdaptiveCci != null)
                    cacheAdaptiveCci.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheAdaptiveCci = tmp;
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
        /// Adaptive CCI
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.AdaptiveCci AdaptiveCci(AdaptiveCciPlots adaptiveCciPlotNo, int altNumbarsPlot1, int altNumbarsPlot2, int altNumbarsPlot3, int altNumbarsPlot4, double cycPart, int periodMultiplier, int smooth)
        {
            return _indicator.AdaptiveCci(Input, adaptiveCciPlotNo, altNumbarsPlot1, altNumbarsPlot2, altNumbarsPlot3, altNumbarsPlot4, cycPart, periodMultiplier, smooth);
        }

        /// <summary>
        /// Adaptive CCI
        /// </summary>
        /// <returns></returns>
        public Indicator.AdaptiveCci AdaptiveCci(Data.IDataSeries input, AdaptiveCciPlots adaptiveCciPlotNo, int altNumbarsPlot1, int altNumbarsPlot2, int altNumbarsPlot3, int altNumbarsPlot4, double cycPart, int periodMultiplier, int smooth)
        {
            return _indicator.AdaptiveCci(input, adaptiveCciPlotNo, altNumbarsPlot1, altNumbarsPlot2, altNumbarsPlot3, altNumbarsPlot4, cycPart, periodMultiplier, smooth);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Adaptive CCI
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.AdaptiveCci AdaptiveCci(AdaptiveCciPlots adaptiveCciPlotNo, int altNumbarsPlot1, int altNumbarsPlot2, int altNumbarsPlot3, int altNumbarsPlot4, double cycPart, int periodMultiplier, int smooth)
        {
            return _indicator.AdaptiveCci(Input, adaptiveCciPlotNo, altNumbarsPlot1, altNumbarsPlot2, altNumbarsPlot3, altNumbarsPlot4, cycPart, periodMultiplier, smooth);
        }

        /// <summary>
        /// Adaptive CCI
        /// </summary>
        /// <returns></returns>
        public Indicator.AdaptiveCci AdaptiveCci(Data.IDataSeries input, AdaptiveCciPlots adaptiveCciPlotNo, int altNumbarsPlot1, int altNumbarsPlot2, int altNumbarsPlot3, int altNumbarsPlot4, double cycPart, int periodMultiplier, int smooth)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.AdaptiveCci(input, adaptiveCciPlotNo, altNumbarsPlot1, altNumbarsPlot2, altNumbarsPlot3, altNumbarsPlot4, cycPart, periodMultiplier, smooth);
        }
    }
}
#endregion
