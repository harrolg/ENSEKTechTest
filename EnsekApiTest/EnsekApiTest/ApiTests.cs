using EnsekApiTest.Helpers;
using FluentAssertions;
using System.Net;

namespace EnsekApiTest
{
    public class Tests
    {
        private string gas = "1";
        private string nuclear = "2";
        private string electric = "3";
        private string oil = "4";

        [OneTimeSetUp]
        public async Task Setup()
        {
            await RequestHelpers.LoginAndGetBearerToken("https://qacandidatetest.ensek.io/ENSEK/");
        }

        [Test]
        public async Task BuyGas()
        {
            await RequestHelpers.ResetData();

            var energyList = await RequestHelpers.GetEnergy();
            var gasAmountAvailable = energyList.gas.quantity_of_units;

            var amount = 350;
            var orderId = await RequestHelpers.BuyEnergy(gas, amount);
            orderId.Should().NotBeNull();

            var remainingUnitsAvailable = gasAmountAvailable - amount;

            var message = await VerificationHelpers.GetExpectedBuyEnergySuccessMessage("gas", amount, remainingUnitsAvailable, orderId);
            RequestHelpers.SuccessMessage.Should().Be(message);

            energyList = await RequestHelpers.GetEnergy();
            energyList.gas.quantity_of_units.Should().Be(remainingUnitsAvailable);
        }

        [Test]
        public async Task BuyElectric()
        {
            await RequestHelpers.ResetData();

            var energyList = await RequestHelpers.GetEnergy();
            var electricAmountAvailable = energyList.electric.quantity_of_units;

            var amount = 350;
            var orderId = await RequestHelpers.BuyEnergy(electric, amount);
            orderId.Should().NotBeNull();

            var remainingUnitsAvailable = electricAmountAvailable - amount;

            var message = await VerificationHelpers.GetExpectedBuyEnergySuccessMessage("electric", amount, remainingUnitsAvailable, orderId);
            RequestHelpers.SuccessMessage.Should().Be(message);

            energyList = await RequestHelpers.GetEnergy();
            energyList.electric.quantity_of_units.Should().Be(remainingUnitsAvailable);
        }

        [Test]
        public async Task BuyOil()
        {
            await RequestHelpers.ResetData();

            var energyList = await RequestHelpers.GetEnergy();
            var oilAmountAvailable = energyList.oil.quantity_of_units;

            var amount = 11;
            var orderId = await RequestHelpers.BuyEnergy(oil, amount);
            orderId.Should().NotBeNull();

            var remainingUnitsAvailable = oilAmountAvailable - amount;

            var message = await VerificationHelpers.GetExpectedBuyEnergySuccessMessage("oil", amount, remainingUnitsAvailable, orderId);
            RequestHelpers.SuccessMessage.Should().Be(message);

            energyList = await RequestHelpers.GetEnergy();
            energyList.oil.quantity_of_units.Should().Be(remainingUnitsAvailable);
        }

        [Test]
        public async Task AttemptToBuyNuclear()
        {
            await RequestHelpers.ResetData();

            var orderId = await RequestHelpers.BuyEnergy(nuclear, 65);
            orderId.Should().BeNull();

            RequestHelpers.SuccessMessage.Should().Be("There is no nuclear fuel to purchase!");

            var energyList = await RequestHelpers.GetEnergy();
            energyList.nuclear.quantity_of_units.Should().Be(0);       
        }

        [Test]
        public async Task BuyEachTypeOfFuelAndCheckOrders()
        {
            await RequestHelpers.ResetData();

            //buy gas
            var gasAmount = 350;
            var gasOrderId = await RequestHelpers.BuyEnergy(gas, gasAmount);
            gasOrderId.Should().NotBeNull();

            //buy electric
            var electricAmount = 1060;
            var electricOrderId = await RequestHelpers.BuyEnergy(electric, electricAmount);
            electricOrderId.Should().NotBeNull();

            //buy oil
            var oilAmount = 11;
            var oilOrderId = await RequestHelpers.BuyEnergy(oil, oilAmount);
            oilOrderId.Should().NotBeNull();

            //get orders
            var ordersList = await RequestHelpers.GetOrders();

            //check ours are added
            ordersList.Should().Contain(g => g.id == gasOrderId);
            ordersList.Should().Contain(e => e.id == electricOrderId);
            ordersList.Should().Contain(o => o.id == oilOrderId);

            //check our order details
            VerificationHelpers.CheckOrderDetails(ordersList, gasOrderId, gasAmount, "gas");
            VerificationHelpers.CheckOrderDetails(ordersList, electricOrderId, electricAmount, "Elec");
            VerificationHelpers.CheckOrderDetails(ordersList, oilOrderId, oilAmount, "Oil");

            //check how many orders before today
            RequestHelpers.GetNumberOfOrdersBeforeCurrentDate(ordersList).Should().Be(5);
        }

        [Test]
        public async Task BuyEnergeyReturnBadRequest()
        {
            await RequestHelpers.ResetData();

            //buy gas
            var amount = 40;
            var response = await RequestHelpers.SendBuyEnergyRequest("buy", "Put", "5", amount);

            response.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task UnauthorisedLogin()
        {
            await RequestHelpers.ResetData();

            var response = await RequestHelpers.Login("test", "notmypassword");
            response.Should().Be(HttpStatusCode.Unauthorized);

        }
    }
}