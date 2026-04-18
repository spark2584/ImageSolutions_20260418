using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

public partial class Utility
{
    public class PaymentProcessing
    {
        public enum CreditCardTypes
        {
            Visa,
            Mastercard
        }

        public static string[] GetCreditCardYear()
        {
            string[] objReturn = new string[10];

            for (int i=0; i< 10; i++)
            {
                objReturn[i] = Convert.ToString(System.DateTime.Now.Year + i);
            }
            return objReturn;
        }

        public static bool IsValidCreditCardExpiration(string TwoDigitMonth, string FourDigitYear)
        {
            try
            {
                if (!Utility.IsInteger(FourDigitYear) || !Utility.IsInteger(TwoDigitMonth)) return false;
                DateTime objCCDate = Convert.ToDateTime(FourDigitYear + "/" + TwoDigitMonth + "/" + 1);
                if (objCCDate < DateTime.Now) return false;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidCreditCardNumber(CreditCardTypes CreditCardType, string CreditCardNumber)
        {
            StringBuilder objString = new StringBuilder();
            string strCreditCardNumber = string.Empty;
            int intAlternate = 0;
            int intTotal = 0;
            int intDigit = 0;
            bool blnReturn = false;

            try
            {
                strCreditCardNumber = CreditCardNumber.Trim().Replace("-", "").Replace(" ", "");

                if (IsValidCreditCardNumberLength(CreditCardType, strCreditCardNumber))
                {
                    char[] arrAccount = strCreditCardNumber.Trim().ToCharArray();

                    for (int intCount = arrAccount.Length; intCount > 0; intCount--)
                    {
                        intDigit = Convert.ToInt32(arrAccount[intCount - 1].ToString());
                        if (intAlternate == 1)
                        {
                            intTotal += CreditCardDigitArraySum(intDigit * 2);
                        }
                        else
                        {
                            intTotal += intDigit;
                        }
                        if (intAlternate == 0)
                        {
                            intAlternate = 1;
                        }
                        else
                        {
                            intAlternate = 0;
                        }
                    }

                    double dblMod = Math.IEEERemainder(intTotal, 10);

                    if (dblMod != 0)
                    {
                        blnReturn = false;
                    }

                    blnReturn = true;
                }
            }
            catch
            {
                blnReturn = false;
            }
            finally
            {
                objString = null;
            }
            return blnReturn;
        }

        private static bool IsValidCreditCardNumberLength(CreditCardTypes CreditCardType, string CreditCardNumber)
        {
            bool blnReturn = false;
            int intLength = CreditCardNumber.Length;

            switch (CreditCardType.ToString().ToLower())
            {
                case ("visa"):
                    if (intLength == 13 || intLength == 16)
                    {
                        if (IsValidCreditCardNumberPrefix(CreditCardNumber, 4, 4, 1)) blnReturn = true;
                    }
                    break;
                case ("mastercard"):
                    if (intLength == 16)
                    {
                        if (IsValidCreditCardNumberPrefix(CreditCardNumber, 51, 55, 2)) blnReturn = true;
                    }
                    break;
                case ("discover"):
                    if (intLength == 16)
                    {
                        if (IsValidCreditCardNumberPrefix(CreditCardNumber, 6011, 6011, 4)) blnReturn = true;
                    }
                    break;
                case ("amex"):
                    if (intLength == 15)
                    {
                        string strCheck = CreditCardNumber.Substring(0, 2);
                        if (strCheck == "34" || strCheck == "37") blnReturn = true;
                    }
                    break;
            }
            return blnReturn;
        }

        private static int CreditCardDigitArraySum(int Param)
        {
            char[] arrValue = Param.ToString().ToCharArray();
            int intReturn = 0;
            for (int intCount = 0; intCount < arrValue.Length; intCount++)
            {
                intReturn += Convert.ToInt32(arrValue[intCount].ToString());
            }
            return intReturn;
        }

        private static bool IsValidCreditCardNumberPrefix(string CreditCardNumber, int LowRange, int HighRange, int Length)
        {
            int intValue = Convert.ToInt16(CreditCardNumber.Substring(0, Length));
            if (intValue >= LowRange && intValue <= HighRange) return true;
            return false;
        }
    }
}
