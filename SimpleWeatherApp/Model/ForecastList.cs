using System;
using System.Collections.ObjectModel;

namespace SimpleWeatherApp
{
	public class ForecastList
	{
		public class CurrentObservation
		{
			public string icon_url { get; set; }
			public string weather { get; set; }
			public string temperature_string { get; set; }
		}

		public class CurrentConditions
		{
			public CurrentObservation current_observation { get; set; }
		}

		public class Location
		{
			public string state { get; set; }
			public string city { get; set; }
		}

		public class CityAndState
		{
			public Location location { get; set; }
		}

		public class Forecastday
		{
			public string icon_url { get; set; }
			public string title { get; set; }
			public string fcttext { get; set; }
		}

		public class TxtForecast
		{
			public ObservableCollection<Forecastday> forecastday { get; set; }
		}

		public class Forecast
		{
			public TxtForecast txt_forecast { get; set; }
		}

		public class RootObject
		{
			public Forecast forecast { get; set; }
		}
	}
}
