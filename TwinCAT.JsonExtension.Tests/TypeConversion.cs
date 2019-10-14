using System;
using Shouldly;
using TwinCAT.PlcOpen;
using Xunit;

namespace TwinCAT.JsonExtension.Tests
{
    public class TypeConversion
    {
        [Theory]
        [ClassData(typeof(CalculatorTestData))]
        public void TestConvertToDotNetManagedType(object obj)
        {
            obj.TryConvertToDotNetManagedType().ShouldBe(obj);
        }

        [Fact]
        public void ConvertDT()
        {
            var expected = new DateTime(2019,11,26,14,26,18);
            var obj = PlcOpenDTConverter.Create(expected);
            obj.TryConvertToDotNetManagedType().ShouldBe(expected);
        }

        [Fact]
        public void ConvertDate()
        {
            var expected = DateTime.Now.Date;
            var obj = PlcOpenDateConverter.Create(expected);
            obj.TryConvertToDotNetManagedType().ShouldBe(expected);
        }

        [Fact]
        public void ConvertTime()
        {
            var expected = TimeSpan.FromMilliseconds(638787);
            var obj = PlcOpenTimeConverter.CreateTime(expected);
            obj.TryConvertToDotNetManagedType().ShouldBe(expected);
        }

        [Fact]
        public void ConvertLTime()
        {
            var expected = TimeSpan.FromMilliseconds(832092);
            var obj = PlcOpenTimeConverter.CreateLTime(expected);
            obj.TryConvertToDotNetManagedType().ShouldBe(expected);
        }

        [Fact]
        public void ConvertDT2()
        {
            var expected = new DateTime(2019, 11, 27, 14, 26, 18);
            var obj = PlcOpenDTConverter.Create(expected);
            obj.TryConvertToDotNetManagedType().ShouldBe(expected);
        }

        [Fact]
        public void ConvertDate2()
        {
            var expected = DateTime.Now.Date-TimeSpan.FromDays(1);
            var obj = PlcOpenDateConverter.Create(expected);
            obj.TryConvertToDotNetManagedType().ShouldBe(expected);
        }

        [Fact]
        public void ConvertTime2()
        {
            var expected = TimeSpan.FromMilliseconds(738787);
            var obj = PlcOpenTimeConverter.CreateTime(expected);
            obj.TryConvertToDotNetManagedType().ShouldBe(expected);
        }

        [Fact]
        public void ConvertLTime2()
        {
            var expected = TimeSpan.FromMilliseconds(932092);
            var obj = PlcOpenTimeConverter.CreateLTime(expected);
            obj.TryConvertToDotNetManagedType().ShouldBe(expected);
        }
    }
}