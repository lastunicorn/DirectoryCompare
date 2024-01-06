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

namespace DustInTheWind.DirectoryCompare.Infrastructure;

public class StatePropertyDescriptor
{
    private readonly Dictionary<string, object> properties;

    private readonly string propertyName;
    private Type propertyType;
    private Type eventType;

    public StatePropertyDescriptor(string propertyName, Dictionary<string, object> properties)
    {
        this.propertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
        this.properties = properties ?? throw new ArgumentNullException(nameof(properties));

        propertyType = typeof(object);
        eventType = null;
    }

    public StatePropertyDescriptor OfType<TValue>()
    {
        propertyType = typeof(TValue);
        return this;
    }

    public StatePropertyDescriptor RaisesChangeEvent<TEvent>()
    {
        eventType = typeof(TEvent);
        return this;
    }

    public void Register()
    {
        Type statePropertyType = typeof(StateProperty<>);
        Type[] typeArguments = { propertyType };
        Type statePropertyGenericType = statePropertyType.MakeGenericType(typeArguments);
        object stateProperty = Activator.CreateInstance(statePropertyGenericType);

        PropertyInfo eventTypePropertyInfo = statePropertyGenericType.GetProperty(nameof(StateProperty<object>.EventType));
        eventTypePropertyInfo?.SetValue(stateProperty, eventType);

        properties.Add(propertyName, stateProperty);
    }
}