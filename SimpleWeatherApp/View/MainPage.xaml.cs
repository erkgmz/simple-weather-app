using System;
using System.Collections.Generic;
using Plugin.Geolocator;

using Xamarin.Forms;

namespace SimpleWeatherApp
{
	public partial class MainPage : ContentPage
	{
		MainPageViewModel vm;

		public MainPage()
		{
			vm = new MainPageViewModel();
			BindingContext = vm;
			InitializeComponent();
		}

		public async void GetLocation(object o, EventArgs e)
		{
			var locator = CrossGeolocator.Current;

			Plugin.Geolocator.Abstractions.Position position = await locator.GetPositionAsync(timeoutMilliseconds: 20000);

			await vm.SetLocation(position);
		}

		public async void OnClicked(object o, EventArgs e)
		{
			string city = City.Text;
			string state = State.Text;
			string url = String.Format("http://api.wunderground.com/api/28af05d873029695/forecast10day/q/{0}/{1}.json",
			                          state,
			                          city);
			
			await vm.GetWeatherAsync(url);
		}
	}
}
