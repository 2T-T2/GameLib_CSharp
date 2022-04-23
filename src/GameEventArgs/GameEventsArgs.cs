using System;
using System.Windows.Forms;

namespace GameLib.GameEventsArgs
{
	/// <summary>Actorオブジェクトが自分の属するSceneにイベントを送る際に使用する引数</summary>
	public class ActorActEventArgs: EventArgs {
		/// <summary>受け取ったオブジェクト</summary>
		public Object receiveObject {get; set;}
		/// <summary>このイベントの名前。イベントの識別に使用する。</summary>
		public string eventName;
		/// <summary>コンストラクタ</summary>
		/// <param name="eName">イベントを識別する際に使用する名前</param>
		/// <param name="sendObject">イベント送信先に送りたいオブジェクト</param>
		public ActorActEventArgs(string eName, Object sendObject) {
			this.receiveObject = sendObject;
			this.eventName = eName;
		}
	}

	/// <summary>SceneオブジェクトがGame(Form)に対してコントロールを追加するイベントを送る際に使用する引数</summary>
	public class AddControlEventArgs {
		/// <summary>追加するControl</summary>
		public Control ctrl { get; set; }
		/// <summary>コンストラクタ</summary>
		/// <param name="ctrl">追加するコントロール</param>
		public AddControlEventArgs(Control ctrl) {
			this.ctrl = ctrl;
		}
	}

	/// <summary>SceneオブジェクトがGameに対してシーン遷移のイベントを送る際に使用する引数</summary>
	public class ChangeSceneEventArgs: EventArgs {
		/// <summary>遷移先Sceneのインデックス</summary>
		public int order;
		/// <summary>イベントを識別するための名前</summary>
		public string eventName;
		/// <summary>遷移先のSceneの名前</summary>
		public string sceneName;
		/// <summary>遷移先のSceneのinit関数に送りたいオブジェクト</summary>
		public object deliveryObject;

		/// <summary>コンストラクタ</summary>
		/// <param name="eName">イベント識別用の名前</param>
		/// <param name="order">遷移先のSceneのインデックス</param>
		/// <param name="deliveryObject">遷移先のSceneのinit関数に送りたいオブジェクト</param>
		public ChangeSceneEventArgs(string eName, int order, object deliveryObject) {
			this.order = order;
			this.eventName = eName;
			this.deliveryObject = deliveryObject;
		}
		/// <summary>コンストラクタ</summary>
		/// <param name="eName">イベント識別用の名前</param>
		/// <param name="name">遷移先のSceneの名前</param>
		/// <param name="deliveryObject">遷移先のSceneのinit関数に送りたいオブジェクト</param>
		public ChangeSceneEventArgs(string eName, string name, object deliveryObject) {
			this.sceneName = name;
			this.eventName = eName;
			this.deliveryObject = deliveryObject;
		}
	}
}
