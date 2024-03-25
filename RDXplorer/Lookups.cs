using RDXplorer.Enumerations;
using System.Collections.Generic;

namespace RDXplorer
{
    public static class Lookups
    {
        private static Dictionary<ItemEnumeration, string> _items = new();
        public static Dictionary<ItemEnumeration, string> Items
        {
            get
            {
                if (_items.Count == 0)
                {
                    _items.Add(ItemEnumeration.None, "None");
                    _items.Add(ItemEnumeration.RocketLauncher, "Rocket Launcher (Bazooka)");
                    _items.Add(ItemEnumeration.AssaultRifle, "Assault Rifle (AK-47)");
                    _items.Add(ItemEnumeration.SniperRifle, "Sniper Rifle (MR7)");
                    _items.Add(ItemEnumeration.Shotgun, "Shotgun (SPAS 12)");
                    _items.Add(ItemEnumeration.HandgunGlock17, "Handgun (Glock 17)");
                    _items.Add(ItemEnumeration.GrenadeLauncher, "Grenade Launcher (M79)");
                    _items.Add(ItemEnumeration.BowGun, "Bow Gun");
                    _items.Add(ItemEnumeration.CombatKnife, "Combat Knife");
                    _items.Add(ItemEnumeration.Handgun, "Handgun (M93R)");
                    _items.Add(ItemEnumeration.CustomHandgun, "Custom Handgun (M93R Burst)");
                    _items.Add(ItemEnumeration.LinearLauncher, "Linear Launcher");
                    _items.Add(ItemEnumeration.HandgunBullets, "Handgun Bullets");
                    _items.Add(ItemEnumeration.MagnumBullets, "Magnum Bullets");
                    _items.Add(ItemEnumeration.ShotgunShells, "Shotgun Shells");
                    _items.Add(ItemEnumeration.GrenadeRounds, "Grenade Rounds");
                    _items.Add(ItemEnumeration.AcidRounds, "Acid Rounds");
                    _items.Add(ItemEnumeration.FlameRounds, "Flame Rounds");
                    _items.Add(ItemEnumeration.BowGunArrows, "Bow Gun Arrows");
                    _items.Add(ItemEnumeration.M93RPart, "M93R Part");
                    _items.Add(ItemEnumeration.FAidSpray, "F. Aid Spray");
                    _items.Add(ItemEnumeration.GreenHerb, "Green Herb");
                    _items.Add(ItemEnumeration.RedHerb, "Red Herb");
                    _items.Add(ItemEnumeration.BlueHerb, "Blue Herb");
                    _items.Add(ItemEnumeration.MixedHerb2Green, "Mixed Herb (2 Green)");
                    _items.Add(ItemEnumeration.MixedHerbRedGreen, "Mixed Herb (Red & Green)");
                    _items.Add(ItemEnumeration.MixedHerbBlueGreen, "Mixed Herb (Blue & Green)");
                    _items.Add(ItemEnumeration.MixedHerb2GreenBlue, "Mixed Herb (2 Green & Blue)");
                    _items.Add(ItemEnumeration.MixedHerb3Green, "Mixed Herb (3 Green)");
                    _items.Add(ItemEnumeration.MixedHerbGreenBlueRed, "Mixed Herb (Green, Blue & Red)");
                    _items.Add(ItemEnumeration.MagnumBulletsInsideCase, "Magnum Bullets (Inside Case)");
                    _items.Add(ItemEnumeration.InkRibbon, "Ink Ribbon");
                    _items.Add(ItemEnumeration.Magnum, "Magnum (Colt Python)");
                    _items.Add(ItemEnumeration.GoldLugers, "Gold Lugers");
                    _items.Add(ItemEnumeration.SubMachineGun, "Sub Machine Gun (Ingram)");
                    _items.Add(ItemEnumeration.BowGunPowder, "Bow Gun Powder");
                    _items.Add(ItemEnumeration.GunPowderArrow, "Gun Powder Arrow");
                    _items.Add(ItemEnumeration.BOWGasRounds, "BOW Gas Rounds");
                    _items.Add(ItemEnumeration.MGunBullets, "M. Gun Bullets (Ingram)");
                    _items.Add(ItemEnumeration.GasMask, "Gas Mask");
                    _items.Add(ItemEnumeration.RifleBullets, "Rifle Bullets (MR7)");
                    _items.Add(ItemEnumeration.DuraluminCaseUnused, "Duralumin (Unused)");
                    _items.Add(ItemEnumeration.ARifleBullets, "A. Rifle Bullets");
                    _items.Add(ItemEnumeration.AlexandersPierce, "Alexander's Pierce");
                    _items.Add(ItemEnumeration.AlexandersJewel, "Alexander's Jewel");
                    _items.Add(ItemEnumeration.AlfredsRing, "Alfred's Ring");
                    _items.Add(ItemEnumeration.AlfredsJewel, "Alfred's Jewel");
                    _items.Add(ItemEnumeration.PrisonersDiary, "Prisoner's Diary");
                    _items.Add(ItemEnumeration.DirectorsMemo, "Director's Memo");
                    _items.Add(ItemEnumeration.Instructions, "Instructions");
                    _items.Add(ItemEnumeration.Lockpick, "Lockpick");
                    _items.Add(ItemEnumeration.GlassEye, "Glass Eye");
                    _items.Add(ItemEnumeration.PianoRoll, "Piano Roll");
                    _items.Add(ItemEnumeration.SteeringWheel, "Steering Wheel");
                    _items.Add(ItemEnumeration.CraneKey, "Crane Key");
                    _items.Add(ItemEnumeration.Lighter, "Lighter");
                    _items.Add(ItemEnumeration.EaglePlate, "Eagle Plate");
                    _items.Add(ItemEnumeration.SidePack, "Side Pack");
                    _items.Add(ItemEnumeration.MapRoll, "Map (Roll)");
                    _items.Add(ItemEnumeration.HawkEmblem, "Hawk Emblem");
                    _items.Add(ItemEnumeration.QueenAntObject, "Queen Ant Object");
                    _items.Add(ItemEnumeration.KingAntObject, "King Ant Object");
                    _items.Add(ItemEnumeration.BiohazardCard, "Biohazard Card");
                    _items.Add(ItemEnumeration.DuraluminCaseM93RParts, "Duralumin (M93R Parts)");
                    _items.Add(ItemEnumeration.Detonator, "Detonator");
                    _items.Add(ItemEnumeration.ControlLever, "Control Lever");
                    _items.Add(ItemEnumeration.GoldDragonfly, "Gold Dragonfly");
                    _items.Add(ItemEnumeration.SilverKey, "Silver Key");
                    _items.Add(ItemEnumeration.GoldKey, "Gold Key");
                    _items.Add(ItemEnumeration.ArmyProof, "Army Proof");
                    _items.Add(ItemEnumeration.NavyProof, "Navy Proof");
                    _items.Add(ItemEnumeration.AirForceProof, "Air Force Proof");
                    _items.Add(ItemEnumeration.KeyWithTag, "Key With Tag");
                    _items.Add(ItemEnumeration.IDCard, "ID Card");
                    _items.Add(ItemEnumeration.Map, "Map");
                    _items.Add(ItemEnumeration.AirportKey, "Airport Key");
                    _items.Add(ItemEnumeration.EmblemCard, "Emblem Card");
                    _items.Add(ItemEnumeration.SkeletonPicture, "Skeleton Picture");
                    _items.Add(ItemEnumeration.MusicBoxPlate, "Music Box Plate");
                    _items.Add(ItemEnumeration.GoldDragonflyNoWings, "Gold Dragonfly (No Wings)");
                    _items.Add(ItemEnumeration.Album, "Album");
                    _items.Add(ItemEnumeration.Halberd, "Halberd");
                    _items.Add(ItemEnumeration.Extinguisher, "Extinguisher");
                    _items.Add(ItemEnumeration.Briefcase, "Briefcase");
                    _items.Add(ItemEnumeration.PadlockKey, "Padlock Key");
                    _items.Add(ItemEnumeration.TG01, "TG-01");
                    _items.Add(ItemEnumeration.SpAlloyEmblem, "Sp. Alloy Emblem");
                    _items.Add(ItemEnumeration.ValveHandle, "Valve Handle");
                    _items.Add(ItemEnumeration.OctaValveHandle, "Octa Valve Handle");
                    _items.Add(ItemEnumeration.MachineRoomKey, "Machine Room Key");
                    _items.Add(ItemEnumeration.MiningRoomKey, "Mining Room Key");
                    _items.Add(ItemEnumeration.BarCodeSticker, "Bar Code Sticker");
                    _items.Add(ItemEnumeration.SterileRoomKey, "Sterile Room Key");
                    _items.Add(ItemEnumeration.DoorKnob, "Door Knob");
                    _items.Add(ItemEnumeration.BatteryPack, "Battery Pack");
                    _items.Add(ItemEnumeration.HemostaticWire, "Hemostatic (Wire)");
                    _items.Add(ItemEnumeration.TurnTableKey, "Turn Table Key");
                    _items.Add(ItemEnumeration.ChemStorageKey, "Chem. Storage Key");
                    _items.Add(ItemEnumeration.ClementAlpha, "Clement Alpha");
                    _items.Add(ItemEnumeration.ClementSigma, "Clement Sigma");
                    _items.Add(ItemEnumeration.TankObject, "Tank Object");
                    _items.Add(ItemEnumeration.SpAlloyEmblemUnused, "Sp. Alloy Emblem (Unused)");
                    _items.Add(ItemEnumeration.AlfredsMemo, "Alfred's Memo");
                    _items.Add(ItemEnumeration.RustedSword, "Rusted Sword");
                    _items.Add(ItemEnumeration.Hemostatic, "Hemostatic");
                    _items.Add(ItemEnumeration.SecurityCard, "Security Card");
                    _items.Add(ItemEnumeration.SecurityFile, "Security File");
                    _items.Add(ItemEnumeration.AlexiasChoker, "Alexia's Choker");
                    _items.Add(ItemEnumeration.AlexiasJewel, "Alexia's Jewel");
                    _items.Add(ItemEnumeration.QueenAntRelief, "Queen Ant Relief");
                    _items.Add(ItemEnumeration.KingAntRelief, "King Ant Relief");
                    _items.Add(ItemEnumeration.RedJewel, "Red Jewel");
                    _items.Add(ItemEnumeration.BlueJewel, "Blue Jewel");
                    _items.Add(ItemEnumeration.Socket, "Socket");
                    _items.Add(ItemEnumeration.SqValveHandle, "Sq. Valve Handle");
                    _items.Add(ItemEnumeration.Serum, "Serum");
                    _items.Add(ItemEnumeration.EarthenwareVase, "Earthenware Vase");
                    _items.Add(ItemEnumeration.PaperWeight, "Paper Weight");
                    _items.Add(ItemEnumeration.SilverDragonflyNoWings, "Silver Dragonfly (No Wings)");
                    _items.Add(ItemEnumeration.SilverDragonfly, "Silver Dragonfly");
                    _items.Add(ItemEnumeration.WingObject, "Wing Object");
                    _items.Add(ItemEnumeration.Crystal, "Crystal");
                    _items.Add(ItemEnumeration.GoldDragonfly1Wing, "Gold Dragonfly (1 Wing)");
                    _items.Add(ItemEnumeration.GoldDragonfly2Wings, "Gold Dragonfly (2 Wings)");
                    _items.Add(ItemEnumeration.GoldDragonfly3Wings, "Gold Dragonfly (3 Wings)");
                    _items.Add(ItemEnumeration.File, "File");
                    _items.Add(ItemEnumeration.PlantPot, "Plant Pot");
                    _items.Add(ItemEnumeration.PictureB, "Picture B");
                    _items.Add(ItemEnumeration.DuraluminCaseBowGunPowder, "Duralumin (Bow Gun Powder)");
                    _items.Add(ItemEnumeration.DuraluminCaseMagnumRounds, "Duralumin (Magnum Rounds)");
                    _items.Add(ItemEnumeration.BowGunPowderUnused, "Bow Gun Powder (Unused)");
                    _items.Add(ItemEnumeration.EnhancedHandgun, "Enhanced Handgun (Modified Glock 17)");
                    _items.Add(ItemEnumeration.Memo, "Memo");
                    _items.Add(ItemEnumeration.BoardClip, "Board Clip");
                    _items.Add(ItemEnumeration.Card, "Card");
                    _items.Add(ItemEnumeration.NewspaperClip, "Newspaper Clip");
                    _items.Add(ItemEnumeration.LugerReplica, "Luger Replica");
                    _items.Add(ItemEnumeration.QueenAntReliefComplete, "Queen Ant Relief (Complete)");
                    _items.Add(ItemEnumeration.FamilyPicture, "Family Picture");
                    _items.Add(ItemEnumeration.FileFolders, "File Folders");
                    _items.Add(ItemEnumeration.RemoteController, "Remote Controller");
                    _items.Add(ItemEnumeration.QuestionA, "? A");
                    _items.Add(ItemEnumeration.M1P, "M-100P");
                    _items.Add(ItemEnumeration.CalicoBullets, "Calico Bullets (M-100P)");
                    _items.Add(ItemEnumeration.ClementMixture, "Clement Mixture");
                    _items.Add(ItemEnumeration.PlayingManual, "Playing Manual");
                    _items.Add(ItemEnumeration.QuestionB, "? B");
                    _items.Add(ItemEnumeration.QuestionC, "? C");
                    _items.Add(ItemEnumeration.QuestionD, "? D");
                    _items.Add(ItemEnumeration.EmptyExtinguisher, "Empty Extinguisher");
                    _items.Add(ItemEnumeration.SquareSocket, "Square Socket");
                    _items.Add(ItemEnumeration.QuestionE, "? E");
                    _items.Add(ItemEnumeration.CrestKeyS, "Crest Key S");
                    _items.Add(ItemEnumeration.CrestKeyG, "Crest Key G");
                    _items.Add(ItemEnumeration.Unknown, "Unknown");
                }

                return _items;
            }
        }

        private static Dictionary<EnemyEnumeration, string> _actors = new();
        public static Dictionary<EnemyEnumeration, string> Enemys
        {
            get
            {
                if (_actors.Count == 0)
                {
                    _actors.Add(EnemyEnumeration.None, "None");
                    _actors.Add(EnemyEnumeration.ExplosiveBarrel, "Explosive Barrel");
                    _actors.Add(EnemyEnumeration.Zombie, "Zombie");
                    _actors.Add(EnemyEnumeration.GlupWorm, "Glup Worm");
                    _actors.Add(EnemyEnumeration.BlackWidow, "Black Widow");
                    _actors.Add(EnemyEnumeration.ZombieDog, "Zombie Dog");
                    _actors.Add(EnemyEnumeration.Hunter, "Hunter");
                    _actors.Add(EnemyEnumeration.Moth, "Moth");
                    _actors.Add(EnemyEnumeration.Bat, "Bat");
                    _actors.Add(EnemyEnumeration.Bandersnatch, "Bandersnatch");
                    _actors.Add(EnemyEnumeration.Ant, "Ant");
                    _actors.Add(EnemyEnumeration.Spotter, "Spotter");
                    _actors.Add(EnemyEnumeration.AlexiaAshfordA, "Alexia Ashford First Stage");
                    _actors.Add(EnemyEnumeration.AlexiaAshfordB, "Alexia Ashford Second Stage");
                    _actors.Add(EnemyEnumeration.AlexiaAshfordC, "Alexia Ashford Final Stage");
                    _actors.Add(EnemyEnumeration.Nosferatu, "Nosferatu");
                    _actors.Add(EnemyEnumeration.SniperRifle, "Sniper Rifle");
                    _actors.Add(EnemyEnumeration.MutatedSteve, "Mutated Steve");
                    _actors.Add(EnemyEnumeration.Tyrant, "Tyrant");
                    _actors.Add(EnemyEnumeration.AlbinoidInfant, "Albinoid Infant");
                    _actors.Add(EnemyEnumeration.AlbinoidAdult, "Albinoid Adult");
                    _actors.Add(EnemyEnumeration.GiantBlackWidow, "Giant Black Widow");
                    _actors.Add(EnemyEnumeration.BabyBlackWidow, "Baby Black Widow");
                    _actors.Add(EnemyEnumeration.AnatomistZombie, "Anatomist Zombie");
                    _actors.Add(EnemyEnumeration.Tenticle, "Tenticle");
                    _actors.Add(EnemyEnumeration.AlexiaBaby, "Alexia Baby");

                    _actors.Add(EnemyEnumeration.Fish, "Unknown");
                    _actors.Add(EnemyEnumeration.Unknown20, "Unknown");
                    _actors.Add(EnemyEnumeration.Unknown25, "Unknown");

                    _actors.Add(EnemyEnumeration.Unknown42, "Unknown");
                    _actors.Add(EnemyEnumeration.Unknown43, "Unknown");
                    _actors.Add(EnemyEnumeration.Unknown44, "Unknown");
                    _actors.Add(EnemyEnumeration.Unknown47, "Unknown");
                    _actors.Add(EnemyEnumeration.Unknown49, "Unknown");

                    _actors.Add(EnemyEnumeration.FatherBurnside, "Father Burnside");
                    _actors.Add(EnemyEnumeration.AlexanderAshford, "Alexander Ashford (Nosferatu)");
                    _actors.Add(EnemyEnumeration.Unknown53, "Unknown");
                    _actors.Add(EnemyEnumeration.Unknown54, "Unknown");
                    _actors.Add(EnemyEnumeration.Unknown55, "Unknown");
                    _actors.Add(EnemyEnumeration.Unknown56, "Unknown");
                    _actors.Add(EnemyEnumeration.Unknown58, "Unknown");
                    _actors.Add(EnemyEnumeration.Unknown59, "Unknown");

                    _actors.Add(EnemyEnumeration.Unknown60, "Unknown");
                    _actors.Add(EnemyEnumeration.Unknown61, "Unknown");
                    _actors.Add(EnemyEnumeration.Unknown62, "Unknown");
                    _actors.Add(EnemyEnumeration.Unknown63, "Unknown");
                    _actors.Add(EnemyEnumeration.Unknown65, "Unknown");
                    _actors.Add(EnemyEnumeration.Unknown66, "Unknown");
                    _actors.Add(EnemyEnumeration.Cockroach, "Cockroach");
                    _actors.Add(EnemyEnumeration.Unknown68, "Unknown");
                    _actors.Add(EnemyEnumeration.Rat, "Rat (D.I.J)");

                    _actors.Add(EnemyEnumeration.Unknown70, "Unknown");
                    _actors.Add(EnemyEnumeration.Unknown71, "Unknown");
                    _actors.Add(EnemyEnumeration.Unknown72, "Unknown");
                    _actors.Add(EnemyEnumeration.Unknown73, "Unknown");
                    _actors.Add(EnemyEnumeration.Unknown74, "Unknown");
                    _actors.Add(EnemyEnumeration.Unknown75, "Unknown");
                    _actors.Add(EnemyEnumeration.Unknown76, "Unknown");
                    _actors.Add(EnemyEnumeration.Unknown77, "Unknown");
                    _actors.Add(EnemyEnumeration.Unknown78, "Unknown");
                    _actors.Add(EnemyEnumeration.Unknown79, "Unknown");

                    _actors.Add(EnemyEnumeration.Unknown81, "Unknown");
                    _actors.Add(EnemyEnumeration.Unknown82, "Unknown");
                    _actors.Add(EnemyEnumeration.Unknown83, "Unknown");
                    _actors.Add(EnemyEnumeration.Unknown84, "Unknown");
                    _actors.Add(EnemyEnumeration.Unknown85, "Unknown");
                    _actors.Add(EnemyEnumeration.BodyBag, "Body Bag");
                    _actors.Add(EnemyEnumeration.Unknown88, "Unknown");
                    _actors.Add(EnemyEnumeration.Unknown89, "Unknown");

                    _actors.Add(EnemyEnumeration.Unknown90, "Unknown");
                    _actors.Add(EnemyEnumeration.Claire, "Claire Redfield");
                    _actors.Add(EnemyEnumeration.Chris, "Chris Redfield");
                    _actors.Add(EnemyEnumeration.Steve, "Steve Burnside");
                    _actors.Add(EnemyEnumeration.Alfred, "Alfred Ashford");
                    _actors.Add(EnemyEnumeration.Alexia, "Alexia Ashford");
                    _actors.Add(EnemyEnumeration.AlfredMakeup, "Alfred Ashford (Makeup)");
                    _actors.Add(EnemyEnumeration.Wesker, "Albert Wesker");
                    _actors.Add(EnemyEnumeration.Rodrigo, "Rodrigo Raval");
                    _actors.Add(EnemyEnumeration.Scientist, "Scientist");
                }

                return _actors;
            }
        }

        private static Dictionary<AOTTypeEnumeration, string> _aottypes = new();
        public static Dictionary<AOTTypeEnumeration, string> AOTTypes
        {
            get
            {
                if (_aottypes.Count == 0)
                {
                    _aottypes.Add(AOTTypeEnumeration.Door, "Door");
                    _aottypes.Add(AOTTypeEnumeration.Unknown1, "Unknown1");
                    _aottypes.Add(AOTTypeEnumeration.Unknown2, "Unknown2");
                    _aottypes.Add(AOTTypeEnumeration.Message, "Message");
                    _aottypes.Add(AOTTypeEnumeration.Item, "Item");
                }

                return _aottypes;
            }
        }
    }
}