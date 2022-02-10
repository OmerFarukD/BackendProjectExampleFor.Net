using Core.Core;
using Core.Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Abstract
{
    public interface IUserDal : IEntityBaseRepository<User>
    {
        List<OperationClaim> GetClaims(User user);
    }
}
