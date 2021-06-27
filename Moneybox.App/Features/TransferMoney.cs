using Moneybox.App.DataAccess;
using System;

namespace Moneybox.App.Features
{
    public class TransferMoney
    {
        private readonly IAccountRepository _accountRepository;

        public TransferMoney(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public void Execute(Guid fromAccountId, Guid toAccountId, decimal amount)
        {
            var from = _accountRepository.GetAccountById(fromAccountId);
            var to = _accountRepository.GetAccountById(toAccountId);

            from.Withdraw(amount);
            to.PayIn(amount);

            _accountRepository.Update(from);
            _accountRepository.Update(to);
        }
    }
}
