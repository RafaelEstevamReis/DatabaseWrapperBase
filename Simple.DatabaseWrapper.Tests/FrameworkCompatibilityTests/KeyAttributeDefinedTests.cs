namespace Simple.DatabaseWrapper.Tests.FrameworkCompatibilityTests;

using System.ComponentModel.DataAnnotations;

public class KeyAttributeDefinedTests
{
    [Key]
    public int prop { get; set; }
}
