namespace GameLib.Util {
	/// <summary>イージング関数群クラス</summary>
	public static class Easing {
		private static readonly double c1 = 1.70158;
		private static readonly double c3 = c1 + 1;
		private static readonly double c2 = c1 * 1.525;
		private static readonly double c4 = (2 * System.Math.PI) / 3;
		private static readonly double c5 = (2 * System.Math.PI) / 4.5;
		// private static readonly double n1 = 7.5625;
		// private static readonly double d1 = 2.75;

		/// <summary>イージング関数</summary>
		/// <param name="x">0.0~1.0 の値</param>
		public static double inSine(double x)  { return 1 - System.Math.Cos((x * System.Math.PI) / 2); }
		/// <summary>イージング関数</summary>
		/// <param name="x">0.0~1.0 の値</param>
		public static double outSine(double x) { return System.Math.Sin((x*System.Math.PI)/2); }
		/// <summary>イージング関数</summary>
		/// <param name="x">0.0~1.0 の値</param>
		public static double inOutSine(double x) { return -(System.Math.Cos(System.Math.PI * x) - 1) / 2; }
		/// <summary>イージング関数</summary>
		/// <param name="x">0.0~1.0 の値</param>
		public static double inQuad(double x) { return x * x; }
		/// <summary>イージング関数</summary>
		/// <param name="x">0.0~1.0 の値</param>
		public static double outQuad(double x) { return 1 - (1 - x) * (1 - x); }
		/// <summary>イージング関数</summary>
		/// <param name="x">0.0~1.0 の値</param>
		public static double inOutQuad(double x) { return x < 0.5 ? 2 * x * x : 1 - System.Math.Pow(-2 * x + 2, 2) / 2; }
		/// <summary>イージング関数</summary>
		/// <param name="x">0.0~1.0 の値</param>
		public static double inCubic(double x) { return x * x * x; }
		/// <summary>イージング関数</summary>
		/// <param name="x">0.0~1.0 の値</param>
		public static double outCubic(double x) { return 1 - System.Math.Pow(1 - x, 3); }
		/// <summary>イージング関数</summary>
		/// <param name="x">0.0~1.0 の値</param>
		public static double inOutCubic(double x) { return x < 0.5 ? 4 * x * x * x : 1 - System.Math.Pow(-2 * x + 2, 3) / 2; }
		/// <summary>イージング関数</summary>
		/// <param name="x">0.0~1.0 の値</param>
		public static double inQuard(double x) { return x * x * x * x; }
		/// <summary>イージング関数</summary>
		/// <param name="x">0.0~1.0 の値</param>
		public static double outQuard(double x) { return  1 - System.Math.Pow(1 - x, 4); }
		/// <summary>イージング関数</summary>
		/// <param name="x">0.0~1.0 の値</param>
		public static double inOutQuard(double x) { return x < 0.5 ? 8 * x * x * x * x : 1 - System.Math.Pow(-2 * x + 2, 4) / 2; }
		/// <summary>イージング関数</summary>
		/// <param name="x">0.0~1.0 の値</param>
		public static double inQuint(double x) { return x * x * x * x * x; }
		/// <summary>イージング関数</summary>
		/// <param name="x">0.0~1.0 の値</param>
		public static double outQuint(double x) { return 1 - System.Math.Pow(1 - x, 5); }
		/// <summary>イージング関数</summary>
		/// <param name="x">0.0~1.0 の値</param>
		public static double inOutQuint(double x) { return x < 0.5 ? 16 * x * x * x * x * x : 1 - System.Math.Pow(-2 * x + 2, 5) / 2; }
		/// <summary>イージング関数</summary>
		/// <param name="x">0.0~1.0 の値</param>
		public static double inExpo(double x) { return x == 0 ? 0 : System.Math.Pow(2, 10 * x - 10); }
		/// <summary>イージング関数</summary>
		/// <param name="x">0.0~1.0 の値</param>
		public static double outExpo(double x) { return x == 1 ? 1 : 1 - System.Math.Pow(2, -10 * x); }
		/// <summary>イージング関数</summary>
		/// <param name="x">0.0~1.0 の値</param>
		public static double inOutExpo(double x) { return x == 0 ? 0 : x == 1 ? 1 : x < 0.5 ? System.Math.Pow(2, 20 * x - 10) / 2 : (2 - System.Math.Pow(2, -20 * x + 10)) / 2; }
		/// <summary>イージング関数</summary>
		/// <param name="x">0.0~1.0 の値</param>
		public static double inCirc(double x) { return 1 - System.Math.Sqrt(1 - System.Math.Pow(x, 2)); }
		/// <summary>イージング関数</summary>
		/// <param name="x">0.0~1.0 の値</param>
		public static double outCirc(double x) { return System.Math.Sqrt(1 - System.Math.Pow(x - 1, 2)); }
		/// <summary>イージング関数</summary>
		/// <param name="x">0.0~1.0 の値</param>
		public static double inOutCirc(double x) { return x < 0.5 ? (1 - System.Math.Sqrt(1 - System.Math.Pow(2 * x, 2))) / 2 : (System.Math.Sqrt(1 - System.Math.Pow(-2 * x + 2, 2)) + 1) / 2; }
		/// <summary>イージング関数</summary>
		/// <param name="x">0.0~1.0 の値</param>
		public static double inBack(double x) { return c3 * x * x * x - c1 * x * x; }
		/// <summary>イージング関数</summary>
		/// <param name="x">0.0~1.0 の値</param>
		public static double outBack(double x) { return 1 + c3 * System.Math.Pow(x - 1, 3) + c1 * System.Math.Pow(x - 1, 2); }
		/// <summary>イージング関数</summary>
		/// <param name="x">0.0~1.0 の値</param>
		public static double inOutBack(double x) { return x < 0.5 ? (System.Math.Pow(2 * x, 2) * ((c2 + 1) * 2 * x - c2)) / 2 : (System.Math.Pow(2 * x - 2, 2) * ((c2 + 1) * (x * 2 - 2) + c2) + 2) / 2; }
		/// <summary>イージング関数</summary>
		/// <param name="x">0.0~1.0 の値</param>
		public static double inElastic(double x) { return x == 0 ? 0 : x == 1 ? 1 : -System.Math.Pow(2, 10 * x - 10) * System.Math.Sin((x * 10 - 10.75) * c4); }
		/// <summary>イージング関数</summary>
		/// <param name="x">0.0~1.0 の値</param>
		public static double outElastic(double x) { return x == 0 ? 0 : x == 1 ? 1 : System.Math.Pow(2, -10 * x) * System.Math.Sin((x * 10 - 0.75) * c4) + 1; }
		/// <summary>イージング関数</summary>
		/// <param name="x">0.0~1.0 の値</param>
		public static double inOutElastic(double x) { return x == 0 ? 0 : x == 1 ? 1 : x < 0.5 ? -(System.Math.Pow(2, 20 * x - 10) * System.Math.Sin((20 * x - 11.125) * c5)) / 2 : (System.Math.Pow(2, -20 * x + 10) * System.Math.Sin((20 * x - 11.125) * c5)) / 2 + 1; }
		/// <summary>イージング関数</summary>
		/// <param name="x">0.0~1.0 の値</param>
		public static double inBounce(double x) { return 1 - outBounce(1 - x); }
		/// <summary>イージング関数</summary>
		/// <param name="x">0.0~1.0 の値</param>
		public static double outBounce(double x) { return x; }
		/// <summary>イージング関数</summary>
		/// <param name="x">0.0~1.0 の値</param>
		public static double inOutBounce(double x) { return x < 0.5 ? (1 - outBounce(1 - 2 * x)) / 2 : (1 + outBounce(2 * x - 1)) / 2; }
	}
}
