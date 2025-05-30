#!/bin/bash
# This script creates a managed identity (if it doesn't exist), a federated credential for it, and assigns the Owner role at the resource group level.
# It will prompt you for all required parameters.

# Prompt for required parameters
read -p "Enter your Azure Subscription ID: " SUBSCRIPTION_ID
read -p "Enter your Resource Group name (will be created if not existing): " RESOURCE_GROUP
read -p "Enter your Managed Identity name (will be created if not existing): " MANAGED_IDENTITY_NAME
read -p "Enter your cloned GitHub repo name (e.g. yourgithubaccount/broken-web-app): " REPO_NAME

# Set the subscription (optional if already set)
az account set --subscription "$SUBSCRIPTION_ID"

# Create the resource group if it does not exist
echo "Checking if resource group '$RESOURCE_GROUP' exists..."
RG_EXISTS=$(az group exists --name "$RESOURCE_GROUP")
if [ "$RG_EXISTS" = false ]; then
  read -p "Enter Azure region for the resource group [centralus]: " REGION
  REGION=${REGION:-centralus}
  echo "Resource group not found. Creating resource group in region '$REGION'..."
  az group create --name "$RESOURCE_GROUP" --location "$REGION"
else
  echo "Resource group already exists."
fi

# Create the managed identity if it does not exist
echo "Checking if managed identity '$MANAGED_IDENTITY_NAME' exists in resource group '$RESOURCE_GROUP'..."
IDENTITY_EXISTS=$(az identity show \
  --name "$MANAGED_IDENTITY_NAME" \
  --resource-group "$RESOURCE_GROUP" \
  --query 'name' -o tsv 2>/dev/null)

if [ -z "$IDENTITY_EXISTS" ]; then
  echo "Managed identity not found. Creating managed identity..."
  az identity create --name "$MANAGED_IDENTITY_NAME" --resource-group "$RESOURCE_GROUP"
else
  echo "Managed identity already exists."
fi

# Federated credential details (edit these if needed)
CREDENTIAL_NAME="cred$(date +%s%N | sha256sum | head -c 8)"
ISSUER="https://token.actions.githubusercontent.com"
SUBJECT="repo:${REPO_NAME}:ref:refs/heads/main"
AUDIENCE="api://AzureADTokenExchange"

# Create the federated credential
az identity federated-credential create \
  --name "$CREDENTIAL_NAME" \
  --identity-name "$MANAGED_IDENTITY_NAME" \
  --resource-group "$RESOURCE_GROUP" \
  --issuer "$ISSUER" \
  --subject "$SUBJECT" \
  --audiences "$AUDIENCE"

# Get the managed identity's object ID
IDENTITY_OBJECT_ID=$(az identity show \
  --name "$MANAGED_IDENTITY_NAME" \
  --resource-group "$RESOURCE_GROUP" \
  --query 'principalId' -o tsv)

# Assign the Owner role to the managed identity at the resource group scope
az role assignment create \
  --assignee "$IDENTITY_OBJECT_ID" \
  --role "Owner" \
  --scope "/subscriptions/$SUBSCRIPTION_ID/resourceGroups/$RESOURCE_GROUP"

# Get the managed identity's client ID, subscription ID, and tenant ID
MANAGED_IDENTITY_CLIENTID=$(az identity show \
  --name "$MANAGED_IDENTITY_NAME" \
  --resource-group "$RESOURCE_GROUP" \
  --query 'clientId' -o tsv)
SUBSCRIPTIONID="$SUBSCRIPTION_ID"
TENANTID=$(az account show --query 'tenantId' -o tsv)

# Output the variables for GitHub secrets

echo "\n==============================="
echo "Add the following values as GitHub repository secrets:" 
echo "MANAGED_IDENTITY_CLIENTID=$MANAGED_IDENTITY_CLIENTID"
echo "SUBSCRIPTIONID=$SUBSCRIPTIONID"
echo "TENANTID=$TENANTID"
echo "===============================\n"
echo "Resource group ensured, managed identity ensured, federated credential created, and Owner role assigned to the managed identity." 