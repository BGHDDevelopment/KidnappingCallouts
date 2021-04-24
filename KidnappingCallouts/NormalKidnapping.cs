using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using FivePD.API;
using FivePD.API.Utils;

namespace KidnappingCallouts
{
    [CalloutProperties("Kidnapping", "BGHDDevelopment", "1.0.0")]
    public class NormalKidnapping : Callout
    {
        private Vehicle car;
        Ped driver, Vic;

        private string[] carList =
        {
            "speedo", "speedo2", "stanier", "stinger", "stingergt", "stratum", "stretch", "taco", "tornado", "tornado2",
            "tornado3", "tornado4", "tourbus", "vader", "voodoo2", "dune5", "youga", "taxi", "tailgater", "sentinel2",
            "sentinel", "sandking2", "sandking", "ruffian", "rumpo", "rumpo2", "oracle2", "oracle", "ninef2", "ninef",
            "minivan", "gburrito", "emperor2", "emperor"
        };

        public NormalKidnapping()
        {
            Random rnd = new Random();
            float offsetX = rnd.Next(100, 700);
            float offsetY = rnd.Next(100, 700);
            InitInfo(World.GetNextPositionOnStreet(Game.PlayerPed.GetOffsetPosition(new Vector3(offsetX, offsetY, 0))));
            ShortName = "Kidnapping";
            CalloutDescription = "Reports show a suspect has kidnapped a person.";
            ResponseCode = 3;
            StartDistance = 150f;
        }

        public async override void OnStart(Ped player)
        {
            base.OnStart(player);
            driver = await SpawnPed(RandomUtils.GetRandomPed(), Location + 2);
            Vic = await SpawnPed(RandomUtils.GetRandomPed(), Location + 1);
            Random random = new Random();
            string cartype = carList[random.Next(carList.Length)];
            VehicleHash Hash = (VehicleHash) API.GetHashKey(cartype);
            car = await SpawnVehicle(Hash, Location);
            driver.SetIntoVehicle(car, VehicleSeat.Driver);
            Vic.SetIntoVehicle(car, VehicleSeat.Passenger);
            PlayerData playerData = Utilities.GetPlayerData();
            string displayName = playerData.DisplayName;
            VehicleData datacar = await Utilities.GetVehicleData(car.NetworkId);
            string vehicleName = datacar.Name;
            Notify("~r~[KidnappingCallouts] ~y~Officer ~b~" + displayName + ",~y~ the suspect is driving a " +
                   vehicleName + "!");

            //Driver Data
            PedData data = new PedData();
            data.BloodAlcoholLevel = 0.01;
            List<Item> items = new List<Item>();
            Item Pistol = new Item
            {
                Name = "Pistol",
                IsIllegal = true
            };
            items.Add(Pistol);
            data.Items = items;
            Utilities.SetPedData(driver.NetworkId, data);

            Utilities.ExcludeVehicleFromTrafficStop(car.NetworkId, true);

            //Tasks
            driver.AlwaysKeepTask = true;
            driver.BlockPermanentEvents = true;
            Vic.AlwaysKeepTask = true;
            Vic.BlockPermanentEvents = true;

            driver.Weapons.Give(WeaponHash.Pistol, 30, true, true);
            API.SetDriveTaskMaxCruiseSpeed(driver.GetHashCode(), 35f);
            API.SetDriveTaskDrivingStyle(driver.GetHashCode(), 524852);
            driver.Task.FleeFrom(player);
            Vic.Task.HandsUp(1000000);
            Notify("~o~Officer ~b~" + displayName + ",~o~ the driver is fleeing with the victim!");
            car.AttachBlip();
            driver.AttachBlip();
            Vic.AttachBlip();
            PedData data2 = await Utilities.GetPedData(Vic.NetworkId);
            PedData data1 = await Utilities.GetPedData(driver.NetworkId);
            string firstname2 = data2.FirstName;
            string firstname = data1.FirstName;
            API.Wait(6000);
            DrawSubtitle("~r~[" + firstname2 + "] ~s~Help me please!", 5000);
            API.Wait(6000);
            DrawSubtitle("~r~[" + firstname + "] ~s~Do not speak!", 5000);
            API.Wait(6000);
            DrawSubtitle("~r~[" + firstname2 + "] ~s~PLEASE HELP!", 5000);
        }

        public async override Task OnAccept()
        {
            InitBlip();
            UpdateData();
        }

        private void Notify(string message)
        {
            API.BeginTextCommandThefeedPost("STRING");
            API.AddTextComponentSubstringPlayerName(message);
            API.EndTextCommandThefeedPostTicker(false, true);
        }

        private void DrawSubtitle(string message, int duration)
        {
            API.BeginTextCommandPrint("STRING");
            API.AddTextComponentSubstringPlayerName(message);
            API.EndTextCommandPrint(duration, false);
        }
    }
}