#!/usr/bin/env python
#
# Copyright (c) Microsoft Corporation. All rights reserved.
# Copyright 2016 Confluent Inc.
# Licensed under the MIT License.
# Licensed under the Apache License, Version 2.0
#
# Original Confluent sample modified for use with Azure Event Hubs for Apache Kafka Ecosystems

from confluent_kafka import Consumer, KafkaException, KafkaError
import sys
import json
import logging
from pprint import pformat

# 'listen' SAS policy
EVENT_HUBS_CONN_STR = 'Endpoint=sb://lambada-events.servicebus.windows.net/;SharedAccessKeyName=listen;SharedAccessKey=XXXXXXXXXXXXXXXX=;EntityPath=trucks-from-aws'

def stats_cb(stats_json_str):
    stats_json = json.loads(stats_json_str)
    print('\nKAFKA Stats: {}\n'.format(pformat(stats_json)))

group = 'KafkaConsumerGroup3'
topics = ['trucks-from-aws']

# Consumer configuration
# See https://github.com/edenhill/librdkafka/blob/master/CONFIGURATION.md
conf = {
    'bootstrap.servers': 'lambada-events.servicebus.windows.net:9093', #update
    'security.protocol': 'SASL_SSL',
    # 'ssl.ca.location': '/path/to/ca-certificate.crt',
    'sasl.mechanism': 'PLAIN',
    'sasl.username': '$ConnectionString',
    'sasl.password': EVENT_HUBS_CONN_STR,
    'group.id': group,
    'client.id': 'python-example-consumer',
    'request.timeout.ms': 60000,
    'session.timeout.ms': 60000,
    'default.topic.config': {'auto.offset.reset': 'smallest'}
}

# Create logger for consumer (logs will be emitted when poll() is called)
logger = logging.getLogger('consumer')
logger.setLevel(logging.DEBUG)
handler = logging.StreamHandler()
handler.setFormatter(logging.Formatter('%(asctime)-15s %(levelname)-8s %(message)s'))
logger.addHandler(handler)

# Create Consumer instance
# Hint: try debug='fetch' to generate some log messages
c = Consumer(conf, logger=logger)

def print_assignment(consumer, partitions):
    print('Assignment:', partitions)

# Subscribe to topics
c.subscribe(topics, on_assign=print_assignment)

# Read messages from Kafka, print to stdout
try:
    while True:
        msg = c.poll(timeout=5)
        if msg is None:
            continue
        if msg.error():
            # Error or event
            if msg.error().code() == KafkaError._PARTITION_EOF:
                # End of partition event
                sys.stderr.write('%% %s [%d] reached end at offset %d\n' %
                                    (msg.topic(), msg.partition(), msg.offset()))
            else:
                # Error
                raise KafkaException(msg.error())
        else:
            # Proper message
            print(msg.value().decode('utf-8'))

except KeyboardInterrupt:
    sys.stderr.write('%% Aborted by user\n')

finally:
    # Close down consumer to commit final offsets.
    c.close()
