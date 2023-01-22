using System.ComponentModel;

namespace SpmcoGateAutomation.Common.Enums
{
    public enum CameraSide : short
    {
        Left,
        Right,
        Back,
        Plate
    }
    public enum GateType : short
    {
        GateIn,
        GateOut
    }
    public enum GateMessagesTemplate : short
    {
        [Description("شماره ی پلاک یافت نشد")]
        PlateNoHasNotBeenFound = 0,

        [Description("مجوز بارگیری معتبر یافت نشد")]
        ValidDeliveryPermissionHasNotBeenFound = 1,

        [Description("ورود تکراری")]
        GateInInfoHasBeenSavedAlready = 2,

        [Description("شماره کانتینر یافت نشد")]
        ContainerNoHasNotBeenFound = 3,

        [Description("خروجی تکراری")]
        GateOutInfoHasBeenSavedAlready = 4,

        [Description("بیجک صادر نشده")]
        BijakHasNotBeenIssued = 5,
    }

    public enum GeneralTypes : byte
    {
        VoyageType = 1,
        VoyageStatus = 2,
        BlType = 3,
        ManifestType = 4,
        FullEmptyStatusManifest = 5,
        EquipmentType = 6,
        ReceiptType = 7,
        ReceiptStatus = 8,
        Color = 10,
        BerthType = 11,
        ImdgType = 12,
        ContainerType = 13,
        DeliverCarryType = 14,
        FullEmptyStatusContainer = 15,
        LoadingInstructionStatus = 16,
        EvaluationType = 17,
        StripType = 17,
        EvaluationReason = 18,
        StriptDestination = 19,
        ExitPermitType = 20,
        SealStatus = 21,
        OtherServiceTariffType = 22,
        ContainerSize = 23,
        MovementType = 24,
        MovementMoveType = 25,
        CommonContainer = 26,
        MovementReqMoveType = 27,
        InvoiceType = 30,
        OwnerAccName = 31,
        EnterExitType = 32,
        ReportDataType = 33,
        ReportOperators = 34,
        ReportParrentType = 35,
        BlCorrectionType = 38,
        ThcActivityType = 39,
        SpecialZoneAction = 40,
        PriceEmptyContainer = 41,
        ConsigneeType = 42,
        LiftingType = 43,
        LiftingMoveType = 44,
        ManifestMovenType = 45,
        ExportInstructionType = 46,
        CalculationType = 47,
        WarehouseSecurityCalcType = 49,
        DeliverSource = 50,
        ConsigneeOtherServices = 57,
        CFSEntryType = 58,
        CFSWareHouseStatus = 59,
        WareHouse = 60,
        WarehouseNo = 62
    }
}