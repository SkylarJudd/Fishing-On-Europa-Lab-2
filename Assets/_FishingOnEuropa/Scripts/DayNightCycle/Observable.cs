using System;
using System.Collections.Generic;

/// <summary>
/// A generic class that encapsulates a value and provides notifications when the value changes.
/// </summary>
/// <typeparam name="T">The type of the value being observed.</typeparam>
[Serializable]
public class Observable<T>
{
    private T value;
    /// <summary>
    /// Event triggered when the value changes.
    /// </summary>
    public event Action<T> ValueChanged;

    /// <summary>
    /// Gets or sets the value. Triggers the <see cref="ValueChanged"/> event when the value is set.
    /// </summary>
    public T Value
    {
        get => value;
        set => Set(value);
    }

    /// <summary>
    /// Implicit conversion from <see cref="Observable{T}"/> to the underlying value type.
    /// </summary>
    /// <param name="observable">The observable instance.</param>
    public static implicit operator T(Observable<T> observable) => observable.value;


    /// <summary>
    /// Overloads the += operator to add a listener to the <see cref="ValueChanged"/> event.
    /// </summary>
    /// <param name="observable">The observable instance.</param>
    /// <param name="handler">The action to be called when the value changes.</param>
    /// <returns>The observable instance.</returns>
    public static Observable<T> operator +(Observable<T> observable, Action<T> handler)
    {
        observable.AddListener(handler);
        return observable;
    }

    /// <summary>
    /// Overloads the -= operator to remove a listener from the <see cref="ValueChanged"/> event.
    /// </summary>
    /// <param name="observable">The observable instance.</param>
    /// <param name="handler">The action to be removed.</param>
    /// <returns>The observable instance.</returns>
    public static Observable<T> operator -(Observable<T> observable, Action<T> handler)
    {
        observable.RemoveListener(handler);
        return observable;
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="Observable{T}"/> class with the specified value and optional change handler.
    /// </summary>
    /// <param name="value">The initial value.</param>
    /// <param name="onValueChanged">An optional action to be called when the value changes.</param>
    public Observable(T value, Action<T> onValueChanged = null)
    {
        this.value = value;

        if (onValueChanged != null)
            ValueChanged += onValueChanged;
    }

    /// <summary>
    /// Sets the value and triggers the <see cref="ValueChanged"/> event if the value has changed.
    /// </summary>
    /// <param name="value">The new value.</param>
    public void Set(T value)
    {
        if (EqualityComparer<T>.Default.Equals(this.value, value))
            return;
        this.value = value;
        Invoke();
    }

    /// <summary>
    /// Invokes the <see cref="ValueChanged"/> event with the current value.
    /// </summary>
    public void Invoke()
    {
        ValueChanged?.Invoke(value);
    }

    /// <summary>
    /// Adds a listener to the <see cref="ValueChanged"/> event.
    /// </summary>
    /// <param name="handler">The action to be called when the value changes.</param>
    private void AddListener(Action<T> handler)
    {
        ValueChanged += handler;
    }

    /// <summary>
    /// Removes a listener from the <see cref="ValueChanged"/> event.
    /// </summary>
    /// <param name="handler">The action to be removed.</param>
    private void RemoveListener(Action<T> handler)
    {
        ValueChanged -= handler;
    }

    /// <summary>
    /// Disposes the observable, clearing all listeners and resetting the value.
    /// </summary>
    public void Dispose()
    {
        ValueChanged = null;
        value = default;
    }
}
