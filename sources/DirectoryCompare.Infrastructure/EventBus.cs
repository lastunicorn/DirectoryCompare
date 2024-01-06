// VeloCity
// Copyright (C) 2022-2023 Dust in the Wind
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

namespace DustInTheWind.DirectoryCompare.Infrastructure;

public class EventBus
{
    private readonly Dictionary<Type, List<object>> subscribersByEvent = new();

    public void Subscribe<TEvent>(Action<TEvent> action)
    {
        List<object> actions = GetBucket<TEvent>() ?? CreateBucket<TEvent>();
        actions.Add(action);
    }

    public void Publish<TEvent>(TEvent @event)
    {
        List<object> bucket = GetBucket<TEvent>();

        if (bucket == null)
            return;

        IEnumerable<Action<TEvent>> actions = bucket.Cast<Action<TEvent>>();

        foreach (Action<TEvent> action in actions)
            action(@event);
    }

    private List<object> GetBucket<TEvent>()
    {
        return subscribersByEvent.ContainsKey(typeof(TEvent))
            ? subscribersByEvent[typeof(TEvent)]
            : null;
    }

    private List<object> CreateBucket<TEvent>()
    {
        List<object> actions = new();
        subscribersByEvent.Add(typeof(TEvent), actions);
        return actions;
    }
}