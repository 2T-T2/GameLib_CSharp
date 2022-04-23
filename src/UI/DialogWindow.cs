using System;
using System.Drawing;
using System.Windows.Forms;

using TPanda.Drawing;

using GameLib.Util;
using GameLib.Base;
using GameLib.GameInformations;

namespace GameLib.UI {
	/// <summary>DialogWindow の閉じる設定</summary>
	public enum DialogCloseType {
		/// <summary>DialogWindow の destroy関数が呼ばれるまで 閉じることはありません</summary>
		NOT_CLOSE,
		/// <summary>DialogWindow でアイテムが選択されなかった場合閉じます</summary>
		NG_CLOSE,
		/// <summary>DialogWindow でアイテムが選択されてもされなくても閉じます</summary>
		OK_NG_CLOSE,
		/// <summary>DialogWindow でアイテムが選択された場合閉じます</summary>
		OK_CLOSE,
	}

	/// <summary>DialogWindowItem がフォーカスされているときの状態を表します</summary>
	[Flags]
	public enum ForcusedDialogItemDrawStyle {
		/// <summary>なにもしませｎ</summary>
		NONE      = 0x0000,
		/// <summary>光ります</summary>
		HIGHLIGHT = 0x0001,
		/// <summary>マークがアイテムの左側に表示されます。DialogWindow の mark で表示するマークを設定します</summary>
		MARK      = 0x0002,
		/// <summary>枠線が表示されます</summary>
		BORDER    = 0x0004,
		/// <summary>点滅します</summary>
		BLINK     = 0x0008,
	}

	/// <summary>アイテムを選択するダイアログ。Actorクラスを継承しています。</summary>
	public class DialogWindow: Actor, IDisposable {
		/// <summary>Actorクラスの tags に含める このクラスのオブジェクトであることを示すための文字列</summary>
		public static string DIALOG_TAG = "DIALOG_TAG";
		/// <summary>このクラスのオブジェクトで 選択 を意味するキー</summary>
		public static Keys  OK_KEY    = Keys.Enter;
		/// <summary>このクラスのオブジェクトで キャンセル を意味するキー</summary>
		public static Keys  NG_KEY    = Keys.Back;
		/// <summary>このクラスのオブジェクトで アイテムのフォーカスを上方向に移動 を意味するキー</summary>
		public static Keys  UP_KEY    = Keys.Up;
		/// <summary>このクラスのオブジェクトで アイテムのフォーカスを下方向に移動 を意味するキー</summary>
		public static Keys  DOWN_KEY  = Keys.Down;
		/// <summary>このクラスのオブジェクトで アイテムのフォーカスを左方向に移動 を意味するキー</summary>
		public static Keys  LEFT_KEY  = Keys.Left;
		/// <summary>このクラスのオブジェクトで アイテムのフォーカスを右方向に移動 を意味するキー</summary>
		public static Keys  RIGHT_KEY = Keys.Right;

		/// <summary>このオブジェクトに属しているDialogWindowItemで使用するフォント</summary>
		public Font font { get { return getFont(); } set { setFont( value ); } }// = new Font( FontFamily.GenericMonospace, 12 );
		private Font _font = new Font( FontFamily.GenericMonospace, 12 );
		/// <summary>背景色</summary>
		public Brush bgBrush { get { return getBgBrush(); } set { setBgBrush( value ); } }//     = new SolidBrush(Color.FromArgb(192, 0,0,0));
		private Brush _bgBrush = new SolidBrush( Color.FromArgb( 192, 0, 0, 0 ) );
		/// <summary>forcusItemDraw が ForcusedDialogItemDrawStyle.HIGHLIGHT、 ForcusedDialogItemDrawStyle.BLINK の時に使用する色</summary>
		public Brush highlight { get { return getHighlightBrush(); } set { setHightlightBrush( value ); } }//= new SolidBrush(Color.FromArgb(128, 255,255,255));
		private Brush _highlight = new SolidBrush( Color.FromArgb( 128, 255, 255, 255 ) );
		/// <summary>枠線色</summary>
		public Pen borderPen { get { return getBorderPen(); } set { setBorderPen(value); } }//= new Pen(Color.FromArgb(192, 255,255,255));
		private Pen _borderPen = new Pen( Color.FromArgb( 192, 255, 255, 255 ) );
		/// <summary>forcusItemDraw が ForcusedDialogItemDrawStyle.BORDER の時に使用する枠線色</summary>
		public Pen selectBorder { get { return getSelectBorder(); } set { setSelectBorder( value ); } }//= new Pen(Color.FromArgb(192, 255,255,255));
		private Pen _selectBorder = new Pen( Color.FromArgb( 192, 255, 255, 255 ) );
		/// <summary>フォーカスされているアイテムに対する書式設定</summary>
		public ForcusedDialogItemDrawStyle forcusItemDraw = ForcusedDialogItemDrawStyle.MARK;
		/// <summary>forcusItemDraw が ForcusedDialogItemDrawStyle.BLINK の時の点滅スピード</summary>
		public int    blinkInterval = 30;
		/// <summary>forcusItemDraw が ForcusedDialogItemDrawStyle.MARK の時に使用するマーク</summary>
		public string mark = "●";
		/// <summary>このダイアログの有効無効を示す</summary>
		public bool   enable = true;
		/// <summary>このダイアログがマウス操作を受け付けるかを示す</summary>
		public bool   mouseEnable = true;

		/// <summary>DialogWindow でアイテムが選択されたときのイベント</summary>
		public event OkEventHandler OK;
		/// <summary>DialogWindow でアイテムが選択されなかったときのイベント</summary>
		public event NgEventHandler NG;

		/// <summary>描画するアイテム</summary>
		protected DialogWindowItem[] items;
		readonly DialogCloseType closeType;
		/// <summary>選択中のアイテムが左からいくつめにあるか</summary>
		protected int selectX = 0;
		/// <summary>選択中のアイテムが上からいくつめにあるか</summary>
		protected int selectY = 0;
		/// <summary>点滅のカウンター</summary>
		protected int blinkCnt;

		/// <summary>縦に並べるアイテムの数</summary>
		protected readonly int hItemNum;
		/// <summary>横に並べるアイテムの数</summary>
		protected readonly int wItemNum;
		/// <summary>並べるアイテムの横幅</summary>
		protected readonly float itemWidth;
		/// <summary>並べるアイテムの縦幅</summary>
		protected readonly float itemHeight;

		/// <summary>コンストラクタ</summary>
		/// <param name="rect">このオブジェクト領域</param>
		/// <param name="items">このオブジェクトが持つアイテム</param>
		/// <param name="hItemNum">縦に並べるアイテム数</param>
		/// <param name="wItemNum">横に並べるアイテム数</param>
		/// <param name="closeType">閉じる条件</param>
		public DialogWindow(Rectangle rect, DialogWindowItem[] items, int hItemNum, int wItemNum, DialogCloseType closeType): base((rect.X+rect.Width/2), (rect.Y+rect.Height/2), new Rectangle(0,0, rect.Width,rect.Height), DIALOG_TAG.Split(',')) {
			this.items = items;
			this.hItemNum = hItemNum;
			this.wItemNum = wItemNum;
			this.closeType = closeType;
			itemWidth  = 1.0f * rect.Width  / wItemNum;
			itemHeight = 1.0f * rect.Height / hItemNum;
			blinkCnt = blinkInterval;
		}

		/// <summary>描画関数。オーバーライド可能。Actorクラスから継承</summary>
		/// <param name="g">描画に使用するグラフィクス</param>
		/// <returns>void型</returns>
		public override void draw(Graphics g) {
			GraphicsEx.fillRoundRect(g, _bgBrush, hitArea, 10);

			PointF[,] iconPos = new PointF[hItemNum, wItemNum];
			for (int y = 0; y < hItemNum; y++)
				for (int x = 0; x < wItemNum; x++)
					iconPos[y,x] = new PointF(System.Single.MaxValue, 0);

			for (int y = 0; y < hItemNum; y++) {
				for (int x = 0; x < wItemNum; x++) {
					if (x+y*wItemNum == items.Length) { break; }
					RectangleF rect = new RectangleF( hitArea.X+(x*itemWidth), hitArea.Y+(y*itemHeight), itemWidth, itemHeight );
					SizeF s = items[x+y*wItemNum].draw(g, _font, rect);
					iconPos[y,x].X = rect.X + (rect.Width-s.Width)/2;
					iconPos[y,x].Y = rect.Y + (rect.Height-s.Height)/2;
				}
			}

			int yIdx = 0;
			for(int y = 0; y < hItemNum; y++) if (iconPos[y, selectX].X < iconPos[yIdx, selectX].X ) yIdx = y;
			iconPos[selectY, selectX].X = iconPos[yIdx, selectX].X;
			if( (forcusItemDraw & ForcusedDialogItemDrawStyle.HIGHLIGHT) > 0) drawHighlight(g);
			if( (forcusItemDraw & ForcusedDialogItemDrawStyle.MARK)      > 0) drawMark(g, iconPos[selectY, selectX]);
			if( (forcusItemDraw & ForcusedDialogItemDrawStyle.BORDER)    > 0) drawBorder(g);
			if( (forcusItemDraw & ForcusedDialogItemDrawStyle.BLINK)     > 0) drawBlink(g);
			GraphicsEx.drawRoundRect(g, _borderPen, hitArea, 10);
			blinkCnt--;
			if (blinkCnt == -1) { blinkCnt = blinkInterval; }
		}
		/// <summary>毎フレーム呼び出される関数。オーバーライド可能。Actorクラスから継承</summary>
		/// <param name="input">入力情報を保持しているInputオブジェクト</param>
		/// <param name="clickPoint">クリックされた座標。されて無い時はnull</param>
		/// <returns>void型</returns>
		public override void update(Input input, Point? clickPoint) {
			if (!enable) { return; }
			if (mouseEnable && clickPoint != null && !hitArea.Contains(((Point)clickPoint))) { NgEvent(); }

			if ( input.getKeyDown(UP_KEY) ) {
				if(selectY != 0) {
					items[selectX+selectY*wItemNum].onToggleSelected(false);
					selectY--;
					items[selectX+selectY*wItemNum].onToggleSelected(true);
				}

			} else if ( input.getKeyDown(DOWN_KEY) ) {
				if( (selectX+(selectY+1)*wItemNum) < items.Length && selectY+1 < hItemNum) {
					items[selectX+selectY*wItemNum].onToggleSelected(false);
					selectY++;
					items[selectX+selectY*wItemNum].onToggleSelected(true);
				}

			} else if ( input.getKeyDown(LEFT_KEY) ) {
				if(selectX != 0) {
					items[selectX+selectY*wItemNum].onToggleSelected(false);
					selectX--;
					items[selectX+selectY*wItemNum].onToggleSelected(true);
				}

			} else if ( input.getKeyDown(RIGHT_KEY) ) {
				if( ((selectX+1)+selectY*wItemNum) < items.Length && selectX+1 < wItemNum) {
					items[selectX+selectY*wItemNum].onToggleSelected(false);
					selectX++;
					items[selectX+selectY*wItemNum].onToggleSelected(true);
				}

			} else if ( input.getKeyDown(OK_KEY) ) {
				OkEvent();

			} else if ( input.getKeyDown(NG_KEY) ) {
				NgEvent();
			}
		}
		/// <summary>マウスがこのオブジェクトに入っている時に呼び出される関数。Actorクラスから継承</summary>
		/// <param name="p">マウス座標</param>
		/// <returns>void型</returns>
		public override void mouseEnter(Point p) {
			if(!enable || !mouseEnable) { return; }
			items[selectX+selectY*wItemNum].onToggleSelected(false);
			selectX = (p.X-hitArea.X) / (int)itemWidth;
			selectY = (p.Y-hitArea.Y) / (int)itemHeight;
			selectX = selectX == wItemNum ? wItemNum-1 : selectX;
			selectY = selectY == hItemNum ? hItemNum-1 : selectY;
			items[selectX+selectY*wItemNum].onToggleSelected(true);
		}
		/// <summary>このオブジェクトがクリックされた時に呼び出される関数。Actorクラスから継承</summary>
		/// <param name="p">マウス座標</param>
		/// <returns>void型</returns>
		public override void clicked(Point p) {
			if(!enable || !mouseEnable) { return; }
			int x = (p.X-hitArea.X) / (int)itemWidth;
			int y = (p.Y-hitArea.Y) / (int)itemHeight;
			x = x == wItemNum ? wItemNum-1 : x;
			y = x == hItemNum ? hItemNum-1 : y;
			OkEvent();
		}
		/// <summary>Actorクラスからの継承。リソースを開放しながらdestroyをします。</summary>
		public override void destroy() {
			base.destroy();
			Dispose();
		}

		/// <summary>フォーカスされているアイテムをハイライト表示する際に呼び出される関数。オーバーライド可能</summary>
		public virtual void drawHighlight(Graphics g) {
			GraphicsEx.fillRoundRect(g, _highlight, new Rectangle(hitArea.X+(int)(selectX*itemWidth),hitArea.Y+(int)(selectY*itemHeight), (int)itemWidth,(int)itemHeight), 10.0f);
		}
		/// <summary>フォーカスされているアイテムにマークを表示する際に呼び出される関数。オーバーライド可能</summary>
		public virtual void drawMark(Graphics g, PointF iconPos) {
			iconPos.X = iconPos.X - g.MeasureString(mark, _font).Width;
			g.DrawString(mark, _font, Brushes.White, iconPos);
		}
		/// <summary>フォーカスされているアイテムに枠線を表示する際に呼び出される関数。オーバーライド可能</summary>
		public virtual void drawBorder(Graphics g) {
			GraphicsEx.drawRoundRect(g, _selectBorder, new Rectangle(hitArea.X+(int)(selectX*itemWidth),hitArea.Y+(int)(selectY*itemHeight), (int)itemWidth,(int)itemHeight), 10.0f);
		}
		/// <summary>フォーカスされているアイテムを点滅表示する際に呼び出される関数。オーバーライド可能</summary>
		public virtual void drawBlink(Graphics g) {
			double d = 1.0d - Easing.inOutQuad( (double)blinkCnt/blinkInterval );
			using(var b = new SolidBrush(Color.FromArgb( (int)(192*d), 255,255,255) )) {
				GraphicsEx.fillRoundRect(g, b, new Rectangle(hitArea.X+(int)(selectX*itemWidth),hitArea.Y+(int)(selectY*itemHeight), (int)itemWidth,(int)itemHeight), 10.0f);
			}
		}

		private void OkEvent() {
			if (OK != null) OK(this, new EventArgs.DialogWindowOKEventArgs(items[selectX+selectY*wItemNum]));
			if ( closeType == DialogCloseType.OK_CLOSE || closeType == DialogCloseType.OK_NG_CLOSE ) { this.destroy(); }
		}
		private void NgEvent() {
			if (NG != null) NG(this, new EventArgs.DialogWindowNGEventArgs());
			if (closeType == DialogCloseType.NG_CLOSE || closeType == DialogCloseType.OK_NG_CLOSE) { this.destroy(); }
		}

		private bool disposedValue = false;

		/// <summary>リソースを開放します</summary>
		/// <param name="disposing">デストラクタから呼ばれたらfalse</param>
		protected virtual void Dispose( bool disposing ) {
			if (!disposedValue) {
				if (disposing) {
					this._bgBrush.Dispose();
					this._borderPen.Dispose();
					this._font.Dispose();
					this._highlight.Dispose();
					this._selectBorder.Dispose();
				}
				disposedValue = true;
			}
		}

		/// <summary>リソースを開放します</summary>
		public void Dispose() {
			Dispose( true );
		}

		/// <summary>描画に使用するフォントをセットします</summary>
		/// <param name="value">描画に使用するフォント</param>
		public void setFont( Font value ) {
			_font.Dispose();
			_font = value;
		}
		/// <summary>描画に使用するフォントをゲットします</summary>
		/// <returns>Font型。描画に使用しているフォント</returns>
		public Font getFont() {
			return _font;
		}
		/// <summary>背景色をセットします</summary>
		/// <param name="value">指定する背景色Brush</param>
		public void setBgBrush( Brush value ) {
			_bgBrush.Dispose();
			_bgBrush = value;
		}
		/// <summary>背景色をゲットします</summary>
		/// <returns>Brush型。背景色Brush</returns>
		public Brush getBgBrush() {
			return _bgBrush;
		}
		/// <summary>forcusItemDraw が ForcusedDialogItemDrawStyle.HIGHLIGHT、 ForcusedDialogItemDrawStyle.BLINK の時に使用する色を指定します</summary>
		/// <param name="value">指定するBrush</param>
		public void setHightlightBrush( Brush value ) {
			_highlight.Dispose();
			_highlight = value;
		}
		/// <summary>forcusItemDraw が ForcusedDialogItemDrawStyle.HIGHLIGHT、 ForcusedDialogItemDrawStyle.BLINK の時に使用する色を取得します</summary>
		/// <returns>Brush型。使用されるBrush</returns>
		public Brush getHighlightBrush() {
			return _highlight;
		}
		/// <summary>枠線色をセットします</summary>
		/// <param name="value">セットする枠線Pen</param>
		public void setBorderPen( Pen value ) {
			_borderPen.Dispose();
			_borderPen = value;
		}
		/// <summary>枠線色を取得します</summary>
		/// <returns>Pen型。枠線色Pen</returns>
		public Pen getBorderPen() {
			return _borderPen;
		}
		/// <summary>forcusItemDraw が ForcusedDialogItemDrawStyle.BORDER の時に使用する枠線色をセットします</summary>
		/// <param name="value">forcusItemDraw が ForcusedDialogItemDrawStyle.BORDER の時に使用する枠線色Brush</param>
		public void setSelectBorder( Pen value ) {
			_selectBorder.Dispose();
			_selectBorder = value;
		}
		/// <summary>forcusItemDraw が ForcusedDialogItemDrawStyle.BORDER の時に使用する枠線色をゲットします</summary>
		/// <returns>Brush型。forcusItemDraw が ForcusedDialogItemDrawStyle.BORDER の時に使用する枠線色Brush</returns>
		public Pen getSelectBorder() {
			return _selectBorder;
		}
	}
}
