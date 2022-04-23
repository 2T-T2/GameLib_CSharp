namespace GameLib.UI.EventArgs {
	/// <summary>DialogWindow でアイテムが選択されたときのイベントの引数クラス</summary>
	public class DialogWindowOKEventArgs: System.EventArgs {
		/// <summary>DialogWindow で選択されたアイテム</summary>
		public readonly DialogWindowItem selectedItem;
		/// <summary>コンストラクタ</summary>
		/// <param name="selectedItem">DialogWindow で選択されたアイテム</param>
		public DialogWindowOKEventArgs(DialogWindowItem selectedItem) {
			this.selectedItem = selectedItem;
		}
	}
	/// <summary>DialogWindow でアイテムが選択されなかったときのイベントの引数クラス</summary>
	public class DialogWindowNGEventArgs: System.EventArgs {}
}
