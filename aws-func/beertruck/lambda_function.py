import time
import urllib
import hmac
import hashlib
import base64
import json
import os
import sys
from urllib import parse, request, error

sb_name = 'lambada-events'
eh_name = 'trucks-from-aws'
eh_publish_url = f'https://{sb_name}.servicebus.windows.net/{eh_name}/messages'
eh_query_params = '?timeout=60&api-version=2014-01'

def lambda_handler(event, context):
    print(event)
    
    sas_value = os.environ.get('EVENTHUB_SEND_KEY')
    if sas_value == None:
        return('Unable to pick up EVENTHUB_SEND_KEY secret.')
    
    sas_token = make_eventhub_sas(sb_name, eh_name, 'send', sas_value)
    
    headers = {}
    headers['Content-Type'] = 'application/json'
    headers['Authorization'] = sas_token
    
    payload = {
        'source': 'aws',
        'message': event
    }

    
    req = request.Request(
        url=eh_publish_url + eh_query_params,
        data=json.dumps(payload).encode('utf-8'),
        headers=headers)
    
    try:
        response = request.urlopen(req)
        body = response.read().decode()
        status = response.status
        print(f'Event Hub response status code: {status}')
        print(body)
    except error.HTTPError as e:
        body = e.read().decode()
        print(body)
    except Exception as e:
        print(e)

def make_eventhub_sas(sb_name, eh_name, sas_name, sas_value):
    """
    Returns an authorization token dictionary 
    for making calls to Event Hubs REST API.
    """
    uri = urllib.parse.quote_plus(f'https://{sb_name}.servicebus.windows.net/{eh_name}')
    sas = sas_value.encode('utf-8')
    expiry = str(int(time.time() + 10000))
    string_to_sign = (uri + '\n' + expiry).encode('utf-8')
    signed_hmac_sha256 = hmac.HMAC(sas, string_to_sign, hashlib.sha256)
    signature = urllib.parse.quote(base64.b64encode(signed_hmac_sha256.digest()))
    
    token = f'SharedAccessSignature sr={uri}&sig={signature}&se={expiry}&skn={sas_name}'
    
    return token