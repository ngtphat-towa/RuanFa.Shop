using RuanFa.Shop.SharedKernel.Models;

namespace RuanFa.Shop.SharedKernel.UnitTests.Models
{
    public class ValueObjectTests
    {
        private class TestMoney : ValueObject
        {
            public int Amount { get; }
            public string Currency { get; }

            public TestMoney(int amount, string currency)
            {
                Amount = amount;
                Currency = currency ?? throw new ArgumentNullException(nameof(currency));
            }

            protected override IEnumerable<object?> GetEqualityComponents()
            {
                yield return Amount;  
                yield return Currency;
            }
        }


        private class TestAddress : ValueObject
        {
            public string Street { get; }
            public string City { get; }

            public TestAddress(string street, string city)
            {
                Street = street ?? throw new ArgumentNullException(nameof(street));
                City = city ?? throw new ArgumentNullException(nameof(city));
            }

            protected override IEnumerable<object?> GetEqualityComponents()
            {
                yield return Street;
                yield return City;
            }
        }

        private class TestVariableComponentVO : ValueObject
        {
            public List<object?> Components { get; }

            public TestVariableComponentVO(params object?[] components)
            {
                Components = new List<object?>(components);
            }

            protected override IEnumerable<object?> GetEqualityComponents()
            {
                return Components;
            }
        }

        private class ComparableComponent : IComparable
        {
            public int Value { get; }

            public ComparableComponent(int value) => Value = value;

            public int CompareTo(object? obj) =>
                obj is ComparableComponent other ?
                Value.CompareTo(other.Value) :
                throw new ArgumentException("Invalid type");
        }

        private class TestComparableComponentVO : ValueObject
        {
            public ComparableComponent Component { get; }

            public TestComparableComponentVO(ComparableComponent component)
            {
                Component = component ?? throw new ArgumentNullException(nameof(component));
            }

            protected override IEnumerable<object?> GetEqualityComponents()
            {
                yield return Component;
            }
        }

        [Fact]
        public void Equals_WhenComponentsAreSame_ReturnsTrue()
        {
            var money1 = new TestMoney(100, "USD");
            var money2 = new TestMoney(100, "USD");

            money1.Equals(money2).ShouldBeTrue();
            (money1 == money2).ShouldBeTrue();
            money1.GetHashCode().ShouldBe(money2.GetHashCode());
        }

        [Fact]
        public void Equals_WhenComponentsDiffer_ReturnsFalse()
        {
            var money1 = new TestMoney(100, "USD");
            var money2 = new TestMoney(200, "USD");

            money1.Equals(money2).ShouldBeFalse();
            (money1 != money2).ShouldBeTrue();
        }

        [Fact]
        public void Equals_WhenOneIsNull_ReturnsFalse()
        {
            var money = new TestMoney(100, "USD");
            TestMoney? nullMoney = null;

            money.Equals(nullMoney).ShouldBeFalse();
            (money == nullMoney).ShouldBeFalse();
            (nullMoney == money).ShouldBeFalse();
        }

        [Fact]
        public void Equals_WhenBothAreNull_ReturnsTrue()
        {
            TestMoney? nullMoney1 = null;
            TestMoney? nullMoney2 = null;

            (nullMoney1 == nullMoney2).ShouldBeTrue();
        }

        [Fact]
        public void Equals_WhenDifferentTypes_ReturnsFalse()
        {
            var money = new TestMoney(100, "USD");
            var address = new TestAddress("Street", "City");

            money.Equals(address).ShouldBeFalse();
            (money == address).ShouldBeFalse();
        }

        [Fact]
        public void CompareTo_WhenComponentsAreSame_ReturnsZero()
        {
            var money1 = new TestMoney(100, "USD");
            var money2 = new TestMoney(100, "USD");

            money1.CompareTo(money2).ShouldBe(0);
        }

        [Fact]
        public void CompareTo_WhenFirstComponentIsGreater_ReturnsPositive()
        {
            var money1 = new TestMoney(200, "USD");
            var money2 = new TestMoney(100, "USD");

            money1.CompareTo(money2).ShouldBeGreaterThan(0);
        }

        [Fact]
        public void CompareTo_WhenSecondComponentIsLess_ReturnsNegative()
        {
            var money1 = new TestMoney(100, "EUR");
            var money2 = new TestMoney(100, "USD");

            money1.CompareTo(money2).ShouldBeLessThan(0);
        }

        [Fact]
        public void CompareTo_WhenOtherIsNull_ReturnsPositive()
        {
            var money = new TestMoney(100, "USD");

            money.CompareTo(null).ShouldBe(1);
        }

        [Fact]
        public void CompareTo_WhenOtherHasMoreComponents_ReturnsNegative()
        {
            var vo1 = new TestVariableComponentVO(1, "A");
            var vo2 = new TestVariableComponentVO(1, "A", 3);

            vo1.CompareTo(vo2).ShouldBe(-1);
        }

        [Fact]
        public void CompareTo_WhenOtherHasFewerComponents_ReturnsPositive()
        {
            var vo1 = new TestVariableComponentVO(1, "A", 3);
            var vo2 = new TestVariableComponentVO(1, "A");

            vo1.CompareTo(vo2).ShouldBe(1);
        }

        [Fact]
        public void CompareTo_WhenComponentsAreDifferentTypesWithSameString_ReturnsZero()
        {
            var vo1 = new TestVariableComponentVO(5);
            var vo2 = new TestVariableComponentVO("5");

            vo1.CompareTo(vo2).ShouldBe(0);
        }

        [Fact]
        public void CompareTo_UsesIComparableOfComponents()
        {
            var vo1 = new TestComparableComponentVO(new ComparableComponent(10));
            var vo2 = new TestComparableComponentVO(new ComparableComponent(5));

            vo1.CompareTo(vo2).ShouldBeGreaterThan(0);
        }

        [Fact]
        public void CompareTo_WhenComponentIsNull_HandlesCorrectly()
        {
            object? nullValue = null;
            var voWithNull = new TestVariableComponentVO(nullValue);
            var voWithNonNull = new TestVariableComponentVO("something");

            voWithNull.CompareTo(voWithNonNull).ShouldBe(-1);
            voWithNonNull.CompareTo(voWithNull).ShouldBe(1);
            voWithNull.CompareTo(voWithNull).ShouldBe(0);
        }

        [Fact]
        public void ToString_ReturnsTypeNameAndComponents()
        {
            var money = new TestMoney(100, "USD");

            money.ToString().ShouldBe("TestMoney[100, USD]");
        }

        [Fact]
        public void Clone_ReturnsEqualObject()
        {
            var money = new TestMoney(100, "USD");
            var clone = money.Clone();

            clone.ShouldBeOfType<TestMoney>();
            clone.ShouldNotBeSameAs(money);
            clone.Equals(money).ShouldBeTrue();
        }

        [Fact]
        public void Equals_WhenComponentsOrderDiffer_ReturnsFalse()
        {
            var vo1 = new TestVariableComponentVO(100, "USD");
            var vo2 = new TestVariableComponentVO("USD", 100);

            vo1.Equals(vo2).ShouldBeFalse();
        }
    }
}
