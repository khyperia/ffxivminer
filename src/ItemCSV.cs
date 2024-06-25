using CsvHelper.Configuration.Attributes;

namespace Parser
{
    // #, Singular, Adjective, Plural, PossessivePronoun, StartsWithVowel, , Pronoun, Article, Description, Name, Icon,
    // Level{Item}, Rarity, FilterGroup, AdditionalData, ItemUICategory, ItemSearchCategory, EquipSlotCategory,
    // ItemSortCategory, , StackSize, IsUnique, IsUntradable, IsIndisposable, Lot, Price{Mid}, Price{Low}, CanBeHq,
    // IsDyeable, IsCrestWorthy, ItemAction, CastTime<s>, Cooldown<s>, ClassJob{Repair}, Item{Repair}, Item{Glamour},
    // Desynth, IsCollectable, AlwaysCollectable, AetherialReduce, Level{Equip}, , EquipRestriction, ClassJobCategory,
    // GrandCompany, ItemSeries, BaseParamModifier, Model{Main}, Model{Sub}, ClassJob{Use}, , Damage{Phys}, Damage{Mag},
    // Delay<ms>, , BlockRate, Block, Defense{Phys}, Defense{Mag}, BaseParam[0], BaseParamValue[0], BaseParam[1],
    // BaseParamValue[1], BaseParam[2], BaseParamValue[2], BaseParam[3], BaseParamValue[3], BaseParam[4],
    // BaseParamValue[4], BaseParam[5], BaseParamValue[5], ItemSpecialBonus, ItemSpecialBonus{Param},
    // BaseParam{Special}[0], BaseParamValue{Special}[0], BaseParam{Special}[1], BaseParamValue{Special}[1],
    // BaseParam{Special}[2], BaseParamValue{Special}[2], BaseParam{Special}[3], BaseParamValue{Special}[3],
    // BaseParam{Special}[4], BaseParamValue{Special}[4], BaseParam{Special}[5], BaseParamValue{Special}[5],
    // MaterializeType, MateriaSlotCount, IsAdvancedMeldingPermitted, IsPvP, SubStatCategory, IsGlamourous


    // 36212,"bolt of AR-Caean velvet",0,"bolts of AR-Caean velvet",0,0,1,0,0,"A rare fabric that delights mankind for
    // unknown and possibly sinister reasons.","AR-Caean
    // Velvet","ui/icon/021000/021620.tex","545",1,12,,"Cloth","Cloth","EquipSlotCategory#0","40",2000,999,False,False,False,False,99999,4,True,False,False,"ItemAction#0",2,0,"adventurer","ItemRepairResource#0","",0,False,False,0,1,0,0,"","None","",0,"0,
    // 0, 0, 0","0, 0, 0,
    // 0","adventurer",0,0,0,0,0,0,0,0,0,"",0,"",0,"",0,"",0,"",0,"",0,"",0,"",0,"",0,"",0,"",0,"",0,"",0,0,0,False,False,0,False
    internal class ItemCSV
    {
        [Name("#")]
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}
