using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;

namespace NinjaTrader.Indicator
{
    [Description("The RSX is a smooth, low lag price-following oscillator that ranges between 0 and 100.")]
    public class RSX : Indicator
    {
        private DataSeries _priceSeries;
        private int _length = 6;

        double _f88;
        double _f90;
        double _f0;
        double _v4;
        double _v8;
        double _vC;
        double _v10;
        double _v14;
        double _v18;
        double _v20;
        double _f8;
        double _f10;
        double _f18;
        double _f20;
        double _f28;
        double _f30;
        double _f38;
        double _f48;
        double _v1C;
        double _f50;
        double _f58;
        double _f60;
        double _f68;
        double _f70;
        double _f78;
        double _f80;
        double _f40;

        protected override void Initialize()
        {
            Add(new Plot(new Pen(Color.Green, 2), "TSX"));
            Plots[0].Pen.Width = 2;

            Add(new Line(Color.Gray, 30, "Lower"));
            Add(new Line(Color.Gray, 70, "Upper"));
            Add(new Line(Color.Gray, 50, "Mid"));
            Lines[0].Pen.DashStyle = DashStyle.Dash;
            Lines[1].Pen.DashStyle = DashStyle.Dash;
            Lines[2].Pen.Width = 1;

            _priceSeries = new DataSeries(this);
            Overlay = false;
        }

        protected override void OnBarUpdate()
        {
            if (CurrentBar == 0)
            {
                _priceSeries.Set(0);
                _f90 = 0;
                _v20 = 0;
                _v14 = 0;
                _f88 = 0;
                _f8 = 0;
                _f20 = 0;
                _f28 = 0;
                _f18 = 0;
                _f38 = 0;
                _f40 = 0;
                _f50 = 0;
                _f60 = 0;
                _f68 = 0;
                _f70 = 0;
                _f78 = 0;
                _f80 = 0;
                _f0 = 0;
                return;
            }

            int limit = CurrentBar - Period - 1;


            for (int shift = limit; shift >= 0; shift--)
            {
                if (_f90 == 0.0)
                {

                    _f90 = 1.0;
                    _f0 = 0.0;
                    _f88 = Period - 1 >= 5 ? Period - 1.0 : 5.0;
                    _f8 = 100.0 * (Close[shift]);
                    _f18 = 3.0 / (Period + 2.0);
                    _f20 = 1.0 - _f18;

                }
                else
                {
                    _f90 = _f88 <= _f90 ? _f88 + 1 : _f90 + 1;
                    _f10 = _f8;
                    _f8 = 100 * Close[shift];
                    _v8 = _f8 - _f10;
                    _f28 = _f20 * _f28 + _f18 * _v8;
                    _f30 = _f18 * _f28 + _f20 * _f30;
                    _vC = _f28 * 1.5 - _f30 * 0.5;
                    _f38 = _f20 * _f38 + _f18 * _vC;
                    _f40 = _f18 * _f38 + _f20 * _f40;
                    _v10 = _f38 * 1.5 - _f40 * 0.5;
                    _f48 = _f20 * _f48 + _f18 * _v10;
                    _f50 = _f18 * _f48 + _f20 * _f50;
                    _v14 = _f48 * 1.5 - _f50 * 0.5;
                    _f58 = _f20 * _f58 + _f18 * Math.Abs(_v8);
                    _f60 = _f18 * _f58 + _f20 * _f60;
                    _v18 = _f58 * 1.5 - _f60 * 0.5;
                    _f68 = _f20 * _f68 + _f18 * _v18;

                    _f70 = _f18 * _f68 + _f20 * _f70;
                    _v1C = _f68 * 1.5 - _f70 * 0.5;
                    _f78 = _f20 * _f78 + _f18 * _v1C;
                    _f80 = _f18 * _f78 + _f20 * _f80;
                    _v20 = _f78 * 1.5 - _f80 * 0.5;

                    if ((_f88 >= _f90) && (_f8 != _f10)) _f0 = 1.0;
                    if ((_f88 == _f90) && (_f0 == 0.0)) _f90 = 0.0;
                }

                if ((_f88 < _f90) && (_v20 > 0.0000000001))
                {
                    _v4 = (_v14 / _v20 + 1.0) * 50.0;
                    if (_v4 > 100.0) _v4 = 100.0;
                    if (_v4 < 0.0) _v4 = 0.0;
                }
                else
                    _v4 = 50.0;

                Value.Set(_v4);
            }
        }

        public override void GetMinMaxValues(ChartControl chartControl, ref double min, ref double max)
        {
            min = -1;
            max = 101;
        }

        #region Properties
        [Description("Numbers of bars used for calculations")]
        [GridCategory("Parameters")]
        public int Period
        {
            get { return _length; }
            set { _length = Math.Max(2, value); }
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
        private RSX[] cacheRSX = null;

        private static RSX checkRSX = new RSX();

        /// <summary>
        /// The RSX is a smooth, low lag price-following oscillator that ranges between 0 and 100.
        /// </summary>
        /// <returns></returns>
        public RSX RSX(int period)
        {
            return RSX(Input, period);
        }

        /// <summary>
        /// The RSX is a smooth, low lag price-following oscillator that ranges between 0 and 100.
        /// </summary>
        /// <returns></returns>
        public RSX RSX(Data.IDataSeries input, int period)
        {
            if (cacheRSX != null)
                for (int idx = 0; idx < cacheRSX.Length; idx++)
                    if (cacheRSX[idx].Period == period && cacheRSX[idx].EqualsInput(input))
                        return cacheRSX[idx];

            lock (checkRSX)
            {
                checkRSX.Period = period;
                period = checkRSX.Period;

                if (cacheRSX != null)
                    for (int idx = 0; idx < cacheRSX.Length; idx++)
                        if (cacheRSX[idx].Period == period && cacheRSX[idx].EqualsInput(input))
                            return cacheRSX[idx];

                RSX indicator = new RSX();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Period = period;
                Indicators.Add(indicator);
                indicator.SetUp();

                RSX[] tmp = new RSX[cacheRSX == null ? 1 : cacheRSX.Length + 1];
                if (cacheRSX != null)
                    cacheRSX.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheRSX = tmp;
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
        /// The RSX is a smooth, low lag price-following oscillator that ranges between 0 and 100.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.RSX RSX(int period)
        {
            return _indicator.RSX(Input, period);
        }

        /// <summary>
        /// The RSX is a smooth, low lag price-following oscillator that ranges between 0 and 100.
        /// </summary>
        /// <returns></returns>
        public Indicator.RSX RSX(Data.IDataSeries input, int period)
        {
            return _indicator.RSX(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The RSX is a smooth, low lag price-following oscillator that ranges between 0 and 100.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.RSX RSX(int period)
        {
            return _indicator.RSX(Input, period);
        }

        /// <summary>
        /// The RSX is a smooth, low lag price-following oscillator that ranges between 0 and 100.
        /// </summary>
        /// <returns></returns>
        public Indicator.RSX RSX(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.RSX(input, period);
        }
    }
}
#endregion
