using EnsekApiTest.Models;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnsekApiTest.Helpers
{
    public static class VerificationHelpers
    {
        public static void CheckOrderDetails(List<Order> orders, string id, int quantity, string fuel)
        {
            var orderDetails = orders.Where(g => g.id == id).ToList();

            orderDetails.Should().HaveCountGreaterThan(0);

            orderDetails[0].quantity.Should().Be(quantity);
            orderDetails[0].fuel.Should().Be(fuel);

            bool isDateTimeFormatCorrect = DateTime.TryParseExact(orderDetails[0].time, "ddd, d MMM yyyy HH:mm:ss 'GMT'",
                                              System.Globalization.CultureInfo.InvariantCulture,
                                              System.Globalization.DateTimeStyles.None,
                                              out DateTime result);

            isDateTimeFormatCorrect.Should().BeTrue();

            DateTime orderDateOnly = DateTime.ParseExact(orderDetails[0].time, "ddd, d MMM yyyy HH:mm:ss 'GMT'",
                System.Globalization.CultureInfo.InvariantCulture).Date;

            orderDateOnly.Should().Be(DateTime.Now.Date);
        }

        public static async Task<string> GetExpectedBuyEnergySuccessMessage(string energy, int amount, int remainingAmount, string orderId)
        {

            var energyList = await RequestHelpers.GetEnergy();

            var unitType = "UNKNOWN";
            float cost = 0;

            switch (energy)
            {
                case "gas":
                    unitType = "m³";
                    cost = amount * energyList.gas.price_per_unit;
                    break;
                case "electric":
                    unitType = "kWh";
                    cost = amount * energyList.electric.price_per_unit;
                    break;
                case "oil":
                    unitType = "Litres";
                    cost = amount * energyList.oil.price_per_unit;
                    break;
                case "nuclear":
                    unitType = "MW";
                    cost = amount * energyList.nuclear.price_per_unit;
                    break;
                default:
                    break;
            }

            var successMessage = $"You have purchased {amount} {unitType} at a cost of {cost} there are {remainingAmount} units remaining. Your order id is {orderId}.";

            return successMessage;
        }
    }
}
