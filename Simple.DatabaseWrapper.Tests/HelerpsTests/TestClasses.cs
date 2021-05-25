using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using Simple.DatabaseWrapper.Attributes;

namespace Simple.DatabaseWrapper.Tests.HelerpsTests
{
    public class TestClassProps
    {
        [Key]
        public int P1 { get; set; }
        [PrimaryKey]
        public int P2 { get; set; }
        [Unique]
        public int P3 { get; set; }
        [AllowNull]
        public string P4 { get; set; }
        [NotNull]
        public string P5 { get; set; }
        [DefaultValue(20)]
        public int P6 { get; set; }
        [XmlIgnore]
        public int P7 { get; set; }
        public int P8 { get; set; }
        public int PGet { get; }
        public int PSet { set { _ = value; } }
    }
    public class TestClassFields
    {
        [Key]
        public int P1;
        [PrimaryKey]
        public int P2;
        [Unique]
        public int P3;
        [AllowNull]
        public string P4;
        [NotNull]
        public string P5;
        [DefaultValue(20)]
        public int P6;
        [XmlIgnore]
        public int P7;
        public int P8;
    }
    public struct TestStructProps
    {
        [Key]
        public int P1 { get; set; }
        [PrimaryKey]
        public int P2 { get; set; }
        [Unique]
        public int P3 { get; set; }
        [AllowNull]
        public string P4 { get; set; }
        [NotNull]
        public string P5 { get; set; }
        [DefaultValue(20)]
        public int P6 { get; set; }
        [XmlIgnore]
        public int P7 { get; set; }
        public int P8 { get; set; }
    }
    public struct TestStructFields
    {
        [Key]
        public int P1;
        [PrimaryKey]
        public int P2;
        [Unique]
        public int P3;
        [AllowNull]
        public string P4;
        [NotNull]
        public string P5;
        [DefaultValue(20)]
        public int P6;
        [XmlIgnore]
        public int P7;
        public int P8;
    }

    public class TestAttributes
    {
        [Key]
        public int P1 { get; set; }
        [PrimaryKey]
        public int P2 { get; set; }
        [Unique]
        public int P3 { get; set; }
        [AllowNull]
        public string P4 { get; set; }
        [NotNull]
        public string P5 { get; set; }
        [DefaultValue(15)]
        public int P6 { get; set; }
    }

    public class TestMyAttributes
    {
        // Key is Sealed

        [MyPrimaryKey]
        public int P2 { get; set; }
        [MyUnique]
        public int P3 { get; set; }
        [MyAllowNull]
        public string P4 { get; set; }
        [MyNotNull]
        public string P5 { get; set; }
        [MyDefaultValue(25)]
        public int P6 { get; set; }
    }


    public class MyPrimaryKeyAttribute : PrimaryKeyAttribute { }
    public class MyUniqueAttribute : UniqueAttribute { }
    public class MyAllowNullAttribute : AllowNullAttribute { }
    public class MyNotNullAttribute : NotNullAttribute { }
    public class MyDefaultValueAttribute : DefaultValueAttribute
    {
        public MyDefaultValueAttribute(object defaultValue) : base(defaultValue) { }
    }
}
