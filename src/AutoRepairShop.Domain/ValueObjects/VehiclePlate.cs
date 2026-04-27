using System.Text.RegularExpressions;

namespace AutoRepairShop.Domain.ValueObjects
{
    public sealed class VehiclePlate
    {
        private static readonly Regex OldPlateRegex = new(@"^[A-Z]{3}[0-9]{4}$");

        private static readonly Regex MercosulPlateRegex = new(@"^[A-Z]{3}[0-9][A-Z][0-9]{2}$");

        public string Value { get; }

        private VehiclePlate(string value)
        {
            Value = value;
        }

        public static VehiclePlate Create(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("Vehicle plate is required");

            var plate = Normalize(input);

            if (!OldPlateRegex.IsMatch(plate) && !MercosulPlateRegex.IsMatch(plate))
            {
                throw new ArgumentException("Invalid vehicle plate");
            }

            return new VehiclePlate(plate);
        }

        private static string Normalize(string value) =>
            value.ToUpper().Replace("-", "").Replace(" ", "");

        public override bool Equals(object obj) =>
            obj is VehiclePlate other && Value == other.Value;

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value;
    }
}
