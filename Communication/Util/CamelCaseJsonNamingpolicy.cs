using System.Text.Json;

namespace hass_desktop_service.Communication.Util
{
    public class CamelCaseJsonNamingpolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name) => name.ToLowerInvariant();
    }
}