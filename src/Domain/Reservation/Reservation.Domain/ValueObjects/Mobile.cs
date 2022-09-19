using System.Text.RegularExpressions;
using ValueOf;

namespace Reservation.Model.ValueObjects;

public class Mobile : ValueOf<string, Mobile>
{ 
    protected override void Validate()
    { 
        var match = Regex.Match(Value, "^[0-9]");
    }
}