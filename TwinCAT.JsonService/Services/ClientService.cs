﻿using System.Net;
using System.Net.Sockets;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using TwinCAT.Ads;
using TwinCAT.Ads.TypeSystem;
using TwinCAT.JsonService.Extensions;
using TwinCAT.JsonService.Interfaces;
using TwinCAT.TypeSystem;

namespace TwinCAT.JsonService.Services
{
    public class ClientService : IClientService, IDisposable
    {
        private readonly ILogger<ClientService> logger;
        private readonly BehaviorSubject<ConnectionState> connectionStateSubject = new BehaviorSubject<ConnectionState>(TwinCAT.ConnectionState.None);
        private readonly CompositeDisposable disposables = new CompositeDisposable();
        private readonly BehaviorSubject<string> adsStateSubject = new BehaviorSubject<string>(TwinCAT.Ads.AdsState.Idle.ToString());
        public ClientService(ILogger<ClientService> logger)
        {
            this.logger = logger;
            Client = new AdsClient();
        }

        public bool ConnectionStarted { get; set; }

        public string CurrentAmsNetId { get; set; }
        public int CurrentPort { get; set; }
        
        public Task Connect(string amsNetId, int port)
        {
            CurrentPort = port;
            CurrentAmsNetId = amsNetId;
            if (!Client.IsConnected)
            {
                if (string.IsNullOrEmpty(CurrentAmsNetId))
                {
                    Client.Connect(port);
                }
                else
                {
                    Client.Connect(amsNetId, port);
                }
            }

            ConnectionStarted = true;
            return Task.FromResult(Unit.Default);
        }

        public AdsClient Client { get; }
        public IObservable<ConnectionState> ConnectionState => connectionStateSubject.AsObservable();
        public IObservable<string> AdsState => adsStateSubject.AsObservable();
        public ISymbolCollection<ISymbol> TreeViewSymbols { get; set; }
        public ISymbolCollection<ISymbol> FlatViewSymbols { get; set; }
        public Task Reload()
        {
            return Task.Run(() => UpdateSymbols(connectionStateSubject.Value));
        }

        public Task Disconnect()
        {
            Client.Disconnect();
            ConnectionStarted = false;
            adsStateSubject.OnNext(TwinCAT.Ads.AdsState.Idle.ToString());
            return Task.FromResult(Unit.Default);
        }

        public void Initialize()
        {
            Observable.FromEventPattern<ConnectionStateChangedEventArgs>(ev => Client.ConnectionStateChanged += ev,
                    ev => Client.ConnectionStateChanged -= ev)
                .Select(pattern => pattern.EventArgs.NewState)
                .Subscribe(connectionStateSubject.OnNext)
                .AddDisposableTo(disposables);
            
            connectionStateSubject
                .DistinctUntilChanged()
                .Where(state => state == TwinCAT.ConnectionState.Connected)
                .Do(UpdateSymbols)
                .Subscribe()
                .AddDisposableTo(disposables);
  
            Observable.Interval(TimeSpan.FromSeconds(1))
                .Do(_ => CheckConnectionHealth())
                .Subscribe()
                .AddDisposableTo(disposables);
            
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

        }

        private void UpdateSymbols(ConnectionState state)
        {
            if (state == TwinCAT.ConnectionState.Connected)
            {
                var loader = SymbolLoaderFactory.Create(Client, new SymbolLoaderSettings(SymbolsLoadMode.VirtualTree));
                TreeViewSymbols = loader.Symbols;

                var loader2 = SymbolLoaderFactory.Create(Client, new SymbolLoaderSettings(SymbolsLoadMode.Flat));
                FlatViewSymbols = loader2.Symbols;
            }
            else
            {
                TreeViewSymbols = null;
            }
        }

        private void CheckConnectionHealth()
        {
            try
            {
                if (ConnectionStarted)
                {
                    if (!Client.IsConnected)
                    {
                        Client.Connect(CurrentAmsNetId, CurrentPort);
                    }
                    else
                        connectionStateSubject.OnNext(TwinCAT.ConnectionState.Connected);
                    
                    var state = Client.ReadState();
                    adsStateSubject.OnNext(state.AdsState.ToString());
                }
            }
            catch (AdsErrorException e)
            {
                adsStateSubject.OnNext(TwinCAT.Ads.AdsState.Invalid+" - "+e.ErrorCode);
                
                if (!Client.IsConnected)
                {
                    connectionStateSubject.OnNext(TwinCAT.ConnectionState.Lost);
                    Client.Disconnect();
                }
            }
        }

        public void Dispose()
        {
            Client.Disconnect();
            Client?.Dispose();
            disposables?.Dispose();
        }
    }
}