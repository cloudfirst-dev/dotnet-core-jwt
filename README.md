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
$ ansible-playbook -e tokens_audience=$TOKENS_AUDIENCE -e tokens_issuer=$TOKENS_ISSUER -i ./.applier/ galaxy/openshift-applier/playbooks/openshift-cluster-seed.yml
```
3. If you are running on an OpenShift 4.x cluster be sure to create a Jenkins instance in the dotnet-core-jwt-build namespace

At this point you should have two projects created (`dotnet-core-jwt-build`, and `dotnet-core-jwt-dev`) with a pipeline in the `-build` project, and our job runner app and job images
