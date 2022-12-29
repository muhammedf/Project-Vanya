using WebApplication1.Core.Model;

namespace Vanya.PublicApi.Services
{
    public interface IInstrumentService
    {
        List<Instrument> GetActiveInstruments();
    }
}
