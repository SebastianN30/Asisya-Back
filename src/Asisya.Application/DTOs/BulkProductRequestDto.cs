using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asisya.Application.DTOs
{
  public record BulkProductRequestDto(int Count, int? BatchSize);
}
