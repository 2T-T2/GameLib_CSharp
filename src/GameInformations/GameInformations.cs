using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using System;

namespace GameLib.GameInformations
{
	/// <summary>Game全体に関する情報クラス</summary>
	public class GameInfo {
		/// <summary>fpsプロパティ</summary>
		public static int fps {get; set;}
		private static Hashtable Images = new Hashtable();

		internal static Hashtable keyHash = new Hashtable();
		internal static Hashtable preKeyHash = new Hashtable();

		/// <summary>キー入力情報を取得する関数</summary>
		/// <returns>Input型。キー入力情報</returns>
		public static Input getInput() {
			Hashtable keyHash = new Hashtable(GameInfo.keyHash);
			Hashtable preKeyHash = new Hashtable(GameInfo.preKeyHash);
			GameInfo.preKeyHash = new Hashtable(GameInfo.keyHash);
			return new Input(keyHash, preKeyHash);
		}
		/// <summary>画像を名前を付けて保持する関数</summary>
		/// <param name="name">名前</param>
		/// <param name="img">読み込む画像</param>
		/// <returns>void型。</returns>
		public static void addImage(string name, Image img) {
			Images[name] = img;
		}
		/// <summary>画像を名前を付けて読み込む</summary>
		/// <param name="name">名前</param>
		/// <param name="path">読み込む画像のファイルパス</param>
		/// <returns>void型。</returns>
		public static void addImage(string name, string path) {
			Images[name] = Image.FromFile(path);
		}
		/// <summary>名前を付けて読み込み保持した画像を取得する関数</summary>
		/// <param name="name">名前</param>
		/// <returns>Iamge型。</returns>
		public static Image getImage(string name) {
			if (Images.ContainsKey(name)) {
				return (Image)Images[name];
			}else {
				return null;
			}
		}
	}

	/// <summary>キー入力情報を保持するクラス</summary>
	public class Input {
		/// <summary>このフレームのキー入力情報</summary>
		public Hashtable keyHash = new Hashtable();
		/// <summary>前のフレームのキー入力情報</summary>
		public Hashtable preKeyHash = new Hashtable();

		/// <summary>コンストラクタ</summary>
		/// <param name="keyHash">このフレームのキー入力情報</param>
		/// <param name="preKeyHash">前のフレームのキー入力情報</param>
		public Input(Hashtable keyHash, Hashtable preKeyHash) {
			this.keyHash = keyHash;
			this.preKeyHash = preKeyHash;
		}
		private bool getKeyFromHash(Keys key, Hashtable hash) {
			if (hash.ContainsKey(key)) {
				return (bool)hash[key];
			}else {
				return false;
			}
		}
		private bool getPreviewKey(Keys key) {
			return getKeyFromHash(key, preKeyHash);
		}

		/// <summary>指定されたキーが押されているかを取得する関数</summary>
		/// <param name="key">押されているかを確認するキー</param>
		/// <returns>bool型。押されていたらtrue</returns>
		public bool getKey(Keys key) {
			return getKeyFromHash(key, keyHash);
		}
		/// <summary>指定されたキーが押されたかを取得する関数</summary>
		/// <param name="key">押されたかを確認するキー</param>
		/// <returns>bool型。押されたらtrue</returns>
		public bool getKeyDown(Keys key) {
			bool previewDown = getPreviewKey(key);
			bool currentDown = getKey(key);
			return !previewDown && currentDown;
		}
		/// <summary>指定されたキーが離されたかを取得する関数</summary>
		/// <param name="key">離されたかを確認するキー</param>
		/// <returns>bool型。離されたらtrue</returns>
		public bool getKeyUp(Keys key) {
			bool previewDown = getPreviewKey(key);
			bool currentDown = getKey(key);
			return previewDown && !currentDown;
		}
	}
}
