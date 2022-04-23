using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

using System;

using GameLib.Base;
using GameLib.GameInformations;
using GameLib.TopView.Informations;
using GameLib.TopView.EventArgs;

namespace GameLib.TopView.Base {
	/// <summary>2D見下ろしマップを描画するScene派生クラス</summary>
	abstract public class Atlas: Scene, IDisposable {
		/// <summary>マップが表示されていない部分に見える背景色</summary>
		protected Brush bgBrush { get { return getBgBrush(); } set { setBgBrush( value ); } }//= new SolidBrush(Color.Blue);
		private Brush _bgBrush;
		/// <summary>レイヤーがすべて描画された結果の画像</summary>
		protected Bitmap viewImage;
		/// <summary>この2D見下ろしマップに描画するCharactorのList配列</summary>
		protected List<Character> characters = new List<Character>();
		/// <summary>このマップのレイヤー</summary>
		protected AtlasLayer[] layers;

		/// <summary>１マスの大きさプロパティ</summary>
		protected int tileSize {get { return this._tileSize; } set { setTileSize(value); } }
		/// <summary>表示される横のマスの個数プロパティ</summary>
		protected int viewTileNumWidth { get; private set; }
		/// <summary>表示される縦のマスの個数プロパティ</summary>
		protected int viewTileNumHeight { get; private set; }
		/// <summary>プレイヤーオブジェクトプロパティ</summary>
		protected Player player {get{ return getPlayer();} set {setPlayer(value);} }

		private List<MessageWindow> msgs = new List<MessageWindow>();
		private Player _player;
		private int _tileSize;

		/// <summary>コンストラクタ</summary>
		/// <param name="name">このシーンの名前</param>
		/// <param name="layers">このAtlasオブジェクトの持つレイヤーオブジェクト</param>
		/// <param name="player">プレイヤーオブジェクト</param>
		public Atlas(string name, AtlasLayer[] layers, Player player): base(name) {
			this.layers = layers;
			setBgBrush( new SolidBrush(Color.Blue) );
			setPlayer(player);
		}

		/// <summary>コンストラクタ</summary>
		/// <param name="name">このシーンの名前</param>
		public Atlas(string name): base(name) {
			setBgBrush( new SolidBrush( Color.Blue ) );
		}

		/// <summary>シーンが切り替わった最初のフレームに行われる処理</summary>
		public override void init() {
			tileSize = 24;
		}

		/// <summary>描画関数。Sceneクラスからの継承関数。</summary>
		/// <param name="g">描画に使用するGraphicsオブジェクト</param>
		/// <returns>void型</returns>
		public override void bgDraw(Graphics g) {
			Size gameSize = getGameSize();

			if( viewImage != null ) viewImage.Dispose();
			viewImage = new Bitmap(gameSize.Width, gameSize.Height);
			int charactersIdx = 0;
			using( var gg = Graphics.FromImage(viewImage)) {
				gg.FillRectangle(_bgBrush, 0,0 ,gameSize.Width,gameSize.Height);
				for(int z = 0; z < layers.Length; z++) {
					layers[z].draw(gg, gameSize.Width,gameSize.Height, viewTileNumWidth, viewTileNumHeight, player.getX(),player.getY(), tileSize);
					if(z == player.getZ()) player.draw(gg, tileSize);
					while(characters.Count != charactersIdx && characters[charactersIdx].getZ() == z) {
						characters[charactersIdx].draw(gg, tileSize);
						charactersIdx++;
					}
				}
				if(player.alwaysVisible) player.draw(gg, tileSize);
				foreach (var character in characters) {
					if (character.alwaysVisible) character.draw(gg, tileSize);
				}
			}
			float x = 0;
			float y = 0;

			if (player.isRightMoving) x -= (player.scroll * player.walkCnt);
			if (player.isLeftMoving)  x += (player.scroll * player.walkCnt);
			if (player.isDownMoving)  y -= (player.scroll * player.walkCnt);
			if (player.isUpMoving)    y += (player.scroll * player.walkCnt);
			NativeMethods.drawBmp(g, viewImage, (int)x,(int)y,gameSize.Width,gameSize.Height, 0,0,viewImage.Width,viewImage.Height, 0x00CC0020 );
		}

		/// <summary>毎フレーム呼び出される関数。Sceneクラスからの継承関数。</summary>
		/// <param name="g">描画に使用するGraphicsオブジェクト</param>
		/// <param name="input">入力情報を保持したInputオブジェクト</param>
		/// <param name="mousePos">マウスのゲーム座標</param>
		/// <param name="clickPoint">クリックされたゲーム座標。されていない場合はnull</param>
		/// <returns>void型</returns>
		public override void update(Graphics g, Input input, Point mousePos, Point? clickPoint) {
			base.update(g, input, mousePos, clickPoint);
			player.calcDrawXY(viewTileNumWidth, viewTileNumHeight, player.getX(), player.getY(), tileSize);
			player.update(input, tileSize);
			charactersUpdate(input);
			loadMessageWindow();
		}
		/// <summary>名前を指定してシーンを変更</summary>
		/// <param name="name">遷移先のシーンの名前</param>
		/// <param name="deliveryObject">遷移先のシーンのinit関数に送るオブジェクト</param>
		/// <returns>void型</returns>
		protected override void changeSceneByName( string name, object deliveryObject ) {
			base.changeSceneByName( name, deliveryObject );
			player.characterEventHander -= characterEventReceiver;
			foreach (var c in characters)
				c.characterEventHander -= characterEventReceiver;
		}
		/// <summary>インデックスを指定してシーンを変更</summary>
		/// <param name="order">遷移先のシーンのインデックス</param>
		/// <param name="deliveryObject">遷移先のシーンのinit関数に送るオブジェクト</param>
		/// <returns>void型</returns>
		protected override void changeSceneByOrder( int order, object deliveryObject ) {
			base.changeSceneByOrder( order, deliveryObject );
			player.characterEventHander -= characterEventReceiver;
			foreach (var c in characters)
				c.characterEventHander -= characterEventReceiver;
		}


		/// <summary>Charactorからのイベントがあったら呼び出される関数。オーバーライド可能。</summary>
		/// <param name="sender">イベントを起こしたCharactorオブジェクト</param>
		/// <param name="e">Charactorからのイベントの引数</param>
		/// <returns>void型</returns>
		protected virtual void characterEventReceiver(Character sender, CharacterEventArgs e) {
			if (e.type == CharacterEventType.CEVENT_SAY.ToString()) {
				showMessageWindowAccount(new MessageWindow(sender.getFaceImage(), e.message.Split('\n')));
			}

			foreach (var character in characters) {
				character.charactorEventDispath(sender, e);
			}
			player.charactorEventDispath(sender, e);
		}

		/// <summary>表示したいMessageWindowオブジェクトを登録します。登録された順に表示されます。</summary>
		/// <param name="msgWindow">MessageWindowオブジェクト</param>
		/// <returns>void型</returns>
		protected void showMessageWindowAccount(MessageWindow msgWindow) {
			msgWindow.msgWinDestroyed = ( MessageWindow destroyed ) => {
				msgs.Remove( destroyed );
			};
			msgs.Add(msgWindow);
		}

		private void loadMessageWindow(){
			if (MessageWindow.isAnyMessageWinsowShow || msgs.Count == 0) { return; }
			/*
			addActor(msgs[0]);
			msgs[0].isMeShow = true;
			MessageWindow.nowShowingMessageWindow = msgs[0];
			MessageWindow.isAnyMessageWinsowShow = true;
			*/
			nextFrameActions.Add( () => {
				addActor( msgs[0] );
				msgs[0].isMeShow = true;
				MessageWindow.nowShowingMessageWindow = msgs[0];
				MessageWindow.isAnyMessageWinsowShow = true;
			});
		}

		private void charactersUpdate(Input input) {
			foreach (var character in characters) {
				character.calcDrawXY(viewTileNumWidth, viewTileNumHeight, player.getX(), player.getY(), tileSize);
				character.update(input, tileSize);
			}
		}

		private bool canEnter(int x, int y, int z, Character.Directions direction) {
			foreach (var character in characters) {
				if (character.getX() == x && character.getY() == y && character.getZ() == z && character.hasCollision )
					return false;
			}
			string[] ok_directions;
			if ( layers.Length <= z || x < 0 || y < 0 || x >= layers[z].getRoad()[0].Length || y >= layers[z].getRoad().Length ) { return false; }
			ok_directions = layers[z].getRoad()[y][x].Split(':');

			foreach( var ok in ok_directions ) {
				if      (ok == AtlasLayer.ENTERABLE    ) { return true; }
				if      (ok == AtlasLayer.NONENTERABLE ) { return false; }
				if      (ok == AtlasLayer.FROM_1_ABLE && direction == Character.Directions.BRIGHT) { return true; }
				else if (ok == AtlasLayer.FROM_2_ABLE && direction == Character.Directions.BACK  ) { return true; }
				else if (ok == AtlasLayer.FROM_3_ABLE && direction == Character.Directions.BLEFT ) { return true; }
				else if (ok == AtlasLayer.FROM_4_ABLE && direction == Character.Directions.RIGHT ) { return true; }
				else if (ok == AtlasLayer.FROM_6_ABLE && direction == Character.Directions.LEFT  ) { return true; }
				else if (ok == AtlasLayer.FROM_7_ABLE && direction == Character.Directions.FRIGHT) { return true; }
				else if (ok == AtlasLayer.FROM_8_ABLE && direction == Character.Directions.FRONT ) { return true; }
				else if (ok == AtlasLayer.FROM_9_ABLE && direction == Character.Directions.FLEFT ) { return true; }
				else if (ok == AtlasLayer.HCROSS && (direction == Character.Directions.LEFT || direction == Character.Directions.RIGHT) ) { return true; }
				else if (ok == AtlasLayer.VCROSS && (direction == Character.Directions.BACK || direction == Character.Directions.FRONT) ) { return true; }
			}
			return false;
		}

		/// <summary>指定したCharactorをこのAtlasオブジェクトに追加します</summary>
		/// <param name="character">このオブジェクトに追加するCharactorオブジェクト</param>
		/// <returns>void型</returns>
		public void addCharactor(Character character) {
			character.characterEventHander += characterEventReceiver;
			character.getGameSize = getGameSize;
			characters.Add(character);
			characters.Sort((Character a, Character b) => a.getZ() - b.getZ());
		}
		/// <summary>指定したCharactorをこのAtlasオブジェクトに追加します</summary>
		/// <param name="characters">このオブジェクトに追加するCharactor[]オブジェクト</param>
		/// <returns>void型</returns>
		public void	addCharactors(Character[] characters) {
			foreach (var character in characters)
				addCharactor(character);
		}
		/// <summary>指定したCharactorをこのAtlasオブジェクトに追加します</summary>
		/// <param name="characters">このオブジェクトに追加するCharactorのListオブジェクト</param>
		/// <returns>void型</returns>
		public void addCharactors(List<Character> characters) {
			foreach (var character in characters)
				addCharactor(character);
		}

		/// <summary>プレイヤオブジェクトを取得します</summary>
		/// <returns>プレイヤオブジェクト</returns>
		protected Player getPlayer() { return this._player; }

		/// <summary>プレイヤオブジェクトをセットします</summary>
		/// <param name="player">セットするプレイヤオブジェクト</param>
		/// <returns>void型</returns>
		protected void setPlayer(Player player) {
			this._player = player;
			this._player.canEnter = this.canEnter;
			this._player.characterEventHander -= characterEventReceiver;
			this._player.characterEventHander += characterEventReceiver;
			this._player.getGameSize = getGameSize;
		}

		private void setTileSize(int size) {
			_tileSize = size;
			Size gameSize = getGameSize();
			viewTileNumWidth = gameSize.Width / size;
			viewTileNumHeight = gameSize.Height / size;
		}
		/// <summary>背景色をセットします</summary>
		/// <param name="value">背景色Brush</param>
		protected void setBgBrush( Brush value ) {
			if(_bgBrush != null)
				_bgBrush.Dispose();
			_bgBrush = value;
		}
		/// <summary>背景色を取得します</summary>
		/// <returns>Brush型。背景色のBrushオブジェクト</returns>
		protected Brush getBgBrush() {
			return _bgBrush;
		}


		private bool disposedValue = false;

		/// <summary>リソースを開放します</summary>
		/// <param name="disposing">デストラクタから呼ばれたらfalse</param>
		protected virtual void Dispose( bool disposing ) {
			if (!disposedValue) {
				if (disposing) {
					viewImage.Dispose();
					_bgBrush.Dispose();
				}
				disposedValue = true;
			}
		}

		/// <summary>リソースを開放します</summary>
		public void Dispose() {
			Dispose( true );
		}
	}
}
