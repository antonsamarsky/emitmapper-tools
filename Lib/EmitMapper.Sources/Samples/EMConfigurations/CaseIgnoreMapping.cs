using System;
using System.Linq;
using EmitObjectMapper;

namespace EOMConfigurations
{
    public class CaseIgnoreMapping
    {
        public static void Configurator(Type from, Type to, MappingSettings settings)
        {
            settings.MappingItems = 
                settings.MappingItems.
                    Where( mi => mi.SourceMember != null).
                    Join(   settings.MappingItems.Where( mi => mi.DestinationMember != null), 
                            mi => mi.SourceMember.Name.ToUpper(),
                            mi => mi.DestinationMember.Name.ToUpper(),
                            (omi, imi) => new MappingItem(omi.SourceMember, imi.DestinationMember)).
                    ToArray();
        }
    }
}
