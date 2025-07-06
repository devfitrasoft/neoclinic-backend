
# Dotnet Neoclinic Backend

neoclinic-backend Originally built using .NET 8.0, a microservice-based .sln which will be the backbone of Neoclinic-v2 system. # neoclinic-backend


## Deployment

Dependencies

- PostgreSQL
- 7Zip (for archiving past logs)
- Configure neo.admin/appsettings.json
```
"ConnectionStrings": {
  "EnterpriseDB": "Host=<db host>;Database=db_neoclinic;Username=<db username>;Password=<db password>"
},
```
- Run the APIs as IIS Server in local environment