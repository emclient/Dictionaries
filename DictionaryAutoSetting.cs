using System;
namespace MailClient.Dictionaries
{
	public class DictionaryAutoSetting : IDictionarySetting
	{
		DictionaryFilePair pair = new DictionaryFilePair("auto", "auto");

		public DictionaryAutoSetting()
		{
		}

		public DictionarySettingType Type
		{
			get { return DictionarySettingType.Auto; }
		}

		public DictionaryFilePair FilePair
		{
			get { return pair; }
		}

		public override string ToString()
		{
			return Resources.Dictionaries_base.AutoDetection;
		}

		public override int GetHashCode()
		{
			return Type.GetHashCode();
		}
		public override bool Equals(object obj)
		{
			return obj is DictionaryAutoSetting autoSetting;
		}
	}
}
