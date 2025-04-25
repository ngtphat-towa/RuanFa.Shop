namespace RuanFa.Shop.SharedKernel.Models;

public abstract class ValueObject : IEquatable<ValueObject>, IComparable<ValueObject>
{
    protected static bool EqualOperator(ValueObject? left, ValueObject? right)
    {
        if (left is null ^ right is null)
        {
            return false;
        }

        return left?.Equals(right!) != false;
    }

    protected static bool NotEqualOperator(ValueObject? left, ValueObject? right)
    {
        return !(EqualOperator(left, right));
    }

    protected abstract IEnumerable<object?> GetEqualityComponents();

    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        return EqualOperator(left, right);
    }

    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        return NotEqualOperator(left, right);
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (ValueObject)obj;
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public virtual bool Equals(ValueObject? other)
    {
        return Equals((object?)other);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();

        foreach (var component in GetEqualityComponents())
        {
            hash.Add(component);
        }

        return hash.ToHashCode();
    }

    public virtual int CompareTo(ValueObject? other)
    {
        if (other is null) return 1;
        if (ReferenceEquals(this, other)) return 0;

        var components = GetEqualityComponents().ToArray();
        var otherComponents = other.GetEqualityComponents().ToArray();

        for (var i = 0; i < components.Length; i++)
        {
            if (i >= otherComponents.Length) return 1;

            var comparison = CompareComponents(components[i], otherComponents[i]);
            if (comparison != 0) return comparison;
        }

        return components.Length.CompareTo(otherComponents.Length);
    }

    private static int CompareComponents(object? object1, object? object2)
    {
        if (object1 is null && object2 is null) return 0;
        if (object1 is null) return -1;
        if (object2 is null) return 1;

        if (object1 is IComparable comparable1 && object2.GetType() == object1.GetType())
        {
            return comparable1.CompareTo(object2);
        }

        return object1.ToString()?.CompareTo(object2.ToString()) ?? -1;
    }

    public override string ToString()
    {
        return $"{GetType().Name}[{string.Join(", ", GetEqualityComponents())}]";
    }

    public virtual ValueObject Clone()
    {
        return (ValueObject)MemberwiseClone();
    }
}
