# Create Storage class & local persistence volumes
k create -f yamls/local-provisioner/fast-disks.yaml
k create -f yamls/local-provisioner/provisioner_generated.yaml

# Deploy MySQL
k create -f yamls/mysql/mysql-pvc.yaml
k create -f yamls/mysql/mysql-deployment.yaml
