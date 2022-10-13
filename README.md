# Twilio caller

This azuzre function was created during an innovation project where we
need to have an app trigger a test call to the phone where the app is 
running on.

This function uses Twilio API in order to trigger a call.

## Usage

Once deployed you can use this function as follows:

```bash
curl --location --request POST 'http://localhost:7071/api/call' \
--header 'Content-Type: application/json' \
--data-raw '{
    "name": "Markus",
    "number": "123456789"
}'
```

This triggers a call to the given number, voicing a personal message
to the callee.

## Development

Open solution with your favorite tools for net6 development.

### Configuration

Create a file called `local.settings.json` inside of `TwilioCaller\`, i.e. next to the funtion project.

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "TWILIO_ACCOUNT_SID": "<your SID here>",
    "TWILIO_ACCOUNT_TOKEN": "<your auth token here>",
    "TWILIO_FROM_NUMBER": "<your twilio number>",
    "TWILIO_VOICE": "Polly.Vicki", //https://www.twilio.com/docs/voice/twiml/say/text-speech#polly-standard-and-neural-voices
    "TEXT_PERSONAL": "Herzlichen Glueckwunsch {0}! Sie haben den Anruf erfolgreich angenommen!",
    "TEXT_NEUTRAL": "Herzlichen Glueckwunsch! Sie haben den Anruf erfolgreich angenommen!"
  }
}
```

Twilio configuration can be fetched from "Voice" services on the Twilio dashboard.
The example above uses a German voice with German text.

## Deployment

- Create a linux based function app on Azure
- Download "publish profile" from "deployment center" on the function management pane in Azure
- Open solution with Visual Studio 2022
- Right click function project and select "Publish..."
- Create new profile and select "Import profile"
- Import `profile.publishsettings` previously downloaded from Azure
- Click publish

### Configuration

Configure all configuration values as environment variables on your function account:
`TWILIO_ACCOUNT_SID`, `TWILIO_ACCOUNT_TOKEN`, `TWILIO_FROM_NUMBER`, `TWILIO_VOICE`, `TEXT_PERSONAL`, `TEXT_NEUTRAL`

## Ideas for extension

- Make proper dependency injection for Twilio in order to allow unit testing
- Check if autodetection of language is possible or add an optional language parameter
- Store texts on storage table instead of environment config