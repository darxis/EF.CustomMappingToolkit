using EF.CustomMappingToolkit.Sample.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace EF.CustomMappingToolkit.Sample.Model
{
    [Table("Person")]
    public class Person
    {
        [Column("Id")]
        public int Id { get; set; }

        [Column("FirstName")]
        public string FirstName { get; set; }

        [Column("LastName")]
        public string LastName { get; set; }

        [NotMapped]
        [MapEnumAsString(nameof(GenderString))]
        public Gender Gender { get; set; }

        [Column("Gender")]
        public string GenderString
        {
            get
            {
                return Gender.ToString();
            }
            private set
            {
                Gender = value.ParseEnum<Gender>();
            }
        }
    }
}
