
namespace MailClient.Dictionaries
{
	public interface IDictionarySetting
	{
		DictionaryFilePair FilePair
		{
			get;
		}

		DictionarySettingType Type
		{
			get;
		}
	}
}
