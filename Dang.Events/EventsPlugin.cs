using Akequ.Plugins;
using Dang.API.Features;
using Dang.API.Interfaces;
using Dang.API.Attribute;
using kcp2k;
using System;
using Log = Dang.API.Features.Log;

namespace Dang.Events
{
    [Plugin("Dang Events", "Plugin for handling events", "Kloer26", "1.0.0")]
    public class EventsPlugin : Plugin<Config>
    {
        public EventsPlugin() { Log.Info("Конструктор EventsPlugin вызван."); }

        public override void OnEnabled()
        {
            try
            {
                Log.Info("Обработчик KcpServer.OnData зарегистрирован.");
            }
            catch (Exception ex)
            {
                Log.Error($"Ошибка при регистрации обработчика KcpServer.OnData: {ex.Message}");
            }
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            try
            {
                Log.Info("Обработчик KcpServer.OnData удалён.");
            }
            catch (Exception ex)
            {
                Log.Error($"Ошибка при удалении обработчика KcpServer.OnData: {ex.Message}");
            }
            base.OnDisabled();
        }
    }
}