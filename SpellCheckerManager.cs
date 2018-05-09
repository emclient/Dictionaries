namespace MailClient.Dictionaries
{
	public static class SpellCheckerManager
	{
		static ISpellChecker instance;

		public static ISpellChecker CreateSpellChecker(SpellCheckerSettings settings)
		{
			if (instance == null)
				instance = CreateSpellCheckerInternal(settings);
			return instance;
		}

		static ISpellChecker CreateSpellCheckerInternal(SpellCheckerSettings settings)
		{
#if MAC
			return MacSpellChecker.Create(settings);
#else
			return HunspellChecker.Create(settings);
#endif
		}
	}
}
