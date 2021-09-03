# Direct communication

Because this application communicates via MQTT, You can query sensor data directly from other compatible programs. This means you can use it outside of homeassistant, allowing for great flexibility, and this document is designed to simplify this process.

## MQTT topics

All of the sensors in this app follow a strict naming convention, and can be accessed using a simple formula:
```
homeassistant/ENTITY TYPE/PC NAME/SENSOR NAME/REQUEST TYPE
```
This means that you can substitute values into this formula to get the topic for any sensor, for example :
```
homeassistant/switch/mediaCenterPC/Shutdown/set
```

## Executing commands

By publishing a message containing `ON` to this example topic :
```
homeassistant/switch/mediaCenterPC/Shutdown/set
```
We can execute the shutdown command, and turn off the media center PC.

## Reading sensor data

Reading sensor data can be done by subscribing to an MQTT topic. For example, if I wanted to know the CPU load, i can subscribe to this topic :
```
homeassistant/sensor/DESKTOP-1/MediaCenterPC/State
```

## MQTT autodiscovery

If your application supports MQTT discovery, than you should be able to see your sensors added to your platform of choice automatically. When using home assistant, you should take care to rename your entities, as the MQTT integration does not integrate the device name into the entity name, and so using the default could potentially cause a collision with multiple devices.
