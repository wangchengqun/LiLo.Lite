﻿//-----------------------------------------------------------------------
// <copyright file="TickPriceView.cs" company="InternetWideWorld.com">
// Copyright (c) George Leithead, InternetWideWorld.  All rights reserved.
//   THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//   OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//   LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
//   FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>
//   Custom ticker price view.
// </summary>
//-----------------------------------------------------------------------

namespace LiLo.Lite.Controls
{
	using LiLo.Lite.Extensions;
	using Xamarin.Forms;

	/// <summary>Custom ticker price view.</summary>
	public class TickPriceView : Label
	{
		/// <summary>Gets or sets the Default Background Colour Property, and it is a bindable property.</summary>
		public static readonly BindableProperty DefaultBackgroundColorProperty = BindableProperty.Create(nameof(DefaultBackgroundColor), typeof(Color), typeof(TickPriceView), Color.Default, BindingMode.Default, null, null);

		/// <summary>Gets or sets the Negative Tick Background Colour Property, and it is a bindable property.</summary>
		public static readonly BindableProperty NegativeTickBackgroundColorProperty = BindableProperty.Create(nameof(NegativeTickBackgroundColor), typeof(Color), typeof(TickPriceView), Color.Default, BindingMode.Default, null, null);

		/// <summary>Gets or sets the Negative Tick Colour Property, and it is a bindable property.</summary>
		public static readonly BindableProperty NegativeTickColorProperty = BindableProperty.Create(nameof(NegativeTickColor), typeof(Color), typeof(TickPriceView), Color.Default, BindingMode.Default, null, null);

		/// <summary>Gets or sets the Positive Tick Background Colour Property, and it is a bindable property.</summary>
		public static readonly BindableProperty PositiveTickBackgroundColorProperty = BindableProperty.Create(nameof(PositiveTickBackgroundColor), typeof(Color), typeof(TickPriceView), Color.Default, BindingMode.Default, null, null);

		/// <summary>Gets or sets the Positive Tick Colour Property, and it is a bindable property.</summary>
		public static readonly BindableProperty PositiveTickColorProperty = BindableProperty.Create(nameof(PositiveTickColor), typeof(Color), typeof(TickPriceView), Color.Default, BindingMode.Default, null, null);

		/// <summary>Gets or sets the Price Property, and it is a bindable property.</summary>
		public static readonly BindableProperty PriceProperty = BindableProperty.Create(nameof(Price), typeof(double), typeof(TickPriceView), 16d, BindingMode.Default, null, OnPricePropertyChanged);

		/// <summary>Gets or sets the Tick Property, and it is a bindable property.</summary>
		public static readonly BindableProperty TickProperty = BindableProperty.Create(nameof(Tick), typeof(string), typeof(TickPriceView), string.Empty, BindingMode.Default, null, OnTickPropertyChanged);

		/// <summary>Animation duration in milliseconds.</summary>
		private const int AnimationDuration = 1000;

		/// <summary>Gets or sets the default background colour.</summary>
		public Color DefaultBackgroundColor
		{
			get => (Color)GetValue(DefaultBackgroundColorProperty);
			set => SetValue(DefaultBackgroundColorProperty, value);
		}

		/// <summary>Gets or sets the negative tick background colour.</summary>
		public Color NegativeTickBackgroundColor
		{
			get => (Color)GetValue(NegativeTickBackgroundColorProperty);
			set => SetValue(NegativeTickBackgroundColorProperty, value);
		}

		/// <summary>Gets or sets the negative tick colour.</summary>
		public Color NegativeTickColor
		{
			get => (Color)GetValue(NegativeTickColorProperty);
			set => SetValue(NegativeTickColorProperty, value);
		}

		/// <summary>Gets or sets the positive tick background colour.</summary>
		public Color PositiveTickBackgroundColor
		{
			get => (Color)GetValue(PositiveTickBackgroundColorProperty);
			set => SetValue(PositiveTickBackgroundColorProperty, value);
		}

		/// <summary>Gets or sets the positive tick colour.</summary>
		public Color PositiveTickColor
		{
			get => (Color)GetValue(PositiveTickColorProperty);
			set => SetValue(PositiveTickColorProperty, value);
		}

		/// <summary>Gets or sets the Price.</summary>
		public double Price
		{
			get => (double)GetValue(PriceProperty);
			set => SetValue(PriceProperty, value);
		}

		/// <summary>Gets or sets the Tick.</summary>
		public string Tick
		{
			get => (string)GetValue(TickProperty);
			set => SetValue(TickProperty, value);
		}

		/// <summary>Invoked when the Price is changed.</summary>
		/// <param name="bindable">The TickPriceView object</param>
		/// <param name="oldValue">The old value</param>
		/// <param name="newValue">The new value</param>
		private static void OnPricePropertyChanged(BindableObject bindable, object oldValue, object newValue)
		{
			TickPriceView lastPriceView = bindable as TickPriceView;
			double newPrice = (double)newValue;
			double oldPrice = (double)oldValue;
			if (oldPrice == newPrice)
			{
				return;
			}

			lastPriceView.TextColor = newPrice < oldPrice ? lastPriceView.NegativeTickColor : lastPriceView.PositiveTickColor;
			if (string.IsNullOrEmpty(lastPriceView.Tick) || lastPriceView.Tick.Equals("X"))
			{
				// Handle where the price changes, but it not related to the tick (such as High/low 24h prices)
				Color defaultBackgroundColor = lastPriceView.DefaultBackgroundColor;
				if (oldPrice <= newPrice)
				{
					lastPriceView.ColorTo(lastPriceView.PositiveTickBackgroundColor, defaultBackgroundColor, l => lastPriceView.BackgroundColor = l, AnimationDuration);
				}
				else
				{
					lastPriceView.ColorTo(lastPriceView.NegativeTickBackgroundColor, defaultBackgroundColor, l => lastPriceView.BackgroundColor = l, AnimationDuration);
				}
			}
		}

		/// <summary>Invoked when the Tick is changed.</summary>
		/// <param name="bindable">The LastPriceView</param>
		/// <param name="oldValue">The old value</param>
		/// <param name="newValue">The new value</param>
		private static void OnTickPropertyChanged(BindableObject bindable, object oldValue, object newValue)
		{
			TickPriceView lastPriceView = bindable as TickPriceView;
			string newText = (string)newValue;
			Color defaultBackgroundColor = lastPriceView.DefaultBackgroundColor;

			if (!string.IsNullOrEmpty(newText))
			{
				switch (newText)
				{
					case "PlusTick":
						lastPriceView.TextColor = lastPriceView.PositiveTickColor;
						lastPriceView.ColorTo(lastPriceView.PositiveTickBackgroundColor, defaultBackgroundColor, l => lastPriceView.BackgroundColor = l, AnimationDuration);
						break;

					case "MinusTick":
						lastPriceView.TextColor = lastPriceView.NegativeTickColor;
						lastPriceView.ColorTo(lastPriceView.NegativeTickBackgroundColor, defaultBackgroundColor, l => lastPriceView.BackgroundColor = l, AnimationDuration);
						break;
						////                    default:
						////                        lastPriceView.TextColor = (Color)Application.Current.Resources["Gray-900"];
						////                        break;
				}
			}
		}
	}
}