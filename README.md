# Web Template Asp.Net

## 目录结构

```
├── README.md
├── src
│   ├── WTA.Web                     # Web API
│   ├── WTA.Application             # 应用层
│   ├── WTA.Infrastructure          # 基础设施层
├── tests
│   ├── WTA.Application.Tests       # 应用层测试
```

## Web API

依赖 应用层 和 基础设施层，不直接依赖三方库。

## Application

应用层，包含领域代码、应用代码。不依赖基础设施层，定义了基础设施层必须实现的接口，不依赖三方库。

## Infrastructure

基础设施层，包含应用层接口的实现和Web层的通用代码，三方库的引用在此项目中。

## Web.UI

前端项目






