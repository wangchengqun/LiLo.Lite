﻿//-----------------------------------------------------------------------
// <copyright file="SocketsService.cs" company="InternetWideWorld.com">
// Copyright (c) George Leithead, InternetWideWorld.  All rights reserved.
//   THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//   OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//   LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
//   FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>
//   Web Sockets Service interface.
// </summary>
//-----------------------------------------------------------------------

namespace LiLo.Lite.Services.Sockets
{
	using Lilo.Lite.Services;
	using LiLo.Lite.Services.Dialog;
	using LiLo.Lite.Services.Markets;
	using System;
	using System.ComponentModel;
	using System.Threading.Tasks;
	using WebSocketSharp;
	using Xamarin.Forms;

	/// <summary>Web Sockets Service interface.</summary>
	public class SocketsService : NotifyPropertyChangedBase, ISocketsService
	{
		/// <summary>Markets helper interface.</summary>
		private readonly IMarketsHelperService marketsHelper;

		/// <summary>Dialog service interface.</summary>
		private readonly IDialogService dialogService;

		/// <summary>Has the service been resumed.</summary>
		private bool isResumed;

		/// <summary>Web Socket.</summary>
		private WebSocket webSocket;

		/// <summary>Initialises a new instance of the <see cref="SocketsService"/> class.</summary>
		/// <param name="marketsHelperServiceConstructor">Markets helper service constructor.</param>
		public SocketsService(IMarketsHelperService marketsHelperServiceConstructor, IDialogService dialogServiceConstructor)
		{
			marketsHelper = marketsHelperServiceConstructor;
			dialogService = dialogServiceConstructor;
		}

		/// <summary>Raised when a public property of this object is set.</summary>
		public override event PropertyChangedEventHandler PropertyChanged
		{
			add { base.PropertyChanged += value; }
			remove { base.PropertyChanged -= value; }
		}

		/// <summary>Gets a value indicating whether the sockets service is connected.</summary>
		public bool IsConnected => webSocket.ReadyState == WebSocketState.Open;

		/// <summary>Initialises task for the sockets service.</summary>
		/// <returns>Task results of initialisation.</returns>
		public Task InitAsync()
		{
			marketsHelper.Init();
			Connect(true);

			return Task.FromResult(true);
		}

		public Task Connect(bool isInit = false)
		{
			if (!isInit)
			{
				marketsHelper.FeedsModel = DataStore.GetFeed();
			}

			Uri bybitWssUrl = marketsHelper.FeedsModel.Wss;
			webSocket = new WebSocket(bybitWssUrl.AbsoluteUri)
			{
				EmitOnPing = true
			};
			webSocket.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
			webSocket.SslConfiguration.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyError) =>
			{
				return true;
			};

			return Task.FromResult(true);
		}

		/// <summary>Handle when the application closes the sockets connection.</summary>
		/// <returns>Successful task</returns>
		public async Task WebSocket_Close()
		{
			await Task.Factory.StartNew(async () =>
			{
				if (webSocket == null)
				{
					await Task.FromResult(true);
					return;
				}

				if (webSocket.IsAlive)
				{
					webSocket.CloseAsync(CloseStatusCode.Normal);
				}

				await Task.FromResult(true);
			});
		}

		/// <summary>Handle when the application requests a sockets connection.</summary>
		/// <returns>Successful task.</returns>
		public async Task WebSocket_OnConnect()
		{
			await Task.Factory.StartNew(async () =>
			{
				if (Device.RuntimePlatform != Device.UWP)
				{
					webSocket.ConnectAsync();
				}
				else
				{
					webSocket.Connect();
				}

				await Task.FromResult(true);
			});
		}

		/// <summary>Handle when the application resumes from sleep.</summary>
		/// <returns>Successful task</returns>
		public async Task WebSocket_OnResume()
		{
			if (!isResumed)
			{
				webSocket.OnClose += WebSocket_OnClose;
				webSocket.OnError += WebSocket_OnError;
				webSocket.OnOpen += WebSocket_OnOpen;
				webSocket.OnMessage += WebSocket_OnMessage;
				webSocket.OnMessage += marketsHelper.WebSockets_OnMessageAsync;
				await WebSocket_OnConnect();
				isResumed = true;
			}

			await Task.FromResult(true);
		}

		/// <summary>Handle when the application goes into sleep.</summary>
		/// <returns>Successful task</returns>
		public async Task WebSocket_OnSleep()
		{
			if (isResumed)
			{
				webSocket.OnClose -= WebSocket_OnClose;
				webSocket.OnError -= WebSocket_OnError;
				webSocket.OnOpen -= WebSocket_OnOpen;
				webSocket.OnMessage -= marketsHelper.WebSockets_OnMessageAsync;
				webSocket.OnMessage -= WebSocket_OnMessage;
				webSocket.CloseAsync(CloseStatusCode.Normal);
				isResumed = false;
			}

			await Task.FromResult(true);
		}

		/// <summary>Handle when the sockets connection closes.</summary>
		/// <param name="sender">Sender object</param>
		/// <param name="e">Close event arguments</param>
		private async void WebSocket_OnClose(object sender, CloseEventArgs e)
		{
			await Task.Factory.StartNew(async () =>
			{
				while (!webSocket.IsAlive)
				{
					Task.Delay(1000).Wait();
					await WebSocket_OnConnect();
				}
			});
		}

		/// <summary>Handle when the sockets connection errors.</summary>
		/// <param name="sender">Sender object</param>
		/// <param name="e">Error event arguments</param>
		private void WebSocket_OnError(object sender, ErrorEventArgs e)
		{
			dialogService.ShowToastAsync(e.Message).ConfigureAwait(true);
			//			throw new Exception(e.Message, e.Exception);
		}

		/// <summary>Handle when the sockets connection receives a message.</summary>
		/// <param name="sender">Sender object</param>
		/// <param name="e">Message event arguments</param>
		private async void WebSocket_OnMessage(object sender, MessageEventArgs e)
		{
			await Task.Factory.StartNew(async () =>
			{
				if (e.IsText)
				{
					webSocket.OnMessage -= WebSocket_OnMessage;
				}

				await Task.FromResult(true);
			});
		}

		/// <summary>Handle when the sockets connection is opened.</summary>
		/// <param name="sender">Sender object</param>
		/// <param name="e">Event arguments</param>
		private async void WebSocket_OnOpen(object sender, EventArgs e)
		{
			await Task.Factory.StartNew(async () =>
			{
				webSocket.Send(marketsHelper.FeedsModel.Subscription);
				await Task.FromResult(true);
			});
		}
	}
}