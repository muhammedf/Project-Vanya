using Vanya.Core.Model;

namespace Vanya.PublicApi.ServiceContracts;

public interface IInstrumentService
{
    List<Instrument> GetActiveInstruments();
}
