import boto3
import json

# Initialize DynamoDB client
dynamodb = boto3.client('dynamodb', region_name='eu-north-1')

# Load JSON file
with open('japan_words.json', 'r', encoding='utf-8-sig') as f:
    words = json.load(f)

# Insert each item into the DynamoDB table
table_name = 'MatchingPairsTable'
for word in words:
    dynamodb.put_item(
        TableName=table_name,
        Item={
            'WordId': {'S': word['WordId']},
            'Japanese': {'S': word['Japanese']},
            'Ukrainian': {'S': word['Ukrainian']}
        }
    )

print("Data loaded successfully!")
