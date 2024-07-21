using Polliakov.ExtendedValidation.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Polliakov.ExtendedValidation.TestApi.Dtos.Requests
{
    public class SomthingRequest
    {
        [Required]
        [MinLength(1)]
        public string Title { get; set; }
        [Required]
        public int Number { get; set; }

        [EntriesHaveRequired(IsRecursive = true, MaxApplyDepth = 1)]
        public List<string[]> InterestingWords { get; set; }

        public List<string> InterestingWords2 { get; set; }

        [EntriesHaveRequired(EntriesType = Core.EntriesType.Keys)]
        public Dictionary<string, string> Dictionary { get; set; }
    }
}
