# ASP.NET Core Web API 项目集合

本仓库包含多个基于 .NET 10.0 的 ASP.NET Core Web API 项目。

---

## 项目列表

| 项目 | 说明 | 状态 |
|------|------|------|
| [NetTask](#nettask任务管理系统) | 企业内部在线任务管理系统 | 已完成 |
| [NetFavorite](#netfavorite书签管理系统) | 书签管理系统 | 已完成 |

---

# NetTask（任务管理系统）

企业内部在线任务管理 API 项目，支持员工、领导、管理员三种角色。

## 功能特性

| 角色 | 权限 |
|------|------|
| 员工 | 管理个人任务（CRUD、状态标记） |
| 领导 | 个人任务 + 查看本部门员工任务 |
| 管理员 | 管理员工和部门信息 |

## 技术栈

- **框架**: .NET 10.0
- **数据库**: SQL Server + Entity Framework Core
- **认证**: JWT Bearer Token
- **密码加密**: PBKDF2 + HMACSHA512
- **API 文档**: Swagger/Swashbuckle

## 项目结构

```
NetTask/
├── Controllers/
│   ├── LoginController.cs              # 登录（允许匿名）
│   ├── DepartmentController.cs         # 部门CRUD（管理员权限）
│   ├── LoginUserController.cs          # 用户CRUD（管理员权限）
│   ├── TaskItemController.cs           # 个人任务CRUD
│   └── DepartmentTaskItemController.cs # 部门任务查看（领导权限）
├── Models/
│   ├── Department.cs
│   ├── LoginUser.cs
│   ├── RolePermission.cs
│   └── TaskItem.cs
├── Utilities/
│   ├── HashPasswordService.cs          # 密码加密
│   ├── ITokenService.cs                # Token接口
│   ├── TokenService.cs                 # JWT Token服务
│   ├── PermissionRequirement.cs        # 权限策略
│   ├── PermissionRequirementHandler.cs # 权限处理
│   ├── PermissionPolicyProvider.cs     # 策略提供程序
│   └── PermissionAuthorizeAttribute.cs # 自定义特性
├── NetTaskDbContext.cs                 # 数据库上下文
├── Program.cs
└── appsettings.json
```

## 数据库设计

### 表结构

| 表名 | 说明 |
|------|------|
| Department | 部门表 |
| LoginUser | 用户表 |
| TaskItem | 任务表 |
| RolePermission | 角色权限表 |

### ER 关系

```
Department (1) ──── (N) LoginUser (1) ──── (N) TaskItem
                          │
                          └── Role (员工/领导/管理员)
```

## API 接口

### 登录认证

| 方法 | 路径 | 权限 | 说明 |
|------|------|------|------|
| GET | /api/Login | 匿名 | 登录获取Token |

### 部门管理（管理员）

| 方法 | 路径 | 权限 | 说明 |
|------|------|------|------|
| GET | /api/Department | 部门列表 | 获取所有部门 |
| GET | /api/Department/{id} | 部门详情 | 获取单个部门 |
| POST | /api/Department | 新增部门 | 创建部门 |
| PUT | /api/Department/{id} | 修改部门 | 修改部门名称 |
| DELETE | /api/Department/{id} | 删除部门 | 删除部门（需无员工） |

### 用户管理（管理员）

| 方法 | 路径 | 权限 | 说明 |
|------|------|------|------|
| GET | /api/LoginUser | 用户列表 | 获取所有用户 |
| GET | /api/LoginUser/{id} | 用户详情 | 获取单个用户 |
| POST | /api/LoginUser | 新增用户 | 创建用户 |
| PUT | /api/LoginUser/{id} | 修改用户 | 修改用户信息 |
| DELETE | /api/LoginUser/{id} | 删除用户 | 删除用户及任务 |

### 个人任务

| 方法 | 路径 | 权限 | 说明 |
|------|------|------|------|
| GET | /api/TaskItem | 个人任务列表 | 获取当前用户任务 |
| GET | /api/TaskItem/{id} | 个人任务详情 | 获取单个任务 |
| POST | /api/TaskItem | 新增个人任务 | 创建任务 |
| PUT | /api/TaskItem/{id} | 修改个人任务 | 修改任务 |
| DELETE | /api/TaskItem/{id} | 删除个人任务 | 删除任务 |

### 部门任务（领导）

| 方法 | 路径 | 权限 | 说明 |
|------|------|------|------|
| GET | /api/DepartmentTaskItem | 部门任务列表 | 获取部门所有任务 |
| GET | /api/DepartmentTaskItem/{id} | 部门任务详情 | 获取部门单个任务 |

## 种子数据

| 账号 | 密码 | 角色 | 部门 |
|------|------|------|------|
| admin | 123456 | 管理员 | 技术部 |
| user01 | 123456 | 领导 | 人事部 |
| user02 | 123456 | 员工 | 人事部 |

## 快速开始

```bash
cd NetTask
dotnet run
```

浏览器访问 Swagger：http://localhost:5000/swagger

---

# NetFavorite（书签管理系统）

基于 .NET Core 10.0 的书签管理系统后端 API，支持用户认证、书签管理、分类管理等功能。

## 功能特性

### 用户管理
- 用户注册（自动加密密码）
- 用户登录（JWT Token 认证）
- 修改密码（验证旧密码，加密新密码）
- 用户信息查询与修改
- 用户删除

### 书签管理
- 书签 CRUD 操作（需认证）
- 用户只能操作自己的书签（通过 Bookmark_LoginUserId 关联）
- 自动记录创建时间

### 安全特性
- 密码加盐哈希加密（PBKDF2 + HMACSHA512，20次迭代）
- JWT Token 认证
- 接口权限控制（[Authorize] / [AllowAnonymous]）

## 项目结构

```
NetFavorite/
├── Controllers/
│   ├── LoginController.cs        # 登录、修改密码、测试接口
│   ├── LoginUserController.cs    # 用户管理
│   ├── BookmarkController.cs     # 书签管理
│   └── TestPasswordController.cs # 密码测试接口
├── Models/
│   ├── LoginUser.cs
│   ├── Bookmark.cs
│   └── LoginRequest.cs
├── Utilities/
│   ├── HashPasswordService.cs    # 密码加密服务
│   └── TokenService.cs           # JWT Token 服务
├── wwwroot/swagger-ui/           # Swagger 自定义资源
├── NetFavoriteDbContext.cs
├── Program.cs
└── appsettings.json
```

## 快速开始

```bash
cd NetFavorite
dotnet run
```

浏览器访问 Swagger：http://localhost:5248/swagger

## 数据库 Schema

#### LoginUser 表
| 字段名 | 类型 | 说明 |
|--------|------|------|
| LoginUser_Id | uniqueidentifier | 主键 |
| LoginUser_Account | varchar(50) | 账号 |
| LoginUser_Password | varchar(500) | 加密后的密码 |
| LoginUser_Role | varchar(50) | 角色 |
| LoginUser_Salt | nvarchar(500) | 盐值 |

#### Bookmark 表
| 字段名 | 类型 | 说明 |
|--------|------|------|
| Bookmark_Id | uniqueidentifier | 主键 |
| Bookmark_Address | varchar(500) | 书签地址 |
| Bookmark_Title | varchar(500) | 书签标题 |
| Bookmark_CreateTime | datetime | 创建时间 |
| Bookmark_LoginUserId | uniqueidentifier | 外键（用户ID） |

## 项目依赖

| 包名 | 版本 | 用途 |
|------|------|------|
| Microsoft.EntityFrameworkCore.SqlServer | 10.0.5 | SQL Server 数据库访问 |
| Microsoft.AspNetCore.Authentication.JwtBearer | 10.0.5 | JWT 认证 |
| Microsoft.AspNetCore.Cryptography.KeyDerivation | 10.0.5 | 密码加密算法 |
| Swashbuckle.AspNetCore | 6.5.0 | Swagger API 文档 |

---

## 许可证

MIT License

## 作者

1aush

---

**最后更新**: 2026-05-26
