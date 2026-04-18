namespace VINWMIVehicles.Domain;

public enum DataSource { Manual, Nhtsa, Iso, EcRegister, JapanRegistry, Scraped, Import }
public enum WmcCodeType { Wmc, NhtsaId, EcCode, JapanCode, LocalRegistry, Other }
public enum FuelType { Petrol, Diesel, Electric, HybridPetrol, HybridDiesel, PlugInHybrid, Lpg, Cng, Hydrogen, Other }
public enum TransmissionType { Manual, Automatic, SemiAutomatic, Dct, Cvt, Amt }
public enum DriveType { Fwd, Rwd, Awd, FourWd }
public enum VehicleCategory { Passenger, Commercial, Motorcycle, Bus, Special }
