// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Azure.Devices.Client.Transport
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Azure.Devices.Client.Extensions;
#if !NETMF && !PCL
    using Microsoft.Azure.Devices.Client.Transport.Mqtt;
#endif
    class TransportHandlerFactory : ITransportHandlerFactory
    {
        public IDelegatingHandler Create(IPipelineContext context)
        {
            var connectionString = context.Get<IotHubConnectionString>();
            var transportSettings = context.Get<ITransportSettings[]>();
            var onMethodCallback = context.Get<DeviceClient.OnMethodCalledDelegate>();

            ITransportSettings transportSetting = null;
            if (transportSettings != null)
            {
                if (transportSettings.Length > 1)
                {
                    transportSetting = transportSettings[0];
                }
                else
                {
                    transportSetting = transportSettings[0];
                }
                switch (transportSetting.GetTransportType())
                {
                    case TransportType.Amqp_WebSocket_Only:
                    case TransportType.Amqp_Tcp_Only:
                        return new AmqpTransportHandler(context, connectionString, transportSetting as AmqpTransportSettings, new Func<MethodRequestInternal, Task>(onMethodCallback));
                    case TransportType.Http1:
                        return new HttpTransportHandler(context, connectionString, transportSetting as Http1TransportSettings);
#if !NETMF && !PCL
                    case TransportType.Mqtt_Tcp_Only:
                    case TransportType.Mqtt_WebSocket_Only:
                        return new MqttTransportHandler(context, connectionString, transportSetting as MqttTransportSettings, new Func<MethodRequestInternal, Task>(onMethodCallback));
#endif
                    default:
                        throw new InvalidOperationException("Unsupported Transport Setting {0}".FormatInvariant(transportSetting));
                }
            }
            return null;
        }
    }
}