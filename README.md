# Pomice Functions

A collection of Azure Functions to help manage Pomice with Contenful and Azure DevOps.

## Current functions

### Update app version
Update the entry with the app version in Contentful after a new release in DevOps. 

- In Contenful go in Settings, API Keys, Content management tokens, Generate personal token and copy the new token.
- In Azure create a Function App.
- Publish the function in the Function App.
- In the Function App app settings set: CONTENTFUL_SPACEID, CONTENTFUL_ENVIRONMENT and CONTENTFUL_CONTENTMANAGEMENTTOKEN.
- In DevOps go in Project settings, Service Hooks, add a Webhook that triggers with a 'Release deployment completed' and set the Function url as target.

## WIP functions

### Prerender content
Queue a new build in DevOps after a content update in Contenful. (Prerendering).

### Notifications
Send web notifications after a new content in Contenful.
