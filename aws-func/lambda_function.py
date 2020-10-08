import boto3
import json
import urllib.parse
import urllib.request
import datetime
import secrets
import os
import time
from base64 import b64encode, b64decode
from hashlib import sha256
from hmac import HMAC

s3_client = boto3.client('s3')

eventgrid_publish_url = 'https://lambada.westeurope-1.eventgrid.azure.net/api/events'
eventgrid_api_version = '?api-version=2018-01-01'
eventgrid_publish_key = os.environ['EventGridPublishKey']

def lambda_handler(event, context):
    bucket_name = event['Records'][0]['s3']['bucket']['name']
    object_name = urllib.parse.unquote_plus(event['Records'][0]['s3']['object']['key'], encoding='utf-8')
    print(f'Triggered on bucket {bucket_name}, object name {object_name}')
    
    # Generate Presigned URL (equivalent to a SAS URL in Azure)
    try:
        presigned_url = s3_client.generate_presigned_url('get_object',
            Params={'Bucket': bucket_name, 'Key': object_name},
            ExpiresIn=3600)
        # print("Presigned URL: " + presigned_url)
    except Exception as e:
        print(e)
        
    # Generate SAS token for Event Grid
    if eventgrid_publish_key == None:
        print('Unable to pick up EventGridPublishKey from env vars.')
        sys.exit(1)
    sas_token = generate_sas_token(eventgrid_publish_url, eventgrid_publish_key)
    
    # Publish S3 event to Azure Event Grid
    headers = {}
    headers['Content-Type'] = 'application/json'
    headers['Authorization'] = sas_token

    payload = [{
      "id": secrets.token_urlsafe(8),
      "eventType": "BeerAlert",
      "subject": "BeerAlert",
      "eventTime": datetime.datetime.now().isoformat(),
      "data": {
        "imgurl": presigned_url
      },
      "dataVersion": "1.0"
    }]

    req = urllib.request.Request(
        url=eventgrid_publish_url + eventgrid_api_version,
        data=json.dumps(payload).encode('utf-8'),
        headers=headers)

    try:
        response = urllib.request.urlopen(req)
        body = response.read().decode()
        status = response.status
        print(f'Event Grid response status code: {status}')
        print(body)
    except urllib.error.HTTPError as e:
        body = e.read().decode()
        print(body)
    except Exception as e:
        print(e)

def generate_sas_token(uri, key, expiry=900):
    # See https://docs.microsoft.com/en-us/azure/event-grid/security-authenticate-publishing-clients#authenticate-using-a-sas-token
    ttl = datetime.datetime.utcnow() + datetime.timedelta(seconds=expiry)
    encoded_resource = urllib.parse.quote_plus(uri)
    encoded_expiration_utc = urllib.parse.quote_plus(ttl.isoformat())
    unsigned_sas = f'r={encoded_resource}&e={encoded_expiration_utc}'
    signature = b64encode(HMAC(b64decode(key), unsigned_sas.encode('utf-8'), sha256).digest()).decode()
    expiration = str(ttl)
    token = 'SharedAccessSignature ' + f'r={encoded_resource}&e={encoded_expiration_utc}&s={signature}'

    return token