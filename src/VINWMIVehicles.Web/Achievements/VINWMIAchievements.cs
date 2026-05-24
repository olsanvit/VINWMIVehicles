using SharedServices.Models.Achievement;

namespace VINWMIVehicles.Web.Achievements;

public static class VINWMIAchievements
{
    public static readonly IReadOnlyList<AchievementDef> All = new List<AchievementDef>
    {
        // ── VIN Vyhledávání ───────────────────────────────────────────────────
        new("VW_VIN_FIRST",           "První VIN",            "Prohledej první VIN číslo",                            "bi-upc-scan",             10, "VIN"),
        new("VW_VIN_DECODED",         "Dekodér",              "Úspěšně dekóduj VIN číslo",                            "bi-code-square",          15, "VIN"),
        new("VW_VIN_CUSTOM",          "Vlastní VIN",          "Zadej vlastní VIN číslo k vyhledání",                  "bi-keyboard",             10, "VIN"),
        new("VW_VIN_MULTI_5",         "5 VINů",               "Prohledej 5 různých VIN čísel",                        "bi-repeat",               15, "VIN"),
        new("VW_VIN_MULTI_10",        "10 VINů",              "Prohledej 10 různých VIN čísel",                       "bi-10-circle",            25, "VIN"),
        new("VW_VIN_MULTI_50",        "50 VINů",              "Prohledej 50 různých VIN čísel",                       "bi-50-circle",            50, "VIN"),
        new("VW_VIN_MULTI_100",       "100 VINů",             "Prohledej 100 různých VIN čísel",                      "bi-100",                  75, "VIN"),
        new("VW_VIN_INVALID",         "Neplatný VIN",         "Vyhledej neplatné VIN číslo",                          "bi-x-circle",             10, "VIN"),
        new("VW_VIN_VALID",           "Platný VIN",           "Ověř platné VIN číslo",                                "bi-check-circle",         10, "VIN"),
        new("VW_VIN_USA",             "Americké auto",        "Najdi americké auto (VIN začíná 1,4,5)",               "bi-flag",                 15, "VIN"),
        new("VW_VIN_GERMANY",         "Německé auto",         "Najdi německé auto (VIN začíná W)",                    "bi-building",             15, "VIN"),
        new("VW_VIN_JAPAN",           "Japonské auto",        "Najdi japonské auto (VIN začíná J)",                   "bi-globe-asia-australia", 15, "VIN"),
        new("VW_VIN_YEAR_OLD",        "Starý model",          "Najdi auto staré 20+ let",                             "bi-clock-history",        20, "VIN"),
        new("VW_VIN_YEAR_NEW",        "Nový model",           "Najdi auto z posledního roku",                         "bi-calendar-check",       15, "VIN"),
        new("VW_VIN_AI",              "AI asistent",          "Použij AI pro analýzu VIN",                            "bi-robot",                25, "VIN"),
        new("VW_VIN_HISTORY",         "Historie VIN",         "Zobraz historii vyhledávání VIN",                      "bi-clock",                10, "VIN"),
        new("VW_VIN_SAVE",            "Uložit VIN",           "Ulož VIN do historie",                                 "bi-floppy",               10, "VIN"),
        new("VW_VIN_COPY",            "Zkopírovat VIN",       "Zkopíruj VIN číslo do schránky",                       "bi-clipboard",             5, "VIN"),
        new("VW_VIN_QUICK",           "Rychlý dekodér",       "Dekóduj VIN za méně než 2 sekundy",                    "bi-lightning",            15, "VIN"),
        new("VW_VIN_ELECTRIC",        "Elektromobil",         "Najdi VIN elektrického vozidla",                       "bi-lightning-charge",     20, "VIN"),
        new("VW_VIN_TRUCK",           "Nákladní",             "Najdi VIN nákladního vozidla",                         "bi-truck",                15, "VIN"),
        new("VW_VIN_MOTORCYCLE",      "Motocykl",             "Najdi VIN motocyklu",                                  "bi-bicycle",              15, "VIN"),
        new("VW_VIN_LUXURY",          "Luxusní vůz",          "Najdi VIN prémiového vozidla",                         "bi-gem",                  20, "VIN"),
        new("VW_VIN_SPORT",           "Sporťák",              "Najdi VIN sportovního vozidla",                        "bi-speedometer2",         20, "VIN"),
        new("VW_VIN_RANDOM",          "Náhodný VIN",          "Vygeneruj a prohledej náhodný VIN",                    "bi-shuffle",              15, "VIN"),

        // ── WMI / WMC ─────────────────────────────────────────────────────────
        new("VW_WMI_FIRST",           "První WMI",            "Prohledej první WMI kód",                              "bi-search",               10, "WMI"),
        new("VW_WMI_MULTI_5",         "5 WMI kódů",           "Prohledej 5 různých WMI kódů",                         "bi-list-ol",              15, "WMI"),
        new("VW_WMI_MULTI_10",        "10 WMI kódů",          "Prohledej 10 různých WMI kódů",                        "bi-collection",           25, "WMI"),
        new("VW_WMC_FIRST",           "První WMC",            "Prohledej první WMC kód",                              "bi-upc",                  10, "WMI"),
        new("VW_WMC_MULTI_5",         "5 WMC kódů",           "Prohledej 5 různých WMC kódů",                         "bi-card-list",            15, "WMI"),
        new("VW_WMI_USA",             "USA výrobce",          "Najdi amerického výrobce přes WMI",                    "bi-flag",                 10, "WMI"),
        new("VW_WMI_EUROPE",          "Evropský výrobce",     "Najdi evropského výrobce přes WMI",                    "bi-globe-europe-africa",  10, "WMI"),
        new("VW_WMI_ASIA",            "Asijský výrobce",      "Najdi asijského výrobce přes WMI",                     "bi-globe-asia-australia", 10, "WMI"),
        new("VW_WMI_KNOWN",           "Známý výrobce",        "Identifikuj světoznámého výrobce",                     "bi-building-fill-check",  15, "WMI"),
        new("VW_WMI_RARE",            "Vzácný kód",           "Najdi méně známého výrobce přes WMI",                  "bi-eye",                  20, "WMI"),
        new("VW_WMI_ALL_REGIONS",     "Světový průzkumník",   "Prohledej WMI ze všech světadílů",                     "bi-globe",                30, "WMI"),
        new("VW_WMI_AI",              "AI WMI",               "Použij AI pro analýzu WMI",                            "bi-robot",                25, "WMI"),
        new("VW_WMI_FAST",            "Rychlý WMI",           "Prohledej WMI za méně než 2 sekundy",                  "bi-lightning",            15, "WMI"),
        new("VW_WMI_10",              "Zkušený",              "Prohledej 10+ různých WMI",                            "bi-award",                20, "WMI"),
        new("VW_WMI_COMPARE",         "Srovnávač",            "Porovnej 2 různé WMI kódy",                            "bi-intersect",            15, "WMI"),

        // ── Historie ──────────────────────────────────────────────────────────
        new("VW_HIST_FIRST",          "První záznam",         "Zobraz první záznam v historii",                       "bi-clock",                 5, "Historie"),
        new("VW_HIST_10",             "10 záznamů",           "Hledej 10 různých VIN/WMI záznamů",                    "bi-clock-history",        15, "Historie"),
        new("VW_HIST_50",             "50 záznamů",           "Hledej 50 různých VIN/WMI záznamů",                    "bi-journals",             30, "Historie"),
        new("VW_HIST_100",            "100 záznamů",          "Hledej 100 různých VIN/WMI záznamů",                   "bi-archive",              50, "Historie"),
        new("VW_HIST_CLEAR",          "Čistá historia",       "Vymaž historii vyhledávání",                           "bi-trash",                10, "Historie"),
        new("VW_HIST_REVISIT",        "Znovu prohledej",      "Klikni na záznam z historie",                          "bi-arrow-clockwise",      10, "Historie"),
        new("VW_HIST_VIN",            "VIN v historii",       "Zobraz VIN záznamy v historii",                        "bi-upc-scan",              5, "Historie"),
        new("VW_HIST_WMI",            "WMI v historii",       "Zobraz WMI záznamy v historii",                        "bi-upc",                   5, "Historie"),
        new("VW_HIST_PAGE",           "Historie stránka",     "Navštiv stránku Historie",                             "bi-list-ul",               5, "Historie"),
        new("VW_HIST_EXPORT",         "Export historie",      "Exportuj historii vyhledávání",                        "bi-download",             15, "Historie"),

        // ── Dashboard ─────────────────────────────────────────────────────────
        new("VW_DASH_VISIT",          "Dashboard",            "Navštiv hlavní dashboard",                             "bi-house",                 5, "Dashboard"),
        new("VW_DASH_STATS",          "Statistiky",           "Zobraz statistiky vyhledávání",                        "bi-graph-up",             10, "Dashboard"),
        new("VW_DASH_CHART",          "Graf",                 "Prohlédni chart na dashboardu",                        "bi-bar-chart",            10, "Dashboard"),
        new("VW_DASH_REVISIT",        "Pravidelný",           "Navštiv dashboard 5×",                                 "bi-arrow-repeat",         10, "Dashboard"),
        new("VW_DASH_DARK",           "Temný mód",            "Přepni na tmavý režim",                                "bi-moon-fill",            10, "Dashboard"),
        new("VW_DASH_THEME",          "Stylista",             "Změň téma aplikace",                                   "bi-palette",               5, "Dashboard"),
        new("VW_DASH_ALL_PAGES",      "Průzkumník",           "Navštiv všechny stránky aplikace",                     "bi-map",                  20, "Dashboard"),
        new("VW_DASH_NAVMENU",        "Navigátor",            "Použij boční menu k navigaci",                         "bi-layout-sidebar",        5, "Dashboard"),
        new("VW_DASH_MOBILE",         "Mobilní průzkum",      "Otevři aplikaci na mobilním zařízení",                 "bi-phone",                15, "Dashboard"),
        new("VW_DASH_FIRST_USE",      "Startovní čára",       "Použij dashboard poprvé",                              "bi-flag-fill",            10, "Dashboard"),

        // ── Speciální ─────────────────────────────────────────────────────────
        new("VW_SPECIAL_WELCOME",     "Vítej!",               "Otevři aplikaci poprvé",                               "bi-door-open",             5, "Speciální"),
        new("VW_SPECIAL_ACHI_PAGE",   "Sběratel",             "Otevři stránku achievementů",                          "bi-trophy-fill",           5, "Speciální"),
        new("VW_SPECIAL_ACHI_10",     "10 achievementů",      "Odemkni 10 achievementů",                              "bi-award",                20, "Speciální"),
        new("VW_SPECIAL_ACHI_25",     "25 achievementů",      "Odemkni 25 achievementů",                              "bi-award-fill",           40, "Speciální"),
        new("VW_SPECIAL_ACHI_50",     "50 achievementů",      "Odemkni 50 achievementů",                              "bi-stars",                75, "Speciální"),
        new("VW_SPECIAL_ACHI_ALL",    "Platinový detektiv",   "Odemkni všechny achievementy",                         "bi-gem",                 200, "Speciální"),
        new("VW_SPECIAL_500_PTS",     "500 bodů",             "Nashromáždi 500 bodů",                                 "bi-circle-fill",          30, "Speciální"),
        new("VW_SPECIAL_1000_PTS",    "1 000 bodů",           "Nashromáždi 1000 bodů",                                "bi-pentagon-fill",        60, "Speciální"),
        new("VW_SPECIAL_NIGHT_OWL",   "Noční detektiv",       "Prohledej VIN po půlnoci",                             "bi-moon-stars",           15, "Speciální"),
        new("VW_SPECIAL_EARLY_BIRD",  "Ranní průzkumník",     "Prohledej VIN před 7. hodinou",                        "bi-sunrise",              15, "Speciální"),
        new("VW_SPECIAL_AI_POWER",    "AI Power User",        "Použij AI pro VIN i WMI analýzu",                      "bi-robot",                35, "Speciální"),
        new("VW_SPECIAL_MULTI_TYPES", "Všestranný",           "Použij VIN, WMI i WMC vyhledávání",                    "bi-collection-fill",      25, "Speciální"),
        new("VW_SPECIAL_POWER_USER",  "Pokročilý",            "Použij všechny funkce aplikace",                       "bi-person-gear",          30, "Speciální"),
        new("VW_SPECIAL_FIRST_WEEK",  "Týdenní detektiv",     "Prohledej VIN/WMI 7 dní za sebou",                     "bi-calendar-week",        40, "Speciální"),
        new("VW_SPECIAL_DETECTIVE",   "Automobilový detektiv","Prohledej 200+ různých VIN čísel",                     "bi-shield-fill-check",   100, "Speciální"),
    };
}
