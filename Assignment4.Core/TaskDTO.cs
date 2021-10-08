using System.Collections.Generic;

namespace Assignment4.Core
{
    //public record TaskCreateDTO(int Id, string Title, string Description, int? AssignedToId,IReadOnlyCollection<string> Tags,State state);
    
    public record TaskDTO
    {
        public int Id { get; init; }
        public string Title { get; init; }
        public string Description { get; init; }
        public int? AssignedToId { get; init; }
        public IReadOnlyCollection<string> Tags { get; init; }
        public State State { get; init; }
    }
}