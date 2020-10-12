import { mqtt, io, iot } from 'aws-iot-device-sdk-v2';
import { TextDecoder } from 'util';

const INTERVAL = 3 * 1000; // in milliseconds

const cert = 'certs/Truck.cert.pem';
const key = 'certs/Truck.private.key';
const ca = 'certs/root-CA.crt';

const count = 100;
const topic = 'topic_1';
const client_id = 'Truck';
const endpoint = 'a11mdjcmr4c4i7-ats.iot.eu-central-1.amazonaws.com';

async function execute_session(connection: mqtt.MqttClientConnection) {
    return new Promise(async (resolve, reject) => {
        try {
            const decoder = new TextDecoder('utf8');
            const on_publish = async (topic: string, payload: ArrayBuffer) => {
                const json = decoder.decode(payload);
                console.log(`Message received on topic ${topic}`);
                const message = JSON.parse(json);
                const m = JSON.stringify(message, null, 4);
                console.log('\n' + m + '\n');
                if (message.sequence == count) {
                    resolve();
                }
            }

            await connection.subscribe(topic, mqtt.QoS.AtLeastOnce, on_publish);

            for (let op_idx = 0; op_idx < count; ++op_idx) {
                let message = {
                    'TruckId': 'Truck-' + Math.floor(Math.random() * 11),
                    'Cargo': 'Happiness-226', // Ra-226 has 1600 years half-life
                    'BottleCount': Math.round(Math.random() * (33000 - 36000) + 36000),
                    'Temperature': (Math.random() * (8.10 - 12.82) + 12.82).toFixed(2),
                    'Humidity': (Math.random() * (31.52 - 44.20) + 44.20).toFixed(2)
                };
                const publish = async () => {
                    const msg = {
                        message: message
                    };
                    const json = JSON.stringify(msg);
                    connection.publish(topic, json, mqtt.QoS.AtLeastOnce);
                }
                setTimeout(publish, op_idx * INTERVAL);
            }
        }
        catch (error) {
            reject(error);
        }
    });
}

async function main() {
    io.enable_logging(3);

    const client_bootstrap = new io.ClientBootstrap();

    let config_builder = iot.AwsIotMqttConnectionConfigBuilder.new_mtls_builder_from_path(cert, key);
    config_builder.with_certificate_authority_from_path(undefined, ca);
    config_builder.with_clean_session(false);
    config_builder.with_client_id(client_id);
    config_builder.with_endpoint(endpoint);

    // force node to wait 60 seconds before killing itself, promises do not keep node alive
    const timer = setTimeout(() => {}, 60 * 1000);

    const config = config_builder.build();
    const client = new mqtt.MqttClient(client_bootstrap);
    const connection = client.new_connection(config);

    await connection.connect()
    await execute_session(connection)

    // Allow node to die if the promise above resolved
    clearTimeout(timer);
}

main();