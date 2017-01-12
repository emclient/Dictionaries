namespace MailClient.Dictionaries
{
	public class SpellCheckerEmailSettings : SpellCheckerSettings
	{
		private static SpellCheckerEmailSettings instance;

		public static SpellCheckerEmailSettings Instance
		{
			get
			{
				lock (typeof(SpellCheckerEmailSettings))
				{
					if (instance == null)
					{
						instance = new SpellCheckerEmailSettings();
					}
					return instance;
				}
			}
		}

		private SpellCheckerEmailSettings()
		{
		}


		protected override string UseSpellCheckSettingsName
		{
			get
			{
				return "UseComposeSpellchecker";
			}
		}
		protected override string TypeSettingsName
		{
			get
			{
				return "SpellCheckerComposeType";
			}
		}
		protected override string SpellCheckerFileSettingsName
		{
			get
			{
				return "SpellCheckerComposeFile";
			}
		}
		protected override DictionarySettingType DefaultDictionarySettingType
		{
			get
			{
				return DictionarySettingType.UseGeneral;
			}
		}	
	}
}
