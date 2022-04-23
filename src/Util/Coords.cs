namespace GameLib.Util {
	/// <summary>座標データクラス</summary>
	public class Coords {
		/// <summary>x座標プロパティ</summary>
		public int x {get; set;}
		/// <summary>y座標プロパティ</summary>
		public int y {get; set;}
		/// <summary>z座標プロパティ</summary>
		public int z {get; set;}

		/// <summary>コンストラクタ</summary>
		/// <param name="x">x座標</param>
		/// <param name="y">y座標</param>
		public Coords(int x, int y) {
			this.x = x;
			this.y = y;
		}
		/// <summary>コンストラクタ</summary>
		/// <param name="x">x座標</param>
		/// <param name="y">y座標</param>
		/// <param name="z">z座標</param>
		public Coords(int x, int y, int z) {
			this.x = x;
			this.y = y;
			this.z = z;
		}

		/// <summary>文字列表現を返します。object 型 からのオーバーライド</summary>
		public override string ToString() { return "{ x= "+this.x+", y= "+this.y+", z= "+this.z+" }"; }
	}
}
