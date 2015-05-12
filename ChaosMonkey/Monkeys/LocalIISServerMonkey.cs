using ChaosMonkey.Domain;
using ChaosMonkey.Infrastructure;
using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChaosMonkey.Monkeys
{
    /// <summary>
    /// IIS Monkey who randomly restart IIS servers / IIS AppPools in a deployment.
    /// </summary>
    /// <remarks>Move this class to different assembly if it needs extra dll references</remarks>
    public class LocalIISServerMonkey : ParentMonkey
    {
        Random _random = new Random(2);
        public LocalIISServerMonkey(Settings settings, ChaosLogger logger) : base(settings, logger)
        {

        }
        public override void Unleash()
        {
            ServerManager manager = new ServerManager();
            switch (GetRandomIISOperation())
            {
                case IISOperation.RecycleAppPool:
                    RecycleRandomAppPool(manager);
                    break;
                case IISOperation.RestartIIS:
                    RestartLocalIIS("localhost");
                    break;
                case IISOperation.RestartAppPool:
                    RestartRandomAppPool(manager);
                    break;
            }
        }

        private void RecycleRandomAppPool(ServerManager manager)
        {
            _logger.Log("Going to recycle random AppPool");
            int randonAppPoolIndex = _random.Next(manager.ApplicationPools.Count);
            ApplicationPool pool = manager.ApplicationPools[randonAppPoolIndex];
            _logger.Log(string.Format("Selected application pool {0} to recycle", pool.Name));
            pool.Recycle();
            _logger.Log(string.Format("Recycled application pool {0}", pool.Name));
        }

        private void RestartRandomAppPool(ServerManager manager)
        {
            _logger.Log("Going to restart random AppPool");
            int randonAppPoolIndex = _random.Next(manager.ApplicationPools.Count);
            ApplicationPool pool = manager.ApplicationPools[randonAppPoolIndex];
            _logger.Log(string.Format("Selected application pool {0} to restart", pool.Name));
            pool.Stop();
            _logger.Log(string.Format("Stopped application pool {0}", pool.Name));
            pool.Start();
            _logger.Log(string.Format("Started application pool {0}", pool.Name));
        }

        private IISOperation GetRandomIISOperation()
        {
            int randomIISOperation = _random.Next(3);
            return (IISOperation)randomIISOperation;
        }

        private void RestartLocalIIS(string serverName)
        {
            _logger.Log("Restarting local IIS");
            new CommandExecuter().ExecuteCommandSync("iisreset");
        }
        private enum IISOperation
        {
            RestartIIS,
            RestartAppPool,
            RecycleAppPool
        }
    }
}
