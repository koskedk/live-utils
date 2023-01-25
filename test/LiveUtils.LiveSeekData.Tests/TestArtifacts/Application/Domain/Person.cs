using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace LiveUtils.LiveSeekData.Tests.TestArtifacts.Application.Domain
{
    [Index(nameof(LiveRowId))]
    public abstract class BaseEntity<T> : Entity<T>
    {
        public long LiveRowId { get; set; }
    }

    public class Person:BaseEntity<Guid>
    {
        [MaxLength(50)]
        public string FirstName { get; set; }
        [MaxLength(50)]
        public string LastName { get; set; }
        [MaxLength(50)]
        public string Gender  { get; set; }
        public DateTime BirthDate { get; set; }

        public Person()
        {
            Id = Guid.NewGuid();
        }
    }
}