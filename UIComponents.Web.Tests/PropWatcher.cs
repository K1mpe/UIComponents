using System.Diagnostics;

namespace CeloxWortelen.DA.DataTypes;
public abstract class PropWatcherBase
{
    public abstract object ObjectValue { get; }

    public long LastGetTick { get; protected set; }
    public long LastUpdateTick { get; protected set; }

    public TimeSpan? LastGet => LastGetTick == 0 ? null : new(Stopwatch.GetTimestamp() - LastGetTick);
    public TimeSpan? LastUpdate => LastUpdateTick == 0 ? null : new(Stopwatch.GetTimestamp() - LastUpdateTick);

    public event EventHandler<PropChangedEventArgs> BeforeValueChanged;
    public event EventHandler<PropChangedEventArgs> AfterValueChanged;

    protected virtual void OnBeforeValueChanged(object oldValue, object newValue)
    {
        BeforeValueChanged?.Invoke(this, new PropChangedEventArgs(oldValue, newValue));
    }

    protected virtual void OnAfterValueChanged(object oldValue, object newValue)
    {
        AfterValueChanged?.Invoke(this, new PropChangedEventArgs(oldValue, newValue));
    }

    public virtual Type ValueType => null;
}

public class PropWatcher<T> : PropWatcherBase
{
    private T _value;

    public PropWatcher(T initialValue = default)
    {
        _value = initialValue;
    }

    public override Type ValueType => typeof(T);

    public T Value
    {
        get
        {
            LastGetTick = Stopwatch.GetTimestamp();
            return _value;
        }
        set
        {
            LastUpdateTick = Stopwatch.GetTimestamp();
            var oldValue = _value;

            if (EqualityComparer<T>.Default.Equals(oldValue, value))
                return;

            OnBeforeValueChanged(oldValue, value);
            _value = value;
            OnAfterValueChanged(oldValue, value);
        }
    }

    /// <summary>
    /// Gets the value as an object (boxed if T is a value type).
    /// Use this only when you really need object representation.
    /// </summary>
    public override object ObjectValue => Value;

    public event EventHandler<PropChangedEventArgs<T>> BeforeValueChangedTyped;
    public event EventHandler<PropChangedEventArgs<T>> AfterValueChangedTyped;

    protected override void OnBeforeValueChanged(object oldValue, object newValue)
    {
        base.OnBeforeValueChanged(oldValue, newValue);
        BeforeValueChangedTyped?.Invoke(this, new PropChangedEventArgs<T>((T)oldValue, (T)newValue));
    }

    protected override void OnAfterValueChanged(object oldValue, object newValue)
    {
        base.OnAfterValueChanged(oldValue, newValue);
        AfterValueChangedTyped?.Invoke(this, new PropChangedEventArgs<T>((T)oldValue, (T)newValue));
    }

    public static implicit operator T(PropWatcher<T> watcher) => watcher.Value;
}

public class PropChangedEventArgs : EventArgs
{
    public PropChangedEventArgs(object oldValue, object newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }

    public object OldValue { get; }
    public object NewValue { get; }

    public bool ValueChanged => !Equals(OldValue, NewValue);
}

public class PropChangedEventArgs<T> : PropChangedEventArgs
{
    public PropChangedEventArgs(T oldValue, T newValue) : base(oldValue, newValue)
    {
    }

    public new T OldValue => (T)base.OldValue;
    public new T NewValue => (T)base.NewValue;
}

