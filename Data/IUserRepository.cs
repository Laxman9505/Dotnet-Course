using DOTNETAPI.Models;

namespace DOTNETAPI.Data
{
    public interface IUserRepository
    {
        public bool SaveChanges();
        public void AddEntity<T>(T entityToAdd);
        public void RemoveEntity<T>(T entityToDelete);

        public IEnumerable<User> GetUsers();

        public User GetUser(int userId);
    }
}