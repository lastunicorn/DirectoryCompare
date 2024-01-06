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

using System.Collections;

namespace DustInTheWind.DirectoryCompare.Domain.Comparison;

/// <summary>
/// This collection stores multiple items grouped by key.
/// </summary>
/// <typeparam name="TKey">The type of the key based on which the items are grouped.</typeparam>
/// <typeparam name="TValue">The type of the items that are stored by the collection.</typeparam>
internal class DuplicatesCollection<TKey, TValue> : IEnumerable<KeyValuePair<TKey, List<TValue>>>
{
    readonly Dictionary<TKey, List<TValue>> dictionary = new();

    public List<TValue> Add(TKey key, TValue value)
    {
        List<TValue> bucket = GetOrCreateBucketFor(key);
        List<TValue> existingValues = bucket.ToList();

        bucket.Add(value);

        return existingValues;
    }

    private List<TValue> GetOrCreateBucketFor(TKey key)
    {
        bool exists = dictionary.TryGetValue(key, out List<TValue> bucket);

        if (!exists)
        {
            bucket = new List<TValue>();
            dictionary.Add(key, bucket);
        }

        return bucket;
    }

    public IEnumerable<TValue> GetAll(TKey key)
    {
        return GetBucketIfExists(key) ?? new List<TValue>();
    }

    private IEnumerable<TValue> GetBucketIfExists(TKey key)
    {
        dictionary.TryGetValue(key, out List<TValue> bucket);
        return bucket;
    }

    public void Clear()
    {
        dictionary.Clear();
    }

    public IEnumerator<KeyValuePair<TKey, List<TValue>>> GetEnumerator()
    {
        return dictionary.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}