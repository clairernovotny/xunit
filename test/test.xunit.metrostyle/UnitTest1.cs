using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace test.xunit.metrostyle
{
    public class MyTests
    {
        [Fact]
        public void AssertIsTrue()
        {
            Assert.True(true);
        }
        
        [Fact]
        public async Task AssertIsTrueAsync()
        {
            await Task.Delay(1000);
            Assert.True(true);
        }

        [Fact]
        public async Task AssertIsFalseAsync()
        {
            await Task.Delay(3000);
            Assert.False(true);
        }
    }
}
