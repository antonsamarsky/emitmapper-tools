using MappingDefinitions;

namespace Domain
{
    public class EntityChild : Entity
    {
        [DataMember]
        public string AdditionalValue { get; set; }
    }
}