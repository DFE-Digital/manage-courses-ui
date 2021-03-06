﻿namespace ManageCoursesUi.Tests.Enums
{
    /// <summary>
    /// Defines the type of data that needs to be generated by the helper class
    /// </summary>
    public enum EnumDataType
    {
        SingleVariantOneMatch,//data setup with one matching variant
        MultiVariantOneMatch,//data setup with multiple variants and only one metching variant
        SingleVariantNoMatch,//data setup with one none matching variant
        MultiVariantNoMatch//data setup with multiple variants none matching
    }
}
