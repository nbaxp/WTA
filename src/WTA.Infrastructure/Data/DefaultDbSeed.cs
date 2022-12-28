using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using WTA.Application.Abstractions;
using WTA.Application.Abstractions.Data;
using WTA.Application.Abstractions.Domain;
using WTA.Application.Abstractions.Extensions;
using WTA.Application.Domain.Blogs;
using WTA.Application.Domain.System;

namespace WTA.Infrastructure.Data;

public class DefaultDbSeed : IDbSeed
{
  private readonly DbContext _db;
  private readonly IStringLocalizer _localizer;
  private readonly IPasswordHasher _passwodHasher;

  public DefaultDbSeed(DbContext db, IStringLocalizer localizer, IPasswordHasher passwodHasher)
  {
    this._db = db;
    this._localizer = localizer;
    this._passwodHasher = passwodHasher;
  }

  public async Task Seed()
  {
    try
    {
      await this.InitSystem();
      // User
      var salt = _passwodHasher.CreateSalt();
      //UserRoles = new List<UserRole>
      //  {
      //    new UserRole {
      //      Role = new Role {
      //        Name = "超级管理员",
      //        RolePermissions =
      //        new List<RolePermission>
      //        {
      //          new RolePermission
      //          {
      //            Permission = new Permission { Name = "测试权限" }
      //          }
      //        }
      //      }
      //    }
      //  }
      // 创建内置角色
      var superRole = new Role { Nummber = "super", Name = "超级管理员" };
      var adminRole = new Role { Nummber = "admin", Name = "管理员" };
      await _db.Set<Role>().AddAsync(superRole);
      await _db.Set<Role>().AddAsync(adminRole);
      var super = new User
      {
        UserName = "super",
        NormalizedUserName = "super".Normalized(),
        SecurityStamp = salt,
        PasswordHash = _passwodHasher.HashPassword("123456", salt),
        UserRoles = new List<UserRole> { new UserRole { RoleId = superRole.Id } }
      };
      var admin = new User
      {
        UserName = "admin",
        NormalizedUserName = "admin".Normalized(),
        SecurityStamp = salt,
        PasswordHash = _passwodHasher.HashPassword("123456", salt),
        UserRoles = new List<UserRole> { new UserRole { RoleId = adminRole.Id } }
      };
      await _db.Set<User>().AddAsync(super);
      await _db.Set<User>().AddAsync(admin);
      await _db.SaveChangesAsync();

      // Blog
      await _db.Set<BlogPost>().AddAsync(new BlogPost
      {
        User = _db.Set<User>().First(o => o.UserName == "admin"),
        Title = "领域驱动系列：三种领域逻辑组织模式的本质",
        CreatedOn = DateTimeOffset.Now,
        BodyOverview = "企业应用架构模式中明确提出了三种领域逻辑组织模式：事务脚本、领域模型和表模块。不少人看的云里雾里的，不少人说的似懂非懂的，主要原因是没有从项目的级别的分析和设计经验，只有单个项目模块的开发经验的人很难理解到位。事务脚本的理解其实最简单，但是很多人说不清，觉得比领域模型还难理解，也对应不到代码。但这只是幻觉，怎么可能最简单的领域逻辑模式都不懂，反而对最复杂的领域模型模式懂了呢。",
        Body = "企业应用架构模式中明确提出了三种领域逻辑组织模式：事务脚本、领域模型和表模块。不少人看的云里雾里的，不少人说的似懂非懂的，主要原因是没有从项目的级别的分析和设计经验，只有单个项目模块的开发经验的人很难理解到位。事务脚本的理解其实最简单，但是很多人说不清，觉得比领域模型还难理解，也对应不到代码。但这只是幻觉，怎么可能最简单的领域逻辑模式都不懂，反而对最复杂的领域模型模式懂了呢。"
      });
      await _db.Set<BlogPost>().AddAsync(new BlogPost
      {
        User = _db.Set<User>().First(o => o.UserName == "admin"),
        Title = "领域驱动系列：澄清一些基础概念",
        CreatedOn = DateTimeOffset.Now,
        BodyOverview = "要研究DDD，必须认清DDD的核心是通用语言和模型驱动设计。即使是DDDLite（技术上的DDD），也必须清楚DDD在架构中的位置和必须的架构知识，否则一路跑到哪里能否回来都是未知了。我们先了解常用架构分层，再了解DDD的所在层次和范畴，然后强调DDD的核心。包括从架构到领域模型设计方面的决策和自己的些许实践。",
        Body = "要研究DDD，必须认清DDD的核心是通用语言和模型驱动设计。即使是DDDLite（技术上的DDD），也必须清楚DDD在架构中的位置和必须的架构知识，否则一路跑到哪里能否回来都是未知了。我们先了解常用架构分层，再了解DDD的所在层次和范畴，然后强调DDD的核心。包括从架构到领域模型设计方面的决策和自己的些许实践。"
      });
      await _db.Set<BlogPost>().AddAsync(new BlogPost
      {
        User = _db.Set<User>().First(o => o.UserName == "admin"),
        Title = "架构系列：逻辑分层总结",
        CreatedOn = DateTimeOffset.Now,
        BodyOverview = "将业务逻辑层独立出来是逻辑架构分层的基础，而将应用逻辑从业务逻辑层中分离出来是服务层（应用层）的基础。高内聚低耦合是分层依赖的基础，因此合理的划分层次，减少层级依赖是逻辑分层架构的核心。分层架构的三个基本层次为：表示层、业务逻辑层和数据访问层。如果按照业务逻辑的分类将业务逻辑层分解为服务层和领域层，则三层扩展为四个层次：表示层、服务层、领域层和数据访问层。",
        Body = "将业务逻辑层独立出来是逻辑架构分层的基础，而将应用逻辑从业务逻辑层中分离出来是服务层（应用层）的基础。高内聚低耦合是分层依赖的基础，因此合理的划分层次，减少层级依赖是逻辑分层架构的核心。分层架构的三个基本层次为：表示层、业务逻辑层和数据访问层。如果按照业务逻辑的分类将业务逻辑层分解为服务层和领域层，则三层扩展为四个层次：表示层、服务层、领域层和数据访问层。"
      });
      await _db.SaveChangesAsync();
    }
    catch
    {
      throw;
    }
  }

  public async Task InitSystem()
  {
    //添加权限
    ///默认用户super、admin
    ///super具有全部权限，admin只具有当前系统定值的权限
    ///super可以给所有用户分配权限，admin可以给除super的其他用户分配自己具有的权限
    ///super和admin无法删除，super和admin两种角色无法删除，super角色无法修改
    //await _db.Set<Permission>().AddAsync(new Permission { Number = "Authorize", Name = "授权权限" });

    var resourceTypes = Assembly.GetAssembly(typeof(BaseEntity))!.GetTypes().Where(t => t.GetCustomAttributes().Any(a => a.GetType().IsAssignableTo(typeof(GroupAttribute))));

    foreach (var item in resourceTypes)
    {
      // 初始化资源
      var resourceAttribute = item.GetCustomAttributes()
        .FirstOrDefault(o => typeof(GroupAttribute).IsAssignableFrom(o.GetType()))
        as GroupAttribute;
      var resourceNumber = $"{resourceAttribute!.Group}.{item.Name}";
      var resourceName = this._localizer[item.GetCustomAttribute<DisplayAttribute>()?.Name ?? item.FullName!];
      var permission = _db.Set<Permission>().FirstOrDefault(o => o.Number == resourceNumber);
      permission ??= _db.Set<Permission>().Add(new Permission { Number = resourceNumber!, Name = resourceName, Type = PermissionType.Resource }).Entity;
      // 初始化资源分组
      Permission? group = null;
      var menuGroups = resourceAttribute?.Group;
      if (menuGroups != null)
      {
        var menuGroupNumbers = menuGroups.Split(".").ToList();
        Permission? prevGroup = null;
        foreach (var menuNumber in menuGroupNumbers)
        {
          group = _db.Set<Permission>().FirstOrDefault(o => o.Type == PermissionType.Group && o.Number == menuNumber);
          group ??= _db.Set<Permission>().Add(new Permission { Number = menuNumber!, Name = _localizer[menuNumber], Type = PermissionType.Group }).Entity;
          if (prevGroup != null)
          {
            group.ParentId = prevGroup.Id;
          }
          prevGroup = group;
        }
        if (group != null)
        {
          permission.ParentId = group.Id;
        }
      }
      // 初始化通用权限
      var permissions = item.GetCustomAttribute<PermissionAttribute>();
      if (permissions != null && permissions.Permissions != OperationType.None)
      {
        foreach (var operationName in Enum.GetNames(typeof(OperationType)))
        {
          if (operationName == OperationType.None.ToString() || operationName == OperationType.All.ToString())
          {
            continue;
          }
          var operation = (OperationType)Enum.Parse(typeof(OperationType), operationName);
          if (permissions.Permissions.HasFlag(operation))
          {
            var number = $"{resourceNumber}.{operation}";
            var name = this._localizer[operation.GetDisplayName()];
            _db.Set<Permission>().Add(new Permission { Number = number, Name = name, Type = PermissionType.Permission, ParentId = permission.Id });
          }
        }
      }
      // 初始化自定义权限
      var customPermissionType = Type.GetType(item.GetType().FullName + "Permissions");
      if (customPermissionType != null)
      {
        foreach (var operationName in Enum.GetNames(customPermissionType))
        {
          var operation = (OperationType)Enum.Parse(customPermissionType, operationName);
          var number = $"{resourceNumber}.{operation}";
          var name = this._localizer[operation.GetDisplayName()];
          _db.Set<Permission>().Add(new Permission { Number = number, Name = name, Type = PermissionType.Permission, ParentId = permission.Id });
        }
      }

      await _db.SaveChangesAsync();
    }
  }
}
