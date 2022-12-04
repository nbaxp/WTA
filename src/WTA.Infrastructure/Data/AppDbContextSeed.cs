using Microsoft.EntityFrameworkCore;
using WTA.Application.Domain.Blogs;
using WTA.Application.Domain.Users;
using WTA.Application.Extensions;
using WTA.Application.Interfaces;

namespace WTA.Infrastructure.Data;

public class AppDbContextSeed
{
  private readonly DbContext _db;
  private readonly IPasswordHasher _passwodHasher;

  public AppDbContextSeed(DbContext db, IPasswordHasher passwodHasher)
  {
    this._db = db;
    this._passwodHasher = passwodHasher;
  }

  public async Task Seed()
  {
    // User
    var salt = _passwodHasher.CreateSalt();
    await _db.Set<User>().AddAsync(new User { UserName = "super", NormalizedUserName = "super".Normalized(), SecurityStamp = salt, PasswordHash = _passwodHasher.HashPassword("123456", salt), UserRoles = new List<UserRole> { new UserRole { Role = new Role { Name = "超级管理员", RolePermissions = new List<RolePermission> { new RolePermission { Permission = new Permission { Name = "测试权限" } } } } } } });
    await _db.Set<User>().AddAsync(new User { UserName = "admin", NormalizedUserName = "admin".Normalized(), SecurityStamp = salt, PasswordHash = _passwodHasher.HashPassword("123456", salt) });
    await _db.Set<User>().AddAsync(new User { UserName = "user", NormalizedUserName = "user".Normalized(), SecurityStamp = salt, PasswordHash = _passwodHasher.HashPassword("123456", salt) });
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
}
