using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DataAccess.Data.Interface.IData
{
    public interface IApplicationUserRepository : IRepository<ApplicationUser>
    {

        // void Update(ApplicationUser data);

    }
}
