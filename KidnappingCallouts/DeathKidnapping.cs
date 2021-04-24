using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using FivePD.API;
using FivePD.API.Utils;

namespace KidnappingCallouts
{

    [CalloutProperties("Van Kidnapping", "BGHDDevelopment", "1.0.0")]
    public class DeathKidnapping : Callout
    {

        private Vehicle car;
        Ped driver, driver2, Vic;

        public DeathKidnapping()
        {
            Random rnd = new Random();
            float offsetX = rnd.Next(100, 700);
            float offsetY = rnd.Next(100, 700);
            InitInfo(World.GetNextPositionOnStreet(Game.PlayerPed.GetOffsetPosition(new Vector3(offsetX, offsetY, 0))));
            ShortName = "Van Kidnapping";
            CalloutDescription = "Reports show two suspects has kidnapped a person.";
            ResponseCode = 3;
            StartDistance = 150f;
        }

        public async override void OnStart(Ped player)
        {
            base.OnStart(player);
            driver = await SpawnPed(RandomUtils.GetRandomPed(), Location + 2);
            driver2 = await SpawnPed(RandomUtils.GetRandomPed(), Location + 1);
            Vic = await SpawnPed(RandomUtils.GetRandomPed(), Location + 1);
            car = await SpawnVehicle(VehicleHash.Speedo2, Location);
            driver.SetIntoVehicle(car, VehicleSeat.Driver);
            driver2.SetIntoVehicle(car, VehicleSeat.RightRear);
            Vic.SetIntoVehicle(car, VehicleSeat.LeftRear);
            PlayerData playerData = Utilities.GetPlayerData();
            string displayName = playerData.DisplayName;
            VehicleData datacar = await Utilities.GetVehicleData(car.NetworkId);
            string vehicleName = datacar.Name;
            Notify("~r~[KidnappingCallouts] ~y~Officer ~b~" + displayName + ",~y~ the suspects are driving a " + vehicleName + "!");
            
            //Driver Data
            PedData data = new PedData();
            data.BloodAlcoholLevel = 0.01;
            List<Item> items = new List<Item>();
            Item Pistol = new Item {
                Name = "Pistol",
                IsIllegal = true
            };
            items.Add(Pistol);
            data.Items = items;
            Utilities.SetPedData(driver.NetworkId,data);
            
            //Driver2 Data
            PedData data2 = new PedData();
            data2.BloodAlcoholLevel = 0.05;
            List<Item> items2 = new List<Item>();
            Item SMG = new Item {
                Name = "SMG",
                IsIllegal = true
            };
            items.Add(SMG);
            data2.Items = items2;
            Utilities.SetPedData(driver2.NetworkId,data2);
            Utilities.ExcludeVehicleFromTrafficStop(car.NetworkId,true);
            
            //Tasks
            driver.AlwaysKeepTask = true;
            driver.BlockPermanentEvents = true;
            driver2.AlwaysKeepTask = true;
            driver2.BlockPermanentEvents = true;
            Vic.AlwaysKeepTask = true;
            Vic.BlockPermanentEvents = true;
            
            driver.Weapons.Give(WeaponHash.Pistol, 30, true, true);
            driver2.Weapons.Give(WeaponHash.SMG, 150, true, true);
            API.SetDriveTaskMaxCruiseSpeed(driver.GetHashCode(), 35f);
            API.SetDriveTaskDrivingStyle(driver.GetHashCode(), 524852);
            driver.Task.FleeFrom(player);
            Vic.Task.HandsUp(1000000);
            Notify("~o~Officer ~b~" + displayName + ",~o~ the driver is fleeing with the victim!");
            car.AttachBlip();
            driver.AttachBlip();
            driver2.AttachBlip();
            Vic.AttachBlip();
            PedData data1 = await Utilities.GetPedData(driver.NetworkId);
            PedData data4 = await Utilities.GetPedData(Vic.NetworkId);
            PedData data3 = await Utilities.GetPedData(driver2.NetworkId);
            
            string firstname2 = data4.FirstName;
            string firstname3 = data3.FirstName;
            string firstname = data1.FirstName;
            API.Wait(6000);
            DrawSubtitle("~r~[" + firstname2 + "] ~s~Help me please!", 5000);
            driver2.Task.FightAgainst(player);
            API.Wait(6000);
            DrawSubtitle("~r~[" + firstname3 + "] ~s~Do not speak!", 5000);
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