# VINWMIVehicles

VIN/WMI vyhledavac vozidel — .NET 10 Blazor Server aplikace pro vyhledavani a spravovani informaci o vozidlech na zaklade VIN kodu, WMI kodu a WMC kodu.

## Technologie

- .NET 10, Blazor Server, `@rendermode InteractiveServer`
- Entity Framework Core — `AppDbContextVehicle` (pres SharedServices)
- `EfCoreService<AppDbContextVehicle>` pro databazove operace
- Bootstrap 5, Bootstrap Icons (`bi bi-*`)
- `PageLoadingSpinner` komponenta pro stavy nacitani

## Struktura projektu

```
src/
  VINWMIVehicles.Web/
    Components/
      Layout/          # NavMenu.razor, MainLayout.razor
      Pages/
        Admin/
          AdminDashboard.razor   # /admin
          WmiImport.razor        # /admin/wmi-import
        Home.razor               # / (dashboard)
        WMISearch.razor          # /wmi-search
        WMCSearch.razor          # /wmc-search
        VINSearch.razor          # /vin-search
        CustomVINSearch.razor    # /custom-vin
        History.razor            # /history
        AccessDenied.razor
  SharedServices/       # git submodule — sdilene modely, DbContext, services
```

## Navigace (NavMenu.razor)

Dashboard, WMI Search, WMC Search, VIN Search, Custom VIN, History, Admin, WMI Import

## Klic modely (SharedServices.Models.Car)

- `WmiEntry` — WMI/WMC kod, `CodeType`, `CountryISO`, `Region`, kolekce `Manufacturers`
- `VinInfo` — `Vin`, `Make`, `Model`, `ModelYear`, `Note`

## Auth a role

- Stranky chrany `@attribute [Authorize]`
- Admin sekce: `@attribute [Authorize(Roles = "Admin")]`
- Pro zobrazeni jmena uzivatele: `@inject AuthenticationStateProvider AuthenticationStateProvider`

## Konvence

- `EfCoreService<TContext>` — univerzalni databazova sluzba (GetWmiEntries, GetVins, ...)
- `PageLoadingSpinner` — zobrazit behem `IsLoading == true`
- Taby na History strance: `ActiveTab` ("wmi" / "vin") ridit `SetTab(string)`
- Search/filter: `@bind` na `_search` + `@oninput` event pro real-time filtrovani
- `ThemePicker` — komponenta pro prepinani temat (svetle/tmave)
