using System.ComponentModel.DataAnnotations;

namespace WTA.Application.Abstractions.Domain;

[Flags]
public enum OperationType
{
  None = 0,

  [Display(Name = "查询")]
  List = 1,

  [Display(Name = "新建")]
  Create = 2,

  [Display(Name = "编辑")]
  Update = 4,

  [Display(Name = "删除")]
  Delete = 8,

  [Display(Name = "导入")]
  Import = 16,

  [Display(Name = "导出")]
  Export = 32,

  All = List | Create | Update | Delete | Import | Export
}
