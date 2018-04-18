using QuantConnect.Data.Market;
using QuantConnect.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wtpEnum = WTP.BLL.Infrastructure.DTO.Enums;
using wtpDto = WTP.BLL.Infrastructure.DTO;

namespace QuantConnect.Brokerages.IbClasses
{
    public class AlgorithmHelper
    {

        public static TimeSpan GetExecutionTime(string time)
        {
            var timeParts = time.Split(':');
            if (timeParts.Length != 3)
            {
                throw new Exception($"GetExecutionTime: {time} parsin failed");
            }
            return new TimeSpan(Convert.ToInt32(timeParts[0]), Convert.ToInt32(timeParts[1]), Convert.ToInt32(timeParts[2]));
        }
        
    }
}
