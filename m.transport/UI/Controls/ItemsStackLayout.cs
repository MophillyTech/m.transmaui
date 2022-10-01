using System.Collections;
using System.Collections.Generic;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public class ItemsStackLayout : StackLayout
	{
		public static readonly BindableProperty ItemsSourceProperty =
			BindableProperty.Create<ItemsStackLayout, IEnumerable>(
				p => p.ItemsSource, new List<object>(), BindingMode.OneWay, null, ItemsSourcePropertyChanged, ItemsSourcePropertyChanging, ItemsSourceCoerce, ItemsSourceCreateDefaultValue);

		private static IEnumerable ItemsSourceCreateDefaultValue(ItemsStackLayout bindable)
		{
			return new List<object>();
		}

		private static IEnumerable ItemsSourceCoerce(BindableObject bindable, IEnumerable value)
		{
			return value;
		}

		private static void ItemsSourcePropertyChanging(BindableObject bindable, IEnumerable oldvalue, IEnumerable newvalue)
		{
			//int i = 17;
		}

		private static void ItemsSourcePropertyChanged(BindableObject bindable, IEnumerable oldvalue, IEnumerable newvalue)
		{
			var self = (ItemsStackLayout)bindable;
			self.Children.Clear();
			foreach (var item in newvalue)
			{
				var instance = self.ItemTemplate.CreateContent() as View;
				if (instance == null)
					continue;
				instance.BindingContext = item;
				self.Children.Add(instance);
			}
		}

		public IEnumerable ItemsSource
		{
			get { return (IEnumerable)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}

		public static readonly BindableProperty ItemTemplateProperty =
			BindableProperty.Create<ItemsStackLayout, DataTemplate>(
				p => p.ItemTemplate, null);

		public DataTemplate ItemTemplate
		{
			get { return (DataTemplate)GetValue(ItemTemplateProperty); }
			set { SetValue(ItemTemplateProperty, value); }
		}

		public static readonly BindableProperty TitleProperty =
			BindableProperty.Create<ItemsStackLayout, string>(p => p.Title, "N/A");

		public string Title
		{
			get { return (string)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}

	}
}
