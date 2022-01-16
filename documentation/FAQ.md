# Frequently asked questions

There are some common problems people encounter with hass workstations service, so if you run into a problem you should search this list. Most browsers have a search function you can access by pressing `CTRL` and `F` simultaniously.

If you cannot solve your problem still, join the [discord server](https://discord.gg/VraYT2N3wd).

### Where are config files located? 

You can find the configuration files inside of `%appdata%\Hass Workstation Service` on windows.

|file|usage|
| --- | --- |
|configured-commands.json|stores all data about commands, including their properties|
|configured-sensors.json|stores information about sensors, including their properties|
|mqttbroker.json|stores data about your MQTT broker, dont share online|


### Are there any client logs?

Check the logs folder, its stored in the same place as config files.


### I cannot find documentation on a new feature?

You can submit a pull request with new documentation, ask on the [discord server](https://discord.gg/VraYT2N3wd), or check the [develop branch](https://github.com/sleevezipper/hass-workstation-service/tree/develop) for updated documentation.
