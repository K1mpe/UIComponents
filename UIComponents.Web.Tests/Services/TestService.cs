using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIComponents.Web.Tests.Services
{
    public class TestService
    {
        public TestService()
        {
        }

        public event EventHandler<TestEventArgs> OnChange;
        public int CurrentCount { get; set; } = 0;


        public void IncreaseNumberByOne()
        {
            
            CurrentCount++;
            OnChange?.Invoke(this, new TestEventArgs() { Count= CurrentCount});
        }
    }
    public class TestEventArgs : EventArgs
    {
        public int Count { get; set; }  
    }

    public static class TestExtensions
    {
        public static void GetClientEvent(this EventHandler handler)
        {

        }
    }
}
