Пример конфига GenConfig.json

{
  "CsProject": "Declaration.csproj",
  "Dll": "bin/Debug/net5.0/publish/Declaration.dll",
  "DbModels": {
    "Namespace": "MobileApi.Dal.Model",
    "EnumNamespaces": [
      "MobileApi.Dal.Common",
      "Dex.CreditCardType.Resolver"
    ],
    "Path": "../MobileApi.Dal/Model/Generated",
    "IsSnakeCase": true
  },
  "DbFluentFk": {
    "Namespace": "MobileApi.Dal.Provider",
    "Path": "../MobileApi.Dal/Provider/Generated/DbFluentFkProvider.g.cs"
  },
  "DbFluentEnum": {
    "Namespace": "MobileApi.Dal.Provider",
    "Path": "../MobileApi.Dal/Provider/Generated/DbFluentEnumProvider.g.cs"
  },
  "DbFluentIndex": {
    "Namespace": "MobileApi.Dal.Provider",
    "Path": "../MobileApi.Dal/Provider/Generated/DbFluentIndexProvider.g.cs"
  },
  "Dto": {
    "Namespace": "MobileApi.Dto",
    "Path": "../MobileApi.Dto/Generated"
  }
}
