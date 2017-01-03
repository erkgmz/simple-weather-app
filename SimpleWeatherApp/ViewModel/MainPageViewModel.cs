using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Plugin.Geolocator;

namespace SimpleWeatherApp
{
	public class MainPageViewModel : INotifyPropertyChanged
	{
		// TODO DRY up requests to Wunderground in a method. Too much repetition.

		public async Task SetLocation(Plugin.Geolocator.Abstractions.Position locator)
		{
			// Get Lat and Lon coordinates to reverse geocode city and state 
			double lat = locator.Latitude;
			double lon = locator.Longitude;

			Lat = lat;
			Lon = lon;

			await GetCityAndStateAsync();
		}

		public async Task GetCityAndStateAsync()
		{
			// Make request to wunderground for city and state data using Lat and Lon
			string url = String.Format("http://api.wunderground.com/api/28af05d873029695/geolookup/q/{0},{1}.json",
			                          Lat,
			                          Lon);
			
			HttpClient client = new HttpClient();
			client.BaseAddress = new Uri(url);

			var response = await client.GetAsync(client.BaseAddress);
			response.EnsureSuccessStatusCode();

			var JsonResult = response.Content.ReadAsStringAsync().Result;

			var cityAndState = JsonConvert.DeserializeObject<ForecastList.CityAndState>(JsonResult);

			string state = cityAndState.location.state;
			string city = cityAndState.location.city;

			State = state;
			City = city;

			await GetCurrentConditions();
		}

		public async Task GetCurrentConditions()
		{
			// Make request for current conditions in City and State
			// CC = Current Conditions
			string url = String.Format("http://api.wunderground.com/api/28af05d873029695/conditions/q/{0}/{1}.json",
									  State,
									  City);

			HttpClient client = new HttpClient();
			client.BaseAddress = new Uri(url);

			var response = await client.GetAsync(client.BaseAddress);
			response.EnsureSuccessStatusCode();

			var JsonResult = response.Content.ReadAsStringAsync().Result;
			var currentConditions = JsonConvert.DeserializeObject<ForecastList.CurrentConditions>(JsonResult);

			string CCTitle = currentConditions.current_observation.weather;
			string CCTemperature = currentConditions.current_observation.temperature_string;
			string CCIconUrl = currentConditions.current_observation.icon_url;

			CurrentTitle = CCTitle;
			CurrentTemperature = CCTemperature;
			CurrentIconUrl = CCIconUrl;

			MakeCurrentConditionString();
		}

		public void MakeCurrentConditionString()
		{
			ConditionString = String.Format("The current condition in {0}, {1} is {2} {3}",
			                                City,
			                                State,
			                                CurrentTitle,
			                                CurrentTemperature);
		}

		public async Task GetWeatherAsync(string url)
		{
			// use HTTPClient for communication with API
			HttpClient client = new HttpClient();
			client.BaseAddress = new Uri(url);

			// get data asynchronously
			var response = await client.GetAsync(client.BaseAddress);
			response.EnsureSuccessStatusCode();

			// deserialize JSON object
			var JsonResult = response.Content.ReadAsStringAsync().Result;

			//  make object and set values for use with XAML
			var weather = JsonConvert.DeserializeObject<ForecastList.RootObject>(JsonResult);
			SetList(weather);
		}

		private void SetList(ForecastList.RootObject weather)
		{
			var forecastList = weather.forecast.txt_forecast.forecastday;
			ObservableCollection<ForecastList.Forecastday> listSource = new ObservableCollection<ForecastList.Forecastday>();

			foreach (var item in forecastList)
			{
				listSource.Add(new ForecastList.Forecastday()
				{
					icon_url = item.icon_url,
					fcttext = item.fcttext,
					title = item.title
				});
			}

			ListSource = listSource;
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		private ObservableCollection<ForecastList.Forecastday> _listSource;
		public ObservableCollection<ForecastList.Forecastday> ListSource
		{
			get
			{
				return _listSource;
			}
			set
			{
				_listSource = value;
				NotifyPropertyChanged();
			}
		}

		private double _lat;
		public double Lat
		{
			get
			{
				return _lat;
			}
			set
			{
				_lat = value;
				NotifyPropertyChanged();
			}
		}

		private double _lon;
		public double Lon
		{
			get
			{
				return _lon;
			}
			set
			{
				_lon = value;
				NotifyPropertyChanged();
			}
		}

		private string _iconUrl;
		public string CurrentIconUrl
		{
			get
			{
				return _iconUrl;
			}
			set
			{
				_iconUrl = value;
				NotifyPropertyChanged();
			}
		}

		private string _currentTemperature;
		public string CurrentTemperature
		{
			get
			{
				return _currentTemperature;
			}
			set
			{
				_currentTemperature = value;
				NotifyPropertyChanged();
			}
		}

		private string _currentTitle;
		public string CurrentTitle
		{
			get
			{
				return _currentTitle;
			}
			set
			{
				_currentTitle = value;
				NotifyPropertyChanged();
			}
		}

		private string _city;
		public string City
		{
			get
			{
				return _city;
			}
			set
			{
				_city = value;
				NotifyPropertyChanged();
			}
		}

		private string _state;
		public string State
		{
			get
			{
				return _state;
			}
			set
			{
				_state = value;
				NotifyPropertyChanged();
			}
		}

		private string _conditionString;
		public string ConditionString
		{
			get
			{
				return _conditionString;
			}
			set
			{
				_conditionString = value;
				NotifyPropertyChanged();
			}
		}
	}
}