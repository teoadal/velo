using System;
using AutoFixture.Xunit2;
using Velo.Logging;
using Velo.Logging.Provider;
using Velo.TestsModels.Boos;
using Xunit;

namespace Velo.Tests.Logging.Service
{
    public class NullProviderShould : TestClass
    {
        private readonly NullLogProvider _provider;
        
        public NullProviderShould()
        {
            _provider = new NullLogProvider();
        }

        [Theory]
        [AutoData]
        public void CallWithoutResults(LogLevel level, Type sender, string template, int arg1, string arg2, Guid arg3, Boo arg4)
        {
            _provider.Write(level, sender, template);
            _provider.Write(level, sender, template, arg1);
            _provider.Write(level, sender, template, arg1, arg2);
            _provider.Write(level, sender, template, arg1, arg2, arg3);
            _provider.Write(level, sender, template, arg1, arg2, arg3, arg4);
        }
    }
}