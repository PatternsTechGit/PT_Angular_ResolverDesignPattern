using Entities;
using Entities.Responses;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Services.Contracts;

namespace Services
{
    public class AccountService : IAccountsService
    {
        private readonly BBBankContext _bbBankContext;
        public AccountService(BBBankContext BBBankContext)
        {
            _bbBankContext = BBBankContext;
        }

        public async Task<List<Account>> GetAllAccounts()
        {
            // totalCount of data available on server.
            return  _bbBankContext.Accounts
                .ToList();
        }
    }
}
