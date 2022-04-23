using System.Drawing;
using System.Collections.Generic;

using GameLib.Base;
using GameLib.Util;
using GameLib.GameInformations;
using GameLib.TopView.Informations;
using GameLib.TopView.EventArgs;

namespace GameLib.TopView.Base
{
	/// <summary>Charactorクラス</summary>
	public partial class Character : System.IDisposable {
		/// <summary>Charactorの方向を表すEnum</summary>
		public enum Directions {
			/// <summary>顔がこちらを向いている</summary>
			FRONT  = 0,
			/// <summary>顔が左を向いている</summary>
			LEFT   = 1,
			/// <summary>顔が右を向いている</summary>
			RIGHT  = 2,
			/// <summary>顔が向こう側を向いている</summary>
			BACK   = 3,
			/// <summary>顔がコチラ向き左を向いている</summary>
			FLEFT  = 4,
			/// <summary>顔がコチラ向き右を向いている</summary>
			FRIGHT = 5,
			/// <summary>顔が向こう向き左を向いている</summary>
			BLEFT  = 6,
			/// <summary>顔が向こう向き右を向いている</summary>
			BRIGHT = 7,
		}
		/// <summary>GameLib.TopOfView.Charactor.Directions Enumを便利に使う為のクラス</summary>
		public static class DirectionsEx {
			/// <summary>指定された向きから、時計回りに45°回転した場合の方向を取得します</summary>
			/// <param name="direction">回転の基準になる方向</param>
			/// <returns>GameLib.TopOfView.Charactor.Directions型。回転後の方向</returns>
			public static Character.Directions rotate45(Character.Directions direction) {
				if (direction == Character.Directions.FRONT) { return Character.Directions.FLEFT; }
				if (direction == Character.Directions.FLEFT) { return Character.Directions.LEFT;  }
				if (direction == Character.Directions.LEFT)  { return Character.Directions.BLEFT; }
				if (direction == Character.Directions.BLEFT) { return Character.Directions.BACK;  }
				if (direction == Character.Directions.BACK)  { return Character.Directions.BRIGHT;}
				if (direction == Character.Directions.BRIGHT){ return Character.Directions.RIGHT; }
				if (direction == Character.Directions.RIGHT) { return Character.Directions.FRIGHT;}
				if (direction == Character.Directions.FRIGHT){ return Character.Directions.FRONT; }
				return Character.Directions.FRONT;
			}
			/// <summary>指定された向きから、時計回りに90°回転した場合の方向を取得します</summary>
			/// <param name="direction">回転の基準になる方向</param>
			/// <returns>GameLib.TopOfView.Charactor.Directions型。回転後の方向</returns>
			public static Character.Directions rotate90(Character.Directions direction) {
				if (direction == Character.Directions.FRONT) { return Character.Directions.LEFT;  }
				if (direction == Character.Directions.FLEFT) { return Character.Directions.BLEFT; }
				if (direction == Character.Directions.LEFT)  { return Character.Directions.BACK;  }
				if (direction == Character.Directions.BLEFT) { return Character.Directions.BRIGHT;}
				if (direction == Character.Directions.BACK)  { return Character.Directions.RIGHT; }
				if (direction == Character.Directions.BRIGHT){ return Character.Directions.FRIGHT;}
				if (direction == Character.Directions.RIGHT) { return Character.Directions.FRONT; }
				if (direction == Character.Directions.FRIGHT){ return Character.Directions.FLEFT; }
				return Character.Directions.FRONT;
			}
			/// <summary>指定された向きから、180°回転した場合の方向を取得します</summary>
			/// <param name="direction">回転の基準になる方向</param>
			/// <returns>GameLib.TopOfView.Charactor.Directions型。回転後の方向</returns>
			public static Character.Directions rotate180(Character.Directions direction) {
				if (direction == Character.Directions.FRONT) { return Character.Directions.BACK;  }
				if (direction == Character.Directions.FLEFT) { return Character.Directions.BRIGHT;}
				if (direction == Character.Directions.LEFT)  { return Character.Directions.RIGHT; }
				if (direction == Character.Directions.BLEFT) { return Character.Directions.FRIGHT;}
				if (direction == Character.Directions.BACK)  { return Character.Directions.FRONT; }
				if (direction == Character.Directions.BRIGHT){ return Character.Directions.FLEFT; }
				if (direction == Character.Directions.RIGHT) { return Character.Directions.LEFT;  }
				if (direction == Character.Directions.FRIGHT){ return Character.Directions.BLEFT; }
				return Character.Directions.FRONT;
			}
		}
		/// <summary>キャラクターチップ画像の種類を示す</summary>
		public enum ImageType {
			/// <summary>4方向分</summary>
			FOUR_DIRECTION = 0,
			/// <summary>8方向分</summary>
			EIGHT_DIRECTION = 4,
		}

		private bool disposedValue = false;

		/// <summary>リソースを開放します</summary>
		/// <param name="disposing">デストラクタから呼ばれたらfalse</param>
		protected virtual void Dispose( bool disposing ) {
			if (!disposedValue) {
				if (disposing) {
					charachip.Dispose();
					faceImage.Dispose();
				}
				disposedValue = true;
			}
		}

		/// <summary>リソースを開放します</summary>
		public void Dispose() {
			Dispose( true );
		}
	}
	abstract public partial class Character: System.IDisposable {
		private List<string> tags;
		private int animateIdx = 0;
		private int w, h;
		internal event CharacterEventHander characterEventHander;
		internal byte speed = 3;
		/// <summary>指定された座標のマスに進入可能かを判定する関数Delegate型</summary>
		protected internal delegate bool canEnterHandler(int x, int y, int z, Character.Directions direction);
		/// <summary>指定された座標のマスに進入可能かを判定する関数オブジェクト。 bool canEnter(int x, int y, int z, Charactor.Directions direction)</summary>
		protected internal canEnterHandler canEnter;
		/// <summary>このキャラクターのキャラチップ画像
		///　　上記の順に各方向に向いた画像を用意する
		///　　パターン１　　　パターン２
		///　　　↓↓↓　　　　　　↓↓↓↙↙↙
		///　　　←←←　　　　　　←←←↘↘↘
		///　　　→→→　　　　　　→→→↖↖↖
		///　　　↑↑↑　　　　　　→→→↗↗↗
		/// </summary>
		protected Bitmap charachip;
		/// <summary>このキャラクターの顔画像</summary>
		protected Bitmap faceImage;
		/// <summary>x座標</summary>
		protected int x;
		/// <summary>y座標</summary>
		protected int y;
		/// <summary>z座標</summary>
		protected int z;
		/// <summary>このキャラクタの方向</summary>
		protected Character.Directions direction = Character.Directions.FRONT;
		/// <summary>このオブジェクトのキャラチップの画像のタイプ</summary>
		protected Character.ImageType imType;
		/// <summary>描画するX座標</summary>
		protected float drawX;
		/// <summary>描画するY座標</summary>
		protected float drawY;
		/// <summary>このオブジェクトのあたり判定</summary>
		public bool hasCollision {protected set; get;}
		/// <summary>このキャラクターのZ座標に関係なく常に最前面に表示します</summary>
		public bool alwaysVisible = false;
		/// <summary>ゲームサイズを取得する関数。戻り値Size</summary>
		public GetGameSize getGameSize;

		/// <summary>コンストラクタ</summary>
		/// <param name="faceImage">顔画像</param>
		/// <param name="charachip">キャラチップ画像</param>
		/// <param name="x">x座標</param>
		/// <param name="y">y座標</param>
		/// <param name="z">z座標</param>
		/// <param name="imType">このオブジェクトのキャラチップの画像のタイプ</param>
		/// <param name="tags">このCharactorの特徴を示すタグ</param>
		public Character(Bitmap faceImage, Bitmap charachip, int x, int y, int z, Character.ImageType imType, List<string> tags) {
			this.faceImage = faceImage;
			this.charachip = charachip;
			this.imType = imType;
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = imType == Character.ImageType.FOUR_DIRECTION ? charachip.Width/3 : charachip.Width/6;
			this.h = charachip.Height/4;
			this.tags = tags;
			this.hasCollision = true;
		}
		/// <summary>コンストラクタ</summary>
		/// <param name="faceImage">顔画像</param>
		/// <param name="charachip">キャラチップ画像</param>
		/// <param name="x">x座標</param>
		/// <param name="y">y座標</param>
		/// <param name="z">z座標</param>
		/// <param name="imType">このオブジェクトのキャラチップの画像のタイプ</param>
		/// <param name="tags">このCharactorの特徴を示すタグ</param>
		public Character(Bitmap faceImage, Bitmap charachip, int x, int y, int z, Character.ImageType imType, string[] tags): this(faceImage, charachip, x,y,z, imType, new List<string>(tags)) {}
		/// <summary>コンストラクタ</summary>
		/// <param name="charachip">キャラチップ画像</param>
		/// <param name="x">x座標</param>
		/// <param name="y">y座標</param>
		/// <param name="z">z座標</param>
		/// <param name="imType">このオブジェクトのキャラチップの画像のタイプ</param>
		/// <param name="tags">このCharactorの特徴を示すタグ</param>
		public Character( Bitmap charachip, int x, int y, int z, Character.ImageType imType, List<string> tags): this(null, charachip, x,y,z, imType, tags) {}
		/// <summary>コンストラクタ</summary>
		/// <param name="charachip">キャラチップ画像</param>
		/// <param name="x">x座標</param>
		/// <param name="y">y座標</param>
		/// <param name="z">z座標</param>
		/// <param name="imType">このオブジェクトのキャラチップの画像のタイプ</param>
		/// <param name="tags">このCharactorの特徴を示すタグ</param>
		public Character( Bitmap charachip, int x, int y, int z, Character.ImageType imType, string[] tags): this(charachip, x,y,z, imType, new List<string>(tags)) {}
		/// <summary>コンストラクタ</summary>
		/// <param name="charachip">キャラチップ画像</param>
		/// <param name="x">x座標</param>
		/// <param name="y">y座標</param>
		/// <param name="z">z座標</param>
		/// <param name="imType">このオブジェクトのキャラチップの画像のタイプ</param>
		public Character( Bitmap charachip, int x, int y, int z, Character.ImageType imType): this(charachip, x,y,z, imType, new List<string>()) {}

		/// <summary>描画関数。 override可能</summary>
		/// <param name="g">描画に用いるGraphicsオブジェクト</param>
		/// <param name="tileSize">１マスの大きさ</param>;
		/// <returns>void型</returns>
		public virtual void draw(Graphics g, int tileSize) {
			RectangleF destRect = new RectangleF(drawX,drawY, tileSize,tileSize);
			RectangleF srcRect  = getCurrentFrameImageRect();
			g.DrawImage(charachip, destRect, srcRect, GraphicsUnit.Pixel);
			return;
		}

		/// <summary>毎フレーム呼び出される関数。 override可能</summary>
		/// <param name="input">入力情報を保持しているInputオブジェクト</param>
		/// <param name="tileSize">１マスの大きさ</param>;
		/// <returns>void型</returns>
		public virtual void update(Input input, int tileSize) {}
		internal void calcDrawXY(int viewTileNumWidth, int viewTileNumHeight, int playerX, int playerY, int tileSize) {
			Size gameSize = getGameSize();
			drawX = (viewTileNumWidth/2-playerX+this.x)*tileSize - (gameSize.Width % tileSize) * 0.5f;
			drawY = (viewTileNumHeight/2-playerY+this.y)*tileSize - (gameSize.Height % tileSize) * 0.5f;
		}
		/// <summary>話しかけられた時に呼び出される関数。 override可能</summary>
		/// <param name="talker">話しかけてきたCharactorオブジェクト(talk関数呼び出した関数)</param>
		/// <param name="message">talk関数で引数として渡した文字列</param>
		/// <returns>void型</returns>
		public virtual void talked(Character talker, string message) {}
		/// <summary>他のCharactorが起こしたイベントを受け取る関数。 override可能</summary>
		/// <param name="sender">イベントを起こしたCharactor</param>
		/// <param name="e">起こされたイベントのCharactorEventArgs</param>
		/// <returns>void型</returns>
		public virtual void charactorEventDispath(Character sender, CharacterEventArgs e) {
			if ( e.target.x == this.x && e.target.y == this.y && e.target.z == this.z && e.type == CharacterEventType.CEVENT_TALK.ToString() ) { talked(sender, e.message); }
		}

		/// <summary>座標を移動する関数。現在位置からの相対座標で指定する</summary>
		/// <param name="x">移動するx座標の量</param>
		/// <param name="y">移動するy座標の量</param>
		/// <param name="z">移動するz座標の量</param>
		/// <param name="direction">移動後に向く方向</param>
		public bool move(int x, int y, int z, Character.Directions direction) {
			this.direction = direction;
			if ( !canEnter(this.x + x, this.y + y, this.z, direction) ) { return false; }
			this.x += x;
			this.y += y;
			this.z += z;
			return true;
		}

		/// <summary>発言関数。自分の顔と指定した文字列を表示するMessageWindowを表示します</summary>
		/// <param name="message">表示する文字列</param>
		/// <returns>void型</returns>
		public void say (string message) { sendEvent(new CharacterEventArgs(new Coords(-1,-1,-1), CharacterEventType.CEVENT_SAY, message)); }
		/// <summary>話しかける関数。指定した座標のCharactorのtalked関数を呼び出します</summary>
		/// <param name="x">話しかけるCharactorのx座標の量</param>
		/// <param name="y">話しかけるCharactorのy座標の量</param>
		/// <param name="z">話しかけるCharactorのz座標の量</param>
		/// <param name="message">対象Charactorオブジェクトのtalked関数に送るメッセージ</param>
		/// <returns>void型</returns>
		public void talk(int x, int y, int z, string message) { sendEvent(new CharacterEventArgs(new Coords(x,y,z) , CharacterEventType.CEVENT_TALK, message)); }
		/// <summary>話しかける関数。同じz座標で向いている方向の隣にいるCharactorオブジェクトのtalked関数を呼び出す関数。</summary>
		/// <param name="message">対象Charactorオブジェクトのtalked関数に送るメッセージ</param>
		/// <returns>void型</returns>
		public void talk(string message) {
			if (direction == Character.Directions.LEFT)   { this.talk(this.x-1, this.y  , this.z, message); return; }
			if (direction == Character.Directions.RIGHT)  { this.talk(this.x+1, this.y  , this.z, message); return; }
			if (direction == Character.Directions.BACK)   { this.talk(this.x  , this.y-1, this.z, message); return; }
			if (direction == Character.Directions.FRONT)  { this.talk(this.x  , this.y+1, this.z, message); return; }
			if (direction == Character.Directions.BLEFT)  { this.talk(this.x-1, this.y-1, this.z, message); return; }
			if (direction == Character.Directions.FLEFT)  { this.talk(this.x-1, this.y+1, this.z, message); return; }
			if (direction == Character.Directions.BRIGHT) { this.talk(this.x+1, this.y-1, this.z, message); return; }
			if (direction == Character.Directions.FRIGHT) { this.talk(this.x+1, this.y+1, this.z, message); return; }
		}
		/// <summary>指定した方向にあるく関数。オーバーライド可能</summary>
		/// <param name="direction">あるく方向</param>
		/// <returns>bool型。指定した方向に歩けたら、Trueを返す。</returns>
		public virtual bool walk(Character.Directions direction) {
			// アニメーションしながら動く処理を書け！！
			return true;
		}
		/// <summary>キャラチップの画像を動かします</summary>
		/// <returns>void型</returns>
		public void animate() { animateIdx = ((++animateIdx)%3); }
		/// <summary>指定した方向に向けます</summary>
		/// <param name="direction">向く方向</param>
		/// <returns>指定したImageType(imType)の値では向くことが出来ない方向を指定した場合Falseが返ります</returns>
		public bool turnTo(Character.Directions direction) {
			if (imType == Character.ImageType.FOUR_DIRECTION && (int)direction > 3) { System.Console.WriteLine("その方向には向けません"); return false; }
			this.direction = direction;
			return true;
		}
		/// <summary>右方向に回転させます</summary>
		/// <returns>void型</returns>
		public void rotateRight() {
			if (imType == Character.ImageType.FOUR_DIRECTION) {
				direction = Character.DirectionsEx.rotate90(direction);
			}else if ( imType == Character.ImageType.EIGHT_DIRECTION ) {
				direction = Character.DirectionsEx.rotate45(direction);
			}
		}

		/// <summary>イベントを発生させます。AtlasのreceiveEvent関数が呼び出され、CharactorのDicpatchEvent関数が呼び出されます</summary>
		/// <param name="e">イベントに対して送るCharactorEventArgs</param>
		/// <returns>void型</returns>
		protected void sendEvent(CharacterEventArgs e) {
			if (characterEventHander == null) { System.Console.WriteLine("イベントハンドラが設定されていません。"); return; }
			characterEventHander(this, e);
		}

		/// <summary>このキャラクターの特徴であるタグを追加します</summary>
		/// <param name="tag">追加するタグ</param>
		/// <returns>void型</returns>
		public void addTag(string tag) { tags.Add(tag); }
		/// <summary>このキャラクターの特徴であるタグを追加します</summary>
		/// <param name="tags">追加するタグ</param>
		/// <returns>void型</returns>
		public void addTags(string[] tags) {
			foreach (var tag in tags)
				addTag(tag);
		}
		/// <summary>このキャラクターの特徴であるタグを追加します</summary>
		/// <param name="tags">追加するタグ</param>
		/// <returns>void型</returns>
		public void addTags(List<string> tags) {
			foreach (var tag in tags)
				addTag(tag);
		}

		/// <summary>このキャラクターの特徴であるタグを検索します</summary>
		/// <param name="tag">検索するタグ</param>
		/// <returns>bool型。指定したタグが含まれるかを返します。</returns>
		public bool hasTag(string tag) {
			foreach (var t in tags) {
				if (tag == t) return true;
			}
			return false;
		}

		/// <summary>現在キャラチップのうち描画される領域を返します</summary>
		/// <returns>RectangleF型。現在キャラチップのうち描画される領域</returns>
		protected RectangleF getCurrentFrameImageRect() {
			int x = (((int)direction >= 4 ? 3 : 0)+animateIdx)*w;
			int y = ((int)direction%4)*h;
			return new RectangleF(x,y, w,h);
		}
		/// <summary>x座標を返します</summary>
		/// <returns>int型。x座標</returns>
		public int getX() { return this.x; }
		/// <summary>y座標を返します</summary>
		/// <returns>int型。y座標</returns>
		public int getY() { return this.y; }
		/// <summary>z座標を返します</summary>
		/// <returns>int型。z座標</returns>
		public int getZ() { return this.z; }

		/// <summary>このオブジェクトの顔画像を返します</summary>
		/// <returns>Image型。このオブジェクトの顔画像</returns>
		public Image getFaceImage() { return this.faceImage; }
		/// <summary>このオブジェクトの向いている方向を返します</summary>
		/// <returns>Charactor.Direction型。このオブジェクトの向いている方向</returns>
		public Character.Directions getDirection() { return this.direction; }

		/// <summary>指定した座標に移動させます</summary>
		/// <param name="x">x座標</param>
		/// <param name="y">y座標</param>
		/// <param name="z">z座標</param>
		/// <returns>void型</returns>
		public void setPosition(int x, int y, int z) {
			this.x = x;
			this.y = y;
			this.z = z;
		}
		/// <summary>指定したZ座標に移動させます</summary>
		/// <param name="z">z座標</param>
		/// <returns>void型</returns>
		public void setZ(int z) {
			this.z = z;
		}
	}
}
