using System;
using System.Collections.Generic;
using System.Text;

namespace Battleship.Domain.Configuration
{
    public class FleetConfiguration
    {
        public FleetConfiguration(IEnumerable<FleetItem> fleet)
        {
            if(fleet == null)
            {
                throw new ArgumentNullException(nameof(fleet), "You must provide a fleet to be placed.");
            }

            var fleetList = fleet.ToList();

            if(fleetList.Count == 0)
            {
                throw new ArgumentException(nameof(fleet), "There must be at least 1 ship to be placed.");
            }

            var duplicates = fleetList.GroupBy(item => item.Definition.Name)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key)
                .ToList();

            if(duplicates.Any())
            {
                var items = string.Join(", ", duplicates);
                throw new ArgumentException($"These duplicate ships were found in the fleet configuration: {items}");
            }

            Fleet = fleetList;
        }

        public IReadOnlyList<FleetItem> Fleet { get;}
    }
}
