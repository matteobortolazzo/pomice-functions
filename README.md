# Pomice Functions

A collection of Azure Functions to help manage Pomice with Contenful and Azure DevOps.

## Setup
- 2 build and 2 release definitions.
- The first build handles code updates, triggered on code push and its release calls the "update app version" function.
- The second build handles content updates, triggered by Contentful webhooks.

## Current functions

### Update app version
Update the entry with the app version in Contentful after a new release in DevOps. 

- In Azure create a Function App.
- Publish the function in the Function App.
- In Contenful go in Settings, API Keys, Content management tokens, Generate personal token and copy the new token.
- In the Function App app settings set: CONTENTFUL_SPACEID, CONTENTFUL_ENVIRONMENT and CONTENTFUL_CONTENTMANAGEMENTTOKEN.
- In DevOps go in Project settings, Service Hooks, add a Webhook that triggers with a 'Release deployment completed' and set the function url as target.

### Prerender content
Queue a new build in DevOps after a content update in Contenful. (Prerendering).

- In Azure create a Function App.
- Publish the function in the Function App.
- In DevOps click in your profile image, Security, New token, authorize the "Read & execute" Build scope and copy the new token.
- In the Function App app settings set: DEVOPS_ORGANIZATION, DEVOPS_PROJECT, DEVOPS_BUILDDEFINITIONID and DEVOPS_ACCESSTOKEN.
- In Contentful go in Settings, Webhooks and add a new webhook.
- Set the function url as target (POST), trigger it on entry publish and add a filters on content type id equals post.

## WIP functions

### Notifications
Send web notifications after a new content in Contenful.
