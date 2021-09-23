# magentoapi.soap-svcutil
Creating service classes

## Install cake

```
dotnet tool install --global Cake.Tool --version 1.2.0
```

if cake is already install update

```
dotnet tool update --global Cake.Tool --version 1.2.0
```

## Build

Call the build target in solution root.

```
dotnet cake --target="Build"
```

## Deploy

Call the deploy target in solution root. The argument ```NugetDeployApiKey``` is mandatory.

```
dotnet cake --target="Deploy" --NugetDeployApiKey=ABC1234
```

## Usage

### Install as global tool

```
dotnet tool install --global Compori.MagentoApi.SoapSvcUtil
```

### Uninstall as global tool
```
dotnet tool uninstall --global Compori.MagentoApi.SoapSvcUtil
```

### List

The following command extracts the services from the shop and saves them as a list in services.txt

```
compori.magentoapi.soap-svcutil list -a http://the-magento-shop/ -u adminuser -p adminpassword -o services.txt
```

### Build service source files

The following command takes the services from service.txt and creates the source files for usage in a project.

```
compori.magentoapi.soap-svcutil build -a http://the-magento-shop/ -u adminuser -p adminpassword -o c:\temp\service-files -n MyNamesSpace.RemoteServices
```
