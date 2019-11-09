using Microsoft.EntityFrameworkCore;

namespace WebAPIAzureAD.Models
{
    internal static class DatabaseSeed
    {
        internal static void Initialize(PeopleContext peopleContext)
        {
            if (peopleContext.Database.EnsureCreated())
            {
                peopleContext.Person.Add(new Person("Hans", "Müller"));

                peopleContext.SaveChanges();
            }
        }
    }
}
