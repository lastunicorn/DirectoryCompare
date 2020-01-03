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
using System.Linq;
using DustInTheWind.DirectoryCompare.Domain;
using DustInTheWind.DirectoryCompare.Domain.DataAccess;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Application.GetPot
{
    public class GetPotRequestHandler : RequestHandler<GetPotRequest, List<Pot>>
    {
        private readonly IPotRepository potRepository;

        public GetPotRequestHandler(IPotRepository potRepository)
        {
            this.potRepository = potRepository ?? throw new ArgumentNullException(nameof(potRepository));
        }

        protected override List<Pot> Handle(GetPotRequest request)
        {
            return potRepository.Get()
                .OrderBy(x=>x.Name)
                .ToList();
        }
    }
}
