using GameLib.TopView.Base;
using GameLib.TopView.EventArgs;

namespace GameLib.TopView {
	/// <summary>Charactorが起こしたイベントのイベントハンドラ。</summary>
	/// <param name="sender">イベントを起こしたCharactor</param>
	/// <param name="e">Charactorが起こしたイベントのCharactorEventArgs</param>
	/// <returns>void型</returns>
	public delegate void CharacterEventHander(Character sender, CharacterEventArgs e);
}
