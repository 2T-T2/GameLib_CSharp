using System;
using GameLib.Util;

namespace GameLib.TopView.EventArgs {
	/// <summary>Characterがデフォルトで起こすイベントを定義したEnum</summary>
	public enum CharacterEventType {
		/// <summary>Characterがデフォルトで起こす「話しかける」イベント</summary>
		CEVENT_TALK,
		/// <summary>Characterがデフォルトで起こす「発言」イベント</summary>
		CEVENT_SAY,
	}
	/// <summary>キャラクターが起こすイベントの引数</summary>
	public class CharacterEventArgs {
		/// <summary>イベントを他のマスのCharactorに働きかける場合にそのターゲットとなる座標</summary>
		public readonly Coords target;
		/// <summary>イベントを識別するための名前</summary>
		public readonly string type;
		/// <summary>送るメッセージ</summary>
		public readonly string message;

		/// <summary>コンストラクタ</summary>
		/// <param name="target">イベントを他のマスのCharactorに働きかける場合にそのターゲットとなる座標</param>
		/// <param name="type">イベントを識別するための名前</param>
		/// <param name="message">送るメッセージ</param>
		public CharacterEventArgs(Coords target, string type, string message) {
			foreach (string name in Enum.GetNames(typeof(CharacterEventType))) {
				if (name == type)
					throw new FormatException("第一引数の値に「" + type + "」は使用できません。");
			}
			this.target = target;
			this.type = type;
			this.message = message;
		}
		/// <summary>コンストラクタ</summary>
		/// <param name="x">イベントを他のマスのCharactorに働きかける場合にそのターゲットとなるX座標</param>
		/// <param name="y">イベントを他のマスのCharactorに働きかける場合にそのターゲットとなるY座標</param>
		/// <param name="type">イベントを識別するための名前</param>
		/// <param name="message">送るメッセージ</param>
		public CharacterEventArgs(int x, int y, string type, string message) {
			foreach (string name in Enum.GetNames(typeof(CharacterEventType))) {
				if (name == type)
					throw new FormatException("第一引数の値に「" + type + "」は使用できません。");
			}
			this.target = new Coords(x, y);
			this.type = type;
			this.message = message;
		}
		internal CharacterEventArgs(Coords target, CharacterEventType cEvType, string message) {
			this.target = target;
			this.type = cEvType.ToString();
			this.message = message;
		}
	}
}
