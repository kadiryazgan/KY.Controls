using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KY.Controls
{
    public class RegularExpressions
    {
        public const string PersonName = @"^[a-zA-Z''-'\sĞÜŞİÖÇğüşıöç]{1,40}$";
        public const string LoginName = @"^[a-zA-Z0-9]{3,40}$";
        public const string Currency = @"^\d{1,10}((\.|,|)\d{1,2})?$";
        public const string CurrencySigned = @"^-?\d{1,3}((\.|,|)\d{1,2})?$";
        public const string Email = @"^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$";
        public const string Integer = @"^\d+$";
        public const string PhoneNumber = @"^\d{3}[-\s]?\d{7}(\s?\(\d{1,10}\))?$";
        public const string Password = @"^[\w\d]{4,16}$";
        public const string GsmNumber = @"^5\d{2}[-\s]?\d{7}$";
        public const string Date = @"^(((0?[1-9])|([12])([0-9]?)|(3[01]?))(-|\/|\.)(0?[13578]|10|12)(-|\/|\.)((\d{4})|(\d{2}))|((0?[1-9])|([12])([0-9]?)|(3[0]?))(-|\/|\.)(0?[2469]|11)(-|\/|\.)((\d{4}|\d{2})))$";
        public const string DateTime = @"^(((0?[1-9])|([12])([0-9]?)|(3[01]?))(-|\/|\.)(0?[13578]|10|12)(-|\/|\.)((\d{4})|(\d{2}))|((0?[1-9])|([12])([0-9]?)|(3[0]?))(-|\/|\.)(0?[2469]|11)(-|\/|\.)((\d{4}|\d{2})))( ([01][0-9]|[2][0123]):([0-5][0123456789])(:([0-5][0123456789]))?)?$";
        public const string Hour = @"^(([01][0-9])|(2[0-3])):[0-5][0-9]$";
        public const string Url = @"^(http|https)\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*$";

    }
}
