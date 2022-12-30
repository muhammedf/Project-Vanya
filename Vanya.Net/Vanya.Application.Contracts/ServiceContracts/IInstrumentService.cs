using Vanya.Core.Model;

namespace Vanya.Application.Contracts.ServiceContracts;

public interface IInstrumentService
{
    List<Instrument> GetActiveInstruments();
}
