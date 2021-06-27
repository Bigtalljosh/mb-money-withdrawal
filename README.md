# Moneybox Money Withdrawal

The solution contains a .NET core library (Moneybox.App) which is structured into the following 3 folders:

* Domain - this contains the domain models for a user and an account, and a notification service.
* Features - this contains two operations, one which is implemented (transfer money) and another which isn't (withdraw money)
* DataAccess - this contains a repository for retrieving and saving an account (and the nested user it belongs to)

## The task

The task is to implement a money withdrawal in the WithdrawMoney.Execute(...) method in the features folder. For consistency, the logic should be the same as the TransferMoney.Execute(...) method i.e. notifications for low funds and exceptions where the operation is not possible.

As part of this process however, you should look to refactor some of the code in the TransferMoney.Execute(...) method into the domain models, and make these models less susceptible to misuse. We're looking to make our domain models rich in behaviour and much more than just plain old objects, however we don't want any data persistance operations (i.e. data access repositories) to bleed into our domain. This should simplify the task of implementing WithdrawMoney.Execute(...).

## Guidelines

* You should spend no more than 1 hour on this task, although there is no time limit
* You should fork or copy this repository into your own public repository (Github, BitBucket etc.) before you do your work
* Your solution must compile and run first time
* You should not alter the notification service or the the account repository interfaces
* You may add unit/integration tests using a test framework (and/or mocking framework) of your choice
* You may edit this README.md if you want to give more details around your work (e.g. why you have done something a particular way, or anything else you would look to do but didn't have time)

Once you have completed test, zip up your solution, excluding any build artifacts to reduce the size, and email it back to our recruitment team.

Good luck!

---

## Personal Notes

So what I've understood from this spec, I need to:

* Refactor the code in TransferMoney and move the generic logic around balance alerts and invalid operations into the domain models themselves
* Write code for a WithdrawMoney method
* It sounds like tests are a nice to have if there's time left. Given the 1 hour time constraint I'm not planning to tackle this TDD, however I will add some Unit Testing.

## Process

First up I'm going to handle the refactor before implementing the new code. It might seem a little backwards but in this case I think it makes sense.
I've noticed that moving the validation and account logic into the Account entity means they're not going to be "pure" functions, what I mean by this is that there *could* be side effects to what gets done here. For example if the balance is below 500 then we're going to send a notification, this is a side effect. So I'm going to extract the setters into methods and have a publically accessible getter. I think this providers a cleaner developer experience, it should be obvious that the Balance property is read only, but there are functions exposed to PayIn and Withdraw in order to update the balance.

Things to note with this though - EF won't like the auto properties in the entity now because there's no setter, will need some additional configuration to make this work.

Secondly, now I've refactored the code I can clean up the existing TransferMoney feature which makes it really slim, clean and easy to read and understand.

Thirdly, I can now easily implement the WithdrawMoney feature by simply calling the Withdraw function I wrote during the refactor and making sure to call update for the repository to save the change (were the repository implemented anyway).

Lastly, I'm writing a handful of tests to test the logic in the Account entity to prove the changes I've made have the intended effects.

I would continue to cover the features in tests but I'm slightly over the hour limit now and it feels a bit like cheating to carry on!

---

## Post-Submission thoughts

### Remove dependency in Account entity

I've been thinking how I could remove the dependency on `INotificationService` in the `Account` entity. I have an idea but it's a tradeof, a bit of code duplication but removes a dependency.
It works by returning a response from the methods in the Entity such as:

```csharp
public abstract record WithdrawResponse;
public record SuccessWithdrawResponse : WithdrawResponse;
public record SuccessLowFundsWithdrawResponse : WithdrawResponse;
public record FailInsufficientFundsWithdrawResponse : WithdrawResponse;
```

And then in the `Feature.Execute` we could do some pattern matching on the response and for example a `SuccessLowFundsWithdrawResponse` we could then send the notification to the customer about a low balance.

### Potential failure between saves and inconsistent state

Another concern I had earlier and should've documented is the fact none of this is in a transaction, so we could end up with a problem where we send a notification for low funds, but then fail on one of the `AccountRepository.Update` calls, or wore still the first `Update` is successful but the second isn't. In these cases we would end up in an inconsistent state where we took money from account A, but did not credit it in account B.
