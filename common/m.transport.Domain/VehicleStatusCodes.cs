/// <summary>
/// Summary description for VehicleStatusCodes
/// </summary>
public class VehicleStatusCodes
{
    public const int STATUS_NEW = 0;
    public const int STATUS_INVENTORYING = 1;
    public const int STATUS_INVENTORIED = 2;
    public const int STATUS_LOCATING = 3;
    public const int STATUS_LOCATED = 4;
    public const int STATUS_INSPECTING = 5;
    public const int STATUS_INSPECTED = 6;
    public const int STATUS_ASSIGNED = 7;
    public const int STATUS_RESERVED = 8;
    public const int STATUS_NOT_LOADED = 9;
    public const int STATUS_LOADING = 10;
    public const int STATUS_LOADED = 11;
    public const int STATUS_DELIVERING = 12;
    public const int STATUS_DELIVERED = 13;
    public const int STATUS_REMOVING = 14;
    public const int STATUS_REMOVED = 15;

    /// <summary>
    /// Array value of index i corresponds to Vehicle Status code i
    /// </summary>
    public static string[] VehicleStatusCodeText = new string[]
        {
            "New",
            "Inventorying",
            "Inventoried",
            "Locating",
            "Located",
            "Inspecting",
            "Inspected",
            "Assigned",
            "Reserved",
            "Not Loaded",
            "Loading",
            "Loaded",
            "Delivering",
            "Delivered",
            "Removing",
            "Removed"
        };
    
    public const int EXCPTN_NONE = 0;                 // "No Error", // 0
    public const int EXCPTN_VIN_NOT_FOUND = 1;        // "VIN Not Found", // 1
    public const int EXCPTN_VIN_ERRROR = 2;           // "Server Error Retrieving VIN", //2
    public const int EXCPTN_LOADING_DELIVERED = 3;    // "VIN already delivered", // 3
    public const int EXCPTN_LOADING_LOADED = 4;       // "VIN already loaded by another driver", // 4
    public const int EXCPTN_LOADING_ASSIGNED = 5;     // "VIN assigned to another driver", // 5
    public const int EXCPTN_LOADING_RESERVED = 6;     // "VIN reserved by another driver", // 6
    public const int EXCPTN_LOADING_NOT_ASSIGNED = 7; // "VIN was not assigned", // 7
    public const int EXCPTN_DELIVERING_DELIVERED = 8; // "VIN already delivered", // 8
    public const int EXCPTN_UNEXPECTED_LOCATION = 9;  // "Unexpected Delivery Location", // 9
    public const int EXCPTN_DELIV_NO_LOADID = 10;     // "Server Error: No Load ID Found", // 10
    public const int EXCPTN_STOREDPROCEDURE_ERR = 11; // "Stored Procedure returned error: ", // 11
    public const int EXCPTN_ASSIGNED_NOTLOADED = 12;  // "Assigned VIN not loaded.  OK to REMOVE FROM ASSIGNED LOAD", // 12
    public const int EXCPTN_RESERVED_NOTLOADED = 13;  // "Reserved VIN not loaded.  OK to REMOVE RESERVATION", // 13
    public const int EXCPTN_INVALID_PICKUP_TIME = 14;     // "Invalid Pickup DateTime", // 14
    public const int EXCPTN_INVALID_DROPOFF_TIME = 15;    // "Invalid Dropoff DateTime", // 15
    public const int EXCPTN_INVALID_INSPECTION_TIME = 16; // "Invalid Inspection DateTime", // 16
    public const int EXCPTN_GENERAL = 17;                 // "Unexpected error, please call dispatch", // 17
    public const int EXCPTN_VEHICLE_REMOVED_FROM_LOAD = 20;

    public static string[] ExceptionCodeText = new string[]
        {
            "No Error", // 0
            "VIN Not Found", // . OK to LOAD ANYWAY (please call dispatch)", // 1
            "Server Error Retrieving VIN", // .  Please call dispatch", //2
            "VIN already delivered", // . OK to LOAD ANYWAY", // 3
            "VIN already loaded by another driver", // . OK to LOAD ANYWAY", // 4
            "VIN assigned to another driver", // . OK to LOAD ANYWAY", // 5
            "VIN reserved by another driver", // . OK to LOAD ANYWAY", // 6
            "VIN was not assigned", // . OK to LOAD ANYWAY", // 7
            "VIN already delivered", // . OK to DELIVER ANYWAY", // 8
            "Unexpected Delivery Location", // . OK to DELIVER ANYWAY", // 9
            "Server Error: No Load ID Found", // . OK to DELIVER ANYWAY", // 10
            "Stored Procedure returned error: ", // 11
            "Assigned VIN not loaded", // .  OK to REMOVE FROM ASSIGNED LOAD", // 12
            "Reserved VIN not loaded", // .  OK to REMOVE RESERVATION", // 13
            "Invalid Pickup DateTime", // 14
            "Invalid Dropoff DateTime", // 15
            "Invalid Inspection DateTime", // 16
            "Unexpected error, please call dispatch", // 17
            "", // 18
            "", // 19
            "VIN Removed From Load" // 20
        };
}
