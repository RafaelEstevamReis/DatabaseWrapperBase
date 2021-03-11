using System.ComponentModel.DataAnnotations;

namespace Simple.DatabaseWrapper.Tests.FrameworkCompatibilityTests
{
    public class KeyAttributeDefinedTests
    {
        [Key]
        public int prop { get; set; }
    }
}
