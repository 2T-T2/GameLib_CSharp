using System.Drawing;
using TPanda.Drawing;

namespace GameLib.UI {
	/// <summary>DialogWindowに表示するアイテム</summary>
	public class DialogWindowItem: System.IDisposable {
		/// <summary>このオブジェクトの名前、表示テキスト。</summary>
		public string name {get; private set;}
		/// <summary>このオブジェクトがフォーカスされているかを示す。</summary>
		public bool   isSelected {get; private set;}
		/// <summary>DialogWindowItem のフォーカスが変わった時のイベント</summary>
		public event ToggleSelectedEventHandler toggleSelected;
		readonly Brush strCol = new SolidBrush(Color.White);

		/// <summary>コンストラクタ</summary>
		/// <param name="str">このオブジェクトの名前、表示テキスト。</param>
		public DialogWindowItem(string str) {
			this.name = str;
		}
		/// <summary>描画関数</summary>
		/// <param name="g">描画に使用するグラフィクス</param>
		/// <param name="font">このアイテムが属している DialogWindow オブジェクトで設定されているフォント</param>
		/// <param name="area">このアイテムが描画に使用できるRectangleF</param>
		/// <returns>SizeF型。描画に使用した領域のサイズ</returns>
		public virtual SizeF draw(Graphics g, Font font, RectangleF area) {
			return GraphicsEx.drawStringCentering(g, this.name, font, strCol, area);
		}

		/// <summary>選択状態を切り替える関数</summary>
		/// <param name="selected">選択状態</param>
		/// <returns>void型</returns>
		public void onToggleSelected(bool selected) {
			this.isSelected = selected;
			if ( toggleSelected != null)
				toggleSelected(this, new System.EventArgs());
		}

		/// <summary>このアイテムの名前、表示テキストを変更します</summary>
		/// <param name="name">このアイテムの名前、表示テキスト</param>
		/// <returns>void型</returns>
		public void setName(string name) { this.name = name; }

		private bool disposedValue = false;
		/// <summary>リソースを開放します</summary>
		/// <param name="disposing">デストラクタから呼ばれるとfalse</param>
		protected virtual void Dispose( bool disposing ) {
			if (!disposedValue) {
				if (disposing) {
					this.strCol.Dispose();
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
