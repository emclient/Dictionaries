namespace MailClient.Dictionaries
{
	public class SpellCheckerGeneralSettings : SpellCheckerSettings
	{
		internal SpellCheckerGeneralSettings()
		{
			
		}


		protected override string UseSpellCheckSettingsName
		{
			get
			{
				return "UseSpellchecker";
			}
		}
		protected override string TypeSettingsName
		{
			get
			{
				return "SpellCheckerType";
			}
		}
		protected override string SpellCheckerFileSettingsName
		{
			get
			{
				return "SpellCheckerFile";
			}
		}
		protected override DictionarySettingType DefaultDictionarySettingType
		{
			get
			{
				return DictionarySettingType.Language;
			}
		}		

	}
}