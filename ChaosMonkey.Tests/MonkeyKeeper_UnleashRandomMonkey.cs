using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChaosMonkey.Domain;
using ChaosMonkey.Infrastructure;
using Moq;
using System.Collections.Generic;
using ChaosMonkey.Monkeys;

namespace ChaosMonkey.Tests
{
    [TestClass]
    public class MonkeyKeeper_UnleashRandomMonkey
    {
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void WhenMonkeyListProviderGivesEmptyList_ThrowException()
        {
            Settings settings = new Settings();
            ChaosLogger logger = new ChaosLogger("");
            var mock = new Mock<MonkeyListBuilder>();
            var mockMonkey = new Mock<ParentMonkey>();
            //mockMonkey.Verify(monkey => monkey.Unleash(),Times.Once);
            mock.Setup(builder => builder.GetMonkeys(settings, logger)).Returns(
                new List<ParentMonkey>() { }
                );
            MonkeyKeeper keeper = new MonkeyKeeper(
                settings,
                logger,
                mock.Object);
            keeper.UnleashRandomMonkey();
        }
        [TestMethod]
        public void WhenMonkeyListProviderGivenSingleMonkey_UseThatMonkey()
        {
            Settings settings = new Settings();
            ChaosLogger logger = new ChaosLogger("");
            var mock = new Mock<MonkeyListBuilder>();
            var mockMonkey = new Mock<ParentMonkey>(settings, logger);
            mock.Setup(builder => builder.GetMonkeys(settings, logger))
                    .Returns(new List<ParentMonkey>() { mockMonkey.Object });
            MonkeyKeeper keeper = new MonkeyKeeper(
                settings,
                logger,
                mock.Object);
            keeper.UnleashRandomMonkey();
            mockMonkey.Verify(monkey => monkey.Unleash(), Times.Exactly(1));
        }
        [TestMethod]
        public void WhenMonkeyListProviderGivesMultipleMonkeysAndUnleashMultipleTimes_UseThoseMonkeysAtleastOnce()
        {
            Settings settings = new Settings();
            ChaosLogger logger = new ChaosLogger("");
        
            var mockMonkey1 = new Mock<ParentMonkey>(settings, logger);
            var mockMonkey2 = new Mock<ParentMonkey>(settings, logger);
            var mockMonkey3 = new Mock<ParentMonkey>(settings, logger);

            var mockMonkeyListBuilder = new Mock<MonkeyListBuilder>();

            mockMonkeyListBuilder.Setup(builder => builder.GetMonkeys(settings, logger))
                    .Returns(new List<ParentMonkey>() {
                        mockMonkey1.Object,
                        mockMonkey2.Object,
                        mockMonkey3.Object
                    });

            MonkeyKeeper keeper = new MonkeyKeeper(
                settings,
                logger,
                mockMonkeyListBuilder.Object);

            keeper.UnleashRandomMonkey();
            keeper.UnleashRandomMonkey();
            keeper.UnleashRandomMonkey();
            keeper.UnleashRandomMonkey();
            keeper.UnleashRandomMonkey();

            mockMonkey1.Verify(monkey => monkey.Unleash(), Times.AtLeastOnce);
            mockMonkey2.Verify(monkey => monkey.Unleash(), Times.AtLeastOnce);
            mockMonkey3.Verify(monkey => monkey.Unleash(), Times.AtLeastOnce);

        }

    }
}
