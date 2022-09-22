using TwinCAT.Ads;
using TwinCAT.TypeSystem;

namespace TwinCAT.JsonService.Interfaces;

public interface IClientService
{
    Task Connect(string amsNetId, int port);
    AdsClient Client { get; }
    IObservable<ConnectionState> ConnectionState { get; }
    IObservable<string> AdsState { get; }
    ISymbolCollection<ISymbol> TreeViewSymbols { get; }
    ISymbolCollection<ISymbol> FlatViewSymbols { get; }
    Task Reload();
    Task Disconnect();
}