using System;
using System.ComponentModel;
using System.Drawing;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;

namespace NinjaTrader.Indicator
{
	/// <summary>
	/// ChandesTrendScore
	/// </summary>
	[Description("ChandesTrendScore")]
	public class ChandesTrendScore : Indicator
	{
		private		int			k;
		private		int			lookBack		= 20;
		private		int			lookBackLenght	= 20;
		private		double		score;

		protected override void Initialize()
		{
			Add(new Plot(new Pen(Color.Blue, 2), PlotStyle.Line, "TrendScore"));
			Overlay				= false;
			PriceTypeSupported	= false;
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar < LookBack + LookBackLenght) return;
			score = 0;
			for (k = 0; k < LookBackLenght; k++)
				score = Close[0] >= Close[k + LookBack] ? score + 1 : score - 1;

			Value.Set(score / LookBackLenght);
		}

		[Description("")]
		[GridCategory("Parameters")]
		public int LookBack
		{
			get { return lookBack; }
			set { lookBack = Math.Max(1, value); }
		}

		[Description("")]
		[GridCategory("Parameters")]
		public int LookBackLenght
		{
			get { return lookBackLenght; }
			set { lookBackLenght = Math.Max(1, value); }
		}
	}
}
