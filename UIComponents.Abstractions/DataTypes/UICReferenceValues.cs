
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using UIComponents.Abstractions.Helpers;

namespace UIComponents.Abstractions.DataTypes;

/// <summary>
/// This acts as a collection of properties from another model.
/// </summary>
public class UICReferenceValues
{
    protected Dictionary<string, object> _propertyValues = new();

    public IReadOnlyDictionary<string, object> PropertyValues => new ReadOnlyDictionary<string, object>(_propertyValues);

    public UICReferenceValues AssignProperties(params string[] properties)
    {
        foreach(var property in properties)
        {
            if(!_propertyValues.ContainsKey(property))
                _propertyValues[property] = null;
        }
        return this;
    }

    public object GetPropertyValue(string propertyName)
    {
        return _propertyValues[propertyName];
    }
    public UICReferenceValues SetPropertyValue(string propertyName, object value)
    {
        _propertyValues[propertyName] = value;
        return this;
    }
    public virtual UICReferenceValues SetValueInReference(object sourceObject)
    {
        if (sourceObject == null)
            throw new ArgumentNullException();

        var objType = sourceObject.GetType();
        foreach(var property in  _propertyValues.Keys) 
        {
            var propertyInfo = objType.GetProperty(property);
            if (propertyInfo == null)
                continue;
            if(propertyInfo.CanRead)
                _propertyValues[property] = propertyInfo.GetValue(sourceObject);
        }
        return this;
    }

    public virtual UICReferenceValues SetValueInSource(ref object sourceObject)
    {
        if (sourceObject == null)
            throw new ArgumentNullException();

        var objType = sourceObject.GetType();
        foreach (var property in _propertyValues.Keys)
        {
            var propertyInfo = objType.GetProperty(property);
            if (propertyInfo == null)
                continue;

            var value = _propertyValues[property];
            propertyInfo.SetValue(sourceObject, value);
        }
        return this;
    }

    public override string ToString()
    {
        string toString = string.Empty;
        foreach(var property in _propertyValues.OrderBy(x => x.Key))
        {
            toString = string.Join("\r\n", toString, $"{property.Key}: {property.Value}");
        }
        return toString;
    }
}

public class UICReferenceValues<T> : UICReferenceValues where T : class
{

    public UICReferenceValues<T> AssignProperties(params Expression<Func<T, object>>[] expressions)
    {
        foreach(var expression in expressions)
        {
            var propertyInfo = InternalHelper.GetPropertyInfoFromExpression(expression);
            if (!_propertyValues.ContainsKey(propertyInfo.Name))
                _propertyValues[propertyInfo.Name] = null;
        }
        return this;
    }
    public TValue GetPropertyValue<TValue>(Expression<Func<T, TValue>> expression)
    {
        var propertyInfo = InternalHelper.GetPropertyInfoFromExpression(expression);
        var value = GetPropertyValue(propertyInfo.Name);
        try
        {
            return (TValue) value;
        }
        catch(Exception ex)
        {
            if (value == null)
                throw new Exception($"Value was not yet assigned. (Return Null)", ex);
            throw new Exception($"Failed to parse {value.GetType()} to {typeof(TValue)}", ex);
        }
    }
    public UICReferenceValues<T> SetPropertyValue<TValue>(Expression<Func<T, TValue>> expression, TValue value)
    {
        var propertyInfo = InternalHelper.GetPropertyInfoFromExpression(expression);
        SetPropertyValue(propertyInfo.Name, value);
        return this;
    }


    public virtual UICReferenceValues<T> SetValueInReference(T sourceObject)
    {
        SetValueInReference((object)sourceObject);
        return this;
    }

    public override string ToString()
    {
        string tostring = $"Referencing {typeof(T).FullName}";
        tostring = string.Join("\r\n", tostring, base.ToString());
        return tostring;
    }
}