using EF.CustomMappingToolkit.Sample.Database;
using EF.CustomMappingToolkit.Sample.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EF.CustomMappingToolkit.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var people = new List<Person>
            {
                new Person
                {
                    FirstName = "Emma",
                    LastName = "Smith",
                    Gender = Gender.Female
                },
                new Person
                {
                    FirstName = "Emily",
                    LastName = "Johnson",
                    Gender = Gender.Female
                },
                new Person
                {
                    FirstName = "Jacob",
                    LastName = "Williams",
                    Gender = Gender.Male
                }
            };

            using (var ctx = new PeopleContext())
            {
                // Insert people if they are not already present
                foreach (var person in people)
                {
                    Console.WriteLine($"Searching for person {person.FirstName} {person.LastName}.");
                    var found = ctx.People.FirstOrDefault(x => x.FirstName == person.FirstName && x.LastName == person.LastName);
                    if (found == null)
                    {
                        Console.WriteLine($"{person.FirstName} {person.LastName} was not found in database. This person is going to be added.");
                        ctx.People.Add(person);
                    }
                    else
                    {
                        Console.WriteLine($"{person.FirstName} {person.LastName} is already present in database.");
                    }
                }

                ctx.SaveChanges();

                // Now search for people
                // Find all women
                var query = ctx.People.Where(x => x.Gender == Gender.Female);
                IList<Person> women, men;

                try
                {
                    // The following will throw an exception, because Gender is not a mapped column
                    Console.WriteLine($"Querying all women using a not mapped column.");
                    Console.WriteLine(query);
                    women = query.ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Linq2Sql thrown an exception: {ex.Message}");
                }

                var mappingCompatibleQuery = ctx.People.AsCustomMappingCompatible().Where(x => x.Gender == Gender.Female);
                Console.WriteLine($"Querying all women using EF.CustomMappingToolkit.");
                Console.WriteLine(mappingCompatibleQuery);
                women = mappingCompatibleQuery.ToList();

                Console.WriteLine($"There are {women.Count} women in the database.");

                Expression<Func<Person, bool>> menCriteria = p => p.Gender == Gender.Male;
                Console.WriteLine("You can also use criteria expressions.");
                Console.WriteLine($"Criteria expression without using toolkit: {menCriteria}");
                var customMappingCompatibleMenCriteria = menCriteria.MakeCustomMappingCompatible();
                Console.WriteLine($"Criteria expression using toolkit: {customMappingCompatibleMenCriteria}");

                query = ctx.People.Where(menCriteria);
                try
                {
                    // The following will also throw an exception
                    Console.WriteLine($"Querying all men using a not mapped column.");
                    Console.WriteLine(query);
                    men = query.ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Linq2Sql thrown an exception: {ex.Message}");
                }

                mappingCompatibleQuery = ctx.People.Where(customMappingCompatibleMenCriteria);

                men = mappingCompatibleQuery.ToList();

                Console.WriteLine($"There are {men.Count} men in the database.");
            }
        }
    }
}
