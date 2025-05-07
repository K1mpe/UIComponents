namespace CeloxWortelen.DA.DataTypes;
public class PropWatcher
{
    private object _value;
    public object Value
    {
        get
        {
            LastGet = DateTime.Now;
            return _value;
        }
        set
        {
            LastUpdate = DateTime.Now;
            var oldValue = _value;
            BeforeValueChanged?.Invoke(this, new(oldValue, value));
            _value = value;
            AfterValueChanged?.Invoke(this, new(oldValue, value));
        }
    }
    public DateTime LastGet { get; set; }
    public DateTime LastUpdate { get; set; }

    public virtual Type ValueType { get; }

    public event EventHandler<PropChangedEventArgs> BeforeValueChanged;
    public event EventHandler<PropChangedEventArgs> AfterValueChanged;

}
public class PropChangedEventArgs : EventArgs
{
    public PropChangedEventArgs(object oldValue, object newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }

    public object OldValue { get; init; }
    public object NewValue { get; init; }

    public bool ValueChanged => !OldValue.Equals(NewValue);

}

public class PropChangedEventArgs<T> : PropChangedEventArgs
{
    public PropChangedEventArgs(object oldValue, object newValue) : base(oldValue, newValue)
    {
    }
    public new T OldValue => (T)base.OldValue;
    public new T NewValue => (T)base.NewValue;
}

public class PropWatcher<T> : PropWatcher
{
    public PropWatcher(T value = default(T))
    {
        Value = value;

        base.BeforeValueChanged += PropWatcher_BeforeValueChanged;
        base.AfterValueChanged += PropWatcher_AfterValueChanged;
    }


    public override Type ValueType => typeof(T);

    public new T Value { get => (T)base.Value; set => base.Value = value; }

    public new event EventHandler<PropChangedEventArgs<T>> BeforeValueChanged;
    public new event EventHandler<PropChangedEventArgs<T>> AfterValueChanged;

    public static implicit operator T(PropWatcher<T> watcher) => watcher.Value;

    private void PropWatcher_BeforeValueChanged(object sender, PropChangedEventArgs e)
    {
        BeforeValueChanged?.Invoke(sender, ConvertArgs(e));
    }
    private void PropWatcher_AfterValueChanged(object sender, PropChangedEventArgs e)
    {
        AfterValueChanged?.Invoke(sender, ConvertArgs(e));
    }
    private PropChangedEventArgs<T> ConvertArgs(PropChangedEventArgs args)
    {
        return new PropChangedEventArgs<T>((T)args.OldValue, (T)args.NewValue);
    }
}

