using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using CitizenFX.Core;
using CalloutAPI;
using CitizenFX.Core.Native;

namespace KidnappingCallouts
{

    [CalloutProperties("Kidnapping", "BGHDDevelopment", "0.0.2", Probability.Medium)]
    public class NormalKidnapping : Callout
    {

        private Vehicle car;
        Ped driver;
        Ped Vic;
        List<object> items = new List<object>();
        private string[] carList = { "speedo", "speedo2", "stanier", "stinger", "stingergt", "stratum", "stretch", "taco", "tornado", "tornado2", "tornado3", "tornado4", "tourbus", "vader", "voodoo2", "dune5", "youga", "taxi", "tailgater", "sentinel2", "sentinel", "sandking2", "sandking", "ruffian", "rumpo", "rumpo2", "oracle2", "oracle", "ninef2", "ninef", "minivan", "gburrito", "emperor2", "emperor"};
        
        public NormalKidnapping()
        {
            Random rnd = new Random();
            float offsetX = rnd.Next(100, 700);
            float offsetY = rnd.Next(100, 700);
            InitBase(World.GetNextPositionOnStreet(Game.PlayerPed.GetOffsetPosition(new Vector3(offsetX, offsetY, 0))));
            ShortName = "Kidnapping";
            CalloutDescription = "Reports show a suspect has kidnapped a person.";
            ResponseCode = 3;
            StartDistance = 150f;
        }

        public async override void OnStart(Ped player)
        {
            base.OnStart(player);
            dynamic playerData = GetPlayerData();
            string displayName = playerData.DisplayName;
            driver.Weapons.Give(WeaponHash.Pistol, 30, true, true);
            API.SetDriveTaskMaxCruiseSpeed(driver.GetHashCode(), 35f);
            API.SetDriveTaskDrivingStyle(driver.GetHashCode(), 524852);
            driver.Task.FleeFrom(player);
            Vic.Task.HandsUp(1000000);
            Notify("~o~Officer ~b~" + displayName + ",~o~ the driver is fleeing with the victim!");
            car.AttachBlip();
            driver.AttachBlip();
            Vic.AttachBlip();
            dynamic data2 = await GetPedData(Vic.NetworkId);
            dynamic data1 = await GetPedData(driver.NetworkId);
            string firstname2 = data2.Firstname;
            string firstname = data1.Firstname;
            API.Wait(6000);
            DrawSubtitle("~r~[" + firstname2 + "] ~s~Help me please!", 5000);
            API.Wait(6000);
            DrawSubtitle("~r~[" + firstname + "] ~s~Do not speak!", 5000);
            API.Wait(6000);
            DrawSubtitle("~r~[" + firstname2 + "] ~s~PLEASE HELP!", 5000);

        }

        public async override Task Init()
        {
            OnAccept();
            driver = await SpawnPed(GetRandomPed(), Location + 2);
            Vic = await SpawnPed(GetRandomPed(), Location + 1);
            Random random = new Random();
            string cartype = carList[random.Next(carList.Length)];
            VehicleHash Hash = (VehicleHash) API.GetHashKey(cartype);
            car = await SpawnVehicle(Hash, Location);
            driver.SetIntoVehicle(car, VehicleSeat.Driver);
            Vic.SetIntoVehicle(car, VehicleSeat.Passenger);
            dynamic playerData = GetPlayerData();
            string displayName = playerData.DisplayName;
            Notify("~r~[KidnappingCallouts] ~y~Officer ~b~" + displayName + ",~y~ the suspect is driving a " + cartype + "!");
            
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
            
            //Tasks
            driver.AlwaysKeepTask = true;
            driver.BlockPermanentEvents = true;
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