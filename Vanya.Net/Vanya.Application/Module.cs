using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Vanya.Application.Services;
using Vanya.PublicApi.ServiceContracts;

namespace Vanya.Application;
public class Module : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<MockInstrumentService>().As<IInstrumentService>();
        builder.RegisterType<DefaultTradingService>().As<ITradingService>();
    }
}
