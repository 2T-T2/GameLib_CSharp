using System;
using System.Drawing;

using GameLib.GameInformations;
using GameLib.GameEventsArgs;

namespace GameLib.Base {
	/// <summary>ゲーム上で、描画されたりイベントを送ったりする抽象クラス</summary>
	public abstract class Actor {
		/// <summary>actorActionEventHandlerのDelegate</summary>
		public delegate void ActorActionEventHandler(object sender, ActorActEventArgs e );
		/// <summary>属しているSceneオブジェクトに対するイベント</summary>
		public event ActorActionEventHandler actorActionHandler;
		/// <summary>ゲームサイズを取得する関数。戻り値Size</summary>
		public GetGameSize getGameSize;

		/// <summary>当たり判定を持つかを表すプロパティ</summary>
		public bool hasCollision {get; set;}
		/// <summary>中央X座標</summary>
		public int centerX;
		/// <summary>中央Y座標</summary>
		public int centerY;
		/// <summary>当たり判定領域</summary>
		public Rectangle hitArea;
		/// <summary>このオブジェくのと特徴を示すタグ情報</summary>
		public string[] tags;

		/// <summary>コンストラクタ</summary>
		/// <param name="x">中央X座標</param>
		/// <param name="y">中央Y座標</param>
		/// <param name="hitArea">中央X座標</param>
		/// <param name="tags">このActorオブジェクトの特徴を表すタグ情報</param>
		public Actor(int x, int y, Rectangle hitArea, string[] tags) {
			this.centerX = x;
			this.centerY = y;
			this.hitArea = new Rectangle( x+hitArea.X-hitArea.Width/2, y+hitArea.Y-hitArea.Height/2, hitArea.Width, hitArea.Height );
			this.tags = tags;
			this.hasCollision = true;
		}

		/// <summary>描画関数。オーバーライドする。</summary>
		/// <param name="g">描画に使用するグラフィクス</param>
		/// <returns>void型</returns>
		public abstract void draw(Graphics g);
		/// <summary>毎フレーム呼び出される関数</summary>
		/// <param name="input">入力情報を保持しているInputオブジェクト</param>
		/// <param name="clickPoint">クリック座標Point。クリックされていない場合はnull</param>
		/// <returns>void型</returns>
		public abstract void update(Input input, Point? clickPoint);
		/// <summary>シーンに属したときに呼び出される関数</summary>
		/// <returns>void型</returns>
		public virtual void init() {}
		/// <summary>他のActorオブジェクトと重なった時に呼び出される関数</summary>
		/// <param name="other">このオブジェクトと衝突したActorオブジェクト</param>
		/// <returns>void型</returns>
		public virtual void collision(Actor other){}
		/// <summary>クリックされたときに呼び出される関数</summary>
		/// <param name="p">クリックされたゲーム座標。</param>
		/// <returns>void型</returns>
		public virtual void clicked(Point p){}
		/// <summary>マウスがこのオブジェクトに入っているときに呼び出される関数</summary>
		/// <param name="p">マウスのゲーム座標。</param>
		/// <returns>void型</returns>
		public virtual void mouseEnter(Point p){}

		/// <summary>このオブジェクトに指定されたタグが含まれるかを判定する関数</summary>
		/// <param name="tag">検索するタグ</param>
		/// <returns>bool型。含まれていたらtrue</returns>
		public bool hasTag(string tag) {
			return Array.IndexOf(tags, tag) != -1;
		}

		/// <summary>このオブジェクトを移動させる関数</summary>
		/// <param name="dx">移動させるx座標の量</param>
		/// <param name="dy">移動させるy座標の量</param>
		/// <returns>void型</returns>
		public void move(int dx, int dy) {
			centerX = centerX + dx;
			centerY = centerY + dy;
			hitArea.Offset(dx, dy);
		}
		/// <summary>このオブジェクトからこのオブジェクトが属するSceneに対してイベントを送る</summary>
		/// <param name="e">イベントに送るActorEventArgs</param>
		/// <returns>void型</returns>
		protected void actToScene(ActorActEventArgs e) {
			if (actorActionHandler != null) {
				actorActionHandler( this, e );
			}else {
				Console.WriteLine("actorActionHandler is null");
			}
		}
		/// <summary>このオブジェクトからこのオブジェクトが属するSceneに対してActorを加えるイベントを送る</summary>
		/// <param name="actor">追加するActorオブジェクト</param>
		/// <returns>void型</returns>
		protected void spawnActor(Actor actor) {
			actToScene( new ActorActEventArgs( "spawn", actor ) );
			/*
			if (actorActionHandler != null) {
				actorActionHandler(this, new ActorActEventArgs("spawn", actor));
			}else {
				Console.WriteLine("actorActionHandler is null");
			}
			*/
		}
		/// <summary>このオブジェクトからこのオブジェクトが属するSceneに対して自分を削除するイベントを送る</summary>
		/// <returns>void型</returns>
		public virtual void destroy() {
			actToScene( new ActorActEventArgs( "destroy", this ) );
			/*
			if (actorActionHandler != null) {
				actorActionHandler( this, new ActorActEventArgs("destroy", this));
			}else {
				Console.WriteLine("actorActionHandler is null");
			}
			*/
		}
	}
}
