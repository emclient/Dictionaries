using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MailClient.Dictionaries
{
	public class CultureDto
	{
		public string Name { get; set; }
		public string[] Urls { get; set; }

		public CultureDto()
		{

		}

		public CultureDto(string name, string url)
		{
			Name = name;
			Urls = new [] { url };
		}
	}

	public class DictionaryService
	{
		private readonly HttpClient _httpClient;
		public event EventHandler<GetAllDictionaryCulturesCompletedEventArgs> GetAllDictionaryCulturesCompleted;
		public DictionaryService(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task<List<CultureDto>> GetAllDictionaryCulturesAsync()
		{
			var url = "https://services.emclient.com/dictionaries";
			var data = await _httpClient.GetStreamAsync(url);
			var cultureInfoList = await JsonSerializer.DeserializeAsync<List<CultureDto>>(data, new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			});

			return cultureInfoList;
		}

		public IEnumerable<CultureDto> GetAllDictionaryCultures()
		{
			return Task.Run(async () => await GetAllDictionaryCulturesAsync()).GetAwaiter().GetResult();
		}

		public void BeginGetAllDictionaryCulturesAsEvent()
		{
			Task.Run<Task>(async () =>
			{
				try
				{
					var result = await GetAllDictionaryCulturesAsync();
					GetAllDictionaryCulturesCompleted?.Invoke(this, new GetAllDictionaryCulturesCompletedEventArgs(result, null, false));
				}
				catch (TaskCanceledException e)
				{
					GetAllDictionaryCulturesCompleted?.Invoke(this, new GetAllDictionaryCulturesCompletedEventArgs(null, e, true));

				}
				catch (Exception e)
				{
					GetAllDictionaryCulturesCompleted?.Invoke(this, new GetAllDictionaryCulturesCompletedEventArgs(null, e, false));
				}
			});
		}

		public class GetAllDictionaryCulturesCompletedEventArgs : AsyncCompletedEventArgs
		{
			private List<CultureDto> results;

			internal GetAllDictionaryCulturesCompletedEventArgs(List<CultureDto> results, Exception exception, bool cancelled):
				base(exception, cancelled, null)
			{
				this.results = results;
			}

			/// <remarks/>
			public List<CultureDto> Result
			{
				get
				{
					this.RaiseExceptionIfNecessary();
					return this.results;
				}
			}
		}
	}
}
