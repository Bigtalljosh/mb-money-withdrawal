using Moneybox.App.Domain.Services;
using System;

namespace Moneybox.App
{
    public class Account
    {
        private readonly INotificationService _notificationService;

        public Account(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public const decimal PayInLimit = 4000m;

        public Guid Id { get; set; }

        public User User { get; set; }

        private decimal _balance;

        public decimal Balance
        {
            get => _balance;
            private set
            {
                if (value < 0)
                {
                    throw new InvalidOperationException("Insufficient funds");
                }

                _balance = value;
            }
        }

        private decimal _withdrawn;

        public decimal Withdrawn => _withdrawn;

        private decimal _paidIn;

        public decimal PaidIn => _paidIn;

        public void PayIn(decimal value)
        {
            var newPaidIn = _paidIn + value;
            if (newPaidIn > PayInLimit)
            {
                throw new InvalidOperationException("Account pay in limit reached");
            }

            if (PayInLimit - newPaidIn < 500m)
            {
                _notificationService.NotifyApproachingPayInLimit(User.Email);
            }

            Balance += value;
            _paidIn = newPaidIn;
        }

        public void Withdraw(decimal value)
        {
            Balance -= value;

            if (Balance < 500m)
            {
                _notificationService.NotifyFundsLow(User.Email);
            }

            _withdrawn -= value;
        }
    }
}
