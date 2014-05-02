using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui.Design;

namespace NinjaTrader.Indicator
{
	[Description("Color the chart background for up to three custom time frames.")]
	public class ColorTimeRegions : Indicator
	{
		private bool	alertBool		= true;
		private Color	region01Color	= Color.LightGray;
		private Color	region02Color	= Color.LightGreen;
		private Color	region03Color	= Color.LightGray;
		private Color	region04Color	= Color.LightGreen;
		private Color	region05Color	= Color.LightGray;
		private Color	region06Color	= Color.LightGreen;
		private Color	region07Color	= Color.LightGray;
		private Color	region08Color	= Color.LightGreen;
		private Color	region09Color	= Color.LightGray;
		private Color	region10Color	= Color.LightGreen;
		private Color	region11Color	= Color.LightGray;
		private Color	region12Color	= Color.LightGreen;
		private Color	region13Color	= Color.LightGray;
		private string	s01;
		private string	s02;
		private string	s03;
		private string	s04;
		private string	s05;
		private string	s06;
		private string	s07;
		private string	s08;
		private string	s09;
		private string	s10;
		private string	s11;
		private string	s12;
		private string	s13;
		private Color	textColor		= Color.Black;
		private Font	textFont		= new Font("Arial", 15);
		private int		zn01HrEn		= 9;
		private int		zn01HrSt		= 8;
		private int		zn01MinEn;
		private int		zn01MinSt		= 30;
		private int		zn02HrEn		= 9;
		private int		zn02HrSt		= 9;
		private int		zn02MinEn		= 30;
		private int		zn02MinSt;
		private int		zn03HrEn		= 10;
		private int		zn03HrSt		= 9;
		private int		zn03MinEn;
		private int		zn03MinSt		= 30;
		private int		zn04HrEn		= 10;
		private int		zn04HrSt		= 10;
		private int		zn04MinEn		= 30;
		private int		zn04MinSt;
		private int		zn05HrEn		= 11;
		private int		zn05HrSt		= 10;
		private int		zn05MinEn;
		private int		zn05MinSt		= 30;
		private int		zn06HrEn		= 11;
		private int		zn06HrSt		= 11;
		private int		zn06MinEn		= 30;
		private int		zn06MinSt;
		private int		zn07HrEn		= 12;
		private int		zn07HrSt		= 11;
		private int		zn07MinEn;
		private int		zn07MinSt		= 30;
		private int		zn08HrEn		= 12;
		private int		zn08HrSt		= 12;
		private int		zn08MinEn		= 30;
		private int		zn08MinSt;
		private int		zn09HrEn		= 13;
		private int		zn09HrSt		= 12;
		private int		zn09MinEn;
		private int		zn09MinSt		= 30;
		private int		zn10HrEn		= 13;
		private int		zn10HrSt		= 13;
		private int		zn10MinEn		= 30;
		private int		zn10MinSt;
		private int		zn11HrEn		= 14;
		private int		zn11HrSt		= 13;
		private int		zn11MinEn;
		private int		zn11MinSt		= 30;
		private int		zn12HrEn		= 14;
		private int		zn12HrSt		= 14;
		private int		zn12MinEn		= 30;
		private int		zn12MinSt;
		private int		zn13HrEn		= 15;
		private int		zn13HrSt		= 14;
		private int		zn13MinEn;
		private int		zn13MinSt		= 30;

		public ColorTimeRegions() { ColorAll = false; }

		[Description("Hour for 1st time region to begin, 24 hour clock.")]
		[Category("01st Time Region")]
		[Gui.Design.DisplayName("Begin Hour")]
		public int Zn01HrSt
		{
			get { return zn01HrSt; }
			set { zn01HrSt = Math.Max(0, value); }
		}

		[Description("Minute for 1st time region to begin")]
		[Category("01st Time Region")]
		[Gui.Design.DisplayName("Begin Minute")]
		public int Zn01MinSt
		{
			get { return zn01MinSt; }
			set { zn01MinSt = Math.Max(0, value); }
		}

		[Description("Hour for 1st time region to end, 24 hour clock.")]
		[Category("01st Time Region")]
		[Gui.Design.DisplayName("End Hour")]
		public int Zn01HrEn
		{
			get { return zn01HrEn; }
			set { zn01HrEn = Math.Max(0, value); }
		}

		[Description("Minute for 1st time region to end")]
		[Category("01st Time Region")]
		[Gui.Design.DisplayName("End Minute")]
		public int Zn01MinEn
		{
			get { return zn01MinEn; }
			set { zn01MinEn = Math.Max(0, value); }
		}

		[Description("Hour for 2nd time region to begin, 24 hour clock.")]
		[Category("02nd Time Region")]
		[Gui.Design.DisplayName("Begin Hour")]
		public int Zn02HrSt
		{
			get { return zn02HrSt; }
			set { zn02HrSt = Math.Max(0, value); }
		}

		[Description("Minute for 2nd time region to begin")]
		[Category("02nd Time Region")]
		[Gui.Design.DisplayName("Begin Minute")]
		public int Zn02MinSt
		{
			get { return zn02MinSt; }
			set { zn02MinSt = Math.Max(0, value); }
		}

		[Description("Hour for 2nd time region to end, 24 hour clock.")]
		[Category("02nd Time Region")]
		[Gui.Design.DisplayName("End Hour")]
		public int Zn02HrEn
		{
			get { return zn02HrEn; }
			set { zn02HrEn = Math.Max(0, value); }
		}

		[Description("Minute for 2nd time region to end")]
		[Category("02nd Time Region")]
		[Gui.Design.DisplayName("End Minute")]
		public int Zn02MinEn
		{
			get { return zn02MinEn; }
			set { zn02MinEn = Math.Max(0, value); }
		}

		[Description("Hour for 3rd time region to begin, 24 hour clock.")]
		[Category("03rd Time Region")]
		[Gui.Design.DisplayName("Begin Hour")]
		public int Zn03HrSt
		{
			get { return zn03HrSt; }
			set { zn03HrSt = Math.Max(0, value); }
		}

		[Description("Minute for 3rd time region to begin")]
		[Category("03rd Time Region")]
		[Gui.Design.DisplayName("Begin Minute")]
		public int Zn03MinSt
		{
			get { return zn03MinSt; }
			set { zn03MinSt = Math.Max(0, value); }
		}

		[Description("Hour for 3rd time region to end, 24 hour clock.")]
		[Category("03rd Time Region")]
		[Gui.Design.DisplayName("End Hour")]
		public int Zn03HrEn
		{
			get { return zn03HrEn; }
			set { zn03HrEn = Math.Max(0, value); }
		}

		[Description("Minute for 3rd time region to end")]
		[Category("03rd Time Region")]
		[Gui.Design.DisplayName("End Minute")]
		public int Zn03MinEn
		{
			get { return zn03MinEn; }
			set { zn03MinEn = Math.Max(0, value); }
		}

		[Description("Hour for 4th time region to begin, 24 hour clock.")]
		[Category("04th Time Region")]
		[Gui.Design.DisplayName("Begin Hour")]
		public int Zn04HrSt
		{
			get { return zn04HrSt; }
			set { zn04HrSt = Math.Max(0, value); }
		}

		[Description("Minute for 4th time region to begin")]
		[Category("04th Time Region")]
		[Gui.Design.DisplayName("Begin Minute")]
		public int Zn04MinSt
		{
			get { return zn04MinSt; }
			set { zn04MinSt = Math.Max(0, value); }
		}

		[Description("Hour for 4th time region to end, 24 hour clock.")]
		[Category("04th Time Region")]
		[Gui.Design.DisplayName("End Hour")]
		public int Zn04HrEn
		{
			get { return zn04HrEn; }
			set { zn04HrEn = Math.Max(0, value); }
		}

		[Description("Minute for 4th time region to end")]
		[Category("04th Time Region")]
		[Gui.Design.DisplayName("End Minute")]
		public int Zn04MinEn
		{
			get { return zn04MinEn; }
			set { zn04MinEn = Math.Max(0, value); }
		}

		[Description("Hour for 5th time region to begin, 24 hour clock.")]
		[Category("05th Time Region")]
		[Gui.Design.DisplayName("Begin Hour")]
		public int Zn05HrSt
		{
			get { return zn05HrSt; }
			set { zn05HrSt = Math.Max(0, value); }
		}

		[Description("Minute for 5th time region to begin")]
		[Category("05th Time Region")]
		[Gui.Design.DisplayName("Begin Minute")]
		public int Zn05MinSt
		{
			get { return zn05MinSt; }
			set { zn05MinSt = Math.Max(0, value); }
		}

		[Description("Hour for 5th time region to end, 24 hour clock.")]
		[Category("05th Time Region")]
		[Gui.Design.DisplayName("End Hour")]
		public int Zn05HrEn
		{
			get { return zn05HrEn; }
			set { zn05HrEn = Math.Max(0, value); }
		}

		[Description("Minute for 5th time region to end")]
		[Category("05th Time Region")]
		[Gui.Design.DisplayName("End Minute")]
		public int Zn05MinEn
		{
			get { return zn05MinEn; }
			set { zn05MinEn = Math.Max(0, value); }
		}

		[Description("Hour for 6th time region to begin, 24 hour clock.")]
		[Category("06th Time Region")]
		[Gui.Design.DisplayName("Begin Hour")]
		public int Zn06HrSt
		{
			get { return zn06HrSt; }
			set { zn06HrSt = Math.Max(0, value); }
		}

		[Description("Minute for 6th time region to begin")]
		[Category("06th Time Region")]
		[Gui.Design.DisplayName("Begin Minute")]
		public int Zn06MinSt
		{
			get { return zn06MinSt; }
			set { zn06MinSt = Math.Max(0, value); }
		}

		[Description("Hour for 6th time region to end, 24 hour clock.")]
		[Category("06th Time Region")]
		[Gui.Design.DisplayName("End Hour")]
		public int Zn06HrEn
		{
			get { return zn06HrEn; }
			set { zn06HrEn = Math.Max(0, value); }
		}

		[Description("Minute for 6th time region to end")]
		[Category("06th Time Region")]
		[Gui.Design.DisplayName("End Minute")]
		public int Zn06MinEn
		{
			get { return zn06MinEn; }
			set { zn06MinEn = Math.Max(0, value); }
		}

		[Description("Hour for 7th time region to begin, 24 hour clock.")]
		[Category("07th Time Region")]
		[Gui.Design.DisplayName("Begin Hour")]
		public int Zn07HrSt
		{
			get { return zn07HrSt; }
			set { zn07HrSt = Math.Max(0, value); }
		}

		[Description("Minute for 7th time region to begin")]
		[Category("07th Time Region")]
		[Gui.Design.DisplayName("Begin Minute")]
		public int Zn07MinSt
		{
			get { return zn07MinSt; }
			set { zn07MinSt = Math.Max(0, value); }
		}

		[Description("Hour for 7th time region to end, 24 hour clock.")]
		[Category("07th Time Region")]
		[Gui.Design.DisplayName("End Hour")]
		public int Zn07HrEn
		{
			get { return zn07HrEn; }
			set { zn07HrEn = Math.Max(0, value); }
		}

		[Description("Minute for 7th time region to end")]
		[Category("07th Time Region")]
		[Gui.Design.DisplayName("End Minute")]
		public int Zn07MinEn
		{
			get { return zn07MinEn; }
			set { zn07MinEn = Math.Max(0, value); }
		}

		[Description("Hour for 8th time region to begin, 24 hour clock.")]
		[Category("08th Time Region")]
		[Gui.Design.DisplayName("Begin Hour")]
		public int Zn08HrSt
		{
			get { return zn08HrSt; }
			set { zn08HrSt = Math.Max(0, value); }
		}

		[Description("Minute for 8th time region to begin")]
		[Category("08th Time Region")]
		[Gui.Design.DisplayName("Begin Minute")]
		public int Zn08MinSt
		{
			get { return zn08MinSt; }
			set { zn08MinSt = Math.Max(0, value); }
		}

		[Description("Hour for 8th time region to end, 24 hour clock.")]
		[Category("08th Time Region")]
		[Gui.Design.DisplayName("End Hour")]
		public int Zn08HrEn
		{
			get { return zn08HrEn; }
			set { zn08HrEn = Math.Max(0, value); }
		}

		[Description("Minute for 8th time region to end")]
		[Category("08th Time Region")]
		[Gui.Design.DisplayName("End Minute")]
		public int Zn08MinEn
		{
			get { return zn08MinEn; }
			set { zn08MinEn = Math.Max(0, value); }
		}

		[Description("Hour for 9th time region to begin, 24 hour clock.")]
		[Category("09th Time Region")]
		[Gui.Design.DisplayName("Begin Hour")]
		public int Zn09HrSt
		{
			get { return zn09HrSt; }
			set { zn09HrSt = Math.Max(0, value); }
		}

		[Description("Minute for 9th time region to begin")]
		[Category("09th Time Region")]
		[Gui.Design.DisplayName("Begin Minute")]
		public int Zn09MinSt
		{
			get { return zn09MinSt; }
			set { zn09MinSt = Math.Max(0, value); }
		}

		[Description("Hour for 9th time region to end, 24 hour clock.")]
		[Category("09th Time Region")]
		[Gui.Design.DisplayName("End Hour")]
		public int Zn09HrEn
		{
			get { return zn09HrEn; }
			set { zn09HrEn = Math.Max(0, value); }
		}

		[Description("Minute for 9th time region to end")]
		[Category("09th Time Region")]
		[Gui.Design.DisplayName("End Minute")]
		public int Zn09MinEn
		{
			get { return zn09MinEn; }
			set { zn09MinEn = Math.Max(0, value); }
		}

		[Description("Hour for 10th time region to begin, 24 hour clock.")]
		[Category("10th Time Region")]
		[Gui.Design.DisplayName("Begin Hour")]
		public int Zn10HrSt
		{
			get { return zn10HrSt; }
			set { zn10HrSt = Math.Max(0, value); }
		}

		[Description("Minute for 10th time region to begin")]
		[Category("10th Time Region")]
		[Gui.Design.DisplayName("Begin Minute")]
		public int Zn10MinSt
		{
			get { return zn10MinSt; }
			set { zn10MinSt = Math.Max(0, value); }
		}

		[Description("Hour for 10th time region to end, 24 hour clock.")]
		[Category("10th Time Region")]
		[Gui.Design.DisplayName("End Hour")]
		public int Zn10HrEn
		{
			get { return zn10HrEn; }
			set { zn10HrEn = Math.Max(0, value); }
		}

		[Description("Minute for 10th time region to end")]
		[Category("10th Time Region")]
		[Gui.Design.DisplayName("End Minute")]
		public int Zn10MinEn
		{
			get { return zn10MinEn; }
			set { zn10MinEn = Math.Max(0, value); }
		}

		[Description("Hour for 11th time region to begin, 24 hour clock.")]
		[Category("11th Time Region")]
		[Gui.Design.DisplayName("Begin Hour")]
		public int Zn11HrSt
		{
			get { return zn11HrSt; }
			set { zn11HrSt = Math.Max(0, value); }
		}

		[Description("Minute for 11th time region to begin")]
		[Category("11th Time Region")]
		[Gui.Design.DisplayName("Begin Minute")]
		public int Zn11MinSt
		{
			get { return zn11MinSt; }
			set { zn11MinSt = Math.Max(0, value); }
		}

		[Description("Hour for 11th time region to end, 24 hour clock.")]
		[Category("11th Time Region")]
		[Gui.Design.DisplayName("End Hour")]
		public int Zn11HrEn
		{
			get { return zn11HrEn; }
			set { zn11HrEn = Math.Max(0, value); }
		}

		[Description("Minute for 11th time region to end")]
		[Category("11th Time Region")]
		[Gui.Design.DisplayName("End Minute")]
		public int Zn11MinEn
		{
			get { return zn11MinEn; }
			set { zn11MinEn = Math.Max(0, value); }
		}

		[Description("Hour for 12th time region to begin, 24 hour clock.")]
		[Category("12th Time Region")]
		[Gui.Design.DisplayName("Begin Hour")]
		public int Zn12HrSt
		{
			get { return zn12HrSt; }
			set { zn12HrSt = Math.Max(0, value); }
		}

		[Description("Minute for 12th time region to begin")]
		[Category("12th Time Region")]
		[Gui.Design.DisplayName("Begin Minute")]
		public int Zn12MinSt
		{
			get { return zn12MinSt; }
			set { zn12MinSt = Math.Max(0, value); }
		}

		[Description("Hour for 12th time region to end, 24 hour clock.")]
		[Category("12th Time Region")]
		[Gui.Design.DisplayName("End Hour")]
		public int Zn12HrEn
		{
			get { return zn12HrEn; }
			set { zn12HrEn = Math.Max(0, value); }
		}

		[Description("Minute for 12th time region to end")]
		[Category("12th Time Region")]
		[Gui.Design.DisplayName("End Minute")]
		public int Zn12MinEn
		{
			get { return zn12MinEn; }
			set { zn12MinEn = Math.Max(0, value); }
		}

		[Description("Hour for 13th time region to begin, 24 hour clock.")]
		[Category("13th Time Region")]
		[Gui.Design.DisplayName("Begin Hour")]
		public int Zn13HrSt
		{
			get { return zn13HrSt; }
			set { zn13HrSt = Math.Max(0, value); }
		}

		[Description("Minute for 13th time region to begin")]
		[Category("13th Time Region")]
		[Gui.Design.DisplayName("Begin Minute")]
		public int Zn13MinSt
		{
			get { return zn13MinSt; }
			set { zn13MinSt = Math.Max(0, value); }
		}

		[Description("Hour for 13th time region to end, 24 hour clock.")]
		[Category("13th Time Region")]
		[Gui.Design.DisplayName("End Hour")]
		public int Zn13HrEn
		{
			get { return zn13HrEn; }
			set { zn13HrEn = Math.Max(0, value); }
		}

		[Description("Minute for 13th time region to end")]
		[Category("13th Time Region")]
		[Gui.Design.DisplayName("End Minute")]
		public int Zn13MinEn
		{
			get { return zn13MinEn; }
			set { zn13MinEn = Math.Max(0, value); }
		}

		[Description("Colors across all panels or just main price display panel"), Category("Parameters"),
		 Gui.Design.DisplayName("Color all?")]
		public bool ColorAll { get; set; }

		[Description("Text for 1st region")]
		[Category("01st Time Region")]
		[Gui.Design.DisplayName("Region Text")]
		public string Region01Text
		{
			get { return s01; }
			set { s01 = value; }
		}

		[Description("Text for 2nd region")]
		[Category("02nd Time Region")]
		[Gui.Design.DisplayName("Region Text")]
		public string Region02Text
		{
			get { return s02; }
			set { s02 = value; }
		}

		[Description("Text for 3rd region")]
		[Category("03rd Time Region")]
		[Gui.Design.DisplayName("Region Text")]
		public string Region03Text
		{
			get { return s03; }
			set { s03 = value; }
		}

		[Description("Text for 4th region")]
		[Category("04th Time Region")]
		[Gui.Design.DisplayName("Region Text")]
		public string Region04Text
		{
			get { return s04; }
			set { s04 = value; }
		}

		[Description("Text for 5th region")]
		[Category("05th Time Region")]
		[Gui.Design.DisplayName("Region Text")]
		public string Region05Text
		{
			get { return s05; }
			set { s05 = value; }
		}

		[Description("Text for 6th region")]
		[Category("06th Time Region")]
		[Gui.Design.DisplayName("Region Text")]
		public string Region06Text
		{
			get { return s06; }
			set { s06 = value; }
		}

		[Description("Text for 7th region")]
		[Category("07th Time Region")]
		[Gui.Design.DisplayName("Region Text")]
		public string Region07Text
		{
			get { return s07; }
			set { s07 = value; }
		}

		[Description("Text for 8th region")]
		[Category("08th Time Region")]
		[Gui.Design.DisplayName("Region Text")]
		public string Region08Text
		{
			get { return s08; }
			set { s08 = value; }
		}

		[Description("Text for 9th region")]
		[Category("09th Time Region")]
		[Gui.Design.DisplayName("Region Text")]
		public string Region09Text
		{
			get { return s09; }
			set { s09 = value; }
		}

		[Description("Text for 10th region")]
		[Category("10th Time Region")]
		[Gui.Design.DisplayName("Region Text")]
		public string Region10Text
		{
			get { return s10; }
			set { s10 = value; }
		}

		[Description("Text for 11th region")]
		[Category("11th Time Region")]
		[Gui.Design.DisplayName("Region Text")]
		public string Region11Text
		{
			get { return s11; }
			set { s11 = value; }
		}

		[Description("Text for 12th region")]
		[Category("12th Time Region")]
		[Gui.Design.DisplayName("Region Text")]
		public string Region12Text
		{
			get { return s12; }
			set { s12 = value; }
		}

		[Description("Text for 13th region")]
		[Category("13th Time Region")]
		[Gui.Design.DisplayName("Region Text")]
		public string Region13Text
		{
			get { return s13; }
			set { s13 = value; }
		}

		[XmlIgnore]
		[Description("Color for 1st region")]
		[Category("01st Time Region")]
		[Gui.Design.DisplayName("Region Color")]
		public Color Region01Color
		{
			get { return region01Color; }
			set { region01Color = value; }
		}

		[Browsable(false)]
		public string Region01ColorSerialize
		{
			get { return SerializableColor.ToString(region01Color); }
			set { region01Color = SerializableColor.FromString(value); }
		}

		[XmlIgnore]
		[Description("Color for 2nd region")]
		[Category("02nd Time Region")]
		[Gui.Design.DisplayName("Region Color")]
		public Color Region02Color
		{
			get { return region02Color; }
			set { region02Color = value; }
		}

		[Browsable(false)]
		public string Region02ColorSerialize
		{
			get { return SerializableColor.ToString(region02Color); }
			set { region02Color = SerializableColor.FromString(value); }
		}

		[XmlIgnore]
		[Description("Color for 3rd region")]
		[Category("03rd Time Region")]
		[Gui.Design.DisplayName("Region Color")]
		public Color Region03Color
		{
			get { return region03Color; }
			set { region03Color = value; }
		}

		[Browsable(false)]
		public string Region03ColorSerialize
		{
			get { return SerializableColor.ToString(region03Color); }
			set { region03Color = SerializableColor.FromString(value); }
		}

		[XmlIgnore]
		[Description("Color for 4th region")]
		[Category("04th Time Region")]
		[Gui.Design.DisplayName("Region Color")]
		public Color Region04Color
		{
			get { return region04Color; }
			set { region04Color = value; }
		}

		[Browsable(false)]
		public string Region04ColorSerialize
		{
			get { return SerializableColor.ToString(region04Color); }
			set { region04Color = SerializableColor.FromString(value); }
		}

		[XmlIgnore]
		[Description("Color for 5th region")]
		[Category("05th Time Region")]
		[Gui.Design.DisplayName("Region Color")]
		public Color Region05Color
		{
			get { return region05Color; }
			set { region05Color = value; }
		}

		[Browsable(false)]
		public string Region05ColorSerialize
		{
			get { return SerializableColor.ToString(region05Color); }
			set { region05Color = SerializableColor.FromString(value); }
		}

		[XmlIgnore]
		[Description("Color for 6th region")]
		[Category("06th Time Region")]
		[Gui.Design.DisplayName("Region Color")]
		public Color Region06Color
		{
			get { return region06Color; }
			set { region06Color = value; }
		}

		[Browsable(false)]
		public string Region06ColorSerialize
		{
			get { return SerializableColor.ToString(region06Color); }
			set { region06Color = SerializableColor.FromString(value); }
		}

		[XmlIgnore]
		[Description("Color for 7th region")]
		[Category("07th Time Region")]
		[Gui.Design.DisplayName("Region Color")]
		public Color Region07Color
		{
			get { return region07Color; }
			set { region07Color = value; }
		}

		[Browsable(false)]
		public string Region07ColorSerialize
		{
			get { return SerializableColor.ToString(region07Color); }
			set { region07Color = SerializableColor.FromString(value); }
		}

		[XmlIgnore]
		[Description("Color for 8th region")]
		[Category("08th Time Region")]
		[Gui.Design.DisplayName("Region Color")]
		public Color Region08Color
		{
			get { return region08Color; }
			set { region08Color = value; }
		}

		[Browsable(false)]
		public string Region08ColorSerialize
		{
			get { return SerializableColor.ToString(region08Color); }
			set { region08Color = SerializableColor.FromString(value); }
		}

		[XmlIgnore]
		[Description("Color for 9th region")]
		[Category("09th Time Region")]
		[Gui.Design.DisplayName("Region Color")]
		public Color Region09Color
		{
			get { return region09Color; }
			set { region09Color = value; }
		}

		[Browsable(false)]
		public string Region09ColorSerialize
		{
			get { return SerializableColor.ToString(region09Color); }
			set { region09Color = SerializableColor.FromString(value); }
		}

		[XmlIgnore]
		[Description("Color for 10th region")]
		[Category("10th Time Region")]
		[Gui.Design.DisplayName("Region Color")]
		public Color Region10Color
		{
			get { return region10Color; }
			set { region10Color = value; }
		}

		[Browsable(false)]
		public string Region10ColorSerialize
		{
			get { return SerializableColor.ToString(region10Color); }
			set { region10Color = SerializableColor.FromString(value); }
		}

		[XmlIgnore]
		[Description("Color for 11th region")]
		[Category("11th Time Region")]
		[Gui.Design.DisplayName("Region Color")]
		public Color Region11Color
		{
			get { return region11Color; }
			set { region11Color = value; }
		}

		[Browsable(false)]
		public string Region11ColorSerialize
		{
			get { return SerializableColor.ToString(region11Color); }
			set { region11Color = SerializableColor.FromString(value); }
		}

		[XmlIgnore]
		[Description("Color for 12th region")]
		[Category("12th Time Region")]
		[Gui.Design.DisplayName("Region Color")]
		public Color Region12Color
		{
			get { return region12Color; }
			set { region12Color = value; }
		}

		[Browsable(false)]
		public string Region12ColorSerialize
		{
			get { return SerializableColor.ToString(region12Color); }
			set { region12Color = SerializableColor.FromString(value); }
		}

		[XmlIgnore]
		[Description("Color for 13th region")]
		[Category("13th Time Region")]
		[Gui.Design.DisplayName("Region Color")]
		public Color Region13Color
		{
			get { return region13Color; }
			set { region13Color = value; }
		}

		[Browsable(false)]
		public string Region13ColorSerialize
		{
			get { return SerializableColor.ToString(region13Color); }
			set { region13Color = SerializableColor.FromString(value); }
		}


		[Description("Sound and display an alert with Alerts window open ")]
		[Category("Parameters")]
		[Gui.Design.DisplayName("Alert on Begin/End?")]
		public bool AlertBool
		{
			get { return alertBool; }
			set { alertBool = value; }
		}

		[XmlIgnore]
		[Description("Text Color")]
		[Category("Text")]
		[Gui.Design.DisplayName("Text Color")]
		public Color TextColor
		{
			get { return textColor; }
			set { textColor = value; }
		}

		[Browsable(false)]
		public string TextColorSerialize
		{
			get { return SerializableColor.ToString(textColor); }
			set { textColor = SerializableColor.FromString(value); }
		}

		[XmlIgnore]
		[Description("Text Font")]
		[Category("Text")]
		[Gui.Design.DisplayName("Text Font")]
		public Font TextFont
		{
			get { return textFont; }
			set { textFont = value; }
		}

		[Browsable(false)]
		public string TextFontSerialize
		{
			get { return SerializableFont.ToString(textFont); }
			set { textFont = SerializableFont.FromString(value); }
		}

		protected override void Initialize()
		{
			Overlay = true;
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar == 0) return;

			int cZn01St = ((Zn01HrSt * 10000) + (Zn01MinSt * 100));
			int cZn01En = ((Zn01HrEn * 10000) + (Zn01MinEn * 100));

			int cZn02St = ((Zn02HrSt * 10000) + (Zn02MinSt * 100));
			int cZn02En = ((Zn02HrEn * 10000) + (Zn02MinEn * 100));

			int cZn03St = ((Zn03HrSt * 10000) + (Zn03MinSt * 100));
			int cZn03En = ((Zn03HrEn * 10000) + (Zn03MinEn * 100));

			int cZn04St = ((Zn04HrSt * 10000) + (Zn04MinSt * 100));
			int cZn04En = ((Zn04HrEn * 10000) + (Zn04MinEn * 100));

			int cZn05St = ((Zn05HrSt * 10000) + (Zn05MinSt * 100));
			int cZn05En = ((Zn05HrEn * 10000) + (Zn05MinEn * 100));

			int cZn06St = ((Zn06HrSt * 10000) + (Zn06MinSt * 100));
			int cZn06En = ((Zn06HrEn * 10000) + (Zn06MinEn * 100));

			int cZn07St = ((Zn07HrSt * 10000) + (Zn07MinSt * 100));
			int cZn07En = ((Zn07HrEn * 10000) + (Zn07MinEn * 100));

			int cZn08St = ((Zn08HrSt * 10000) + (Zn08MinSt * 100));
			int cZn08En = ((Zn08HrEn * 10000) + (Zn08MinEn * 100));

			int cZn09St = ((Zn09HrSt * 10000) + (Zn09MinSt * 100));
			int cZn09En = ((Zn09HrEn * 10000) + (Zn09MinEn * 100));

			int cZn10St = ((Zn10HrSt * 10000) + (Zn10MinSt * 100));
			int cZn10En = ((Zn10HrEn * 10000) + (Zn10MinEn * 100));

			int cZn11St = ((Zn11HrSt * 10000) + (Zn11MinSt * 100));
			int cZn11En = ((Zn11HrEn * 10000) + (Zn11MinEn * 100));

			int cZn12St = ((Zn12HrSt * 10000) + (Zn12MinSt * 100));
			int cZn12En = ((Zn12HrEn * 10000) + (Zn12MinEn * 100));

			int cZn13St = ((Zn13HrSt * 10000) + (Zn13MinSt * 100));
			int cZn13En = ((Zn13HrEn * 10000) + (Zn13MinEn * 100));

			if (!ColorAll)
			{
				if (ToTime(Time[0]) > cZn01St && ToTime(Time[0]) <= cZn01En) BackColor = Region01Color;
				if (ToTime(Time[0]) > cZn02St && ToTime(Time[0]) <= cZn02En) BackColor = Region02Color;
				if (ToTime(Time[0]) > cZn03St && ToTime(Time[0]) <= cZn03En) BackColor = Region03Color;
				if (ToTime(Time[0]) > cZn04St && ToTime(Time[0]) <= cZn04En) BackColor = Region04Color;
				if (ToTime(Time[0]) > cZn05St && ToTime(Time[0]) <= cZn05En) BackColor = Region05Color;
				if (ToTime(Time[0]) > cZn06St && ToTime(Time[0]) <= cZn06En) BackColor = Region06Color;
				if (ToTime(Time[0]) > cZn07St && ToTime(Time[0]) <= cZn07En) BackColor = Region07Color;
				if (ToTime(Time[0]) > cZn08St && ToTime(Time[0]) <= cZn08En) BackColor = Region08Color;
				if (ToTime(Time[0]) > cZn09St && ToTime(Time[0]) <= cZn09En) BackColor = Region09Color;
				if (ToTime(Time[0]) > cZn10St && ToTime(Time[0]) <= cZn10En) BackColor = Region10Color;
				if (ToTime(Time[0]) > cZn11St && ToTime(Time[0]) <= cZn11En) BackColor = Region11Color;
				if (ToTime(Time[0]) > cZn12St && ToTime(Time[0]) <= cZn12En) BackColor = Region12Color;
				if (ToTime(Time[0]) > cZn13St && ToTime(Time[0]) <= cZn13En) BackColor = Region13Color;
			}
			else if (ColorAll)
			{
				if (ToTime(Time[0]) > cZn01St && ToTime(Time[0]) <= cZn01En) BackColorAll = Region01Color;
				if (ToTime(Time[0]) > cZn02St && ToTime(Time[0]) <= cZn02En) BackColorAll = Region02Color;
				if (ToTime(Time[0]) > cZn03St && ToTime(Time[0]) <= cZn03En) BackColorAll = Region03Color;
				if (ToTime(Time[0]) > cZn04St && ToTime(Time[0]) <= cZn04En) BackColorAll = Region04Color;
				if (ToTime(Time[0]) > cZn05St && ToTime(Time[0]) <= cZn05En) BackColorAll = Region05Color;
				if (ToTime(Time[0]) > cZn06St && ToTime(Time[0]) <= cZn06En) BackColorAll = Region06Color;
				if (ToTime(Time[0]) > cZn07St && ToTime(Time[0]) <= cZn07En) BackColorAll = Region07Color;
				if (ToTime(Time[0]) > cZn08St && ToTime(Time[0]) <= cZn08En) BackColorAll = Region08Color;
				if (ToTime(Time[0]) > cZn09St && ToTime(Time[0]) <= cZn09En) BackColorAll = Region09Color;
				if (ToTime(Time[0]) > cZn10St && ToTime(Time[0]) <= cZn10En) BackColorAll = Region10Color;
				if (ToTime(Time[0]) > cZn11St && ToTime(Time[0]) <= cZn11En) BackColorAll = Region11Color;
				if (ToTime(Time[0]) > cZn12St && ToTime(Time[0]) <= cZn12En) BackColorAll = Region12Color;
				if (ToTime(Time[0]) > cZn13St && ToTime(Time[0]) <= cZn13En) BackColorAll = Region13Color;
			}


			if (AlertBool)
				if (ToTime(Time[0]) == cZn01St)
					Alert("r1st", Priority.Medium, "Beginning of Time Region #1", "Alert2.wav", 0, Region01Color, Color.Black);
				else if (ToTime(Time[0]) == cZn01En)
					Alert("r1en", Priority.Medium, "End of Time Region #1", "Alert2.wav", 0, Region01Color, Color.Black);
				else if (ToTime(Time[0]) == cZn02St)
					Alert("r2st", Priority.Medium, "Beginning of Time Region #2", "Alert2.wav", 0, Region02Color, Color.Black);
				else if (ToTime(Time[0]) == cZn02En)
					Alert("r2en", Priority.Medium, "End of Time Region #2", "Alert2.wav", 0, Region02Color, Color.Black);
				else if (ToTime(Time[0]) == cZn03St)
					Alert("r3st", Priority.Medium, "Beginning of Time Region #3", "Alert2.wav", 0, Region03Color, Color.Black);
				else if (ToTime(Time[0]) == cZn03En)
					Alert("r3en", Priority.Medium, "End of Time Region #3", "Alert2.wav", 0, Region03Color, Color.Black);
				else if (ToTime(Time[0]) == cZn04St)
					Alert("r4st", Priority.Medium, "Beginning of Time Region #4", "Alert2.wav", 0, Region04Color, Color.Black);
				else if (ToTime(Time[0]) == cZn04En)
					Alert("r4en", Priority.Medium, "End of Time Region #4", "Alert2.wav", 0, Region04Color, Color.Black);
				else if (ToTime(Time[0]) == cZn05St)
					Alert("r5st", Priority.Medium, "Beginning of Time Region #5", "Alert2.wav", 0, Region05Color, Color.Black);
				else if (ToTime(Time[0]) == cZn05En)
					Alert("r5en", Priority.Medium, "End of Time Region #5", "Alert2.wav", 0, Region05Color, Color.Black);
				else if (ToTime(Time[0]) == cZn06St)
					Alert("r6st", Priority.Medium, "Beginning of Time Region #6", "Alert2.wav", 0, Region06Color, Color.Black);
				else if (ToTime(Time[0]) == cZn06En)
					Alert("r6en", Priority.Medium, "End of Time Region #6", "Alert2.wav", 0, Region06Color, Color.Black);
				else if (ToTime(Time[0]) == cZn07St)
					Alert("r7st", Priority.Medium, "Beginning of Time Region #7", "Alert2.wav", 0, Region07Color, Color.Black);
				else if (ToTime(Time[0]) == cZn07En)
					Alert("r7en", Priority.Medium, "End of Time Region #7", "Alert2.wav", 0, Region07Color, Color.Black);
				else if (ToTime(Time[0]) == cZn08St)
					Alert("r8st", Priority.Medium, "Beginning of Time Region #8", "Alert2.wav", 0, Region08Color, Color.Black);
				else if (ToTime(Time[0]) == cZn08En)
					Alert("r8en", Priority.Medium, "End of Time Region #8", "Alert2.wav", 0, Region08Color, Color.Black);
				else if (ToTime(Time[0]) == cZn09St)
					Alert("r9st", Priority.Medium, "Beginning of Time Region #9", "Alert2.wav", 0, Region09Color, Color.Black);
				else if (ToTime(Time[0]) == cZn09En)
					Alert("r9en", Priority.Medium, "End of Time Region #9", "Alert2.wav", 0, Region09Color, Color.Black);
				else if (ToTime(Time[0]) == cZn10St)
					Alert("r10st", Priority.Medium, "Beginning of Time Region #10", "Alert2.wav", 0, Region10Color, Color.Black);
				else if (ToTime(Time[0]) == cZn10En)
					Alert("r10en", Priority.Medium, "End of Time Region #10", "Alert2.wav", 0, Region10Color, Color.Black);
				else if (ToTime(Time[0]) == cZn11St)
					Alert("r11st", Priority.Medium, "Beginning of Time Region #11", "Alert2.wav", 0, Region11Color, Color.Black);
				else if (ToTime(Time[0]) == cZn11En)
					Alert("r11en", Priority.Medium, "End of Time Region #11", "Alert2.wav", 0, Region11Color, Color.Black);
				else if (ToTime(Time[0]) == cZn12St)
					Alert("r12st", Priority.Medium, "Beginning of Time Region #12", "Alert2.wav", 0, Region12Color, Color.Black);
				else if (ToTime(Time[0]) == cZn12En)
					Alert("r12en", Priority.Medium, "End of Time Region #12", "Alert2.wav", 0, Region12Color, Color.Black);
				else if (ToTime(Time[0]) == cZn13St)
					Alert("r13st", Priority.Medium, "Beginning of Time Region #13", "Alert2.wav", 0, Region13Color, Color.Black);

			if (ToTime(Time[0]) > cZn01St && ToTime(Time[1]) <= cZn01St && !string.IsNullOrEmpty(s01))
				DrawText(CurrentBar.ToString(CultureInfo.InvariantCulture), true, s01, 0, High[0], 0, TextColor, TextFont, StringAlignment.Near, Color.Empty, Color.Empty, 10);
			else if (ToTime(Time[0]) > cZn02St && ToTime(Time[1]) <= cZn02St && !string.IsNullOrEmpty(s02))
				DrawText(CurrentBar.ToString(CultureInfo.InvariantCulture), true, s02, 0, High[0], 0, TextColor, TextFont, StringAlignment.Near, Color.Empty, Color.Empty, 10);
			else if (ToTime(Time[0]) > cZn03St && ToTime(Time[1]) <= cZn03St && !string.IsNullOrEmpty(s03))
				DrawText(CurrentBar.ToString(CultureInfo.InvariantCulture), true, s03, 0, High[0], 0, TextColor, TextFont, StringAlignment.Near, Color.Empty, Color.Empty, 10);
			else if (ToTime(Time[0]) > cZn04St && ToTime(Time[1]) <= cZn04St && !string.IsNullOrEmpty(s04))
				DrawText(CurrentBar.ToString(CultureInfo.InvariantCulture), true, s04, 0, High[0], 0, TextColor, TextFont, StringAlignment.Near, Color.Empty, Color.Empty, 10);
			else if (ToTime(Time[0]) > cZn05St && ToTime(Time[1]) <= cZn05St && !string.IsNullOrEmpty(s05)) 
				DrawText(CurrentBar.ToString(CultureInfo.InvariantCulture), true, s05, 0, High[0], 0, TextColor, TextFont, StringAlignment.Near, Color.Empty, Color.Empty, 10);
			else if (ToTime(Time[0]) > cZn06St && ToTime(Time[1]) <= cZn06St && !string.IsNullOrEmpty(s06))
				DrawText(CurrentBar.ToString(CultureInfo.InvariantCulture), true, s06, 0, High[0], 0, TextColor, TextFont, StringAlignment.Near, Color.Empty, Color.Empty, 10);
			else if (ToTime(Time[0]) > cZn07St && ToTime(Time[1]) <= cZn07St && !string.IsNullOrEmpty(s07))
				DrawText(CurrentBar.ToString(CultureInfo.InvariantCulture), true, s07, 0, High[0], 0, TextColor, TextFont, StringAlignment.Near, Color.Empty, Color.Empty, 10);
			else if (ToTime(Time[0]) > cZn08St && ToTime(Time[1]) <= cZn08St && !string.IsNullOrEmpty(s08))
				DrawText(CurrentBar.ToString(CultureInfo.InvariantCulture), true, s08, 0, High[0], 0, TextColor, TextFont, StringAlignment.Near, Color.Empty, Color.Empty, 10);
			else if (ToTime(Time[0]) > cZn09St && ToTime(Time[1]) <= cZn09St && !string.IsNullOrEmpty(s09))
				DrawText(CurrentBar.ToString(CultureInfo.InvariantCulture), true, s09, 0, High[0], 0, TextColor, TextFont, StringAlignment.Near, Color.Empty, Color.Empty, 10);
			else if (ToTime(Time[0]) > cZn10St && ToTime(Time[1]) <= cZn10St && !string.IsNullOrEmpty(s10))
				DrawText(CurrentBar.ToString(CultureInfo.InvariantCulture), true, s10, 0, High[0], 0, TextColor, TextFont, StringAlignment.Near, Color.Empty, Color.Empty, 10);
			else if (ToTime(Time[0]) > cZn11St && ToTime(Time[1]) <= cZn11St && !string.IsNullOrEmpty(s11))
				DrawText(CurrentBar.ToString(CultureInfo.InvariantCulture), true, s11, 0, High[0], 0, TextColor, TextFont, StringAlignment.Near, Color.Empty, Color.Empty, 10);
			else if (ToTime(Time[0]) > cZn12St && ToTime(Time[1]) <= cZn12St && !string.IsNullOrEmpty(s12))
				DrawText(CurrentBar.ToString(CultureInfo.InvariantCulture), true, s12, 0, High[0], 0, TextColor, TextFont, StringAlignment.Near, Color.Empty, Color.Empty, 10);
			else if (ToTime(Time[0]) > cZn13St && ToTime(Time[1]) <= cZn13St && !string.IsNullOrEmpty(s13))
				DrawText(CurrentBar.ToString(CultureInfo.InvariantCulture), true, s13, 0, High[0], 0, TextColor, TextFont, StringAlignment.Near, Color.Empty, Color.Empty, 10);
		}
	}
}
