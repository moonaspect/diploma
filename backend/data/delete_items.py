import boto3

# Initialize DynamoDB client
dynamodb = boto3.resource('dynamodb')
table = dynamodb.Table('KatakanaTable')

# Scan the table to get all items
response = table.scan()
items = response.get('Items', [])

# Loop through items and delete them
with table.batch_writer() as batch:
    for item in items:
        batch.delete_item(Key={'WordId': item['WordId']})
