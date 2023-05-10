using EWallet.Domain.Models;

namespace EWallet.Domain.Data;

public interface IUserRepository
{
    Task<dynamic> GetUserByIdAsync(string id);
}