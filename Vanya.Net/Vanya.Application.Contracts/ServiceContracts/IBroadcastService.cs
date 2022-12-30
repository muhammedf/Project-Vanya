using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanya.Application.Contracts.ServiceContracts;

public interface IBroadcastService
{
    Task SendToGroup(string channel, string group, object data);
}
