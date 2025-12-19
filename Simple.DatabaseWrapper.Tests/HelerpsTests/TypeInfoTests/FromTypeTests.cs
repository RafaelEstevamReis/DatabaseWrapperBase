using System.ComponentModel.DataAnnotations;
using Simple.DatabaseWrapper.Attributes;
using Simple.DatabaseWrapper.TypeReader;
using Xunit;

namespace Simple.DatabaseWrapper.Tests.HelerpsTests.TypeReaderTests
{
    public class FromTypeTests
    {
        [Fact]
        public void FromType_BasicTestsT()
        {
            var tr = TypeInfo.FromType<TestClassProps>();

            Assert.Equal("TestClassProps", tr.TypeName);
            Assert.Equal(10, tr.Items.Length);
        }
        [Fact]
        public void FromType_BasicTestsType()
        {
            var tr = TypeInfo.FromType(typeof(TestClassProps));

            Assert.Equal("TestClassProps", tr.TypeName);
            Assert.Equal(10, tr.Items.Length);
        }

        [Fact]
        public void FromType_BasicTestsDecimal()
        {
            var tr = TypeInfo.FromType<decimal>();

            Assert.Equal("Decimal", tr.TypeName);
            Assert.Equal(6, tr.Items.Length); // NET8 changed the type
        }

        [Fact]
        public void FromType_ClassProps()
        {
            var tr = TypeInfo.FromType<TestClassProps>();
            Assert.Equal("TestClassProps", tr.TypeName);
            checkItems(tr.Items, ItemType.Property);
        }
        [Fact]
        public void FromType_StructProps()
        {
            var tr = TypeInfo.FromType<TestStructProps>();
            Assert.Equal("TestStructProps", tr.TypeName);
            checkItems(tr.Items, ItemType.Property);
        }

        [Fact]
        public void FromType_ClassFields()
        {
            var tr = TypeInfo.FromType<TestClassFields>();
            Assert.Equal("TestClassFields", tr.TypeName);
            checkItems(tr.Items, ItemType.Field);
        }
        [Fact]
        public void FromType_StructFields()
        {
            var tr = TypeInfo.FromType<TestStructFields>();
            Assert.Equal("TestStructFields", tr.TypeName);
            checkItems(tr.Items, ItemType.Field);
        }

        private void checkItems(TypeItemInfo[] items, ItemType itemType)
        {
            checkItem(items[0], itemType, "P1", "Int32", 1, ColumnAttributes.PrimaryKey);
            checkItem(items[1], itemType, "P2", "Int32", 1, ColumnAttributes.PrimaryKey);
            checkItem(items[2], itemType, "P3", "Int32", 1, ColumnAttributes.Unique);
            checkItem(items[3], itemType, "P4", "String", 1, ColumnAttributes.AllowNull);
            checkItem(items[4], itemType, "P5", "String", 1, ColumnAttributes.NotNull);
            checkItem(items[5], itemType, "P6", "Int32", 1, ColumnAttributes.DefaultValue);
            checkItem(items[6], itemType, "P7", "Int32", 0, ColumnAttributes.AllowNull);
            checkItem(items[7], itemType, "P8", "Int32", 0, ColumnAttributes.AllowNull);
        }
        private void checkItem(TypeItemInfo typeItemInfo, ItemType property, string itemName, string typeName, int attributeCount, ColumnAttributes attribute)
        {
            Assert.Equal(property, typeItemInfo.ItemType);
            Assert.Equal(itemName, typeItemInfo.Name);
            Assert.Equal(typeName, typeItemInfo.Type.Name);
            Assert.Equal(attributeCount, typeItemInfo.DBAttributes.Length);

            if (attributeCount > 0)
            {
                Assert.Equal(attribute, typeItemInfo.DBAttributes[0].ColumnAttributes);
            }
        }

        [Fact]
        public void FromType_AttributesCheck()
        {
            var tr = TypeInfo.FromType<TestAttributes>();
            Assert.Equal("TestAttributes", tr.TypeName);

            Assert.Equal(6, tr.Items.Length);

            Assert.Single(tr.Items[0].DBAttributes);
            Assert.Equal(ColumnAttributes.PrimaryKey, tr.Items[0].DBAttributes[0].ColumnAttributes);
            Assert.IsType<KeyAttribute>(tr.Items[0].DBAttributes[0].Attribute);

            Assert.Single(tr.Items[1].DBAttributes);
            Assert.Equal(ColumnAttributes.PrimaryKey, tr.Items[1].DBAttributes[0].ColumnAttributes);
            Assert.IsType<PrimaryKeyAttribute>(tr.Items[1].DBAttributes[0].Attribute);

            Assert.Single(tr.Items[2].DBAttributes);
            Assert.Equal(ColumnAttributes.Unique, tr.Items[2].DBAttributes[0].ColumnAttributes);
            Assert.IsType<UniqueAttribute>(tr.Items[2].DBAttributes[0].Attribute);

            Assert.Single(tr.Items[3].DBAttributes);
            Assert.Equal(ColumnAttributes.AllowNull, tr.Items[3].DBAttributes[0].ColumnAttributes);
            Assert.IsType<AllowNullAttribute>(tr.Items[3].DBAttributes[0].Attribute);

            Assert.Single(tr.Items[4].DBAttributes);
            Assert.Equal(ColumnAttributes.NotNull, tr.Items[4].DBAttributes[0].ColumnAttributes);
            Assert.IsType<NotNullAttribute>(tr.Items[4].DBAttributes[0].Attribute);

            Assert.Single(tr.Items[5].DBAttributes);
            Assert.Equal(ColumnAttributes.DefaultValue, tr.Items[5].DBAttributes[0].ColumnAttributes);
            Assert.IsType<DefaultValueAttribute>(tr.Items[5].DBAttributes[0].Attribute);

            Assert.Equal(15, (tr.Items[5].DBAttributes[0].Attribute as DefaultValueAttribute).DefaultValue);
        }


        [Fact]
        public void FromType_TestMultiIndex()
        {
            var tr = TypeInfo.FromType<TestMultiIndex>();
            Assert.Equal("TestMultiIndex", tr.TypeName);

            Assert.Equal(4, tr.Items.Length);

            Assert.Single(tr.Items[0].DBAttributes);
            Assert.Equal(ColumnAttributes.PrimaryKey, tr.Items[0].DBAttributes[0].ColumnAttributes);
            Assert.IsType<KeyAttribute>(tr.Items[0].DBAttributes[0].Attribute);

            Assert.Single(tr.Items[1].DBAttributes);
            Assert.Equal(ColumnAttributes.PrimaryKey, tr.Items[1].DBAttributes[0].ColumnAttributes);
            Assert.IsType<PrimaryKeyAttribute>(tr.Items[1].DBAttributes[0].Attribute);

            Assert.Equal(2, tr.Items[2].DBAttributes.Length);
            Assert.Equal(ColumnAttributes.Indexed, tr.Items[2].DBAttributes[0].ColumnAttributes);
            Assert.Equal(ColumnAttributes.Indexed, tr.Items[2].DBAttributes[1].ColumnAttributes);
            Assert.IsType<IndexAttribute>(tr.Items[2].DBAttributes[0].Attribute);

            Assert.Single(tr.Items[3].DBAttributes);
            Assert.Equal(ColumnAttributes.Indexed, tr.Items[3].DBAttributes[0].ColumnAttributes);
            Assert.IsType<IndexAttribute>(tr.Items[3].DBAttributes[0].Attribute);

        }


        [Fact]
        public void FromType_MyAttributesCheck()
        {
            var tr = TypeInfo.FromType<TestMyAttributes>();
            Assert.Equal("TestMyAttributes", tr.TypeName);

            Assert.Equal(5, tr.Items.Length);

            Assert.Single(tr.Items[0].DBAttributes);
            Assert.Equal(ColumnAttributes.PrimaryKey, tr.Items[0].DBAttributes[0].ColumnAttributes);
            Assert.IsType<MyPrimaryKeyAttribute>(tr.Items[0].DBAttributes[0].Attribute);

            Assert.Single(tr.Items[1].DBAttributes);
            Assert.Equal(ColumnAttributes.Unique, tr.Items[1].DBAttributes[0].ColumnAttributes);
            Assert.IsType<MyUniqueAttribute>(tr.Items[1].DBAttributes[0].Attribute);

            Assert.Single(tr.Items[2].DBAttributes);
            Assert.Equal(ColumnAttributes.AllowNull, tr.Items[2].DBAttributes[0].ColumnAttributes);
            Assert.IsType<MyAllowNullAttribute>(tr.Items[2].DBAttributes[0].Attribute);

            Assert.Single(tr.Items[3].DBAttributes);
            Assert.Equal(ColumnAttributes.NotNull, tr.Items[3].DBAttributes[0].ColumnAttributes);
            Assert.IsType<MyNotNullAttribute>(tr.Items[3].DBAttributes[0].Attribute);

            Assert.Single(tr.Items[4].DBAttributes);
            Assert.Equal(ColumnAttributes.DefaultValue, tr.Items[4].DBAttributes[0].ColumnAttributes);
            Assert.IsType<MyDefaultValueAttribute>(tr.Items[4].DBAttributes[0].Attribute);

            Assert.Equal(25, (tr.Items[4].DBAttributes[0].Attribute as MyDefaultValueAttribute).DefaultValue);
        }

    }
}
