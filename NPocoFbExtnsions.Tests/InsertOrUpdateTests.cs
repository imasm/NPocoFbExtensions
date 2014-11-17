using System;
using NPoco.Tests.Common;
using NPocoFbExtensions;
using NUnit.Framework;

namespace NPocoFbExtnsions.Tests
{
    [TestFixture]
    public class InsertOrUpdateTests : BaseDBDecoratedTest
    {
        [Test]
        public void UpdateOrInsert()
        {
            const string dataName = "John Doe";
            const int dataAge = 56;
            const decimal dataSavings = (decimal)345.23;
            var dataDateOfBirth = DateTime.Now;

            const string secondName = "John Doe modified";
            const int secondAge = 60;
            const decimal secondSavings = (decimal)111.11;
            var secondDateOfBirth = DateTime.Now.AddDays(1);

            var poco = new UserDecorated();
            poco.UserId = 9999;
            poco.Name = dataName;
            poco.Age = dataAge;
            poco.Savings = dataSavings;
            poco.DateOfBirth = dataDateOfBirth;
            Database.UpdateOrInsert(poco);
          
            var verify = Database.SingleOrDefaultById<UserDecorated>(poco.UserId);
            Assert.IsNotNull(verify);

            Assert.AreEqual(poco.UserId, verify.UserId);
            Assert.AreEqual(dataName, verify.Name);
            Assert.AreEqual(dataAge, verify.Age);
            Assert.AreEqual(dataSavings, verify.Savings);

            poco.Name = secondName;
            poco.Age = secondAge;
            poco.Savings = secondSavings;
            poco.DateOfBirth = secondDateOfBirth;
            Database.UpdateOrInsert(poco);

            verify = Database.SingleOrDefaultById<UserDecorated>(poco.UserId);
            Assert.IsNotNull(verify);

            Assert.AreEqual(poco.UserId, verify.UserId);
            Assert.AreEqual(secondName, verify.Name);
            Assert.AreEqual(secondAge, verify.Age);
            Assert.AreEqual(secondSavings, verify.Savings);
        }

    }
}
