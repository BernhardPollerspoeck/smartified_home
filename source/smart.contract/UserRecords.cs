using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace smart.contract;

public record CreateUserDto(
    string Username,
    string Password);

