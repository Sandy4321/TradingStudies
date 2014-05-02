using System;
using System.ComponentModel;
using System.Drawing;
using System.Xml.Serialization;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
using System.Collections;

namespace NinjaTrader.Indicator
{
	/// <summary>
	/// Short Term Trading Indicator. Buy when the price crosses above the green line.  Sell when the price crosses the blue line.
	/// </summary>
	[Description("Short Term Trading Indicator. Buy when the price crosses above the green line.  Sell when the price crosses the blue line.")]
	public class CsInd : Indicator
	{
		#region Variables
		// Wizard generated variables
		private DeviationType devType = DeviationType.Points; // Default setting for DevType
		private double devValue = 8; // Default setting for DevValue
		private int period = 8; // Default setting for Period
		// User defined variables (add any user defined variables below)
		private double dVal = 0; // Deviation Value adjusted for points or percent
		private double pk = 0; private int pkBar = 0; // Current peak value
		private double pkRS = 0; // Peak reversal signal; peak formed when Low[0] declines below pkRS
		private double tr = 0; private int trBar = 0; // Current trough value
		private double trRS = 0; // Trough reversal signal; trough formed when High[0] rises above trRS
		private sbyte swing = 0; // Swing; 1 = up, -1 = down
		private sbyte trend = 0; // 1 = uptrend, -1 = downtrend
		private double slope = 0; // slope of trend
		public int init = 0; // marks start of strategy
		private double bs = 0; // temp value for buy stop
		private double ss = 0; // temp value for sell stop

		private ArrayList peak = new ArrayList(); // Tracks peaks
		private ArrayList peakBar = new ArrayList(); // Tracks peak bars
		private ArrayList trough = new ArrayList(); // Tracks troughs
		private ArrayList troughBar = new ArrayList(); // Tracks trough bars
		private ArrayList midPoint = new ArrayList(); // Tracks midpoints of up/down swings
		private ArrayList midBar = new ArrayList(); // Tracks midpoint bars

		private double hh = 0,
					   ll = double.MaxValue;


		#endregion

		protected override void Initialize()
		{
			Add(new Plot(new Pen(Color.FromKnownColor(KnownColor.Green), 2), PlotStyle.Line, "BStop"));
			Add(new Plot(new Pen(Color.FromKnownColor(KnownColor.Blue), 2), PlotStyle.Line, "SStop"));
			CalculateOnBarClose = true;
			Overlay = true;
			PriceTypeSupported = false;
			BarsRequired = 0;
			ClearOutputWindow();
		}

		protected override void OnBarUpdate()
		{

			#region Filtered Wave

			if (CurrentBar == 0) //initialize variables
			{
				dVal = devType == DeviationType.Points ? devValue * TickSize : devValue / 100; // adjust deviation value to points or percent
				ResetPK(); ResetTR();
				return;
			}

			else if (WaveCount() == 0) //update variables unitl first peak or trough is formed
			{
				if (Comp(High[0], pk) >= 0) ResetPK(); // new high
				else if (Comp(Low[0], tr) <= 0) ResetTR(); // new low
			}

			if (Comp(Low[0], pkRS) <= 0) //Peak has formed; downswing
			{
				if (swing != -1) // Set peak and switch to downswing
				{
					peak.Insert(0, pk);
					peakBar.Insert(0, pkBar);
					if (trough.Count > 0)
					{
						midPoint.Insert(0, ((double)peak[0] + (double)trough[0]) / 2);
						midBar.Insert(0, ((int)peakBar[0] + (int)troughBar[0]) / 2);
						if (midBar.Count > 1)
						{
							int xMid = CurrentBar - (int)midBar[0];
							DrawDot("Mid" + CurrentBar.ToString(), true, xMid, (double)midPoint[0], Color.Gold);
						}
					}
					int xPeak = CurrentBar - pkBar;
					DrawDot("Peak" + pkBar.ToString(), true, xPeak, (double)peak[0], Color.Green);
					swing = -1;
				}
				ResetTR();
				pk = trRS; pkRS = pk - LDev(pk, dVal);
			}

			else if (Comp(High[0], trRS) >= 0) //Trough has formed; upswing
			{
				if (swing != 1) // Set trough and switch to upswing
				{
					trough.Insert(0, tr);
					troughBar.Insert(0, trBar);
					if (peak.Count > 0)
					{
						midPoint.Insert(0, ((double)peak[0] + (double)trough[0]) / 2);
						midBar.Insert(0, ((int)peakBar[0] + (int)troughBar[0]) / 2);
						if (midBar.Count > 1)
						{
							int xMid = CurrentBar - (int)midBar[0];
							DrawDot("Mid" + CurrentBar.ToString(), true, xMid, (double)midPoint[0], Color.Gold);
						}
					}
					int xTrough = CurrentBar - trBar;
					DrawDot("Trough" + trBar, true, xTrough, (double)trough[0], Color.Blue);
					swing = 1;
				}
				ResetPK();
				tr = pkRS; trRS = tr + HDev(tr, dVal);
			}

			#endregion

			#region Set Initial Stops

			if (WaveCount() < 5)
			{
				ll = Math.Min(ll, Low[0]);
				hh = Math.Max(hh, High[0]);
				return;
			}
			if (WaveCount() == 5)
			{
				init += 1;
				if (init == 1)
				{
					//double b = MAX(High, CurrentBar)[0] + TickSize;
					//double s = MIN(Low, CurrentBar)[0] - TickSize;
					double b = hh + TickSize;
					double s = ll - TickSize;
					BStop.Set(b);
					SStop.Set(s);
					return;
				}
			}

			#endregion

			#region Strategy

			switch (trend)
			{
				case 0:  // neutral trend - strategy starting
					if (Comp(High[0], BStop[1]) >= 0)		// up trend
					{
						BStop.Set(High[0] + TickSize); SStop.Set(SStop[1]);
						trend = 1; ss = SStop[1]; slope = 0;
					}
					else if (Comp(Low[0], SStop[1]) <= 0)	// down trend
					{
						SStop.Set(Low[0] - TickSize); BStop.Set(BStop[1]);
						trend = -1; bs = BStop[1]; slope = 0;
					}
					else
					{ BStop.Set(BStop[1]); SStop.Set(SStop[1]); }
					break;
				case 1:
					if (Comp(Low[0], SStop[1]) <= 0)	// switch to down trend
					{
						SStop.Set(Low[0] - TickSize); BStop.Set(BStop[1]);
						trend = -1; bs = BStop[1]; slope = 0;
					}
					else // continue uptrend
					{
						BStop.Set(Math.Max(BStop[1], High[0] + TickSize));
						slope = Math.Max(slope, MSlope());
						ss += slope;
						double s = Math.Floor(ss * (1.0 / TickSize)) / (1.0 / TickSize);
						if (Comp(s, MIN(Low, period)[0] - TickSize) >= 0)
						{ s = MIN(Low, period)[0] - TickSize; ss = s; }
						SStop.Set(s);
					}

					break;
				case -1:
					if (Comp(High[0], BStop[1]) >= 0)		// switch to up trend
					{
						BStop.Set(High[0] + TickSize); SStop.Set(SStop[1]);
						trend = 1; ss = SStop[1]; slope = 0;
					}
					else // continue downtrend
					{
						SStop.Set(Math.Min(SStop[1], Low[0] - TickSize));
						slope = Math.Min(slope, MSlope());
						bs += slope;
						double b = Math.Ceiling(bs * (1.0 / TickSize)) / (1.0 / TickSize);
						//b = Math.Max(b, MAX(High, period)[0] + TickSize);
						if (Comp(b, MAX(High, period)[0] + TickSize) <= 0)
						{ b = MAX(High, period)[0] + TickSize; bs = b; }
						BStop.Set(b);
					}

					break;
			}

			#endregion
		}

		#region Methods & Functions

		private int Comp(double p1, double p2) // compares two numbers of type double
		{
			return Instrument.MasterInstrument.Compare(p1, p2);
		}

		private void ResetPK() // raise peak target
		{
			pk = High[0]; pkBar = CurrentBar; pkRS = pk - LDev(pk, dVal);
		}

		private void ResetTR() // lower trough target
		{
			tr = Low[0]; trBar = CurrentBar; trRS = tr + HDev(tr, dVal);
		}

		private int WaveCount() // count of filtered waves
		{
			return (Math.Max(peak.Count + trough.Count - 1, 0));
		}

		private double LDev(double val, double dev) //gets lower deviation value (points or percent)
		{
			return devType == DeviationType.Points ? dev : val - val / (1 + dev);
		}

		private double HDev(double val, double dev) //gets higher deviation value (points or percent)
		{
			return devType == DeviationType.Points ? dev : val * (1 + dev) - val;
		}

		private double MSlope() //gets higher deviation value (points or percent)
		{
			int x1 = (int)midBar[1];
			double y1 = (double)midPoint[1];
			int x2 = (int)midBar[0];
			double y2 = (double)midPoint[0];

			if (Comp(pk, (double)peak[0]) == 1)
			{
				x1 = (int)midBar[0]; x2 = (pkBar + (int)troughBar[0]) / 2;
				y1 = (double)midPoint[0]; y2 = (pk + (double)trough[0]) / 2;
			}
			else if (Comp(tr, (double)trough[0]) == -1)
			{
				x1 = (int)midBar[0]; x2 = ((int)peakBar[0] + trBar) / 2;
				y1 = (double)midPoint[0]; y2 = ((double)peak[0] + tr) / 2;
			}

			/*
			int x = (int)midBar[0] - (int)midBar[1];
			double y = (double)midPoint[0] - (double)midPoint[1];
			
			return y/x;*/

			return (y2 - y1) / (x2 - x1);
		}




		#endregion

		#region Properties

		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
		[XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
		public DataSeries BStop
		{
			get { return Values[0]; }
		}

		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
		[XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
		public DataSeries SStop
		{
			get { return Values[1]; }
		}

		[Description("Deviation Type (Points or Percent)")]
		[Category("Parameters")]
		[Gui.Design.DisplayName("Deviation Type")]
		public DeviationType DevType
		{
			get { return devType; }
			set { devType = value; }
		}

		[Description("Value of Deviation (Points or Percent)")]
		[Category("Parameters")]
		[Gui.Design.DisplayName("Deviation Value")]
		public double DevValue
		{
			get { return devValue; }
			set { devValue = Math.Max(0.00005, value); }
		}

		[Description("Look-back period for stops")]
		[Category("Parameters")]
		[Gui.Design.DisplayName("Period")]
		public int Period
		{
			get { return period; }
			set { period = Math.Max(2, value); }
		}

		#endregion
	}
}
