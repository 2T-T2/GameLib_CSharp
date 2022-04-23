using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

using GameLib.GameInformations;
using GameLib.GameEventsArgs;

namespace GameLib.Base {
	/// <summary> ゲームのシーンを扱うための抽象クラス </summary>
	public abstract class Scene {
/*
		/// <summary>コントロール追加イベントのDelegate</summary>
		public delegate void addControlEventHandler<T>(T args);
		/// <summary>画面にWindows Forms Control を追加するイベント</summary>
		public event addControlEventHandler<AddControlEventArgs> addControlHandler;
*/
	/// <summary>シーン遷移イベントのDelegate</summary>
		public delegate void changeSceneEventHandler<T>(object sender, T e);
		/// <summary>シーン遷移を行うイベント</summary>
		public event changeSceneEventHandler<ChangeSceneEventArgs> changeSceneHandler;

		/// <summary>ゲーム画面のサイズを取得する関数</summary>
		public GetGameSize getGameSize;
		/// <summary>シーンの名前を表す変数</summary>
		public string name = "";
		/// <summary>このシーンで描画するActorオブジェクトのArrayList型配列</summary>
		public ArrayList actors = new ArrayList();
		/// <summary>このシーンで描画をやめるActorオブジェクトのArrayList型配列</summary>
		public ArrayList destroyedActors = new ArrayList();
		private ArrayList spawnActors = new ArrayList();

		/// <summary>つぎのフレームに行いたい処理を示すデリゲート。 void NextFrameAction() </summary>
		protected delegate void NextFrameAction();
		/// <summary>つぎのフレームに行いたい処理リスト</summary>
		protected List<NextFrameAction> nextFrameActions = new List<NextFrameAction>();

//		private ArrayList ctrlStack = new ArrayList();

		/// <summary>シーンの名前を指定するコンストラクタ</summary>
		/// <param name="name">このシーンの名前</param>
		public Scene(string name) {
			this.name = name;
		}

		/// <summary>このシーンに切り替わった時に一度だけ呼び出される関数。オーバーライドして使用。リソース等をここで読み込むとメモリを節約可能！！</summary>
		/// <returns>void型</returns>
		public virtual void init() {}
		/// <summary>このシーンに切り替わった時に一度だけ呼び出される関数。オーバーライドして使用。リソース等をここで読み込むとメモリを節約可能！！</summary>
		/// <returns>void型</returns>
		public virtual void init(object initializeArg) {
			init();
		}

		/// <summary>毎フレーム呼び出される関数。オーバーライド可能</summary>
		/// <param name="g">描画に使用するGraphicsオブジェクト</param>
		/// <param name="input">入力情報を保持しているInputオブジェクト</param>
		/// <param name="mousePos">マウスの位置情報</param>
		/// <param name="clickPoint">クリック座標。クリックされていない場合はnull</param>
		/// <returns>void型</returns>
		public virtual void update(Graphics g, Input input, Point mousePos, Point? clickPoint) {
			// if (ctrlStack.Count != 0) { addControlInStack(); }
			doPreFrameActions();
			bgDraw(g);
			checkClickActors(clickPoint);
			checkMouseEnterActors(mousePos);
			updateActors(input, clickPoint);
			collision();
			destroyActors();
			_addActors();
			drawActors( g );
		}
		/// <summary>描画関数</summary>
		/// <returns>void型</returns>
		public abstract void bgDraw(Graphics g);

		private void doPreFrameActions() {
			foreach (var action in nextFrameActions) {
				action();
			}
			nextFrameActions.Clear();
		}
		private void checkClickActors(Point? clickPoint) {
			if (clickPoint == null) { return; }
			Point p = (Point)clickPoint;
			for (int i = 0; i < actors.Count; i++) {
				Actor actor = ((Actor)actors[i]);
				if ( actor.hitArea.Contains(p) ) { actor.clicked(p); }
			}
		}
		private void checkMouseEnterActors(Point p) {
			for (int i = 0; i < actors.Count; i++) {
				Actor actor = ((Actor)actors[i]);
				if ( actor.hitArea.Contains(p) ) { actor.mouseEnter(p); }
			}
		}
		private void updateActors(Input input, Point? clickPoint) {
			for ( int i = 0; i < actors.Count; i++ ) { ((Actor)actors[i]).update(input, clickPoint); }
		}
		private void collision() {
			if (actors.Count < 2) { return; }
			for(int i = 0; i < actors.Count; i++){
					Actor actor1 = (Actor)actors[i];
					if (!actor1.hasCollision) { continue; }
				for(int j = i+1; j < actors.Count; j++) {
					Actor actor2 = (Actor)actors[j];
					if (!actor2.hasCollision) {continue;}
					if ( actor1.hitArea.IntersectsWith( actor2.hitArea ) ) {
						actor1.collision(actor2);
						actor2.collision(actor1);
					}
				}
			}
		}
		private void destroyActors() {
			for(int i = 0; i < destroyedActors.Count; i++) { actors.Remove(destroyedActors[i]); }
			destroyedActors.Clear();
		}
		private void drawActors(Graphics g) {
			foreach(Actor actor in actors) { actor.draw(g); }
		}

		/// <summary>描画するActorオブジェクトを追加する関数</summary>
		/// <param name="actor">追加するActorオブジェクト</param>
		/// <returns>void型</returns>
		public void addActor(Actor actor) {
			spawnActors.Add( actor );
		}
		private void _addActors() {
			foreach (Actor actor in spawnActors) {
				actor.actorActionHandler += actorAction;
				actor.getGameSize = getGameSize;
				actor.init();
				actors.Add( actor );
			}
			spawnActors.Clear();
		}
		/*
			/// <summary>Windows Forms Control を追加する関数</summary>
			/// <param name="ctrl">追加するコントロール</param>
			/// <returns>void型</returns>
			protected void addControl(Control ctrl){
				if (addControlHandler != null) {
					addControlHandler(new AddControlEventArgs(ctrl));
				}else {
					ctrlStack.Add(ctrl);
				}
			}
			private void addControlInStack() {
				for (int i = 0; i < ctrlStack.Count; i++) { addControl( (Control)ctrlStack[i] ); }
				ctrlStack.Clear();
			}
		*/

		/// <summary>インデックスを指定してシーンを変更</summary>
		/// <param name="order">遷移先のシーンのインデックス</param>
		/// <param name="deliveryObject">遷移先のシーンのinit関数に送るオブジェクト</param>
		/// <returns>void型</returns>
		protected virtual void changeSceneByOrder(int order, object deliveryObject) {
			if (changeSceneHandler != null) {
				changeSceneHandler(this, new ChangeSceneEventArgs("byOrder", order, deliveryObject));
				foreach (Actor actor in actors)
					actor.actorActionHandler -= actorAction;
			} else {
				Console.WriteLine("Handler is null !");
			}
		}
		/// <summary>インデックスを指定してシーンを変更</summary>
		/// <param name="order">遷移先のシーンのインデックス</param>
		/// <returns>void型</returns>
		protected void changeSceneByOrder(int order) { changeSceneByOrder(order, null); }

		/// <summary>名前を指定してシーンを変更</summary>
		/// <param name="name">遷移先のシーンの名前</param>
		/// <param name="deliveryObject">遷移先のシーンのinit関数に送るオブジェクト</param>
		/// <returns>void型</returns>
		protected virtual void changeSceneByName(string name, object deliveryObject) {
			if (changeSceneHandler != null) {
				changeSceneHandler(this, new ChangeSceneEventArgs("byName", name, deliveryObject));
				foreach (Actor actor in actors)
					actor.actorActionHandler -= actorAction;
			} else {
				Console.WriteLine("Handler is null !");
			}
		}
		/// <summary>名前を指定してシーンを変更</summary>
		/// <param name="name">遷移先のシーンの名前</param>
		/// <returns>void型</returns>
		protected void changeSceneByName(string name) {
			changeSceneByName(name, null);
		}

		/// <summary>このシーンに属したActorオブジェクトがイベントを起こしたときに呼ばれる関数</summary>
		/// <param name="sender">このイベントを送ったActorオブジェクト</param>
		/// <param name="e">ActorEventArgsオブジェクト</param>
		/// <returns>void型</returns>
		protected virtual void actorAction(object sender, ActorActEventArgs e) {
			if (e.eventName == "spawn") {
				addActor((Actor)e.receiveObject);
			}else if (e.eventName == "destroy") {
				destroyedActors.Add(sender);
			}
		}
	}
}
