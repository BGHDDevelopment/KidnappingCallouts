using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using FivePD.API;

namespace KidnappingCallouts
{

    [CalloutProperties("Van Kidnapping", "BGHDDevelopment", "0.0.3")]
    public class DeathKidnapping : Callout
    {

        private Vehicle car;
        Ped driver, driver2;
        Ped Vic;
        List<object> items = new List<object>();
        List<object> items2 = new List<object>();

        public DeathKidnapping()
        {
            Random rnd = new Random();
            float offsetX = rnd.Next(100, 700);
            float offsetY = rnd.Next(100, 700);
            InitBase(World.GetNextPositionOnStreet(Game.PlayerPed.GetOffsetPosition(new Vector3(offsetX, offsetY, 0))));
            ShortName = "Van Kidnapping";
            CalloutDescription = "Reports show two suspects has kidnapped a person.";
            ResponseCode = 3;
            StartDistance = 150f;
        }

        public async override void OnStart(Ped player)
        {
            base.OnStart(player);
            dynamic playerData = GetPlayerData();
            string displayName = playerData.DisplayName;
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
            dynamic data2 = await GetPedData(Vic.NetworkId);
            dynamic data1 = await GetPedData(driver.NetworkId);
            dynamic data3 = await GetPedData(driver2.NetworkId);
            string firstname2 = data2.Firstname;
            string firstname3 = data3.Firstname;
            string firstname = data1.Firstname;
            API.Wait(6000);
            DrawSubtitle("~r~[" + firstname2 + "] ~s~Help me please!", 5000);
            driver2.Task.FightAgainst(player);
            API.Wait(6000);
            DrawSubtitle("~r~[" + firstname3 + "] ~s~Do not speak!", 5000);
            API.Wait(6000);
            DrawSubtitle("~r~[" + firstname2 + "] ~s~PLEASE HELP!", 5000);

        }

        public async override Task Init()
        {
            OnAccept();
            driver = await SpawnPed(GetRandomPed(), Location + 2);
            driver2 = await SpawnPed(GetRandomPed(), Location + 1);
            Vic = await SpawnPed(GetRandomPed(), Location + 1);
            car = await SpawnVehicle(VehicleHash.Speedo2, Location);
            driver.SetIntoVehicle(car, VehicleSeat.Driver);
            driver2.SetIntoVehicle(car, VehicleSeat.RightRear);
            Vic.SetIntoVehicle(car, VehicleSeat.LeftRear);
            dynamic playerData = GetPlayerData();
            string displayName = playerData.DisplayName;
            Notify("~r~[KidnappingCallouts] ~y~Officer ~b~" + displayName + ",~y~ the suspects are driving a van!");
            
            //Driver Data
            dynamic data = new ExpandoObject();
            data.alcoholLevel = 0.01;
            object Pistol = new {
                Name = "Pistol",
                IsIllegal = true
            };
            items.Add(Pistol);
            data.items = items;
            SetPedData(driver.NetworkId,data);
            
            //Driver2 Data
            dynamic data2 = new ExpandoObject();
            data2.alcoholLevel = 0.05;
            object SMG = new {
                Name = "SMG",
                IsIllegal = true
            };
            items.Add(SMG);
            data2.items2 = items2;
            SetPedData(driver2.NetworkId,data2);
            
            //Tasks
            driver.AlwaysKeepTask = true;
            driver.BlockPermanentEvents = true;
            driver2.AlwaysKeepTask = true;
            driver2.BlockPermanentEvents = true;
            Vic.AlwaysKeepTask = true;
            Vic.BlockPermanentEvents = true;
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

        public override void OnCancelBefore()
        {
        }
    }

}