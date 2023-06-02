using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    internal interface ITokenAuth
    {
        Task<bool> validaToken();
    }
}
