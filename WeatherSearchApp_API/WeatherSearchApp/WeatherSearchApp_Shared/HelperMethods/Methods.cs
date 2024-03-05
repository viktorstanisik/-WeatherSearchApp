using System.Security.Cryptography;
using System.Text;

namespace WeatherSearchApp_Shared.HelperMethods
{
    public static class Methods
    {
        public static string GenerateSha512Hash(string input)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha512.ComputeHash(inputBytes);

                // Convert the byte array to a lowercase hexadecimal string
                string hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

                return hashString;
            }
        }

        public static string GenerateSecurityCode(int length)
        {
            const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random(Guid.NewGuid().GetHashCode());
            char[] code = new char[length];

            for (int i = 0; i < length; i++)
            {
                code[i] = validChars[random.Next(0, validChars.Length)];
            }

            return new string(code);
        }


        public static void TrimStringProperties(this object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            // Get all properties of the object
            var properties = obj.GetType().GetProperties();

            foreach (var property in properties)
            {
                // Check if the property is a string
                if (property.PropertyType == typeof(string))
                {
                    // Get the current string value
                    var value = (string)property.GetValue(obj);

                    // Trim the string and set it back to the property
                    property.SetValue(obj, value?.Trim());
                }
            }
        }
    }
}
