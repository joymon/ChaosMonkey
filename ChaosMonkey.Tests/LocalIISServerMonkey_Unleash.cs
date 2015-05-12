using ChaosMonkey.Domain;
using ChaosMonkey.Infrastructure;
using ChaosMonkey.Monkeys;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChaosMonkey.Tests
{
    [TestClass]
    public class LocalIISServerMonkey_Unleash
    {
        [TestMethod]
        public void IfCalledOnce_OneRandomActionShouldBePerformed()
        {

            Mock<ChaosLogger> mockLogger = new Mock<ChaosLogger>("");
            LocalIISServerMonkey monkey = new LocalIISServerMonkey(new Settings(), mockLogger.Object);
            int timesAppPoolRecycleOccured=0, timesIISResetOccured=0, timesAppPoolRestartOccured = 0;
            mockLogger.Setup(logger => logger.Log("Going to recycle random AppPool")).Callback(
                () => timesAppPoolRecycleOccured++);

            mockLogger.Setup(logger => logger.Log("Going to restart random AppPool")).Callback(
                () => timesAppPoolRestartOccured++);

            mockLogger.Setup(logger => logger.Log("Restarting local IIS")).Callback(
                () => timesIISResetOccured++
                );

            monkey.Unleash();

            Assert.IsTrue(timesIISResetOccured + timesAppPoolRecycleOccured + timesAppPoolRestartOccured == 1,
                "Either LocalIISServerMonkey didn't perform operation or performed multiple times");

        }
        /// <summary>
        /// Make sure the Unleash() logic is randomly picking operations.
        /// </summary>
        /// <remarks>Need to revisit as
        /// 1.This seems tricky test as its testing Random class in .Net framework. 
        /// 2.Not able to Mock ServerManager as its sealed. Need to write extra ILocalIISServerManager Layer. It takes more time.
        /// 3.There are chances of test failures.
        /// 4.Depends on Local IIS availability</remarks>
        [TestMethod]
        public void IfCalled10Times_ShouldAttemptDifferentLocalIISOperationsRandomly()
        {
            Mock<ChaosLogger> mockLogger = new Mock<ChaosLogger>("");

            LocalIISServerMonkey monkey = new LocalIISServerMonkey(new Settings(), mockLogger.Object);
            monkey.Unleash();
            monkey.Unleash();
            monkey.Unleash();
            monkey.Unleash();
            monkey.Unleash();
            monkey.Unleash();
            monkey.Unleash();
            monkey.Unleash();
            monkey.Unleash();
            monkey.Unleash();
            mockLogger.Verify(logger => logger.Log("Going to recycle random AppPool"), Times.AtLeastOnce());
            mockLogger.Verify(logger => logger.Log("Restarting local IIS"), Times.AtLeastOnce());
            mockLogger.Verify(logger => logger.Log("Going to restart random AppPool"), Times.AtLeastOnce());
        }
    }
}
