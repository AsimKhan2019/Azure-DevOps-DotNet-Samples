# Contributing samples



1. All samples must have an accompanying test 
2. Organization and naming
   1 Samples for a particular area should live in a folder with that name (e.g. `Notification`)
   2. The class name should be `{Resource}Sample.cs`, where resource is the name of the resource (e.g. `Subscriptions`)
   3. Namespace should be `VstsSamples.Client.{Area}`
2. Style
   1. Use line breaks and empty lines to help deliniate important sections or lines that need to stand out
   2. Use the same "dummy" data across all samples so it's easier to correlate similar concepts
3. Coding
   1. Avoid `var` typed variables
   2. Go out of your way to show types so it's clear from the sample what types are being used  
   3. Include examples of exceptions when exceptions for a particular API are common
   2. Use constants from the client libraries for property names, etc instead of hard-coded strings
4. All samples/snippets should be runnable on their own (without any input)

