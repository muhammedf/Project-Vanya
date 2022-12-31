using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Vanya.Application.Contracts.ServiceContracts;
using Vanya.Application.Services;

namespace Vanya.Application;
public class Module : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<MockInstrumentService>().As<IInstrumentService>();
        builder.RegisterType<DefaultTradingService>().As<ITradingService>();
        builder.RegisterType<DefaultBroadcastService>().As<IBroadcastService>();
    }
}
