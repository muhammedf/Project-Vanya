using Vanya.Core.Model;
using Vanya.PublicApi.ServiceContracts;

namespace Vanya.Application.Services;

internal class MockInstrumentService : IInstrumentService
{
    List<Instrument> mockInstruments = new()
    {
        new Instrument(){ Id = 1, Name = "VanyaCoin"},
        new Instrument(){ Id = 2, Name = "DogeCoin"},
        new Instrument(){ Id = 3, Name = "BitCoin"},
    };

    public List<Instrument> GetActiveInstruments()
    {
        return mockInstruments.ToList();
    }
}
