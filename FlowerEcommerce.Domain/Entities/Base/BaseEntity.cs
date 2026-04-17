using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FlowerEcommerce.Domain.Entities.Base
{
    public interface IBaseEntity
    {
        [Key] public ulong Id { get; set; }
    }

    public class BaseEntity : IBaseEntity
    {
        [Key] public ulong Id { get; set; }
    }
}
