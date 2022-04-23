using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

using GameLib.GameInformations;
using GameLib.TopView.Informations;
using GameLib.TopView.EventArgs;

namespace GameLib.TopView.Base {
	/// <summary>2D見下ろしマップにおけるプレイヤーを表すクラス</summary>
	abstract public class Player: Character {
		/// <summary>プレイヤーであることを示すデフォルトで付けられるタグの内容</summary>
		public static string PLAYER_TAG = "PLAYER";
		internal int  walkCnt = 0;
		internal bool isRightMoving = false;
		internal bool isLeftMoving  = false;
		internal bool isUpMoving    = false;
		internal bool isDownMoving  = false;

		/// <summary>walkメソッド呼び出し時の画面のスクロールサイズ</summary>
		public float scroll {get; private set;}

		/// <summary>コンストラクタ</summary>
		/// <param name="faceImage">このプレイヤーの顔画像(メッセージウィンドウ等に使用)</param>
		/// <param name="charachip">このプレイヤーのキャラクターチップ画像。画像のフォーマットは、Wolf Editorなどの形式</param>
		/// <param name="x">このプレイヤーのx座標</param>
		/// <param name="y">このプレイヤーのy座標</param>
		/// <param name="z">このプレイヤーのz座標</param>
		/// <param name="imType">このプレイヤーのキャラクターチップ画像の形式。</param>
		public Player( Bitmap faceImage, Bitmap charachip, int x, int y, int z, Character.ImageType imType): base(faceImage,charachip, x,y,z, imType, new string[] {PLAYER_TAG} ) {}
		/// <summary>コンストラクタ</summary>
		/// <param name="faceImage">このプレイヤーの顔画像(メッセージウィンドウ等に使用)</param>
		/// <param name="charachip">このプレイヤーのキャラクターチップ画像。画像のフォーマットは、Wolf Editorなどの形式</param>
		/// <param name="x">このプレイヤーのx座標</param>
		/// <param name="y">このプレイヤーのy座標</param>
		/// <param name="z">このプレイヤーのz座標</param>
		/// <param name="imType">このプレイヤーのキャラクターチップ画像の形式。</param>
		/// <param name="tags">このプレイヤーの特徴を表すタグ</param>
		public Player(Bitmap faceImage, Bitmap charachip, int x, int y, int z, Character.ImageType imType, string[] tags): base(faceImage,charachip, x,y,z, imType, tags )     { addTag(PLAYER_TAG); }
		/// <summary>コンストラクタ</summary>
		/// <param name="faceImage">このプレイヤーの顔画像(メッセージウィンドウ等に使用)</param>
		/// <param name="charachip">このプレイヤーのキャラクターチップ画像。画像のフォーマットは、Wolf Editorなどの形式</param>
		/// <param name="x">このプレイヤーのx座標</param>
		/// <param name="y">このプレイヤーのy座標</param>
		/// <param name="z">このプレイヤーのz座標</param>
		/// <param name="imType">このプレイヤーのキャラクターチップ画像の形式。</param>
		/// <param name="tags">このプレイヤーの特徴を表すタグ</param>
		public Player(Bitmap faceImage, Bitmap charachip, int x, int y, int z, Character.ImageType imType, List<string> tags): base(faceImage,charachip, x,y,z, imType, tags ) { addTag(PLAYER_TAG); }
		/// <summary>コンストラクタ</summary>
		/// <param name="charachip">このプレイヤーのキャラクターチップ画像。画像のフォーマットは、Wolf Editorなどの形式</param>
		/// <param name="x">このプレイヤーのx座標</param>
		/// <param name="y">このプレイヤーのy座標</param>
		/// <param name="z">このプレイヤーのz座標</param>
		/// <param name="imType">このプレイヤーのキャラクターチップ画像の形式。</param>
		public Player(Bitmap charachip, int x, int y, int z, Character.ImageType imType): base(charachip, x,y,z, imType, new string[] {PLAYER_TAG} ) {}
		/// <summary>コンストラクタ</summary>
		/// <param name="charachip">このプレイヤーのキャラクターチップ画像。画像のフォーマットは、Wolf Editorなどの形式</param>
		/// <param name="x">このプレイヤーのx座標</param>
		/// <param name="y">このプレイヤーのy座標</param>
		/// <param name="z">このプレイヤーのz座標</param>
		/// <param name="imType">このプレイヤーのキャラクターチップ画像の形式。</param>
		/// <param name="tags">このプレイヤーの特徴を表すタグ</param>
		public Player(Bitmap charachip, int x, int y, int z, Character.ImageType imType, string[] tags): base(charachip, x,y,z, imType, tags )     { addTag(PLAYER_TAG); }
		/// <summary>コンストラクタ</summary>
		/// <param name="charachip">このプレイヤーのキャラクターチップ画像。画像のフォーマットは、Wolf Editorなどの形式</param>
		/// <param name="x">このプレイヤーのx座標</param>
		/// <param name="y">このプレイヤーのy座標</param>
		/// <param name="z">このプレイヤーのz座標</param>
		/// <param name="imType">このプレイヤーのキャラクターチップ画像の形式。</param>
		/// <param name="tags">このプレイヤーの特徴を表すタグ</param>
		public Player(Bitmap charachip, int x, int y, int z, Character.ImageType imType, List<string> tags): base(charachip, x,y,z, imType, tags ) { addTag(PLAYER_TAG); }

		/// <summary>毎フレーム呼ばれる関数。オーバーライド可能。Charactorクラスからの継承。</summary>
		/// <param name="input">入力情報を保持したInputオブジェクト</param>
		/// <param name="tileSize">１マスの大きさ</param>
		/// <returns>void型</returns>
		public override void update(Input input, int tileSize) {
			walkCnt = (walkCnt+1)%speed;
			if ( (isRightMoving || isLeftMoving || isUpMoving || isDownMoving) && walkCnt > speed-1) { walkCnt = 0; }
			if( walkCnt == speed-1 ) {
				if (isRightMoving) {
					isRightMoving = false;
					move(1, 0, 0, Character.Directions.RIGHT);
				}
				if (isLeftMoving) {
					isLeftMoving  = false;
					move(-1, 0, 0, Character.Directions.LEFT);
				}
				if (isUpMoving) {
					isUpMoving    = false;
					move(0, -1, 0, Character.Directions.BACK);
				}
				if (isDownMoving) {
					isDownMoving  = false;
					move(0, 1, 0, Character.Directions.FRONT);
				}
			}

			scroll = (float)tileSize/speed;
			if (isRightMoving) { drawX += (scroll * walkCnt); }
			if (isLeftMoving)  { drawX -= (scroll * walkCnt); }
			if (isDownMoving)  { drawY += (scroll * walkCnt); }
			if (isUpMoving)    { drawY -= (scroll * walkCnt); }
		}

		/// <summary>あるく関数。オーバーライド可能。スクロールしながら移動します。Charactorクラスからの継承。</summary>
		/// <param name="direction">移動する方向</param>
		/// <returns>bool型。移動に成功した場合、True。</returns>
		public override bool walk(Character.Directions direction) {
			if ( isRightMoving || isLeftMoving || isUpMoving || isDownMoving) { return false; }
			turnTo(direction);

			int dx = 0; int dy = 0;
			if      ( direction == Character.Directions.RIGHT ) { dx++; }
			else if ( direction == Character.Directions.LEFT  ) { dx--; }
			else if ( direction == Character.Directions.BACK  ) { dy--; }
			else if ( direction == Character.Directions.FRONT ) { dy++; }
			else if ( direction == Character.Directions.BRIGHT) { dx++;dy--; }
			else if ( direction == Character.Directions.FRIGHT) { dx++;dy++; }
			else if ( direction == Character.Directions.BLEFT ) { dx--;dy--; }
			else if ( direction == Character.Directions.FLEFT ) { dx--;dy++; }

			if      (dx == 1) isRightMoving = canEnter(this.x+dx, this.y+dy, this.z, direction);
			else if (dx == -1) isLeftMoving = canEnter(this.x+dx, this.y+dy, this.z, direction);
			if      (dy == 1) isDownMoving = canEnter(this.x+dx, this.y+dy, this.z, direction);
			else if (dy == -1) isUpMoving  = canEnter(this.x+dx, this.y+dy, this.z, direction);

			walkCnt = 0;
			return canEnter(this.x+dx, this.y+dy, this.z, direction);
		}
	}
}
