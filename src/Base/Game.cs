using System;
using System.Drawing;
using System.Windows.Forms;

using GameLib.GameEventsArgs;
using GameLib.GameInformations;

using System.Reflection;
[assembly: AssemblyVersion( "1.0.0.0" )]
[assembly: AssemblyFileVersion( "1.0.0.0" )]
[assembly: AssemblyInformationalVersion( "0.0" )]
[assembly: AssemblyTitle( "GameLib" )]
[assembly: AssemblyDescription( "Windows Form 向けゲームライブラリ" )]
[assembly: AssemblyProduct( "GameLib" )]
[assembly: AssemblyCulture( "" )]


namespace GameLib.Base {
	/// <summary>ゲームサイズを取得する関数Delegate</summary>
	public delegate Size GetGameSize();

	/// <summary>ゲーム抽象クラス</summary>
	/// <example>
	/// <code>
	/// public class MyGame: Game {
	/// 	[STAThread]
	/// 	public static void Main(string[] args) {
	/// 		Application.Run( new MyGame(new Size(480, 360), 30) );
	/// 	}
	/// 	MyGame(Size size, int fps): base(size, fps) {
	/// 		Scene[] s = new Scene[1];
	/// 		s[0] = Scene01("Scene01");    // Sceneクラスを継承したクラス
	/// 		setScenes(s);
	/// 	}
	/// }
	/// </code>
	/// </example>
	public abstract class Game :Form {
		Point? clickPoint = null;
		int order = 0;
		Scene[] scenes;
		string[] sceneNames;

		readonly float aspect;
		Bitmap gameScreen;
		Rectangle drawArea;

		/// <summary>コンストラクタ</summary>
		/// <param name="size">ゲームのサイズ</param>
		/// <param name="fps">ゲームのfps</param>
		public Game (Size size, int fps) {
			drawArea = new Rectangle(this.Location, size);
			GameInfo.fps = fps;
			aspect = (float)size.Width/size.Height;

			this.DoubleBuffered = true;
			this.ClientSize = size;
			gameScreen = new Bitmap(size.Width, size.Height);

			var t = new Timer(){
				Interval = 1000/GameInfo.fps,
				Enabled = true,
			};
			t.Tick += new EventHandler(tick);
			this.KeyDown += keyDown;
			this.KeyUp += keyUp;
			this.MouseClick += click;

			// GameLib.TopView.Informations.TopViewInfo.setTileSize(24, size);
		}

		/// <summary>コンストラクタ</summary>
		/// <param name="size">ゲームのサイズ</param>
		/// <param name="fps">ゲームのfps</param>
		/// <param name="ico">ウィンドウのIcon</param>
		public Game (Size size, int fps, Icon ico) {
			drawArea = new Rectangle(this.Location, size);
			GameInfo.fps = fps;
			aspect = size.Width/size.Height;

			this.Icon = ico;
			this.DoubleBuffered = true;
			this.ClientSize = size;
			gameScreen = new Bitmap(size.Width, size.Height);

			var t = new Timer(){
				Interval = 1000/GameInfo.fps,
				Enabled = true,
			};
			t.Tick += new EventHandler(tick);
			this.KeyDown += keyDown;
			this.KeyUp += keyUp;
			this.MouseClick += click;

			// GameLib.TopView.Informations.TopViewInfo.setTileSize(24, size);
		}

		void tick(object sender, EventArgs e) {
			this.Invalidate();
		}
		/// <summary>Formから継承した関数</summary>
		/// <param name="e">PaintArgs</param>
		/// <returns>void型</returns>
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			drawArea.Height = (int)(System.Math.Min(ClientSize.Width/aspect, ClientSize.Height));
			drawArea.Width  = (int)(drawArea.Height*aspect);
			drawArea.Y = (ClientSize.Height-drawArea.Height)/2;
			drawArea.X = (ClientSize.Width-drawArea.Width)/2;

			Input input = GameInfo.getInput();
			update(input);

			var windowGraphics = e.Graphics;
			using(var g = Graphics.FromImage(gameScreen)) {
				Point mousePos = this.PointToClient(Cursor.Position);
				mousePos = clientToImage(mousePos);

				scenes[order].update(g, input, mousePos, clickPoint);
				clickPoint = null;
			}

			windowGraphics.FillRectangle(Brushes.Black, 0,0, ClientSize.Width,ClientSize.Height);
			using(var bitmap = gameScreen.Clone( new RectangleF(0,0, gameScreen.Width,gameScreen.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb )) {
				NativeMethods.drawBmp(windowGraphics, bitmap, drawArea.X,drawArea.Y,drawArea.Width,drawArea.Height, 0,0, gameScreen.Width,gameScreen.Height, 0x00CC0020);
			}
		}
		/// <summary>Formから継承した関数</summary>
		/// <param name="e">PaintArgs</param>
		/// <returns>void型</returns>
		protected override void OnPaintBackground(PaintEventArgs e) {}
		/// <summary>毎フレーム呼び出される関数</summary>
		/// <param name="input">入力情報を保持しているInputオブジェクト</param>
		/// <returns>void型</returns>
		protected virtual  void update(Input input) {}
		/// <summary>シーンを登録する関数</summary>
		/// <param name="s">このゲームで使用するSceneオブジェクトの配列</param>
		/// <returns>void型</returns>
		public void setScenes(Scene[] s) {
			this.scenes = new Scene[s.Length];
			sceneNames = new string[s.Length];
			for (int i = 0; i < s.Length; i++){
				this.scenes[i] = s[i];
//				this.scenes[i].addControlHandler += addControl;
				this.scenes[i].changeSceneHandler += changeScene;
				this.scenes[i].getGameSize = getGameSize;
				sceneNames[i] = s[i].name;
			}
			this.scenes[0].init(null);
		}

		/// <summary>最終的にウィンドウに描画されるゲームの領域を取得する関数</summary>
		/// <returns>RectangleF型。描画されるゲームの領域</returns>
		public RectangleF getDrawArea() { return drawArea; }
		/// <summary>ゲームのサイズを取得する関数。ウィンドウがリサイズされてもこの値は変化しない。</summary>
		/// <returns>Size型。ゲームのサイズ</returns>
		public Size getGameSize() { return gameScreen.Size; }

		private void changeScene(object sender, ChangeSceneEventArgs e) {
			if (e.eventName == "byOrder") {
				this.order = e.order;
			}else if (e.eventName == "byName") {
				this.order = getSceneOrderByName(e.sceneName);
			}
			Controls.Clear();
			scenes[order].init(e.deliveryObject);
		}
		private void addControl(AddControlEventArgs e) {
			this.Controls.Add(e.ctrl);
		}

		private void keyDown(object sender, KeyEventArgs e) {
			GameInfo.keyHash[e.KeyCode] = true;
		}
		private void keyUp(object sender, KeyEventArgs e) {
			GameInfo.keyHash[e.KeyCode] = false;
		}
		private void click(object sender, MouseEventArgs e) {
			clickPoint = clientToImage(e.Location);
		}
		private Point clientToImage(Point p) {
			p.X += -(int)drawArea.Left;
			p.Y += -(int)drawArea.Top;
			float scale = gameScreen.Height / drawArea.Height;
			p.X = (int)(p.X * scale);
			p.Y = (int)(p.Y * scale);
			return p;
		}
		private int getSceneOrderByName(string name) {
			return Array.IndexOf(sceneNames, name);
		}
	}
}
