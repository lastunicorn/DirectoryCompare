// DirectoryCompare
// Copyright (C) 2017-2019 Dust in the Wind
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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DustInTheWind.DirectoryCompare.Entities
{
    public class HItemCollection<T> : Collection<T>
        where T : HItem
    {
        private HItem parent;

        public HItem Parent
        {
            get => parent;
            set
            {
                if (ReferenceEquals(value, parent))
                    return;

                parent = value;

                foreach (T item in Items)
                {
                    item.Parent = value;
                }
            }
        }

        public HItemCollection(HItem parent)
        {
            this.parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        protected override void InsertItem(int index, T item)
        {
            if (index < 0 || index > Items.Count - 1)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (item == null)
                throw new ArgumentNullException(nameof(item));

            item.Parent = Parent;

            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            if (index < 0 || index > Items.Count - 1)
                throw new ArgumentOutOfRangeException(nameof(index));

            Items[index].Parent = null;
            base.RemoveItem(index);
        }

        protected override void SetItem(int index, T item)
        {
            if (index < 0 || index > Items.Count - 1)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (item == null)
                throw new ArgumentNullException(nameof(item));

            Items[index].Parent = null;

            base.SetItem(index, item);

            item.Parent = parent;
        }

        protected override void ClearItems()
        {
            foreach (T item in Items)
                item.Parent = null;

            base.ClearItems();
        }

        public void AddRange(IEnumerable<T> items)
        {
            IEnumerable<T> notNullItems = items.Where(x => x != null);

            foreach (T item in notNullItems)
            {
                Items.Add(item);
                item.Parent = parent;
            }
        }
    }
}