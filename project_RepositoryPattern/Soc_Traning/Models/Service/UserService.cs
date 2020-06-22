using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Soc_Traning.Models.Service
{
    public class UserService : IRepository<UserModel, int>
    {
        private readonly APPContext _appContext;
        public UserService(APPContext aPPContext)
        {
            _appContext = aPPContext;
        }

        public int Create(UserModel entity)
        {
            _appContext.Users.Add(entity);
            _appContext.SaveChanges();
            return entity.Id;
        }

        public void Delete(int id)
        {
            _appContext.Users.Remove(_appContext.Users.Single(x => x.Id == id));
            _appContext.SaveChanges();
        }

        public IEnumerable<UserModel> Find(Expression<Func<UserModel, bool>> expression)
        {
            return _appContext.Users.Where(expression);
        }

        public IEnumerable<UserModel> FindAll()
        {
            return _appContext.Users.ToList();
        }

        public UserModel FindById(int id)
        {
            return _appContext.Users.SingleOrDefault(x => x.Id == id);
        }

        public void Update(UserModel entity)
        {
            var oriUser = _appContext.Users.Single(x => x.Id == entity.Id);
            _appContext.Entry(oriUser).CurrentValues.SetValues(entity);
            _appContext.SaveChanges();
        }
    }
}
