using BookieDookie.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using BookieDookie.Models;

namespace BookieDookie.Repositories;

public class UserRepo(DbContext context) : IUserRepo
{
    public void Create(User user) => context.Set<User>().Add(user);
    public void Update(User user) => context.Set<User>().Update(user);
    public void Remove(User user) => context.Set<User>().Remove(user);
    public IQueryable GetQueryable() => context.Set<User>();
    public void Commit() => context.SaveChanges();
}