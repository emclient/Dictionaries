namespace MailClient.Dictionaries
{
	public class SpellCheckerGeneralSettings : SpellCheckerSettings
	{
		private static SpellCheckerGeneralSettings instance;

		public static SpellCheckerGeneralSettings Instance
		{
			get
			{
				lock (typeof(SpellCheckerGeneralSettings))
				{
					if (instance == null)
					{
						instance = new SpellCheckerGeneralSettings();
					}
					return instance;
				}
			}
		}
		

		private SpellCheckerGeneralSettings()
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