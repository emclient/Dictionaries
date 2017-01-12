namespace MailClient.Dictionaries
{
	public class DictionaryKeyboardSetting : IDictionarySetting
	{

		public DictionaryKeyboardSetting()
		{
		}

		public DictionarySettingType Type
		{
			get { return DictionarySettingType.Keyboard; }
		}

		public DictionaryFilePair FilePair
		{
			get { return DictionaryManager.FindBestDictionary(System.Windows.Forms.InputLanguage.CurrentInputLanguage.Culture); }
		}

		public override string ToString()
		{
			return Resources.Dictionaries_base.KeyboardLanguage;
		}

		public override int GetHashCode()
		{
			return Type.GetHashCode();
		}
		public override bool Equals(object obj)
		{
			DictionaryKeyboardSetting culture = obj as DictionaryKeyboardSetting;

			return culture != null;
		}
	}
}
