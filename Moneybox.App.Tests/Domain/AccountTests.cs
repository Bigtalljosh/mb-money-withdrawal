using Moneybox.App.Domain.Services;
using NSubstitute;
using System;
using Xunit;

namespace Moneybox.App.Tests.Domain
{
    public class AccountTests
    {
        private readonly User _fakeUser = new() { Email = "test@test" };

        [Fact]
        public void PayIn_WhenPaidInPlusAmountIsMoreThanLimit_ThrowsInvalidOperation()
        {
            var notification = Substitute.For<INotificationService>();
            var account = new Account(notification)
            {
                User = _fakeUser
            };

            account.PayIn(3900);

            Assert.Throws<InvalidOperationException>(() => account.PayIn(200));
            Assert.Equal(3900, account.Balance);
        }

        [Fact]
        public void PayIn_WhenPaidInPlusAmountIsCloseToLimit_SendsNotification()
        {
            var notification = Substitute.For<INotificationService>();
            var account = new Account(notification)
            {
                User = _fakeUser
            };

            account.PayIn(3400);

            account.PayIn(500);

            notification.Received(1).NotifyApproachingPayInLimit(Arg.Any<string>());
            Assert.Equal(3900, account.Balance);
        }

        [Fact]
        public void PayIn_WhenPaidInPlusAmountIsEqualToLimit_SendsNotification()
        {
            var notification = Substitute.For<INotificationService>();
            var account = new Account(notification)
            {
                User = _fakeUser
            };

            account.PayIn(3500);

            account.PayIn(500);

            notification.Received(1).NotifyApproachingPayInLimit(Arg.Any<string>());
            Assert.Equal(4000, account.Balance);
        }

        [Fact]
        public void PayIn_WhenWellBelowLimit_DoesNotSendNotification()
        {
            var notification = Substitute.For<INotificationService>();
            var account = new Account(notification)
            {
                User = _fakeUser
            };

            account.PayIn(100);

            account.PayIn(500);

            notification.DidNotReceive().NotifyApproachingPayInLimit(Arg.Any<string>());
            Assert.Equal(600, account.Balance);
        }

        [Fact]
        public void Withdraw_WhenBalanceIsLessThanAmount_ThrowsInvalidOperation()
        {
            var notification = Substitute.For<INotificationService>();
            var account = new Account(notification)
            {
                User = _fakeUser
            };

            account.PayIn(100);

            Assert.Throws<InvalidOperationException>(() => account.Withdraw(200));
            Assert.Equal(100, account.Balance);
        }

        [Fact]
        public void Withdraw_WhenBalanceIsSlightlyMoreThanAmount_SendsNotification()
        {
            var notification = Substitute.For<INotificationService>();
            var account = new Account(notification)
            {
                User = _fakeUser
            };

            account.PayIn(200);

            account.Withdraw(150);

            notification.Received(1).NotifyFundsLow(Arg.Any<string>());
            Assert.Equal(50, account.Balance);
        }

        [Fact]
        public void Withdraw_WhenBalanceIsWellAboveAmount_DoesNotSendNotification()
        {
            var notification = Substitute.For<INotificationService>();
            var account = new Account(notification)
            {
                User = _fakeUser
            };

            account.PayIn(1000);

            account.Withdraw(150);

            notification.DidNotReceive().NotifyFundsLow(Arg.Any<string>());
            Assert.Equal(850, account.Balance);
        }
    }
}
