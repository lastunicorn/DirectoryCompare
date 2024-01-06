// DirectoryCompare
// Copyright (C) 2017-2024 Dust in the Wind
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System.Reflection;
using System.Runtime.CompilerServices;
using BindingFlags = System.Reflection.BindingFlags;

namespace DustInTheWind.DirectoryCompare.Infrastructure;

public abstract class ApplicationStateBase
{
    private readonly EventBus eventBus;
    private readonly Dictionary<string, object> properties = new();

    protected ApplicationStateBase(EventBus eventBus)
    {
        this.eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
    }

    protected StatePropertyDescriptor CreateProperty(string propertyName)
    {
        return new StatePropertyDescriptor(propertyName, properties);
    }

    protected void RegisterProperty<TValue, TEvent>(string propertyName)
    {
        if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));

        StateProperty<TValue> stateProperty = new()
        {
            EventType = typeof(TEvent)
        };
        properties.Add(propertyName, stateProperty);
    }

    protected TValue GetValue<TValue>([CallerMemberName] string propertyName = null)
    {
        if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));

        StateProperty<TValue> stateProperty = properties[propertyName] as StateProperty<TValue>;

        if (stateProperty == null)
            throw new Exception($"Property does not exist: {propertyName}");

        return stateProperty.Value;
    }

    protected void SetValue<TValue>(TValue value, [CallerMemberName] string propertyName = null)
    {
        if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));

        StateProperty<TValue> stateProperty = properties[propertyName] as StateProperty<TValue>;

        if (stateProperty == null)
            throw new Exception($"Property does not exist: {propertyName}");

        stateProperty.Value = value;

        Type eventType = stateProperty.EventType;

        if (eventType != null)
        {
            dynamic ev = CreateEventInstance(eventType, value);

            //dynamic ev = Activator.CreateInstance(eventType, value);
            eventBus.Publish(ev);
        }
    }

    private dynamic CreateEventInstance<TValue>(Type eventType, TValue value)
    {
        ConstructorInfo constructorInfo = CreateEventInstanceWithParameterInConstructor(eventType, value);

        if (constructorInfo != null)
            return constructorInfo;

        return CreateEventInstanceWithProperty(eventType, value);
    }

    private static dynamic CreateEventInstanceWithParameterInConstructor<TValue>(Type eventType, TValue value)
    {
        ConstructorInfo constructorInfo = eventType.GetConstructor(new[] { typeof(TValue) });

        return constructorInfo == null
            ? null
            : constructorInfo.Invoke(new object[] { value });
    }

    private static dynamic CreateEventInstanceWithProperty<TValue>(Type eventType, TValue value)
    {
        ConstructorInfo constructorInfo = eventType.GetConstructor(Type.EmptyTypes);

        if (constructorInfo != null)
        {
            object eventInstance = constructorInfo.Invoke(Array.Empty<object>());

            PropertyInfo propertyInfo = eventType.GetProperties()
                .FirstOrDefault(x => x.PropertyType == typeof(TValue) && x.CanWrite);

            if (propertyInfo != null)
                propertyInfo.SetValue(eventInstance, value);

            return eventInstance;
        }

        return null;
    }
}