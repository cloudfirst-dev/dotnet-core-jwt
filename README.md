## Prequesites



## Automated Deployment

This quickstart can be deployed quickly using Ansible. Here are the steps.

1. Clone [this repo](https://github.com/cloudfirst-dev/dotnet-core-jwt)
2. `cd dotnet-core-jwt`
3. Run `ansible-galaxy install -r requirements.yml --roles-path=galaxy`
2. Log into an OpenShift cluster, then run the following command.
```
$ export TOKENS_AUDIENCE=[valid audience for oauth provider used for token validation]
$ export TOKENS_ISSUER=[valid issuer for oauth provider used for token validation]
$ export AUTH_CLIENT_ID=[valid oauth client id]
$ export AUTH_URL=[valid oauth auth url]
$ export AUTH_PUBLIC_KEY=$(cat /path/to/public/key | base64)
$ export BASE_API_URL=[route address for deployed api]
$ ansible-playbook \
-e tokens_audience=$TOKENS_AUDIENCE \
-e tokens_issuer=$TOKENS_ISSUER \
-e auth_url=$AUTH_URL \
-e base_api_url=$BASE_API_URL \
-e auth_client_id=$AUTH_CLIENT_ID \
-e auth_public_key=$AUTH_PUBLIC_KEY \
-i ./.applier/ galaxy/openshift-applier/playbooks/openshift-cluster-seed.yml
```
3. If you are running on an OpenShift 4.x cluster be sure to create a Jenkins instance in the dot-net-auth-build namespace
```
oc new-app jenkins-ephemeral -n [build-namespace]
```

At this point you should have two projects created (`dotnet-core-jwt-build`, and `dotnet-core-jwt-dev`) with a pipeline in the `-build` project, and our job runner app and job images
