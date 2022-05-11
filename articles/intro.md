# Introduction

FarmCraft.Users is the microservice for FarmCraft that handles everything
related to users, roles, and organizations. It's the responsibility of
FarmCraft.Users to subscribe to Azure AD and keep track of all user related
activities such as registration and profile updates.

## Definitions

In relation to FarmCraft, users are the individuals who subscribe to and log in
to the FarmCraft System. They have the ability to register devices to their
account and belong to one or more organizations.

You can think of organizations as a "company" or "entity." Each organization has
an owner, and the owner can invite additional users to their organization.

Roles are assigned to a user, and are specific to an organization. They
determine what a user can do within the organization.
