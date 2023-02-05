
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace WTA.Web.Models;

[Display(Name = "登录")]//已扩展到title
[Description("登录简介")]
public class TestViewModel
{
    [Description("数字、字母、下划线组成，长度不超过20")]
    [StringLength(20, MinimumLength = 5)]
    [RegularExpression("[a-zA-Z0-9_]+")]
    [UIHint("UserNameUI")]//已扩展到format
    [DataType("custom")]//已扩展到format
    [Required]
    [Remote("Valid", "Home", ErrorMessage = "RemoteAttribute")]//未解析，待解决
    [Display(Name = "UserName", Description = "UserName.Desc")]
    public string? UserName { get; set; }

    [Required]
    [DataType(DataType.Password)]//已扩展到format
    [Display(Name = "密码")]
    public string? Password { get; set; }

    [Required]
    [Compare(nameof(Password))]
    [DataType(DataType.Password)]//已扩展到format
    [Display(Name = "确认密码")]
    public string? ConfirmPassword { get; set; }

    [DefaultValue("default value")]
    [Display(Name = "默认值")]
    public string? Default { get; set; }

    [Editable(true)]
    [Display(Name = "可编辑")]
    public string? Editable { get; set; }

    [Range(typeof(int), "1", "3")]
    [Display(Name = "整数范围必选")]
    public int Range1 { get; set; }

    [Range(typeof(int), "1", "3")]
    [Display(Name = "整数范围可选")]
    public int? Range2 { get; set; }

    [DataType(DataType.MultilineText)]
    [Display(Name = "多行文本必选")]
    public string? MultilineText1 { get; set; }

    [DataType(DataType.MultilineText)]
    [Display(Name = "多行文本可选")]
    public string? MultilineText2 { get; set; }

    [Display(Name = "开关必选")]
    public bool Switch1 { get; set; }

    [Display(Name = "开关可选")]
    public bool? Switch2 { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "日期必选")]
    public DateTime Date1 { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "日期可选")]
    public DateTime? Date2 { get; set; }

    [Required]
    [DataType(DataType.Time)]
    [Display(Name = "时间必选")]
    public DateTime Time1 { get; set; }

    [DataType(DataType.Time)]
    [Display(Name = "时间可选")]
    public DateTime? Time2 { get; set; }

    [Required]
    [Display(Name = "日期时间必选")]
    public DateTime DateTime1 { get; set; }

    [Display(Name = "日期时间可选")]
    public DateTime? DateTime2 { get; set; }

    [Display(Name = "枚举必选")]
    public TestEnum Enum1 { get; set; }

    [Display(Name = "枚举可选")]
    public TestEnum Enum2 { get; set; }

    [DataType("selectlist")]
    [Display(Name = "单选必选")]
    public string? SelectList1 { get; set; }

    [DataType("selectlist")]
    [Display(Name = "单选可选")]
    public string? SelectList2 { get; set; }

    [Display(Name = "子属性")]//????
    public TestViewModel2? Sub { get; set; }

    [Display(Name = "集合属性")]//????
    public List<string> Values1 { get; set; } = new List<string> { "1", "2", "3" };

    [Display(Name = "集合属性")]//????
    public List<TestViewModel2> Values2 { get; set; } = new List<TestViewModel2> { new TestViewModel2(), new TestViewModel2() };
}

public enum TestEnum
{
    [Display(Name = "Option1")]
    Option1,

    [Display(Name = "枚举2")]
    Option2
}

public class TestViewModel2
{
    public int IntValue { get; set; }
    public string? StringValue { get; set; }
}
