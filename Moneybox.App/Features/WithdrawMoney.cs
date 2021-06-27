using Moneybox.App.DataAccess;
using System;

namespace Moneybox.App.Features
{
    public class WithdrawMoney
    {
        private readonly IAccountRepository _accountRepository;

        public WithdrawMoney(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public void Execute(Guid fromAccountId, decimal amount)
        {
            var account = _accountRepository.GetAccountById(fromAccountId);

            account.Withdraw(amount);
            _accountRepository.Update(account);
        }
    }
}
