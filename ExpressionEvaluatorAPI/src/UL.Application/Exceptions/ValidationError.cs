using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UL.Application.Exceptions;
public record class ValidationError(string PropertyName, string ErrorMessage)
{
}
