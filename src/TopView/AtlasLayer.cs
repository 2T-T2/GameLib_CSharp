using System;
using System.Drawing;

using GameLib.TopView.Informations;

namespace GameLib.TopView {
	/// <summary>Atlasオブジェクトで扱うレイヤークラス。使用する画像の、RGB(0,0,0) の黒の部分は、透過されて表示されます。</summary>
	public class AtlasLayer {
		/// <summary>全方向から進入可能なマスであることを表すシンボル文字</summary>
		public static readonly string ENTERABLE    = "o";
		/// <summary>全方向から進入不可能なマスであることを表すシンボル文字</summary>
		public static readonly string NONENTERABLE = "x";
		/// <summary>左右から進入可能なマスであることを表すシンボル文字</summary>
		public static readonly string HCROSS       = "-";    // 4:6 6:4 と同じ意味。
		/// <summary>上下から進入可能なマスであることを表すシンボル文字</summary>
		public static readonly string VCROSS       = "|";    // 2:8 8:2 と同じ意味。

		/// <summary>左下から進入可能なマスであることを表すシンボル文字</summary>
		public static readonly string FROM_1_ABLE  = "1";
		/// <summary>真下から進入可能なマスであることを表すシンボル文字</summary>
		public static readonly string FROM_2_ABLE  = "2";
		/// <summary>右下から進入可能なマスであることを表すシンボル文字</summary>
		public static readonly string FROM_3_ABLE  = "3";
		/// <summary>左から進入可能なマスであることを表すシンボル文字</summary>
		public static readonly string FROM_4_ABLE  = "4";
		/// <summary>右から進入可能なマスであることを表すシンボル文字</summary>
		public static readonly string FROM_6_ABLE  = "6";
		/// <summary>左上から進入可能なマスであることを表すシンボル文字</summary>
		public static readonly string FROM_7_ABLE  = "7";
		/// <summary>上から進入可能なマスであることを表すシンボル文字</summary>
		public static readonly string FROM_8_ABLE  = "8";
		/// <summary>右上から進入可能なマスであることを表すシンボル文字</summary>
		public static readonly string FROM_9_ABLE  = "9";

		/// <summary>進入可能かどうかを表す</summary>
		protected string[][] road;

		private readonly int[][] mapChipIdxs;
		private readonly int[][] tileNums;

		/// <summary>コンストラクタ</summary>
		/// <param name="road">どこのマスから進入可能であるかを示した配列。マップのマス＋上下左右1マス分多く指定します。</param>
		/// <param name="mapChipIdxs">各マスで描画するタイルを MapChip.init() で指定したどのマップチップ画像から使用するかを、MapChip.init() の引数で指定した Bitmap 配列のインデックスで指定します</param>
		/// <param name="tileNums">各マスで描画するタイルを第二引数で指定したマップチップ画像のどこにあるかを指定します。左上から右下に数えて何番目かを指定します。</param>
		public AtlasLayer(string[][] road, int[][] mapChipIdxs, int[][] tileNums) {
			this.road = road;
			this.tileNums = tileNums;
			this.mapChipIdxs = mapChipIdxs;
		}

		/// <summary>描画関数。オーバーライド可能。</summary>
		/// <param name="g">描画に使用するグラフィックスオブジェクト</param>
		/// <param name="gameWidth">ゲームの横幅</param>
		/// <param name="gameHeight">ゲームの縦幅</param>
		/// <param name="viewTileNumWidth">ゲーム画面内に表示されるタイルの横の数</param>
		/// <param name="viewTileNumHeight">ゲーム画面内に表示されるタイルの縦の数</param>
		/// <param name="playerX">プレイヤーの現在位置のX座標</param>
		/// <param name="playerY">プレイヤーの現在位置のY座標</param>
		/// <param name="tileSize">１マスの大きさ</param>;
		/// <returns>void型。</returns>
		public virtual void draw(Graphics g, int gameWidth, int gameHeight, int viewTileNumWidth, int viewTileNumHeight, int playerX, int playerY, int tileSize) {
			for (int chipX = playerX - viewTileNumWidth / 2; chipX < playerX + viewTileNumWidth / 2 + (viewTileNumWidth % 2) + 2; chipX++) {
				for (int chipY = playerY - viewTileNumHeight / 2; chipY < playerY + viewTileNumHeight / 2 + (viewTileNumHeight % 2) + 2; chipY++) {
					if (chipX < 0 || chipY < 0 || chipX >= this.tileNums[0].Length || chipY >= this.tileNums.Length) continue;
					if (mapChipIdxs[chipY][chipX] >= 0 && this.tileNums[chipY][chipX] >= 0) MapChip.drawChip(g, mapChipIdxs[chipY][chipX], this.tileNums[chipY][chipX], chipX + (viewTileNumWidth / 2) - playerX, chipY + (viewTileNumHeight / 2) - playerY, gameWidth, gameHeight, tileSize);
				}
			}
		}
		/// <summary>各マスの進入状態を表した二次元ジャグ配列を返します</summary>
		/// <returns>string[][]型。各マスの進入状態を表した二次元ジャグ配列</returns>
		public string[][] getRoad() { return road; }
	}
}
