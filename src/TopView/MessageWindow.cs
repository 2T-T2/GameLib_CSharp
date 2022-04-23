using System;
using System.Drawing;
using System.Windows.Forms;

using GameLib.Base;
using GameLib.GameInformations;

namespace GameLib.TopView {
	/// <summary>メッセージウィンドウクラス</summary>
	public class MessageWindow: Actor, System.IDisposable {
		internal delegate void MsgWinDestroyed( MessageWindow msgwin );
		internal MsgWinDestroyed msgWinDestroyed;

		/// <summary>メッセージウィンドウであることを示すタグの内容</summary>
		public static readonly string MSGWINDOW_TAG = "MSGWINDOW_TAG";
		/// <summary>現在、MessageWindowが表示されている状態かを示すプロパティ</summary>
		public static bool isAnyMessageWinsowShow { get; internal set; }
		/// <summary>現在、表示されているMessageWindowを示すプロパティ</summary>
		public static MessageWindow nowShowingMessageWindow { get; internal set; }

		/// <summary>文字を表示する行の間隔 </summary>
		protected int padding = 10;
		/// <summary>文字を表示する行の間隔 </summary>
		protected int textLineHeight = 10;

		/// <summary>背景色Brushオブジェクト</summary>
		public Brush bgBrush { get { return getBgBrush(); } set { setBgBrush( value );  } }
		Brush _bgBrush = new SolidBrush( Color.FromArgb( 192, 0, 0, 0 ) );
		/// <summary>枠線Penオブジェクト</summary>
		public Pen border { get { return getBorder();  } set { setBorder(value);  } }
		Pen _border = new Pen( Color.FromArgb( 192, 255, 255, 255 ) ) { Width = 2, };
		/// <summary>描画に使用するFontオブジェクト</summary>
		public Font font { get { return getFont(); } set { setFont( value ); } }
		Font _font;
		/// <summary>文字色</summary>
		public Color txtCol = Color.White;

		Rectangle iconRect;

		/// <summary>このMessageWindowが表示されているかを示すプロパティ</summary>
		public bool isMeShow { get; internal set; }

		/// <summary>メッセージウィンドウに表示するアイコン(左側に) </summary>
		protected Image icon;
		string[] message;
		int mesIdx = 0;

		/// <summary>コンストラクタ</summary>
		/// <param name="icon">メッセージとともに表示したい顔などの画像</param>
		/// <param name="message">表示したいメッセージ文字列。改行ごとに配列</param>
		public MessageWindow( Image icon, string[] message ) : base( 0, 0, default(Rectangle), new string[] { MSGWINDOW_TAG } ) {
			this.icon = icon;
			this.message = message;
		}

		/// <summary>Actorクラスから継承された関数</summary>
		/// <returns>void型。</returns>
		public override void init() {
			hitArea.Width = getGameSize().Width - padding;
			hitArea.Height = getGameSize().Height * 3 / 10 - padding;
			move( padding/2, getGameSize().Height-hitArea.Height-padding/2 );

			if (_font == null)
				_font = new Font( FontFamily.GenericMonospace, hitArea.Height / 3 - textLineHeight );
			iconRect = new Rectangle( hitArea.Height * 1 / (5 * 2), hitArea.Height * 1 / (5 * 2), hitArea.Height * 4 / 5 * 3 / 4, hitArea.Height * 4 / 5 );
		}

		/// <summary>Acotrからの継承。描画関数</summary>
		/// <param name="g">描画に使用するGraohics</param>
		public override void draw( Graphics g ) {
			TPanda.Drawing.GraphicsEx.fillRoundRect( g, _bgBrush, hitArea, 10 );
			TPanda.Drawing.GraphicsEx.drawRoundRect( g, _border, hitArea, 10 );

			int x = hitArea.X + textLineHeight;
			if (this.icon != null) {
				x += iconRect.Width;
				g.DrawImage( icon, iconRect.Left + centerX - hitArea.Width / 2, iconRect.Top + centerY - hitArea.Height / 2, iconRect.Width, iconRect.Height );
			}

			for (int i = 0; i < 3; i++) {
				if (mesIdx + i >= message.Length) { break; }
				using (var brush = new SolidBrush( txtCol ))
					g.DrawString( message[mesIdx + i], _font, brush, new Rectangle(x, hitArea.Y+hitArea.Height/3*i, hitArea.Width,hitArea.Height/3) );
			}
//			isAnyMessageWinsowShow = true;
		}
		/// <summary>Actorクラスから継承された関数</summary>
		/// <param name="input">入力情報を保持しているInputオブジェクト</param>
		/// <param name="clickPoint">クリック座標</param>
		/// <returns>void型。</returns>
		public override void update( Input input, Point? clickPoint ) {
//			centerY = (getGameSize().Height * 2 - (hitArea.Height + padding)) / 2;
		}
		/// <summary>Actorクラスから継承。</summary>
		public override void destroy() {
			base.destroy();
			msgWinDestroyed( this );
			Dispose();
		}


		/// <summary>3行以上表示させるメッセージがある場合は、次のメッセージが表示されます。無い場合は、このオブジェクトは描画されなくなります。</summary>
		/// <returns>void型。</returns>
		public void next() {
			mesIdx += 3;
			if (mesIdx >= message.Length) { destroy(); isAnyMessageWinsowShow = false; }
		}

		/// <summary>背景色をセットします。</summary>
		/// <param name="value">背景色として使うBrushオブジェクト</param>
		public void setBgBrush( Brush value ) {
			if (_bgBrush != null)
				_bgBrush.Dispose();
			_bgBrush = value;
		}
		/// <summary>背景色を取得します</summary>
		/// <returns>Brush型。背景色</returns>
		public Brush getBgBrush() {
			return _bgBrush;
		}
		/// <summary>枠線Penオブジェクトをセットします</summary>
		/// <param name="value">枠線Penオブジェクト</param>
		public void setBorder( Pen value ) {
			if (_border != null)
				_border.Dispose();
			_border = value;
		}
		/// <summary>枠線Penオブジェクトを取得します</summary>
		/// <returns>Brush型。枠線Penオブジェクト</returns>
		public Pen getBorder() {
			return _border;
		}
		/// <summary>描画に使用するFontをセットします</summary>
		/// <param name="value">描画したいFont</param>
		public void setFont( Font value ) {
			if (_font != null)
				_font.Dispose();
			_font = value;
		}
		/// <summary>描画に使用するFontを取得します</summary>
		/// <returns>Font型。描画に使用するFont</returns>
		public Font getFont() {
			return _font;
		}

		private bool disposedValue = false; // 重複する呼び出しを検出するには
		/// <summary>リソースを破棄します</summary>
		/// <param name="disposing">デストラクタから呼ぶ場合はtrue</param>
		protected virtual void Dispose( bool disposing ) {
			if (!disposedValue) {
				if (disposing) {
					_bgBrush.Dispose();
					_font.Dispose();
					_border.Dispose();
				}
				disposedValue = true;
			}
		}
		/// <summary>リソースを破棄します</summary>
		public void Dispose() {
			Dispose( true );
		}
	}
}
