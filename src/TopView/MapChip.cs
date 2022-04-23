using System.Drawing;

namespace GameLib.TopView {
	/// <summary>マップチップを管理、描画するクラス</summary>
	public static class MapChip {
		private static Bitmap[] imgs;
		private static int[] widths;
		private static int[] heights;
		private static int[] tileNumHorizontals;

		/// <summary>マップチップを初期化します</summary>
		/// <param name="imgs">マップチップ画像を指定します</param>
		/// <param name="tileNumHorizontals">指定したマップチップ画像でいくつタイルが横に並んでいるかを指定します</param>
		/// <param name="tileNumVerticals">指定したマップチップ画像でいくつタイルが縦に並んでいるかを指定します</param>
		/// <returns>void型</returns>
		public static void init(Bitmap[] imgs, int[] tileNumHorizontals, int[] tileNumVerticals) {
			if(!(imgs.Length == tileNumVerticals.Length && imgs.Length == tileNumHorizontals.Length)) throw new System.ArgumentException("それぞれのパラメータの配列の長さは同じである必要があります");
			MapChip.imgs = imgs;
			MapChip.tileNumHorizontals = tileNumHorizontals;

			MapChip.widths = new int[imgs.Length];
			MapChip.heights = new int[imgs.Length];
			for (int i = 0; i < imgs.Length; i++) {
				MapChip.widths[i] = imgs[i].Width / tileNumHorizontals[i];
				MapChip.heights[i] = imgs[i].Height / tileNumVerticals[i];
			}
		}

		/// <summary>指定したマップチップ画像から指定したタイルを描画します</summary>
		/// <param name="g">描画に使用するグラフィクスオブジェクト</param>
		/// <param name="idx">描画するマップチップ画像を initで指定した マップチップ画像の Bitmap 配列 のインデックスで指定します。</param>
		/// <param name="chipX">描画するタイルがマップチップ画像の左からいくつめに、あるかを指定します</param>
		/// <param name="chipY">描画するタイルがマップチップ画像の上からいくつめに、あるかを指定します</param>
		/// <param name="dstX">左から何マス目に描画するかを指定します</param>
		/// <param name="dstY">上から何マス目に描画するかを指定します</param>
		/// <param name="gameWidth">ゲームの横幅を指定します。Scene.getGameSize().Width などで取得できます</param>
		/// <param name="gameHeight">ゲームの縦幅を指定します。Scene.getGameSize().Height などで取得できます</param>
		/// <param name="tileSize">描画するタイルの大きさをしていします。</param>
		/// <returns>void型</returns>
		public static void drawChip(Graphics g, int idx, int chipX, int chipY, int dstX, int dstY, int gameWidth, int gameHeight, int tileSize) {
			float padX = (gameWidth % tileSize) * 0.5f;
			float padY = (gameHeight % tileSize) * 0.5f;
			g.DrawImage( MapChip.imgs[idx], new RectangleF( dstX * tileSize - padX, dstY * tileSize - padY, tileSize, tileSize ), new RectangleF( chipX * MapChip.widths[idx], chipY * MapChip.heights[idx], MapChip.widths[idx]-1, MapChip.heights[idx]-1), GraphicsUnit.Pixel );
		}

		/// <summary>指定したマップチップ画像から指定したタイルを描画します</summary>
		/// <param name="g">描画に使用するグラフィクスオブジェクト</param>
		/// <param name="idx">描画するマップチップ画像を initで指定した マップチップ画像の Bitmap 配列 のインデックスで指定します。</param>
		/// <param name="chipNum">描画するタイルがマップチップ画像の左上から右下に数えた時いくつめに、あるかを指定します</param>
		/// <param name="dstX">左から何マス目に描画するかを指定します</param>
		/// <param name="dstY">上から何マス目に描画するかを指定します</param>
		/// <param name="gameWidth">ゲームの横幅を指定します。Scene.getGameSize().Width などで取得できます</param>
		/// <param name="gameHeight">ゲームの縦幅を指定します。Scene.getGameSize().Height などで取得できます</param>
		/// <param name="tileSize">描画するタイルの大きさをしていします。</param>
		/// <returns>void型</returns>
		public static void drawChip(Graphics g, int idx, int chipNum, int dstX, int dstY, int gameWidth, int gameHeight, int tileSize) { MapChip.drawChip(g, idx, chipNum%MapChip.tileNumHorizontals[idx], chipNum/MapChip.tileNumHorizontals[idx], dstX, dstY, gameWidth, gameHeight, tileSize); }
	}
}
