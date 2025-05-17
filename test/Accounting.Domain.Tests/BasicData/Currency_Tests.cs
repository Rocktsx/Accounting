using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Modularity;
using Xunit;

namespace Accounting.BasicData
{
    public abstract class Currency_Tests
    {
        [Fact]
        public void ShouldThrowException_WhenTheSourceAndTargetCurrencyIsSame()
        {
            // Arrange
            string sourceCurrency = "USD";
            string targetCurrency = "USD";
            decimal sourceAmount = 100;
            decimal targetAmount = 80;
            decimal exchangeRate = 1.25m;
            DateOnly effectiveDate = DateOnly.FromDateTime(DateTime.Now);
            bool isActive = true;
            // Act
            var exception = Assert.Throws<BusinessException>(() =>
                new Currency(sourceCurrency, targetCurrency, sourceAmount, targetAmount, exchangeRate, effectiveDate, isActive));
            //Assert
            exception.Code.ShouldBe(AccountingDomainErrorCodes.SourceAndTargetCurrencyMustNotBeSame);
        }
        [Theory]
        [InlineData(0, 100, 1.25)]
        [InlineData(100, 0, 1.25)]
        [InlineData(100, 100, 0)]
        public void ShouldThrowException_WhenAmountOrRateIsZero(decimal sourceAmount, decimal targetAmount, decimal rate)
        {   
            // Arrange
            string sourceCurrency = "USD";
            string targetCurrency = "EUR";
            DateOnly effectiveDate = DateOnly.FromDateTime(DateTime.Now);
            bool isActive = true;
            // Act
            var exception = Assert.Throws<BusinessException>(() =>
                new Currency(sourceCurrency, targetCurrency, sourceAmount, targetAmount, rate, effectiveDate, isActive));
            //Assert
            exception.Code.ShouldBe(AccountingDomainErrorCodes.AmountsMustNotBeZero);
        }
        [Fact]
        public  void Should_Create_An_Valid_Currency()
        {
            // Arrange
            string sourceCurrency = "CNY";
            string targetCurrency = "USD";
            decimal sourceAmount = 720;
            decimal targetAmount = 100;
            decimal exchangeRate = 7.2m;
            DateOnly effectiveDate = DateOnly.FromDateTime(DateTime.Now);
            bool isActive = true;

            // Act
            var currency = new Currency(sourceCurrency, targetCurrency, sourceAmount, targetAmount, exchangeRate, effectiveDate, isActive);

            //Assert
            currency.SourceCurrency.ShouldBe(sourceCurrency);
            currency.TargetCurrency.ShouldBe(targetCurrency);
            currency.SourceAmount.ShouldBe(sourceAmount);
            currency.TargetAmount.ShouldBe(targetAmount);
            currency.ExchangeRate.ShouldBe(exchangeRate);
        }
    }
}
