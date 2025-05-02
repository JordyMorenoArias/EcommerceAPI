namespace EcommerceAPI.Utilities
{
    public class CardValidator
    {
        public static bool IsValidCardNumber(string cardNumber)
        {
            // Remove any non-digit characters
            cardNumber = cardNumber.Replace(" ", "").Replace("-", "");

            // Check if the card number is numeric and has a valid length
            if (!long.TryParse(cardNumber, out _) || cardNumber.Length < 13 || cardNumber.Length > 19)
                return false;

            // Luhn algorithm for checksum validation
            int sum = 0;
            bool alternate = false;
            for (int i = cardNumber.Length - 1; i >= 0; i--)
            {
                int n = int.Parse(cardNumber[i].ToString());
                if (alternate)
                {
                    n *= 2;
                    if (n > 9) n -= 9;
                }
                sum += n;
                alternate = !alternate;
            }
            return sum % 10 == 0;
        }
    }
}
