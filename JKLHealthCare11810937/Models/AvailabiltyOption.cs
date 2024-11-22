using System.ComponentModel.DataAnnotations;
using System.Reflection;
using JKLHealthCare11810937.Models;

namespace JKLHealthCare11810937.Models
{
    public enum AvailabilityOption
    {
        [Display(Name = "Monday Morning (9am-12pm)")]
        MondayMorning,

        [Display(Name = "Monday Afternoon (1pm-5pm)")]
        MondayAfternoon,

        [Display(Name = "Tuesday Morning (9am-12pm)")]
        TuesdayMorning,

        [Display(Name = "Tuesday Afternoon (1pm-5pm)")]
        TuesdayAfternoon,

        [Display(Name = "Wednesday Morning (9am-12pm)")]
        WednesdayMorning,

        [Display(Name = "Wednesday Afternoon (1pm-5pm)")]
        WednesdayAfternoon,

        [Display(Name = "Thursday Morning (9am-12pm)")]
        ThursdayMorning,

        [Display(Name = "Thursday Afternoon (1pm-5pm)")]
        ThursdayAfternoon,

        [Display(Name = "Friday Morning (9am-12pm)")]
        FridayMorning,

        [Display(Name = "Friday Afternoon (1pm-5pm)")]
        FridayAfternoon,

        [Display(Name = "Saturday Morning (9am-12pm)")]
        SaturdayMorning,

        [Display(Name = "Saturday Afternoon (1pm-5pm)")]
        SaturdayAfternoon,

        [Display(Name = "Sunday Morning (9am-12pm)")]
        SundayMorning,

        [Display(Name = "Sunday Afternoon (1pm-5pm)")]
        SundayAfternoon
    }
}

public static class EnumExtensions
{
    public static string GetDisplayName(this AvailabilityOption enumValue)
    {
        var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
        var displayAttribute = fieldInfo?.GetCustomAttribute<DisplayAttribute>();

        return displayAttribute?.Name ?? enumValue.ToString();
    }
}